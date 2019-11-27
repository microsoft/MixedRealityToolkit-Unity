#include "os/c-api/il2cpp-config-platforms.h"

#if IL2CPP_TARGET_ANDROID

#include "os/File.h"
#include "os/TimeZoneInfo.h"
#include "os/c-api/Allocator.h"
#include "utils/MemoryMappedFile.h"
#include "mono-structs.h"
#include <sys/system_properties.h>
#include <stdlib.h>
#include <string>

using il2cpp::os::FileHandle;

#if IL2CPP_BYTE_ORDER == IL2CPP_BIG_ENDIAN
#define CONVERT_ENDIANNESS(value) value
#else
#define CONVERT_ENDIANNESS(value) (((value >> 24) & 0xFF) | ((value >> 8) & 0xFF00) | ((value << 8) & 0xFF0000) | ((value << 24)))
#endif

namespace il2cpp
{
namespace os
{
#pragma pack(push, p1, 1)
    struct AndroidTzDataHeader
    {
        char signature[12];
        uint32_t indexOffset;
        uint32_t dataOffset;
        uint32_t zoneTabOffset;
    };

    struct AndroidTzDataEntry
    {
        char id[40];
        uint32_t byteOffset;
        uint32_t length;
        int32_t rawUtcOffset;
    };
#pragma pack(pop, p1)

    FileHandle* OpenAndroidConfig()
    {
        std::string config_files[] =
        {
            getenv("ANDROID_DATA") + std::string("/misc/zoneinfo/tzdata"),
            getenv("ANDROID_ROOT") + std::string("/usr/share/zoneinfo/tzdata")
        };

        for (int i = 0; i < sizeof(config_files) / sizeof(config_files[0]); i++)
        {
            int err = 0;
            FileHandle* hdl = File::Open(config_files[i], kFileModeOpen, kFileAccessRead, kFileShareRead, 0, &err);

            if (err)
                continue;

            return hdl;
        }
        return NULL;
    }

    bool TimeZoneInfo::UsePalForTimeZoneInfo()
    {
        return true;
    }

    void* TimeZoneInfo::GetTimeZoneIDs()
    {
        FileHandle* hdl = OpenAndroidConfig();

        if (!hdl)
            return NULL;

        AndroidTzDataHeader* dataHeader = (AndroidTzDataHeader*)utils::MemoryMappedFile::Map(hdl);

        uint32_t dataHeaderIndexOffset = CONVERT_ENDIANNESS(dataHeader->indexOffset);
        uint32_t dataHeaderDataOffset = CONVERT_ENDIANNESS(dataHeader->dataOffset);
        uint32_t dataEntrySize =  dataHeaderDataOffset - dataHeaderIndexOffset;
        uint32_t dataEntryCount = dataEntrySize / sizeof(AndroidTzDataEntry);

        VoidPtrArray timezoneIDsArray;

        for (int i = 0; i < dataEntryCount; i++)
        {
            uint32_t currentEntryOffset =  sizeof(AndroidTzDataEntry) * i;
            AndroidTzDataEntry* currentEntry = (AndroidTzDataEntry*)((char*)dataHeader + dataHeaderIndexOffset + currentEntryOffset);
            char* currentName = Allocator::CopyToAllocatedStringBuffer(currentEntry->id);
            timezoneIDsArray.push_back(currentName);
        }

        utils::MemoryMappedFile::Unmap(dataHeader);

        int err = 0;
        File::Close(hdl, &err);

        return (void*)void_ptr_array_to_gptr_array(timezoneIDsArray);
    }

    bool TimeZoneInfo::GetLocalTimeZoneData(void** nativeRawData, char** nativeID, int* size)
    {
        *nativeRawData = NULL;
        *nativeID = NULL;
        *size = 0;

        const int LOCAL_NAME_LENGTH = PROP_VALUE_MAX * sizeof(char) + 1;
        char localTimeZoneName[LOCAL_NAME_LENGTH];
        memset(localTimeZoneName, 0, LOCAL_NAME_LENGTH);

        if (!__system_property_get("persist.sys.timezone", localTimeZoneName))
            return false;

        if (!GetTimeZoneDataForID(localTimeZoneName, nativeRawData, size))
            return false;

        *nativeID = Allocator::CopyToAllocatedStringBuffer(localTimeZoneName);

        return true;
    }

    bool TimeZoneInfo::GetTimeZoneDataForID(char* id, void** nativeRawData, int* size)
    {
        *nativeRawData = NULL;
        *size = 0;

        int err = 0;
        FileHandle* hdl = OpenAndroidConfig();

        if (!hdl)
            return false;

        AndroidTzDataHeader* dataHeader = (AndroidTzDataHeader*)utils::MemoryMappedFile::Map(hdl);

        uint32_t dataHeaderIndexOffset = CONVERT_ENDIANNESS(dataHeader->indexOffset);
        uint32_t dataHeaderDataOffset = CONVERT_ENDIANNESS(dataHeader->dataOffset);
        uint32_t dataEntrySize =  dataHeaderDataOffset - dataHeaderIndexOffset;
        uint32_t dataEntryCount = dataEntrySize / sizeof(AndroidTzDataEntry);

        AndroidTzDataEntry* dataEntryForName = NULL;
        for (int i = 0; i < dataEntryCount; i++)
        {
            uint32_t currentEntryOffset =  sizeof(AndroidTzDataEntry) * i;
            AndroidTzDataEntry* currentEntry = (AndroidTzDataEntry*)((char*)dataHeader + dataHeaderIndexOffset + currentEntryOffset);
            if (strcmp((char*)currentEntry->id, id) == 0)
            {
                dataEntryForName = currentEntry;
                break;
            }
        }

        bool foundEntry = dataEntryForName != NULL;

        if (foundEntry)
        {
            void* tzData = (char*)dataHeader + dataHeaderDataOffset + CONVERT_ENDIANNESS(dataEntryForName->byteOffset);

            *size = CONVERT_ENDIANNESS(dataEntryForName->length);
            *nativeRawData = Allocator::Allocate(*size);
            memcpy(*nativeRawData, tzData, *size);
        }

        utils::MemoryMappedFile::Unmap(dataHeader);
        File::Close(hdl, &err);

        return foundEntry;
    }
}
}

#endif
