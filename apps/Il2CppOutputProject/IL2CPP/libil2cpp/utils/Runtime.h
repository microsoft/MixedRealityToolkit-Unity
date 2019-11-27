#pragma once

#include <string>

namespace il2cpp
{
namespace utils
{
    class LIBIL2CPP_CODEGEN_API Runtime
    {
    public:
        static NORETURN void Abort();
#if !defined(RUNTIME_MONO)
        static void SetDataDir(const char *path);
#endif
        static std::string GetDataDir();
    };
} // utils
} // il2cpp
