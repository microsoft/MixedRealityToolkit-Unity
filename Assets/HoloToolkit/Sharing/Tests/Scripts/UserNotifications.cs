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

        private void Start()
        {
            // SharingStage should be valid at this point.
            SharingStage.Instance.SharingManagerConnected += Connected;
        }

        private void Connected(object sender, EventArgs e)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;

            usersTracker = SharingStage.Instance.SessionUsersTracker;

            Debug.LogFormat("{0} users in room.", usersTracker.CurrentUsers.Count);

            foreach (User currentUser in usersTracker.CurrentUsers)
            {
                Debug.LogFormat(currentUser.GetName());
            }

            usersTracker.UserJoined += NotifyUserJoined;
            usersTracker.UserLeft += NotifyUserLeft;
        }

        private static void NotifyUserJoined(User user)
        {
            Debug.LogFormat("User {0} has joined the room.", user.GetName());
        }

        private static void NotifyUserLeft(User user)
        {
            Debug.LogFormat("User {0} has left the room.", user.GetName());
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
