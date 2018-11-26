// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// The ServerSessionsTracker manages the list of sessions on the server and the users in these sessions.
    /// Instance is created by Sharing Stage when a connection is found.
    /// </summary>
    public class ServerSessionsTracker : IDisposable
    {
        private bool disposed;
        private SessionManager sessionManager;
        private SessionManagerAdapter sessionManagerAdapter;

        public event Action<bool, string> SessionCreated;
        public event Action<Session> SessionAdded;
        public event Action<Session> SessionClosed;

        public event Action<Session, User> UserChanged;
        public event Action<Session, User> UserJoined;
        public event Action<Session, User> UserLeft;
        public event Action<Session> CurrentUserLeft;
        public event Action<Session> CurrentUserJoined;

        public event Action ServerConnected;
        public event Action ServerDisconnected;

        /// <summary>
        /// List of sessions on the server.
        /// </summary>
        public List<Session> Sessions { get; private set; }

        /// <summary>
        /// Indicates whether there is a connection to the server or not.
        /// </summary>
        public bool IsServerConnected { get; private set; }

        public ServerSessionsTracker(SessionManager sessionMgr)
        {
            sessionManager = sessionMgr;
            Sessions = new List<Session>();

            if (sessionManager != null)
            {
                sessionManagerAdapter = new SessionManagerAdapter();
                sessionManagerAdapter.ServerConnectedEvent += OnServerConnected;
                sessionManagerAdapter.ServerDisconnectedEvent += OnServerDisconnected;
                sessionManagerAdapter.SessionClosedEvent += OnSessionClosed;
                sessionManagerAdapter.SessionAddedEvent += OnSessionAdded;
                sessionManagerAdapter.CreateSucceededEvent += OnSessionCreatedSuccess;
                sessionManagerAdapter.CreateFailedEvent += OnSessionCreatedFail;
                sessionManagerAdapter.UserChangedEvent += OnUserChanged;
                sessionManagerAdapter.UserJoinedSessionEvent += OnUserJoined;
                sessionManagerAdapter.UserLeftSessionEvent += OnUserLeft;
                sessionManager.AddListener(sessionManagerAdapter);
            }
        }

        /// <summary>
        /// Retrieves the current session, if any.
        /// </summary>
        /// <returns>Current session the app is in. Null if no session is joined.</returns>
        public Session GetCurrentSession()
        {
            return sessionManager.GetCurrentSession();
        }

        /// <summary>
        ///  Join the specified session.
        /// </summary>
        /// <param name="session">Session to join.</param>
        /// <returns>True if the session is being joined or is already joined.</returns>
        public bool JoinSession(Session session)
        {
            // TODO Should prevent joining multiple sessions at the same time
            if (session != null)
            {
                return session.IsJoined() || session.Join();
            }

            return false;
        }

        /// <summary>
        ///  Leave the current session.
        /// </summary>
        public void LeaveCurrentSession()
        {
            using (Session currentSession = GetCurrentSession())
            {
                if (currentSession != null)
                {
                    currentSession.Leave();
                }
            }
        }

        /// <summary>
        /// Creates a new session on the server.
        /// </summary>
        /// <param name="sessionName">Name of the session.</param>
        /// <returns>True if a session was created, false if not.</returns>
        public bool CreateSession(string sessionName)
        {
            if (disposed)
            {
                return false;
            }

            return sessionManager.CreateSession(sessionName);
        }

        private void OnSessionCreatedFail(XString reason)
        {
            using (reason)
            {
                SessionCreated.RaiseEvent(false, reason.GetString());
            }
        }

        private void OnSessionCreatedSuccess(Session session)
        {
            using (session)
            {
                SessionCreated.RaiseEvent(true, session.GetName().GetString());
            }
        }

        private void OnUserChanged(Session session, User user)
        {
            UserChanged.RaiseEvent(session, user);
        }

        private void OnUserLeft(Session session, User user)
        {
            UserLeft.RaiseEvent(session, user);

            if (IsLocalUser(user))
            {
                CurrentUserLeft.RaiseEvent(session);
            }
        }

        private void OnUserJoined(Session session, User newUser)
        {
            UserJoined.RaiseEvent(session, newUser);

            if (IsLocalUser(newUser))
            {
                CurrentUserJoined.RaiseEvent(session);
            }
        }

        private void OnSessionAdded(Session newSession)
        {
            Sessions.Add(newSession);
            SessionAdded.RaiseEvent(newSession);
        }

        private void OnSessionClosed(Session session)
        {
            for (int i = 0; i < Sessions.Count; i++)
            {
                if (Sessions[i].GetName().ToString().Equals(session.GetName().ToString()))
                {
                    SessionClosed.RaiseEvent(Sessions[i]);
                    Sessions.Remove(Sessions[i]);
                }
            }
        }

        private void OnServerDisconnected()
        {
            IsServerConnected = false;

            // Remove all sessions
            for (int i = 0; i < Sessions.Count; i++)
            {
                Sessions[i].Dispose();
            }

            Sessions.Clear();
            ServerDisconnected.RaiseEvent();
        }

        private void OnServerConnected()
        {
            IsServerConnected = true;
            ServerConnected.RaiseEvent();
        }

        private bool IsLocalUser(User user)
        {
            int changedUserId = user.GetID();
            using (User localUser = sessionManager.GetCurrentUser())
            {
                int localUserId = localUser.GetID();
                return localUserId == changedUserId;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                OnServerDisconnected();

                if (sessionManagerAdapter != null)
                {
                    sessionManagerAdapter.ServerConnectedEvent -= OnServerConnected;
                    sessionManagerAdapter.ServerDisconnectedEvent -= OnServerDisconnected;
                    sessionManagerAdapter.SessionClosedEvent -= OnSessionClosed;
                    sessionManagerAdapter.SessionAddedEvent -= OnSessionAdded;
                    sessionManagerAdapter.CreateSucceededEvent -= OnSessionCreatedSuccess;
                    sessionManagerAdapter.CreateFailedEvent -= OnSessionCreatedFail;
                    sessionManagerAdapter.UserChangedEvent -= OnUserChanged;
                    sessionManagerAdapter.UserJoinedSessionEvent -= OnUserJoined;
                    sessionManagerAdapter.UserLeftSessionEvent -= OnUserLeft;
                    sessionManagerAdapter.Dispose();
                    sessionManagerAdapter = null;
                }

                if (sessionManager != null)
                {
                    sessionManager.Dispose();
                    sessionManager = null;
                }
            }

            disposed = true;
        }

        #endregion
    }
}