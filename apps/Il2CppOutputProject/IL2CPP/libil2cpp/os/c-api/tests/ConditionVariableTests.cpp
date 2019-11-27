#if ENABLE_UNIT_TESTS
#if NET_4_0

#include "UnitTest++.h"

#include "../ConditionVariable-c-api.h"
#include "../../ConditionVariable.h"
#include "../../Thread.h"

SUITE(ConditionVariable)
{
    struct ConditionVariableFixture
    {
        ConditionVariableFixture()
        {
            il2cpp::os::Thread::Init();
            localObject = UnityPalConditionVariableNew();
            localFastMutex = NULL;
            localFastMutex = UnityPalFastMutexNew();
        }

        ~ConditionVariableFixture()
        {
            UnityPalConditionVariableDelete(localObject);
            UnityPalFastMutexDelete(localFastMutex);
        }

        UnityPalConditionVariable* localObject;
        UnityPalFastMutex* localFastMutex;
    };

    TEST_FIXTURE(ConditionVariableFixture, InstantiateNewReturnsValidPointer)
    {
        CHECK_NOT_NULL(localObject);
    }
}

#endif // NET_4_0
#endif // ENABLE_UNIT_TESTS
