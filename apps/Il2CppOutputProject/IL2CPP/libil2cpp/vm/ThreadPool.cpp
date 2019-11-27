#include "il2cpp-config.h"

#include "il2cpp-class-internals.h"
#include "il2cpp-api.h"
#include "il2cpp-object-internals.h"
#include "vm/Array.h"
#include "vm/Class.h"
#include "vm/Exception.h"
#include "vm/Image.h"
#include "vm/Object.h"
#include "vm/Thread.h"
#include "vm/ThreadPool.h"
#include "vm/WaitHandle.h"
#include "os/Atomic.h"
#include "os/Environment.h"
#include "os/Event.h"
#include "os/Mutex.h"
#include "os/Semaphore.h"
#include "os/Socket.h"
#include "os/Thread.h"
#include "gc/Allocator.h"
#include "gc/GCHandle.h"
#include "utils/Memory.h"
#include "icalls/System/System.Net.Sockets/Socket.h"

#include <queue>
#include <vector>
#include <list>
#include <limits>
#include <algorithm>

#if IL2CPP_TARGET_POSIX
#include <unistd.h>
#include "os/Posix/PosixHelpers.h"
#elif IL2CPP_TARGET_WINDOWS
#include "os/Win32/WindowsHeaders.h"
#include "os/Win32/ThreadImpl.h"
#include <winsock2.h>
#endif

#if IL2CPP_USE_SOCKET_MULTIPLEX_IO
#include "MultiplexIO.h"
#endif

////TODO: Currently the pool uses a single global lock for each compartment; doesn't scale well and provides room for optimization.

namespace il2cpp
{
namespace vm
{
    enum
    {
        THREADS_PER_CORE = 10
    };

    typedef gc::Allocator<Il2CppAsyncResult*> AsyncResultAllocator;
    typedef std::list<Il2CppAsyncResult*, AsyncResultAllocator> AsyncResultList;
    typedef std::vector<Il2CppAsyncResult*, AsyncResultAllocator> AsyncResultVector;
    typedef std::queue<Il2CppAsyncResult*, AsyncResultList> AsyncResultQueue;


    static const Il2CppClass* g_SocketAsyncCallClass;
    static const Il2CppClass* g_ProcessAsyncCallClass;
    static const Il2CppClass* g_WriteDelegateClass;
    static const Il2CppClass* g_ReadDelegateClass;
////TODO: add System.Net.Sockets.Socket.SendFileHandler?

    static bool IsInstanceOfDelegateClass(Il2CppDelegate* delegate, const char* delegateClassName, const char* outerClassName, const Il2CppClass*& cachePtr)
    {
        Il2CppClass* klass = delegate->object.klass;

        Il2CppClass* declaringType = Class::GetDeclaringType(klass);

        if (cachePtr == 0 &&
            strcmp(klass->name, delegateClassName) == 0 &&
            (strcmp(vm::Image::GetName(klass->image), "System") == 0 ||
             strcmp(vm::Image::GetName(klass->image), "System.dll") == 0) &&
            declaringType && strcmp(declaringType->name, outerClassName) == 0)
        {
            cachePtr = klass;
        }

        return (klass == cachePtr);
    }

    static bool IsSocketAsyncCall(Il2CppDelegate* delegate)
    {
        return IsInstanceOfDelegateClass(delegate, "SocketAsyncCall", "Socket", g_SocketAsyncCallClass);
    }

    static bool IsProcessAsyncCall(Il2CppDelegate* delegate)
    {
        return IsInstanceOfDelegateClass(delegate, "AsyncReadHandler", "Process", g_ProcessAsyncCallClass);
    }

    static bool IsFileStreamAsyncCall(Il2CppDelegate* delegate)
    {
        return IsInstanceOfDelegateClass(delegate, "WriteDelegate", "FileStream", g_WriteDelegateClass) ||
            IsInstanceOfDelegateClass(delegate, "ReadDelegate", "FileStream", g_ReadDelegateClass);
    }

/// Socket operations enumeration taken from Mono. (Mostly) corresponds
/// to System.Net.Sockets.Socket.SocketOperation.
    enum
    {
        AIO_OP_FIRST,
        AIO_OP_ACCEPT = 0,
        AIO_OP_CONNECT,
        AIO_OP_RECEIVE,
        AIO_OP_RECEIVEFROM,
        AIO_OP_SEND,
        AIO_OP_SENDTO,
        AIO_OP_RECV_JUST_CALLBACK,
        AIO_OP_SEND_JUST_CALLBACK,
        AIO_OP_READPIPE,
        AIO_OP_LAST
    };

/// We use a dedicated thread to pre-screen sockets for activity and only then handing them
/// on to the pool. This avoids having async I/O threads being hogged by single long-running
/// network requests. It's basically a separate staging step for socket AsyncResults.
    struct SocketPollingThread
    {
        os::FastMutex mutex;
        AsyncResultQueue queue;
        os::Thread* thread;
        os::Event threadStartupAcknowledged;

#if IL2CPP_USE_SOCKET_MULTIPLEX_IO
        Sockets::MultiplexIO multiplexIO; // container class to allow access to multiplex io socket functions
#elif IL2CPP_TARGET_POSIX || IL2CPP_TARGET_WINDOWS
        /// On POSIX, we have no way to interrupt polls() with user APCs in a way that isn't prone
        /// to race conditions so what we do instead is create a pipe that we include in the poll()
        /// call and then write to that in order to interrupt an ongoing poll().
        ///
        /// On Windows, we used to do QueueUserAPC and throw an exception in order to interrupt poll(),
        /// however, that is not safe and corrupts memory/leaves dangling pointers to the stack as
        /// WinSock2 is not exception safe. So we do it in a similar way as POSIX, but instead of pipes
        /// we use sockets.

