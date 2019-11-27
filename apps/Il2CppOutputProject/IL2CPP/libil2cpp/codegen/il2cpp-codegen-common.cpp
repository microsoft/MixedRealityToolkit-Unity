#include "il2cpp-config.h"

#include "utils/Runtime.h"
#include "gc/GarbageCollector.h"

// This function exists to help with generation of callstacks for exceptions
// on iOS and MacOS x64 with clang 6.0 (newer versions of clang don't have this
// problem on x64). There we call the backtrace function, which does not play nicely
// with NORETURN, since the compiler eliminates the method prologue code setting up
// the address of the return frame (which makes sense). So on iOS we need to make
// the NORETURN define do nothing, then we use this dummy method which has the
// attribute for clang on iOS defined to prevent clang compiler errors for
// method that end by throwing a managed exception.
REAL_NORETURN IL2CPP_NO_INLINE void il2cpp_codegen_no_return()
{
    IL2CPP_UNREACHABLE;
}

REAL_NORETURN void il2cpp_codegen_abort()
{
    il2cpp::utils::Runtime::Abort();
    il2cpp_codegen_no_return();
}

#if IL2CPP_ENABLE_WRITE_BARRIERS
void Il2CppCodeGenWriteBarrier(void** targetAddress, void* object)
{
    il2cpp::gc::GarbageCollector::SetWriteBarrier(targetAddress);
}

#endif
