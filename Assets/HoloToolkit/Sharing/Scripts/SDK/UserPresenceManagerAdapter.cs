//
// Copyright (C) Microsoft. All rights reserved.
//

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of UserPresenceManager to register to receive event callbacks without
    /// having their classes inherit directly from UserPresenceManagerListener
    /// </summary>
    public class UserPresenceManagerAdapter : UserPresenceManagerListener
    {
        public event System.Action<User> UserPresenceChangedEvent;

        public UserPresenceManagerAdapter() { }

        public override void OnUserPresenceChanged(User user)
        {
            if (this.UserPresenceChangedEvent != null)
            {
                this.UserPresenceChangedEvent(user);
            }
        }
    }
}