        enum
        {
            kMessageTerminate,
            kMessageNewAsyncResult
        };

#if IL2CPP_TARGET_POSIX
        typedef int PipeType;
#elif IL2CPP_TARGET_WINDOWS
        typedef SOCKET PipeType;
#endif

        PipeType readPipe;
        PipeType writePipe;

        static inline void WritePipe(PipeType pipe, char message)
        {
#if IL2CPP_TARGET_POSIX
            write(pipe, &message, 1);
#elif IL2CPP_TARGET_WINDOWS
            send(pipe, &message, 1, 0);
#endif
        }

        static inline char ReadPipe(PipeType pipe, char* message, int length)
        {
#if IL2CPP_TARGET_POSIX
            return read(pipe, message, length);
#elif IL2CPP_TARGET_WINDOWS
            return recv(pipe, message, length, 0);
#endif
        }

#endif

        SocketPollingThread()
            : threadStartupAcknowledged(true)
            , thread(NULL)
#if !IL2CPP_USE_SOCKET_MULTIPLEX_IO && (IL2CPP_TARGET_POSIX || IL2CPP_TARGET_WINDOWS)
            , readPipe(0)
            , writePipe(0)
#endif
        {
        }

        void QueueRequest(Il2CppAsyncResult* asyncResult)
        {
            // Put in queue.
            {
                os::FastAutoLock lock(&mutex);
                queue.push(asyncResult);
                gc::GarbageCollector::SetWriteBarrier((void**)&queue.back());
            }

            // Interrupt polling thread to pick up new request.
#if IL2CPP_USE_SOCKET_MULTIPLEX_IO
            multiplexIO.InterruptPoll(); // causes the current blocking poll to abort and recheck the queue
#elif IL2CPP_TARGET_POSIX || IL2CPP_TARGET_WINDOWS
            char message = static_cast<char>(kMessageNewAsyncResult);
            WritePipe(writePipe, message);
#endif
        }

        Il2CppAsyncResult* DequeueRequest()
        {
            os::FastAutoLock lock(&mutex);
            if (queue.empty())
                return NULL;
            Il2CppAsyncResult* asyncResult = queue.front();
            queue.pop();
            return asyncResult;
        }

        bool ResultReady()
        {
            os::FastAutoLock lock(&mutex);
            return !queue.empty();
        }

        void RunLoop();
        void Terminate();
    };

/// Data for a single pool of threads. We compartmentalize the pool to deal with async I/O and "normal" work
/// items separately.
    struct ThreadPoolCompartment
    {
        /// Human readable name of the compartment (mostly for debugging).
        const char* compartmentName;

        /// Minimum number of threads to be kept around. This is the number that the pool will
        /// actively try to maintain. Actual thread count can be less during startup of pool.
        /// NOTE: Can be changed without locking.
        int minThreads;

        /// Maximum number of threads the pool will ever have running at the same time.
        /// NOTE: Can be changed without locking.
        int maxThreads;

        /// Number of threads currently waiting for new work.
        /// NOTE: Changed atomically.
        volatile int32_t numIdleThreads;

        /// Semaphore that worker threads listen on.
        os::Semaphore signalThreads;

        /// Mutex for queue and threads vector.
        os::FastMutex mutex;

        /// Queue of pending items.
        /// NOTE: Requires lock on mutex.
        AsyncResultQueue queue;

        /// List of threads in the pool. Worker threads register and unregister themselves here.
        /// NOTE: Requires lock on mutex.
        std::vector<Il2CppThread*> threads;

        ThreadPoolCompartment()
            : compartmentName(NULL)
            , minThreads(0)
            , maxThreads(4)
            , signalThreads(0, std::numeric_limits<int32_t>::max())
            , numIdleThreads(0)
        {
        }

        void QueueWorkItem(Il2CppAsyncResult* asyncResult);
        Il2CppAsyncResult* DequeueNextWorkItem();

        int AttachThread(Il2CppThread* thread)
        {
            os::FastAutoLock lock(&mutex);
            threads.push_back(thread);
            return (int)threads.size();
        }

        void DetachThread(Il2CppThread* thread)
        {
            os::FastAutoLock lock(&mutex);
            threads.erase(std::remove(threads.begin(), threads.end(), thread),
                threads.end());
        }

        void SignalAllThreads()
        {
            signalThreads.Post((int32_t)threads.size());
        }

        void SpawnNewWorkerThread();
        void WorkerThreadRunLoop();

        enum
        {
            /// Time (in milliseconds) that a worker thread will wait before terminating after finding
            /// that the pool already has enough threads.
            kGracePeriodBeforeExtranenousWorkerThreadTerminates = 5000
        };
    };

    enum
    {
        kWorkerThreadPool,
        kAsyncIOPool,
        kNumThreadPoolCompartments
    };

    static ThreadPoolCompartment* g_ThreadPoolCompartments[kNumThreadPoolCompartments];
    static SocketPollingThread* g_SocketPollingThread;

#if IL2CPP_TARGET_POSIX && !IL2CPP_USE_SOCKET_MULTIPLEX_IO
    typedef pollfd NativePollRequest;
#else
    typedef os::PollRequest NativePollRequest;
#endif


    static Il2CppSocketAsyncResult* GetSocketAsyncResult(Il2CppAsyncResult* asyncResult)
    {
        ////TODO: assert
        return reinterpret_cast<Il2CppSocketAsyncResult*>(asyncResult->async_state);
    }

    static bool IsSocketAsyncOperation(Il2CppAsyncResult* asyncResult)
    {
        int32_t operation = GetSocketAsyncResult(asyncResult)->operation;
        return (operation >= AIO_OP_FIRST && operation <= AIO_OP_LAST);
    }

