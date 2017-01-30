// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Keeps track of users joining and leaving the session.
    /// </summary>
    public class SharingSessionTracker : Singleton<SharingSessionTracker>
    {
        public class SessionJoinedEventArgs : EventArgs
        {
            public User JoiningUser;
        }

        public class SessionLeftEventArgs : EventArgs
        {
            public long ExitingUserId;
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
        private List<long> userIds = new List<long>();

        private Dictionary<long, User> userIdToUser = new Dictionary<long, User>();

        private const uint PollingFrequency = 60;

        private void SendJoinEvent(User user)
        {
            Debug.Log("User joining session: " + user.GetID());

            EventHandler<SessionJoinedEventArgs> joinEvent = SessionJoined;
            if (joinEvent != null)
            {
                var sjea = new SessionJoinedEventArgs();
                sjea.JoiningUser = user;
                joinEvent(this, sjea);
            }

            long userId = user.GetID();
            if (userIdToUser.ContainsKey(userId) == false)
            {
                userIdToUser.Add(userId, user);
            }
        }

        private void SendLeaveEvent(long userId)
        {
            Debug.Log("User leaving session: " + userId);

            EventHandler<SessionLeftEventArgs> leftEvent = SessionLeft;
            if (leftEvent != null)
            {
                SessionLeftEventArgs slea = new SessionLeftEventArgs();
                slea.ExitingUserId = userId;
                leftEvent(this, slea);
            }

            if (userIdToUser.ContainsKey(userId))
            {
                userIdToUser.Remove(userId);
            }
        }

        public User GetUserById(long userId)
        {
            User retval;
            userIdToUser.TryGetValue(userId, out retval);

            if (retval == null)
            {
                Session currentSession = sessionManager.GetCurrentSession();
                if (currentSession != null)
                {
                    int userCount = currentSession.GetUserCount();
                    for (int index = 0; index < userCount; index++)
                    {
                        User user = currentSession.GetUser(index);
                        if (user.GetID() == userId)
                        {
                            retval = user;
                            break;
                        }
                    }
                }
            }

            return retval;
        }

        private void Update()
        {
            // Get an instance of the SessionManager if one does not exist.
            if (sessionManager == null && SharingStage.Instance != null && SharingStage.Instance.Manager != null)
            {
                sessionManager = SharingStage.Instance.Manager.GetSessionManager();
            }

            // Only poll every second.
            if (Time.frameCount % PollingFrequency == 0 && sessionManager != null && sessionManager.GetSessionCount() > 0)
            {
                Session currentSession = sessionManager.GetCurrentSession();
                if (currentSession != null)
                {
                    int userCount = currentSession.GetUserCount();

                    // If we have fewer users in the current session than are
                    // tracked locally then one or more users have exited.
                    // We need to figure out which ones.
                    if (userCount < userIds.Count)
                    {
                        // Gather all of the new users into a new array.
                        var updatedUserIds = new List<long>();

                        for (int index = 0; index < userCount; index++)
                        {
                            User user = currentSession.GetUser(index);
                            long userId = user.GetID();
                            updatedUserIds.Add(userId);
                            Debug.LogFormat("{0}: id: {1} or: {2}", index.ToString(), user.GetID().ToString(), userId.ToString());

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
}