using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System;
using System.Collections.Generic;

/// <summary>
/// Keeps track of users joining and leaving the session.
/// </summary>
public class SharingSessionTracker : Singleton<SharingSessionTracker>
{
    public class SessionJoinedEventArgs : EventArgs
    {
        public User joiningUser;
    }

    public class SessionLeftEventArgs : EventArgs
    {
        public long exitingUserId;
    }

    /// <summary>
    /// SessionJoined event notifies when a user joins a session.
    /// </summary>
    public event EventHandler<SessionJoinedEventArgs> SessionJoined;

    /// <summary>
    /// SessionLeft event notifies when a user leaves a session.
    /// </summary>
    public event EventHandler<SessionLeftEventArgs> SessionLeft;

    public List<long> UserIds
    {
        get { return userIds; }
    }

    // Local cached pointer to the SessionManager
    private SessionManager sessionManager;
    List<long> userIds = new List<long>();

    private Dictionary<long, User> userIdToUser = new Dictionary<long, User>();

    private const uint pollingFrequency = 60;

    void SendJoinEvent(User user)
    {
        Debug.Log("User joining session: " + user.GetID());

        EventHandler<SessionJoinedEventArgs> joinEvent = SessionJoined;
        if (joinEvent != null)
        {
            SessionJoinedEventArgs sjea = new SessionJoinedEventArgs();
            sjea.joiningUser = user;
            joinEvent(this, sjea);
        }

        long userId = user.GetID();
        if (userIdToUser.ContainsKey(userId) == false)
        {
            userIdToUser.Add(userId, user);
        }
    }

    void SendLeaveEvent(long userId)
    {
        Debug.Log("User leaving session: " + userId);

        EventHandler<SessionLeftEventArgs> leftEvent = SessionLeft;
        if (leftEvent != null)
        {
            SessionLeftEventArgs slea = new SessionLeftEventArgs();
            slea.exitingUserId = userId;
            leftEvent(this, slea);
        }

        if (userIdToUser.ContainsKey(userId) == true)
        {
            userIdToUser.Remove(userId);
        }
    }

    public User GetUserById(long userId)
    {
        User retval = null;
        userIdToUser.TryGetValue(userId, out retval);

        if (retval == null)
        {
            Session currentSession = this.sessionManager.GetCurrentSession();
            if (currentSession != null)
            {
                int userCount = currentSession.GetUserCount();
                for (int index = 0; index < userCount; index++)
                {
                    User user = currentSession.GetUser(index);
                    if ((long)user.GetID() == userId)
                    {
                        retval = user;
                        break;
                    }
                }
            }
        }

        return retval;
    }

    void Update()
    {
        // Get an instance of the SessionManager if one does not exist.
        if (sessionManager == null && SharingStage.Instance != null && SharingStage.Instance.Manager != null)
        {
            this.sessionManager = SharingStage.Instance.Manager.GetSessionManager();
        }

        // Only poll every second.
        if (Time.frameCount % pollingFrequency == 0 && this.sessionManager != null && sessionManager.GetSessionCount() > 0)
        {
            Session currentSession = this.sessionManager.GetCurrentSession();
            if (currentSession != null)
            {
                int userCount = currentSession.GetUserCount();

                // If we have fewer users in the current session than are
                // tracked locally then one or more users have exited.
                // We need to figure out which ones.
                if (userCount < userIds.Count)
                {
                    // Gather all of the new users into a new array.
                    List<long> updatedUserIds = new List<long>();

                    for (int index = 0; index < userCount; index++)
                    {
                        User user = currentSession.GetUser(index);
                        long userId = user.GetID();
                        updatedUserIds.Add(userId);
                        Debug.Log(index + ": id: " + user.GetID() + " or: " + userId);

                        // It's an edge case, but if a user joins and a user exits at the same
                        // time, we need to handle that.
                        if (userIds.Contains(userId) == false)
                        {
                            SendJoinEvent(user);
                        }
                    }

                    // Now check to see which IDs are in the old userIds list, but not in the new updatedUserIds list.
                    for (int index = 0; index < userIds.Count; index++)
                    {
                        if (updatedUserIds.Contains(userIds[index]) == false)
                        {
                            SendLeaveEvent(userIds[index]);
                        }
                    }

                    userIds = updatedUserIds;
                }
                else // Same or more users in the session.
                {
                    for (int index = 0; index < userCount; index++)
                    {
                        User user = currentSession.GetUser(index);
                        long userId = user.GetID();
                        if (userIds.Contains(userId) == false)
                        {
                            userIds.Add(userId);
                            SendJoinEvent(user);
                        }
                    }
                }
            }
        }
    }
}