    static void InitPollRequest(NativePollRequest& request, Il2CppSocketAsyncResult* socketAsyncResult, os::SocketHandleWrapper& socketHandle)
    {
        request.revents = os::kPollFlagsNone;
#if IL2CPP_TARGET_POSIX && !IL2CPP_USE_SOCKET_MULTIPLEX_IO
        request.events = 0xFFFF;
#else
        request.events = os::kPollFlagsNone;

        switch (socketAsyncResult->operation)
        {
            case AIO_OP_ACCEPT:
            case AIO_OP_RECEIVE:
            case AIO_OP_RECV_JUST_CALLBACK:
            case AIO_OP_RECEIVEFROM:
            case AIO_OP_READPIPE:
                request.events |= os::kPollFlagsIn;
                break;

            case AIO_OP_SEND:
            case AIO_OP_SEND_JUST_CALLBACK:
            case AIO_OP_SENDTO:
            case AIO_OP_CONNECT:
                request.events |= os::kPollFlagsOut;
                break;

            default: // Should never happen
                IL2CPP_ASSERT(false && "Unrecognized socket async I/O operation");
                break;
        }
#endif

#if !NET_4_0
        // Acquire socket.
        socketHandle.Acquire(os::PointerToSocketHandle(reinterpret_cast<void*>(socketAsyncResult->handle)));
#else
        IL2CPP_ASSERT(false && "Todo .net 4");
#endif
        request.fd = socketHandle.IsValid() ? socketHandle.GetSocket()->GetDescriptor() : -1;
    }

    void SocketPollingThread::RunLoop()
    {
#if !IL2CPP_USE_SOCKET_MULTIPLEX_IO && !IL2CPP_TARGET_POSIX && !IL2CPP_TARGET_WINDOWS
        IL2CPP_ASSERT(false && "Platform has no SocketPollingThread mechanism. This function WILL deadlock.");
#endif

#if IL2CPP_TARGET_POSIX && !IL2CPP_USE_SOCKET_MULTIPLEX_IO
        const short kNativePollIn = POLLIN;
#else
        const os::PollFlags kNativePollIn = os::kPollFlagsIn;
#endif

        // List of poll requests that we pass to os::Socket::Poll().
        std::vector<NativePollRequest> pollRequests;

        // List of AsyncResults corresponding to pollRequests. Needs to be its own list as
        // this is memory that we need the GC to scan.
        AsyncResultVector asyncResults;

        // List of socket handles we're currently using. If destructed, will automatically
        // release all sockets.
        std::vector<os::SocketHandleWrapper> socketHandles;

#if !IL2CPP_USE_SOCKET_MULTIPLEX_IO && (IL2CPP_TARGET_POSIX || IL2CPP_TARGET_WINDOWS)
        {
            NativePollRequest pollRequest;
            pollRequest.fd = readPipe;
            pollRequest.events = kNativePollIn;
            pollRequest.revents = os::kPollFlagsNone;
            pollRequests.push_back(pollRequest);

            // Push back dummy values to asyncResults and socketHandles so their indices match pollrequest indices
            asyncResults.push_back(NULL);
            gc::GarbageCollector::SetWriteBarrier((void**)asyncResults.data(), asyncResults.size() * sizeof(Il2CppAsyncResult));
            socketHandles.push_back(os::SocketHandleWrapper());
        }
#endif

        // Let other threads know we're ready to take requests.
        threadStartupAcknowledged.Set();

        while (true)
        {
            // See if there's anything new in the queue.
            while (ResultReady())
            {
                // Grab next request.
                Il2CppAsyncResult* asyncResult = DequeueRequest();
                if (!asyncResult)
                    break;

                Il2CppSocketAsyncResult* socketAsyncResult = GetSocketAsyncResult(asyncResult);

                // Add socket handle.
                socketHandles.push_back(os::SocketHandleWrapper());
                os::SocketHandleWrapper& socketHandle = socketHandles.back();

                asyncResults.push_back(asyncResult);
                gc::GarbageCollector::SetWriteBarrier((void**)asyncResults.data(), asyncResults.size() * sizeof(Il2CppAsyncResult));

                // Add the request to the list.
                NativePollRequest pollRequest;
                InitPollRequest(pollRequest, socketAsyncResult, socketHandle);
                pollRequests.push_back(pollRequest);
            }

            // Poll the list.
#if IL2CPP_USE_SOCKET_MULTIPLEX_IO
            int32_t errorCode = 0;
            int32_t results = 0;
            multiplexIO.Poll(pollRequests, -1, &results, &errorCode);
#elif IL2CPP_TARGET_POSIX || IL2CPP_TARGET_WINDOWS
#if IL2CPP_TARGET_POSIX
            os::posix::Poll(pollRequests.data(), pollRequests.size(), -1);
#else
            int32_t result, error;
            os::Socket::Poll(pollRequests, -1, &result, &error);
#endif
            if (pollRequests[0].revents != os::kPollFlagsNone)
            {
                char message;
                if (ReadPipe(readPipe, &message, 1) == 1 && message == kMessageTerminate)
                    throw vm::Thread::NativeThreadAbortException();
            }
#endif

            // Go through our requests and see which ones we can forward, which ones are
            // obsolete, and which ones still need to be waited on.
#if IL2CPP_USE_SOCKET_MULTIPLEX_IO || (!IL2CPP_TARGET_POSIX && !IL2CPP_TARGET_WINDOWS)
            const size_t startIndex = 0;
#else
            const size_t startIndex = 1;
#endif
            for (size_t i = startIndex; i < pollRequests.size();)
            {
                // See if there's been some activity that allows us to forward the request
                // to the thread pool. We don't care what event(s) exactly happened on the
                // socket and the socket may even have been closed already. All we want is
                // to forward a socket to the pool as soon as there is some activity and then
                // have the normal processing chain sort out what kind of activity that was.
                if (pollRequests[i].revents)
                {
                    // Yes.
                    g_ThreadPoolCompartments[kAsyncIOPool]->QueueWorkItem(asyncResults[i]);

                    pollRequests.erase(pollRequests.begin() + i);
                    asyncResults.erase(asyncResults.begin() + i);
                    gc::GarbageCollector::SetWriteBarrier((void**)asyncResults.data(), asyncResults.size() * sizeof(Il2CppAsyncResult));
                    socketHandles.erase(socketHandles.begin() + i);
                }
                else
                {
                    ++i;
                }
            }
        }
    }

