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
            string users = string.Empty;

            for (int i = 0; i < usersTracker.CurrentUsers.Count; i++)
            {
                users += "\n" + usersTracker.CurrentUsers[i].GetName();
            }

            Debug.LogFormat("[User Notifications] {0} users in room.{1}", usersTracker.CurrentUsers.Count, users);

            localUser = SharingStage.Instance.Manager.GetLocalUser();

            usersTracker.UserJoined += NotifyUserJoined;
            usersTracker.UserLeft += NotifyUserLeft;
        }

        private static void NotifyUserJoined(User user)
        {
            if (user.IsValid() && localUser != user)
            {
                Debug.LogFormat("[User Notifications] User {0} has joined the room.", user.GetName());
            }
        }

        private static void NotifyUserLeft(User user)
        {
            if (user.IsValid() && localUser != user)
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
