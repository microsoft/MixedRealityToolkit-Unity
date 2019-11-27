#include "il2cpp-config.h"
#include "vm/Assembly.h"
#include "vm/AssemblyName.h"
#include "vm/MetadataCache.h"
#include "vm/Runtime.h"
#include "vm-utils/VmStringUtils.h"
#include "il2cpp-tabledefs.h"
#include "il2cpp-class-internals.h"

#include <vector>
#include <string>

namespace il2cpp
{
namespace vm
{
    static AssemblyVector s_Assemblies;

    AssemblyVector* Assembly::GetAllAssemblies()
    {
        return &s_Assemblies;
    }

    const Il2CppAssembly* Assembly::GetLoadedAssembly(const char* name)
    {
        for (AssemblyVector::const_iterator assembly = s_Assemblies.begin(); assembly != s_Assemblies.end(); ++assembly)
        {
            if (strcmp((*assembly)->aname.name, name) == 0)
                return *assembly;
        }

        return NULL;
    }

    Il2CppImage* Assembly::GetImage(const Il2CppAssembly* assembly)
    {
        return assembly->image;
    }

    void Assembly::GetReferencedAssemblies(const Il2CppAssembly* assembly, AssemblyNameVector* target)
    {
        for (int32_t sourceIndex = 0; sourceIndex < assembly->referencedAssemblyCount; sourceIndex++)
        {
            int32_t indexIntoMainAssemblyTable = MetadataCache::GetReferenceAssemblyIndexIntoAssemblyTable(assembly->referencedAssemblyStart + sourceIndex);
            const Il2CppAssembly* refAssembly = MetadataCache::GetAssemblyFromIndex(indexIntoMainAssemblyTable);

            target->push_back(&refAssembly->aname);
        }
    }

    static bool ends_with(const char *str, const char *suffix)
    {
        if (!str || !suffix)
            return false;

        const size_t lenstr = strlen(str);
        const size_t lensuffix = strlen(suffix);
        if (lensuffix >  lenstr)
            return false;

        return strncmp(str + lenstr - lensuffix, suffix, lensuffix) == 0;
    }

    const Il2CppAssembly* Assembly::Load(const char* name)
    {
        const size_t len = strlen(name);
        utils::VmStringUtils::CaseInsensitiveComparer comparer;

        for (AssemblyVector::const_iterator assembly = s_Assemblies.begin(); assembly != s_Assemblies.end(); ++assembly)
        {
            if (comparer(name, (*assembly)->aname.name))
                return *assembly;
        }

        if (!ends_with(name, ".dll") && !ends_with(name, ".exe"))
        {
            char *tmp = new char[len + 5];

            memset(tmp, 0, len + 5);

            memcpy(tmp, name, len);
            memcpy(tmp + len, ".dll", 4);

            const Il2CppAssembly* result = Load(tmp);

            if (!result)
            {
                memcpy(tmp + len, ".exe", 4);
                result = Load(tmp);
            }

            delete[] tmp;

            return result;
        }
        else
        {
            for (AssemblyVector::const_iterator assembly = s_Assemblies.begin(); assembly != s_Assemblies.end(); ++assembly)
            {
                if (comparer(name, (*assembly)->image->name))
                    return *assembly;
            }

            return NULL;
        }
    }

    void Assembly::Register(const Il2CppAssembly* assembly)
    {
        s_Assemblies.push_back(assembly);
    }

    void Assembly::Initialize()
    {
    }
} /* namespace vm */
} /* namespace il2cpp */