    static void FreeThreadHandle(void* data)
    {
        uint32_t handle = (uint32_t)(uintptr_t)data;
        gc::GCHandle::Free(handle);
    }

#if IL2CPP_TARGET_WINDOWS
    struct ConnectToSocketArgs
    {
        SOCKET s;
        const sockaddr_in* socketAddress;
    };

    static void ConnectToSocket(void* arg)
    {
        ConnectToSocketArgs* connectArgs = static_cast<ConnectToSocketArgs*>(arg);
        const int kRetryCount = 3;

        for (int i = 0; i < kRetryCount; i++)
        {
            int connectResult = connect(connectArgs->s, reinterpret_cast<const sockaddr*>(connectArgs->socketAddress), sizeof(sockaddr_in));

            if (connectResult == 0)
                return;

            Sleep(100);
        }

        IL2CPP_ASSERT(false && "Failed to connect to socket");
    }

#endif

    static void SocketPollingThreadEntryPoint(void* data)
    {
        SocketPollingThread* pollingThread = reinterpret_cast<SocketPollingThread*>(data);

        // Properly attach us to the VM and mark us as a background thread.
        Il2CppThread* managedThread = vm::Thread::Attach(il2cpp_domain_get());
        uint32_t handle = gc::GCHandle::New((Il2CppObject*)managedThread, true);
        vm::Thread::SetState(managedThread, kThreadStateBackground);
        managedThread->GetInternalThread()->handle->SetName("Socket I/O Polling Thread");
        managedThread->GetInternalThread()->handle->SetPriority(os::kThreadPriorityLow);
        managedThread->GetInternalThread()->handle->SetCleanupFunction(&FreeThreadHandle, (void*)(uintptr_t)handle);

        // The socket polling thread is not technically a worker pool thread but for all
        // intents and purposes it is part of the async I/O thread pool. It is important to
        // mark it as a thread pool thread so that the queueing logic correctly detects
        // when it is necessary to spin up a new thread to avoid deadlocks.
        managedThread->GetInternalThread()->threadpool_thread = true;

#if IL2CPP_USE_SOCKET_MULTIPLEX_IO
#elif IL2CPP_TARGET_POSIX
        int pipeHandles[2];
        if (::pipe(pipeHandles) != 0)
        {
            vm::Exception::Raise(vm::Exception::GetExecutionEngineException("Initialization socket polling thread for thread pool failed!"));
        }
        pollingThread->readPipe = pipeHandles[0];
        pollingThread->writePipe = pipeHandles[1];
#elif IL2CPP_TARGET_WINDOWS
        {
            SOCKET server = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            IL2CPP_ASSERT(server != INVALID_SOCKET);

            sockaddr_in serverAddress;
            int serverAddressLength = sizeof(serverAddress);

            ZeroMemory(&serverAddress, sizeof(serverAddress));
            serverAddress.sin_family = AF_INET;
            serverAddress.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");

            int bindResult = bind(server, reinterpret_cast<const sockaddr*>(&serverAddress), serverAddressLength);
            IL2CPP_ASSERT(bindResult == 0);

            int getsocknameResult = getsockname(server, reinterpret_cast<sockaddr*>(&serverAddress), &serverAddressLength);
            IL2CPP_ASSERT(getsocknameResult == 0);

            int listenResult = listen(server, 1);
            IL2CPP_ASSERT(listenResult == 0);

            pollingThread->writePipe = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            IL2CPP_ASSERT(pollingThread->writePipe != INVALID_SOCKET);

            os::Thread connectThread;
            ConnectToSocketArgs args = { pollingThread->writePipe, &serverAddress };
            connectThread.Run(ConnectToSocket, &args);

            sockaddr_in clientAddress = {};
            int clientAddressLength = sizeof(clientAddress);
            pollingThread->readPipe = accept(server, reinterpret_cast<sockaddr*>(&clientAddress), &clientAddressLength);
            if (pollingThread->readPipe == INVALID_SOCKET)
            {
                int error = WSAGetLastError();
                wchar_t errorMessage[512];

                FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM, NULL, error, 0, errorMessage, 256, NULL);
                OutputDebugStringW(errorMessage);
                OutputDebugStringW(L"\r\n");
                IL2CPP_ASSERT(false && "Failed to accept poll interrupt socket connection");
            }

            connectThread.Join();
            closesocket(server);
        }
#endif

        // Do work.
        try
        {
            pollingThread->RunLoop();
        }
        catch (Thread::NativeThreadAbortException)
        {
            // Runtime cleanup asked us to exit.
            // Cleanup pipes/sockets that we created

#if IL2CPP_USE_SOCKET_MULTIPLEX_IO
#elif IL2CPP_TARGET_POSIX
            close(pollingThread->readPipe);
            close(pollingThread->writePipe);
#elif IL2CPP_TARGET_WINDOWS
            closesocket(pollingThread->readPipe);
            closesocket(pollingThread->writePipe);
#endif
        }

