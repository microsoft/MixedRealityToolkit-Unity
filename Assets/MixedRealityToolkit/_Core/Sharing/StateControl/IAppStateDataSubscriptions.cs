using System;
using System.Collections.Generic;

namespace Pixie.StateControl
{
    public interface IAppStateDataSubscriptions
    {
        void SetSubscriptionMode(SubscriptionModeEnum subscriptionType, IEnumerable<Type> subscriptionTypes = null);
    }
}