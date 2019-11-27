#include "LibraryLoader.h"
#include "os/LibraryLoader.h"
#include "utils/StringUtils.h"

namespace il2cpp
{
namespace vm
{
    static Il2CppSetFindPlugInCallback s_FindPluginCallback = NULL;

    void* LibraryLoader::LoadDynamicLibrary(il2cpp::utils::StringView<Il2CppNativeChar> nativeDynamicLibrary)
    {
        if (s_FindPluginCallback)
        {
            StringViewAsNullTerminatedStringOf(Il2CppNativeChar, nativeDynamicLibrary, libraryName);
            const Il2CppNativeChar* modifiedLibraryName = s_FindPluginCallback(libraryName);

            if (modifiedLibraryName != libraryName)
            {
                utils::StringView<Il2CppNativeChar> modifiedDynamicLibrary(modifiedLibraryName, utils::StringUtils::StrLen(modifiedLibraryName));
                return os::LibraryLoader::LoadDynamicLibrary(modifiedDynamicLibrary);
            }
        }

        return os::LibraryLoader::LoadDynamicLibrary(nativeDynamicLibrary);
    }

    void LibraryLoader::SetFindPluginCallback(Il2CppSetFindPlugInCallback method)
    {
        IL2CPP_ASSERT(method == NULL || s_FindPluginCallback == NULL);
        s_FindPluginCallback = method;
    }
} /* namespace vm */
} /* namespace il2cpp */
