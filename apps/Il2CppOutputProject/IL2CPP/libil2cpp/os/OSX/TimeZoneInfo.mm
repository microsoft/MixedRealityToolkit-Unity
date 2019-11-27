#include "os/c-api/il2cpp-config-platforms.h"

#if IL2CPP_TARGET_DARWIN
#import <Foundation/Foundation.h>

#include "os/c-api/Allocator.h"
#include "os/TimeZoneInfo.h"
#include "mono-structs.h"

namespace il2cpp
{
namespace os
{
    bool TimeZoneInfo::UsePalForTimeZoneInfo()
    {
        return true;
    }

    void* TimeZoneInfo::GetTimeZoneIDs()
    {
        @autoreleasepool
        {
            NSArray* nsarray = [NSTimeZone knownTimeZoneNames];
            int count = nsarray.count;

            VoidPtrArray timezoneIDsArray;
            for (int i = 0; i < count; i++)
            {
                NSString* zone = [nsarray objectAtIndex: i];
                char* zoneName = Allocator::CopyToAllocatedStringBuffer(zone.UTF8String);
                timezoneIDsArray.push_back(zoneName);
            }
            return (void*)void_ptr_array_to_gptr_array(timezoneIDsArray);
        }
    }

    bool TimeZoneInfo::GetLocalTimeZoneData(void** nativeRawData, char** nativeID, int* size)
    {
        @autoreleasepool
        {
            *nativeRawData = NULL;
            *nativeID = NULL;
            *size = 0;

            NSTimeZone* TZ = [NSTimeZone localTimeZone];
            NSString* TzName = [TZ name];

            char* localTimeZoneName = (char*)TzName.UTF8String;
            if (!GetTimeZoneDataForID(localTimeZoneName, nativeRawData, size))
            {
                return false;
            }

            *nativeID = Allocator::CopyToAllocatedStringBuffer(localTimeZoneName);

            return true;
        }
    }

    bool TimeZoneInfo::GetTimeZoneDataForID(char* id, void** nativeRawData, int* size)
    {
        @autoreleasepool
        {
            *nativeRawData = NULL;
            *size = 0;

            NSString *timeZoneName = [[NSString alloc] initWithUTF8String: id];
            NSTimeZone* timeZone = [[NSTimeZone alloc] initWithName: timeZoneName];

            if (timeZone)
            {
                NSData *timeZoneData = [timeZone data];
                *size = [timeZoneData length];
                *nativeRawData = Allocator::Allocate(*size);
                memcpy(*nativeRawData, timeZoneData.bytes, *size);
                return true;
            }
            return false;
        }
    }
}
}

#endif
