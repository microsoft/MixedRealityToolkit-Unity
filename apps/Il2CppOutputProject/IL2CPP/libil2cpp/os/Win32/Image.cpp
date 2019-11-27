#include "il2cpp-config.h"

#if IL2CPP_TARGET_WINDOWS

#include "WindowsHeaders.h"

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

namespace il2cpp
{
namespace os
{
namespace Image
{
#if IL2CPP_PLATFORM_SUPPORTS_CUSTOM_SECTIONS
    static void* s_ManagedSectionStart = NULL;
    static void* s_ManagedSectionEnd = NULL;

    static void InitializeManagedSection()
    {
        PIMAGE_NT_HEADERS ntHeaders = (PIMAGE_NT_HEADERS)(((char*)&__ImageBase) + __ImageBase.e_lfanew);
        PIMAGE_SECTION_HEADER sectionHeader = (PIMAGE_SECTION_HEADER)((char*)&ntHeaders->OptionalHeader + ntHeaders->FileHeader.SizeOfOptionalHeader);
        for (int i = 0; i < ntHeaders->FileHeader.NumberOfSections; i++)
        {
            if (strncmp(IL2CPP_BINARY_SECTION_NAME, (char*)sectionHeader->Name, IMAGE_SIZEOF_SHORT_NAME) == 0)
            {
                s_ManagedSectionStart = (char*)&__ImageBase + sectionHeader->VirtualAddress;
                s_ManagedSectionEnd = (char*)s_ManagedSectionStart + sectionHeader->Misc.VirtualSize;
            }
            sectionHeader++;
        }
    }

#endif

    void Initialize()
    {
#if IL2CPP_PLATFORM_SUPPORTS_CUSTOM_SECTIONS
        InitializeManagedSection();
#endif
    }

    void* GetImageBase()
    {
        return &__ImageBase;
    }

#if IL2CPP_PLATFORM_SUPPORTS_CUSTOM_SECTIONS
    bool IsInManagedSection(void* ip)
    {
        IL2CPP_ASSERT(s_ManagedSectionStart != NULL && s_ManagedSectionEnd != NULL);
        return s_ManagedSectionStart <= ip && ip <= s_ManagedSectionEnd;
    }

#endif
}
}
}

#endif
