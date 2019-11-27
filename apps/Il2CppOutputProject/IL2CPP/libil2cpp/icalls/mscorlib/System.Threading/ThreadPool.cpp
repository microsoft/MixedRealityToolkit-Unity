#include "il2cpp-config.h"

#include "icalls/mscorlib/System.Threading/ThreadPool.h"
#include "os/Environment.h"
#include "vm/Exception.h"
#include "vm/ThreadPool.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
namespace Threading
{
    void ThreadPool::GetMaxThreads(int32_t* workerThreads, int32_t* completionPortThreads)
    {
        vm::ThreadPool::Configuration configuration = vm::ThreadPool::GetConfiguration();

        if (workerThreads)
            *workerThreads = configuration.maxThreads;
        if (completionPortThreads)
            *completionPortThreads = configuration.maxAsyncIOThreads;
    }

    void ThreadPool::GetMinThreads(int32_t* workerThreads, int32_t* completionPortThreads)
    {
        vm::ThreadPool::Configuration configuration = vm::ThreadPool::GetConfiguration();

        if (workerThreads)
            *workerThreads = configuration.minThreads;
        if (completionPortThreads)
            *completionPortThreads = configuration.minAsyncIOThreads;
    }

    bool ThreadPool::SetMinThreads(int32_t workerThreads, int32_t completionPortThreads)
    {
        vm::ThreadPool::Configuration configuration = vm::ThreadPool::GetConfiguration();

        int numCoresAvailable = os::Environment::GetProcessorCount();
        if (workerThreads < numCoresAvailable || workerThreads > configuration.maxThreads)
            return false;
        if (completionPortThreads < numCoresAvailable || completionPortThreads > configuration.maxAsyncIOThreads)
            return false;

        configuration.minThreads = workerThreads;
        configuration.minAsyncIOThreads = completionPortThreads;

        vm::ThreadPool::SetConfiguration(configuration);

        return true;
    }

    bool ThreadPool::SetMaxThreads(int32_t workerThreads, int32_t completionPortThreads)
    {
        vm::ThreadPool::Configuration configuration = vm::ThreadPool::GetConfiguration();

        if (workerThreads < configuration.minThreads)
            return false;
        if (completionPortThreads < configuration.minAsyncIOThreads)
            return false;

        configuration.maxThreads = workerThreads;
        configuration.maxAsyncIOThreads = completionPortThreads;

        vm::ThreadPool::SetConfiguration(configuration);

        return true;
    }

    void ThreadPool::GetAvailableThreads(int32_t* workerThreads, int32_t* completionPortThreads)
    {
        vm::ThreadPool::Configuration configuration = vm::ThreadPool::GetConfiguration();

        if (workerThreads)
            *workerThreads = configuration.availableThreads;
        if (completionPortThreads)
            *completionPortThreads = configuration.availableAsyncIOThreads;
    }
} /* namespace Threading */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
