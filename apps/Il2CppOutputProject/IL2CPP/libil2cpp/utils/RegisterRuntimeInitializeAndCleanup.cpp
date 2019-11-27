#include "RegisterRuntimeInitializeAndCleanup.h"
#include <set>
#include <cstddef>

namespace il2cpp
{
namespace utils
{
    typedef std::set<RegisterRuntimeInitializeAndCleanup::CallbackFunction> RegistrationCallbackSet;

    static RegistrationCallbackSet* _registrationCallbacks = NULL;

    RegisterRuntimeInitializeAndCleanup::RegisterRuntimeInitializeAndCleanup(CallbackFunction Initialize, CallbackFunction Cleanup, int order)
    {
        if (!_registrationCallbacks)
            _registrationCallbacks = new RegistrationCallbackSet();
        (*_registrationCallbacks).insert(Initialize);
    }

    void RegisterRuntimeInitializeAndCleanup::ExecuteInitializations()
    {
        if (_registrationCallbacks == NULL)
            return;

        for (RegistrationCallbackSet::iterator iter = (*_registrationCallbacks).begin(); iter != (*_registrationCallbacks).end(); ++iter)
        {
            (*iter)();
        }
    }

    void RegisterRuntimeInitializeAndCleanup::ExecuteCleanup()
    {
        IL2CPP_ASSERT(0);
    }
} /* namespace vm */
} /* namespace utils */
