#include "il2cpp-config.h"

#include "icalls/System/System.Configuration/InternalConfigurationHost.h"
#include "vm/String.h"
#include "vm/Exception.h"
#include "vm/Runtime.h"

namespace il2cpp
{
namespace icalls
{
namespace System
{
namespace System
{
namespace Configuration
{
    Il2CppString* InternalConfigurationHost::get_bundled_machine_config()
    {
        const char *config_xml = vm::Runtime::GetBundledMachineConfig();

        if (config_xml == 0)
            return NULL;

        return vm::String::NewWrapper(config_xml);
    }
} /* namespace Configuration */
} /* namespace System */
} /* namespace System */
} /* namespace icalls */
} /* namespace il2cpp */
