//
// Copyright (C) Microsoft. All rights reserved.
//

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of the SessionManager to register to receive event callbacks without
    /// having their classes inherit directly from SessionManagerListener
    /// </summary>
    public class SessionManagerAdapter : SessionManagerListener
    {
        public event System.Action<Session> CreateSucceededEvent;
        public event System.Action<XString> CreateFailedEvent;
        public event System.Action<Session> SessionAddedEvent;
        public event System.Action<Session> SessionClosedEvent;
        public event System.Action<Session, User> UserJoinedSessionEvent;
        public event System.Action<Session, User> UserLeftSessionEvent;
        public event System.Action<Session, User> UserChangedEvent;
        public event System.Action ServerConnectedEvent;
        public event System.Action ServerDisconnectedEvent;

        public SessionManagerAdapter() { }

        public override void OnCreateSucceeded(Session newSession)
        {
            Profile.BeginRange("OnCreateSucceeded");
            if (this.CreateSucceededEvent != null)
            {
                this.CreateSucceededEvent(newSession);
            }
            Profile.EndRange();
        }

        public override void OnCreateFailed(XString reason)
        {
            Profile.BeginRange("OnCreateFailed");
            if (this.CreateFailedEvent != null)
            {
                this.CreateFailedEvent(reason);
            }
            Profile.EndRange();
        }

        public override void OnSessionAdded(Session newSession)
        {
            Profile.BeginRange("OnSessionAdded");
            if (this.SessionAddedEvent != null)
            {
                this.SessionAddedEvent(newSession);
            }
            Profile.EndRange();
        }

        public override void OnSessionClosed(Session session)
        {
            Profile.BeginRange("OnSessionClosed");
            if (this.SessionClosedEvent != null)
            {
                this.SessionClosedEvent(session);
            }
            Profile.EndRange();
        }

        public override void OnUserJoinedSession(Session session, User newUser)
        {
            Profile.BeginRange("OnUserJoinedSession");
            if (this.UserJoinedSessionEvent != null)
            {
                this.UserJoinedSessionEvent(session, newUser);
            }
            Profile.EndRange();
        }

        public override void OnUserLeftSession(Session session, User user)
        {
            Profile.BeginRange("OnUserLeftSession");
            if (this.UserLeftSessionEvent != null)
            {
                this.UserLeftSessionEvent(session, user);
            }
            Profile.EndRange();
        }

        public override void OnUserChanged(Session session, User user)
        {
            Profile.BeginRange("OnUserChanged");
            if (this.UserChangedEvent != null)
            {
                this.UserChangedEvent(session, user);
            }
            Profile.EndRange();
        }

        public override void OnServerConnected()
        {
            Profile.BeginRange("OnServerConnected");
            if (this.ServerConnectedEvent != null)
            {
                this.ServerConnectedEvent();
            }
            Profile.EndRange();
        }

        public override void OnServerDisconnected()
        {
            Profile.BeginRange("OnServerDisconnected");
            if (this.ServerDisconnectedEvent != null)
            {
                this.ServerDisconnectedEvent();
            }
            Profile.EndRange();
        }
    }
}