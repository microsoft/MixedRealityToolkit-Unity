#include "il2cpp-config.h"
#include "os/Path.h"
#include "os/Environment.h"
#include "utils/Runtime.h"
#include "utils/PathUtils.h"
#include "utils/StringUtils.h"
#include "utils/Environment.h"


#if defined(RUNTIME_MONO)
extern "C"
{
#include <mono/metadata/unity-utils.h>
}
#endif

namespace il2cpp
{
namespace utils
{
    NORETURN void Runtime::Abort()
    {
        os::Environment::Abort();
    }

    static std::string s_DataDirFallback;

#if !defined(RUNTIME_MONO)
    static std::string s_DataDir;

    void Runtime::SetDataDir(const char *path)
    {
        s_DataDir = path;
    }

#endif

    std::string Runtime::GetDataDir()
    {
#if !IL2CPP_DOTS_WITHOUT_DEBUGGER
#if defined(RUNTIME_MONO)
        // use explicit value if set
        char* dataDirCS = mono_unity_get_data_dir();
        if (dataDirCS)
        {
            std::string dataDir(dataDirCS, strlen(dataDirCS));
            if (dataDir.size() > 0)
                return dataDir;
        }
#else
        // use explicit value if set
        if (s_DataDir.size() > 0)
            return s_DataDir;
#endif

        std::string executablePath = os::Path::GetExecutablePath();
        if (!executablePath.empty())
            return PathUtils::Combine(PathUtils::DirectoryName(executablePath), StringView<char>("Data"));

        if (s_DataDirFallback.size() == 0 && Environment::GetNumMainArgs() > 0)
        {
            std::string main = StringUtils::Utf16ToUtf8(Environment::GetMainArgs()[0]);
            s_DataDirFallback = PathUtils::DirectoryName(main);
        }

        return s_DataDirFallback;
#else
        return std::string();
#endif
    }
} // utils
} // il2cpp