        // Clean up.
        vm::Thread::Detach(managedThread);
    }

    static void SpawnSocketPollingThreadIfNeeded()
    {
        if (g_SocketPollingThread->thread)
            return;

        // Spawn thread.
        {
            os::FastAutoLock lock(&g_SocketPollingThread->mutex);
            // Double-check after lock to avoid race condition.
            if (!g_SocketPollingThread->thread)
            {
                g_SocketPollingThread->thread = new os::Thread();
                g_SocketPollingThread->thread->Run(SocketPollingThreadEntryPoint, g_SocketPollingThread);
            }
        }

        // Wait for thread to have started up so we can queue requests on it.
        g_SocketPollingThread->threadStartupAcknowledged.Wait();
    }

    void SocketPollingThread::Terminate()
    {
#if IL2CPP_TARGET_WINDOWS || IL2CPP_TARGET_POSIX
        if (!g_SocketPollingThread->thread)
            return;

#if !IL2CPP_USE_SOCKET_MULTIPLEX_IO
        WritePipe(writePipe, static_cast<char>(kMessageTerminate));
#endif

        g_SocketPollingThread->thread->Join();
#endif
    }

    static bool IsCurrentThreadAWorkerThread()
    {
        Il2CppThread* thread = vm::Thread::Current();
        return thread->GetInternalThread()->threadpool_thread;
    }

    void ThreadPoolCompartment::QueueWorkItem(Il2CppAsyncResult* asyncResult)
    {
        bool forceNewThread = false;
        // Put the item in the queue.
        {
            os::FastAutoLock lock(&mutex);
            queue.push(asyncResult);
            gc::GarbageCollector::SetWriteBarrier((void**)&queue.back());
            IL2CPP_ASSERT(numIdleThreads >= 0);
            if (queue.size() > static_cast<uint32_t>(numIdleThreads))
                forceNewThread = true;
        }

        // If all our worker threads are tied up and we have room to grow, spawn a
        // new worker thread. Also, if an item is queued from within a work item that
        // is currently being processed and we don't have idle threads, force a new
        // thread to be spawned even if we are at max capacity. This prevents deadlocks
        // if the code queuing the item then goes and waits on the item it just queued.
        IL2CPP_ASSERT(maxThreads >= 0);
        if (forceNewThread &&
            (threads.size() < static_cast<uint32_t>(maxThreads) || IsCurrentThreadAWorkerThread()))
        {
            SpawnNewWorkerThread();
        }
        else
        {
            // Signal existing thread.
            signalThreads.Post();
        }
    }

    Il2CppAsyncResult* ThreadPoolCompartment::DequeueNextWorkItem()
    {
        os::FastAutoLock lock(&mutex);

        if (queue.empty())
            return NULL;

        Il2CppAsyncResult* result = queue.front();
        queue.pop();

        return result;
    }

    static void WorkerThreadEntryPoint(void* data);
    void ThreadPoolCompartment::SpawnNewWorkerThread()
    {
        os::Thread* thread = new os::Thread();
        thread->Run(WorkerThreadEntryPoint, this);
    }

    static void HandleSocketAsyncOperation(Il2CppAsyncResult* asyncResult)
    {
        Il2CppSocketAsyncResult* socketAsyncResult = GetSocketAsyncResult(asyncResult);

        const icalls::System::System::Net::Sockets::SocketFlags flags =
            static_cast<icalls::System::System::Net::Sockets::SocketFlags>(socketAsyncResult->socket_flags);
        Il2CppArray* buffer = socketAsyncResult->buffer;
        int32_t offset = socketAsyncResult->offset;
        int32_t count = socketAsyncResult->size;

#if !NET_4_0
        // Perform send/receive.
        switch (socketAsyncResult->operation)
        {
            case AIO_OP_RECEIVE:
                socketAsyncResult->total = icalls::System::System::Net::Sockets::Socket::Receive
                        (socketAsyncResult->handle, buffer, offset, count, flags, &socketAsyncResult->error);
                break;

            case AIO_OP_SEND:
                socketAsyncResult->total = icalls::System::System::Net::Sockets::Socket::Send
                        (socketAsyncResult->handle, buffer, offset, count, flags, &socketAsyncResult->error);
                break;
        }
#else
        IL2CPP_ASSERT(false && "TO DO .net 4");
#endif
    }

