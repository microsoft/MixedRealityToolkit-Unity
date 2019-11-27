#include "il2cpp-config.h"

#include <string>

#include "icalls/System/System.ComponentModel/Win32Exception.h"

#include "vm/String.h"
#include "os/ErrorCodes.h"
#include "os/Messages.h"

#include "il2cpp-class-internals.h"
#include "il2cpp-object-internals.h"
#include "utils/StringUtils.h"

namespace il2cpp
{
namespace icalls
{
namespace System
{
namespace System
{
namespace ComponentModel
{
    Il2CppString *Win32Exception::W32ErrorMessage(int32_t code)
    {
        std::string message = os::Messages::FromCode((os::ErrorCode)code);

        if (message.size() == 0)
        {
            // Note: this is a special case only for il2cpp. We might not want to keep
            // this in the future, but helps with debugging and testing for now.
            message = utils::StringUtils::Printf("Win32 Error message: %d (message string not found in the message table)", code);
        }

        return vm::String::New(message.c_str());
    }
} /* namespace ComponentModel */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
