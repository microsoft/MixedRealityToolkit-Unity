using System;
using System.Collections.Generic;

namespace MRTK.StateControl
{
    public interface IAppStateDataSubscriptions
    {
        Action OnLocalSubscriptionModeChange { get; set; }

        /// <summary>
        /// Sets the subscription mode as well as manual subscription types.
        /// If subscription mode is ALL then subscription types are ignored.
        /// </summary>
        /// <param name="subscriptionMode"></param>
        /// <param name="subscriptionTypes"></param>
        void SetLocalSubscriptionMode(SubscriptionModeEnum subscriptionMode, IEnumerable<Type> subscriptionTypes = null);

        /// <summary>
        /// Returns true if local user is subscribed to this state type, or if subscription type is set to ALL
        /// (Does not check whether state type is included in AppState)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsLocalUserSubscribedToType(Type type);
    }
}