    void ThreadPoolCompartment::WorkerThreadRunLoop()
    {
        bool waitingToTerminate = false;

        // Pump AsyncResults until we're killed.
        while (true)
        {
            // Grab next work item.
            Il2CppAsyncResult* asyncResult = DequeueNextWorkItem();
            if (!asyncResult)
            {
                // There's no work for us to do so decide what to do.

                // If we've exceeded the normal number of threads for the pool (minThreads),
                // wait around for a bit and then, if there is no work to do,
                // terminate.
                IL2CPP_ASSERT(minThreads >= 0);
                if (threads.size() > static_cast<uint32_t>(minThreads))
                {
                    if (waitingToTerminate)
                    {
                        // We've already waited so now is the time to go.
                        break;
                    }

                    waitingToTerminate = true;
                }

                // No item so wait for signal. We need to allow interruptions here as we don't yet
                // have proper abort support and the runtime currently uses interruptions to get
                // background threads to exit.
                os::Atomic::Increment(&numIdleThreads);
                if (waitingToTerminate)
                    signalThreads.Wait(kGracePeriodBeforeExtranenousWorkerThreadTerminates, true);
                else
                    signalThreads.Wait(true);
                os::Atomic::Decrement(&numIdleThreads);

                // Try again.
                continue;
            }
            waitingToTerminate = false;

            // See if it's a socket async call and if so, do whatever I/O we need to
            // do before invoking the delegate.
            Il2CppDelegate* delegate = asyncResult->async_delegate;
            const bool isSocketAsyncCall = IsSocketAsyncCall(delegate);
            if (isSocketAsyncCall)
                HandleSocketAsyncOperation(asyncResult);

            // Invoke delegate.
            Il2CppAsyncCall* asyncCall = asyncResult->object_data;
            Il2CppException* exception = NULL;
            uint32_t argsGCHandle = (uint32_t)((uintptr_t)asyncResult->data);
            Il2CppArray* args = (Il2CppArray*)gc::GCHandle::GetTarget(argsGCHandle);

            const uint8_t paramsCount = delegate->method->parameters_count;

            uint8_t byRefArgsCount = 0;
            for (uint8_t i = 0; i < paramsCount; ++i)
            {
                const Il2CppType* paramType = (Il2CppType*)delegate->method->parameters[i].parameter_type;
                if (paramType->byref)
                    ++byRefArgsCount;
            }

            void** byRefArgs = 0;
            if (byRefArgsCount > 0)
            {
                IL2CPP_OBJECT_SETREF(asyncCall, out_args, vm::Array::New(il2cpp_defaults.object_class, byRefArgsCount));
                byRefArgs = (void**)il2cpp_array_addr(asyncCall->out_args, Il2CppObject*, 0);
            }

            void** argsPtr = (void**)il2cpp_array_addr(args, Il2CppObject*, 0);
            void** params = (void**)IL2CPP_MALLOC(paramsCount * sizeof(void*));

            int byRefIndex = 0;
            for (uint8_t i = 0; i < paramsCount; ++i)
            {
                Il2CppType* paramType = (Il2CppType*)delegate->method->parameters[i].parameter_type;
                const Il2CppClass* paramClass = il2cpp_class_from_type(paramType);
                const bool isValueType = il2cpp_class_is_valuetype(paramClass);

                if (paramType->byref)
                {
                    if (isValueType)
                    {
                        // Value types are always boxed
                        il2cpp_array_setref(asyncCall->out_args, byRefIndex, il2cpp_object_unbox((Il2CppObject*)argsPtr[i]));
                        params[i] = byRefArgs[byRefIndex++];
                    }
                    else
                    {
                        il2cpp_array_setref(asyncCall->out_args, byRefIndex, argsPtr[i]);
                        params[i] = &byRefArgs[byRefIndex++];
                    }
                }
                else
                {
                    params[i] = isValueType
                        ? il2cpp_object_unbox((Il2CppObject*)argsPtr[i]) // Value types are always boxed
                        : argsPtr[i];
                }
            }

            Il2CppObject* result = il2cpp_runtime_invoke(delegate->method, delegate->target, params, &exception);

            IL2CPP_FREE(params);
            gc::GCHandle::Free(argsGCHandle);

            // Store result.
            IL2CPP_OBJECT_SETREF(asyncCall, res, result);
#if !NET_4_0
            IL2CPP_OBJECT_SETREF(asyncCall, msg, exception);
#else
            IL2CPP_OBJECT_SETREF(asyncCall, msg, (Il2CppMethodMessage*)exception);
#endif
            os::Atomic::FullMemoryBarrier();
            asyncResult->completed = true;

            // Invoke callback, if we have one.
            Il2CppDelegate* asyncCallback = asyncCall->cb_target;
            if (asyncCallback)
            {
                void* args[1] = { asyncResult };
                il2cpp_runtime_invoke(asyncCallback->method, asyncCallback->target, args, &exception);
#if !NET_4_0
                IL2CPP_OBJECT_SETREF(asyncCall, msg, exception);
#else
                IL2CPP_OBJECT_SETREF(asyncCall, msg, (Il2CppMethodMessage*)exception);
#endif
            }

            // Signal wait handle, if there's one.
            il2cpp_monitor_enter(&asyncResult->base);
            if (asyncResult->handle)
            {
                os::Handle* osHandle = vm::WaitHandle::GetPlatformHandle(asyncResult->handle);
                osHandle->Signal();
            }

            il2cpp_monitor_exit(&asyncResult->base);
        }
    }

    static void WorkerThreadEntryPoint(void* data)
    {
        ThreadPoolCompartment* compartment = reinterpret_cast<ThreadPoolCompartment*>(data);
        Il2CppThread* managedThread = NULL;

        // Do work.
        try
        {
            // Properly attach us to the VM and mark us as a background
            // worker thread.
            managedThread = vm::Thread::Attach(il2cpp_domain_get());
            uint32_t handle = gc::GCHandle::New((Il2CppObject*)managedThread, true);
            vm::Thread::SetState(managedThread, kThreadStateBackground);
            managedThread->GetInternalThread()->threadpool_thread = true;
            int threadCount = compartment->AttachThread(managedThread);

            // Configure OS thread.
            char name[2048];
            sprintf(name, "%s Thread #%i", compartment->compartmentName, threadCount - 1);
            managedThread->GetInternalThread()->handle->SetName(name);
            managedThread->GetInternalThread()->handle->SetPriority(os::kThreadPriorityLow);
            managedThread->GetInternalThread()->handle->SetCleanupFunction(&FreeThreadHandle, (void*)(uintptr_t)handle);
            compartment->WorkerThreadRunLoop();
        }
        catch (Thread::NativeThreadAbortException)
        {
            // Nothing to do. Runtime cleanup asked us to exit.
        }
        catch (Il2CppExceptionWrapper e)
        {
            // Only eat a ThreadAbortException, as it may have been thrown by the runtime
            // when there was managed code on the stack, but that managed code exited already.
            if (strcmp(e.ex->klass->name, "ThreadAbortException") != 0)
                throw;
        }

        // Clean up.
        if (managedThread)
        {
            compartment->DetachThread(managedThread);
            vm::Thread::Detach(managedThread);
        }
    }

