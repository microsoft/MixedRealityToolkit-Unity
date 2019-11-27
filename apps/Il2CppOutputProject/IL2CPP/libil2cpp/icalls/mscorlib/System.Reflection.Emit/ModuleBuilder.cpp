#include "il2cpp-config.h"
#include "icalls/mscorlib/System.Reflection.Emit/ModuleBuilder.h"
#include "vm/Exception.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
namespace Reflection
{
namespace Emit
{
    void ModuleBuilder::basic_init(Il2CppReflectionModuleBuilder*)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::basic_init);
    }

    void ModuleBuilder::RegisterToken(Il2CppReflectionModuleBuilder*, Il2CppObject*, int)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::RegisterToken);
    }

    void ModuleBuilder::set_wrappers_type(Il2CppReflectionModuleBuilder*, Il2CppReflectionType*)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::set_wrappers_type);
    }

    Il2CppReflectionType * ModuleBuilder::create_modified_type(Il2CppReflectionTypeBuilder*, Il2CppString*)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::create_modified_type);
        return NULL;
    }

    int32_t ModuleBuilder::getUSIndex(Il2CppReflectionModuleBuilder*, Il2CppString* str)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::getUSIndex);
        return 0;
    }

    int32_t ModuleBuilder::getToken(Il2CppReflectionModuleBuilder*, Il2CppObject * obj)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::getToken);
        return 0;
    }

    int32_t ModuleBuilder::getMethodToken(Il2CppReflectionModuleBuilder*, Il2CppReflectionMethod*, Il2CppArray*)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::getMethodToken);
        return 0;
    }

    void ModuleBuilder::build_metadata(Il2CppReflectionModuleBuilder*)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::build_metadata);
    }

    void ModuleBuilder::WriteToFile(Il2CppReflectionModuleBuilder*, intptr_t)
    {
        NOT_SUPPORTED_SRE(ModuleBuilder::WriteToFile);
    }

#if NET_4_0
    int32_t ModuleBuilder::getMethodToken40(Il2CppObject* mb, Il2CppObject* method, Il2CppArray* opt_param_types)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(ModuleBuilder::getMethodToken40);
        IL2CPP_UNREACHABLE;
        return 0;
    }

    int32_t ModuleBuilder::getToken40(Il2CppObject* mb, Il2CppObject* obj, bool create_open_instance)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(ModuleBuilder::getToken40);
        IL2CPP_UNREACHABLE;
        return 0;
    }

    Il2CppObject* ModuleBuilder::GetRegisteredToken(Il2CppObject* _this, int32_t token)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL(ModuleBuilder::GetRegisteredToken);
        IL2CPP_UNREACHABLE;
        return NULL;
    }

#endif
} /* namespace Emit */
} /* namespace Reflection */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
