// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Used to demonstrate how to get notifications when users leave and enter room.
    /// </summary>
    public class UserNotifications : MonoBehaviour
    {
        private SessionUsersTracker usersTracker;
        private static User localUser = null;

        private void Start()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
            {
                Connected();
            }
            else
            {
                SharingStage.Instance.SharingManagerConnected += Connected;
            }
        }

        private void Connected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;

            usersTracker = SharingStage.Instance.SessionUsersTracker;
            localUser = SharingStage.Instance.Manager.GetLocalUser();

            usersTracker.UserJoined += NotifyUserJoined;
            usersTracker.UserLeft += NotifyUserLeft;
        }

        private void NotifyUserJoined(User user)
        {
            if (user.IsValid() && localUser.GetID() != user.GetID())
            {
                Debug.LogFormat("[User Notifications] User {0} has joined the room.", user.GetName());
            }
        }

        private void NotifyUserLeft(User user)
        {
            if (user.IsValid() && localUser.GetID() != user.GetID())
            {
                Debug.LogFormat("[User Notifications] User {0} has left the room.", user.GetName());
            }
        }

        private void OnDestroy()
        {
            if (usersTracker != null)
            {
                usersTracker.UserJoined -= NotifyUserJoined;
                usersTracker.UserLeft -= NotifyUserLeft;
            }
            usersTracker = null;
        }
    }
}