    void ThreadPool::Initialize()
    {
#if NET_4_0
        NOT_SUPPORTED_IL2CPP(ThreadPool::Initialize, "vm::ThreadPool is not supported in .NET 4.5, use threadpool-ms instead");
#else
        g_SocketPollingThread = new SocketPollingThread();
        g_ThreadPoolCompartments[kWorkerThreadPool] = new ThreadPoolCompartment();
        g_ThreadPoolCompartments[kAsyncIOPool] = new ThreadPoolCompartment();

        g_ThreadPoolCompartments[kWorkerThreadPool]->compartmentName = "Worker Pool";
        g_ThreadPoolCompartments[kAsyncIOPool]->compartmentName = "Async I/O Pool";

        int numCores = os::Environment::GetProcessorCount();
        g_ThreadPoolCompartments[kWorkerThreadPool]->minThreads = numCores;
        g_ThreadPoolCompartments[kWorkerThreadPool]->maxThreads = 20 + THREADS_PER_CORE * numCores;
        g_ThreadPoolCompartments[kAsyncIOPool]->minThreads = g_ThreadPoolCompartments[kWorkerThreadPool]->minThreads;
        g_ThreadPoolCompartments[kAsyncIOPool]->maxThreads = g_ThreadPoolCompartments[kWorkerThreadPool]->maxThreads;
#endif
    }

    void ThreadPool::Shutdown()
    {
#if NET_4_0
        NOT_SUPPORTED_IL2CPP(ThreadPool::Shutdown, "vm::ThreadPool is not supported in .NET 4.5, use threadpool-ms instead");
#else
        g_SocketPollingThread->Terminate();
        // avoid cleaning up until we properly handle race condition from other threads queueing jobs
        // delete g_SocketPollingThread;
        // g_SocketPollingThread = NULL;
#endif
    }

    ThreadPool::Configuration ThreadPool::GetConfiguration()
    {
#if NET_4_0
        NOT_SUPPORTED_IL2CPP(ThreadPool::GetConfiguration, "vm::ThreadPool is not supported in .NET 4.5, use threadpool-ms instead");
        IL2CPP_UNREACHABLE;
        return Configuration();
#else
        Configuration configuration;

        configuration.availableThreads = g_ThreadPoolCompartments[kWorkerThreadPool]->numIdleThreads;
        configuration.availableAsyncIOThreads = g_ThreadPoolCompartments[kAsyncIOPool]->numIdleThreads;
        configuration.minThreads = g_ThreadPoolCompartments[kWorkerThreadPool]->minThreads;
        configuration.maxThreads = g_ThreadPoolCompartments[kWorkerThreadPool]->maxThreads;
        configuration.minAsyncIOThreads = g_ThreadPoolCompartments[kAsyncIOPool]->minThreads;
        configuration.maxAsyncIOThreads = g_ThreadPoolCompartments[kAsyncIOPool]->maxThreads;

        return configuration;
#endif
    }

    void ThreadPool::SetConfiguration(const Configuration& configuration)
    {
#if NET_4_0
        NOT_SUPPORTED_IL2CPP(ThreadPool::SetConfiguration, "vm::ThreadPool is not supported in .NET 4.5, use threadpool-ms instead");
#else
        IL2CPP_ASSERT(configuration.maxThreads >= configuration.minThreads && "Invalid configuration");
        IL2CPP_ASSERT(configuration.maxAsyncIOThreads >= configuration.minAsyncIOThreads && "Invalid configuration");
        IL2CPP_ASSERT(configuration.minThreads > 0 && "Invalid configuration");
        IL2CPP_ASSERT(configuration.minAsyncIOThreads > 0 && "Invalid configuration");
        IL2CPP_ASSERT(configuration.maxThreads > 0 && "Invalid configuration");
        IL2CPP_ASSERT(configuration.maxAsyncIOThreads > 0 && "Invalid configuration");

        g_ThreadPoolCompartments[kWorkerThreadPool]->minThreads = configuration.minThreads;
        g_ThreadPoolCompartments[kWorkerThreadPool]->maxThreads = configuration.maxThreads;
        g_ThreadPoolCompartments[kAsyncIOPool]->minThreads = configuration.minAsyncIOThreads;
        g_ThreadPoolCompartments[kAsyncIOPool]->maxThreads = configuration.maxAsyncIOThreads;

        // Get our worker threads to respond and exit, if necessary.
        // The method here isn't very smart and in fact won't even work reliably as idle worker
        // threads will steal the signal from threads that are currently busy.
        g_ThreadPoolCompartments[kWorkerThreadPool]->SignalAllThreads();
        g_ThreadPoolCompartments[kAsyncIOPool]->SignalAllThreads();
#endif
    }

