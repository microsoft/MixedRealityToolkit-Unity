#pragma once

#include <stdint.h>
#include "il2cpp-object-internals.h"
#include "il2cpp-config.h"

struct Il2CppObject;
struct Il2CppDelegate;
struct Il2CppReflectionType;
struct Il2CppReflectionMethod;
struct Il2CppReflectionField;
struct Il2CppArray;
struct Il2CppException;
struct Il2CppReflectionModule;
struct Il2CppAssembly;
struct Il2CppAssemblyName;
struct Il2CppAppDomain;

namespace il2cpp
{
namespace icalls
{
namespace System
{
namespace System
{
namespace IO
{
    typedef int32_t InotifyMask;

    class LIBIL2CPP_CODEGEN_API InotifyWatcher
    {
    public:
        static intptr_t RemoveWatch(intptr_t fd, int32_t wd);
        static int32_t AddWatch(intptr_t fd, Il2CppString* name, InotifyMask mask);
        static intptr_t GetInotifyInstance();
    };
} /* namespace IO */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