    Il2CppAsyncResult* ThreadPool::Queue(Il2CppDelegate* delegate, void** params, Il2CppDelegate* asyncCallback, Il2CppObject* state)
    {
#if NET_4_0
        NOT_SUPPORTED_IL2CPP(ThreadPool::Queue, "vm::ThreadPool is not supported in .NET 4.5, use threadpool-ms instead");
        IL2CPP_UNREACHABLE;
        return NULL;
#else
        // Create AsyncCall.
        Il2CppAsyncCall* asyncCall = (Il2CppAsyncCall*)il2cpp::vm::Object::New(il2cpp_defaults.async_call_class);
        IL2CPP_OBJECT_SETREF(asyncCall, cb_target, asyncCallback);
        IL2CPP_OBJECT_SETREF(asyncCall, state, state);

        // Copy arguments.
        const uint8_t parametersCount = delegate->method->parameters_count;
        IL2CPP_ASSERT(!params[parametersCount] && "Expecting NULL as last element of the params array!");

        Il2CppArray* args = vm::Array::New(il2cpp_defaults.object_class, parametersCount);
        for (uint8_t i = 0; i < parametersCount; ++i)
            il2cpp_array_setref(args, i, params[i]);

        // Create AsyncResult.
        Il2CppAsyncResult* asyncResult = (Il2CppAsyncResult*)il2cpp::vm::Object::New(il2cpp_defaults.asyncresult_class);
        IL2CPP_OBJECT_SETREF(asyncResult, async_delegate, delegate);

        // NOTE: we store a GC handle here because .data is an IntPtr on the managed side and it won't be scanned by the GC.
        asyncResult->data = (void*)(uintptr_t)gc::GCHandle::New((Il2CppObject*)args, true);
        IL2CPP_OBJECT_SETREF(asyncResult, object_data, asyncCall);
        IL2CPP_OBJECT_SETREF(asyncResult, async_state, state);

        // See which compartment we should process this request with and whether we
        // need to first pipe it through the socket polling stage.
        if (IsProcessAsyncCall(delegate))
        {
            NOT_SUPPORTED_IL2CPP(AsyncReadHandler, "Async Process operations are not supported by Il2Cpp");
        }
        else if (IsSocketAsyncCall(delegate))
        {
            Il2CppSocketAsyncResult* socketAsyncResult = GetSocketAsyncResult(asyncResult);
            IL2CPP_OBJECT_SETREF(socketAsyncResult, ares, asyncResult);

            // Apparently, using poll/WSAPoll to listen for connect() isn't reliable, so
            // we bypass the polling stage in that case.
            // Also, Mono has some AIO_OP_xxx operations that are only defined in C# and are not
            // meant to go through the polling stage.
            if ((socketAsyncResult->operation == AIO_OP_CONNECT && socketAsyncResult->blocking)
                || !IsSocketAsyncOperation(asyncResult))
            {
                g_ThreadPoolCompartments[kAsyncIOPool]->QueueWorkItem(asyncResult);
            }
            else
            {
                // Give it to polling thread.
                SpawnSocketPollingThreadIfNeeded();
                g_SocketPollingThread->QueueRequest(asyncResult);
            }
        }
        else if (IsFileStreamAsyncCall(delegate))
        {
            g_ThreadPoolCompartments[kAsyncIOPool]->QueueWorkItem(asyncResult);
        }
        else
        {
            g_ThreadPoolCompartments[kWorkerThreadPool]->QueueWorkItem(asyncResult);
        }

        return asyncResult;
#endif
    }

    Il2CppObject* ThreadPool::Wait(Il2CppAsyncResult* asyncResult, void** outArgs)
    {
#if NET_4_0
        NOT_SUPPORTED_IL2CPP(ThreadPool::Wait, "vm::ThreadPool is not supported in .NET 4.5, use threadpool-ms instead");
        IL2CPP_UNREACHABLE;
        return NULL;
#else
        // Need lock already here to ensure only a single call to EndInvoke() gets to be the first one.
        il2cpp_monitor_enter(&asyncResult->base);

        // Throw if this result has already been waited on.
        if (asyncResult->endinvoke_called)
        {
            il2cpp_monitor_exit(&asyncResult->base);
            Exception::Raise(Exception::GetInvalidOperationException
                    ("Cannot call EndInvoke() repeatedly or concurrently on the same AsyncResult!"));
        }

        asyncResult->endinvoke_called = 1;

        // Wait if the AsyncResult hasn't yet been processed.
        if (!asyncResult->completed)
        {
            if (!asyncResult->handle)
                IL2CPP_OBJECT_SETREF(asyncResult, handle, WaitHandle::NewManualResetEvent(false));
            os::Handle* osHandle = WaitHandle::GetPlatformHandle(asyncResult->handle);

            il2cpp_monitor_exit(&asyncResult->base);

            osHandle->Wait();
        }
        else
            il2cpp_monitor_exit(&asyncResult->base);

        // Raise exception now if the delegate threw while we were running it on
        // the worker thread.
        Il2CppAsyncCall* asyncCall = asyncResult->object_data;
        if (asyncCall->msg != NULL)
            il2cpp_raise_exception((Il2CppException*)asyncCall->msg);

        // Copy out arguments.
        if (asyncCall->out_args != NULL)
        {
            void** outArgsPtr = (void**)il2cpp_array_addr(asyncCall->out_args, Il2CppObject*, 0);
            Il2CppDelegate* delegate = asyncResult->async_delegate;
            const uint8_t paramsCount = delegate->method->parameters_count;

            uint8_t index = 0;
            for (uint8_t i = 0; i < paramsCount; ++i)
            {
                Il2CppType* paramType = (Il2CppType*)delegate->method->parameters[i].parameter_type;
                const Il2CppClass* paramClass = il2cpp_class_from_type(paramType);

                if (!paramType->byref)
                    continue;

                const bool isValueType = il2cpp_class_is_valuetype(paramClass);
                if (isValueType)
                {
                    IL2CPP_ASSERT(paramClass->native_size > 0 && "EndInvoke: Invalid native_size found when trying to copy a value type in the out_args.");

                    // NOTE(gab): in case of value types, we need to copy the data over.
                    memcpy(outArgs[index], outArgsPtr[index], paramClass->native_size);
                }
                else
                {
                    *((void**)outArgs[index]) = outArgsPtr[index];
                }

                ++index;
            }
        }

        return asyncResult->object_data->res;
#endif
    }
} /* namespace vm */
} /* namespace il2cpp */
