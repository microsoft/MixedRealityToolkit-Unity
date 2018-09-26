// -----------------------------------------------------------------------
// <copyright file="Client.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2015 Exit Games GmbH
// </copyright>
// <summary>
//   Extends Photon LoadBalancing API with audio streaming functionality.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;

namespace ExitGames.Client.Photon.Voice
{
    /// <summary>
    /// This class extends LoadBalancingClient with audio streaming functionality.
    /// </summary>
    /// <remarks>
    /// Use LoadBalancing workflow to join Voice room. All standard LoadBalancing features available.
    /// To work with audio:
    /// Create outgoing audio streams with Client.CreateLocalVoice method.
    /// Handle new incoming audio streams info with Client.OnRemoteVoiceInfoAction.
    /// Handle incoming audio streams data with Client.OnAudioFrameAction.
    /// </remarks>
    public class LoadBalancingFrontend : LoadBalancing.LoadBalancingClient, IVoiceFrontend, IDisposable
    {
        // no multiple channels in loadbalancing client
        const int DefaultVoiceChannel = 1;
        
        protected VoiceClient voiceClient;

        public void LogError(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ERROR, string.Format(fmt, args)); }
        public void LogWarning(string fmt, params object[] args) { this.DebugReturn(DebugLevel.WARNING, string.Format(fmt, args)); }
        public void LogInfo(string fmt, params object[] args) { this.DebugReturn(DebugLevel.INFO, string.Format(fmt, args)); }
        public void LogDebug(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ALL, string.Format(fmt, args)); }

        /// <summary>Lost frames counter.</summary>
        public int FramesLost { get { return this.voiceClient.FramesLost; } }

        /// <summary>Received frames counter.</summary>
        public int FramesReceived { get { return this.voiceClient.FramesReceived; } }

        /// <summary>Sent frames counter.</summary>
        public int FramesSent { get { return this.voiceClient.FramesSent; } }

        /// <summary>Average time required voice packet to return to sender.</summary>
        public int FramesSentBytes { get { return this.voiceClient.FramesSentBytes; } }

        /// <summary>Time required voice packet to return to sender.</summary>
        public int RoundTripTime { get { return this.voiceClient.RoundTripTime; } }

        /// <summary>Average round trip time variation.</summary>
        public int RoundTripTimeVariance { get { return this.voiceClient.RoundTripTimeVariance; } }

        /// <summary>Lost frames simulation ratio.</summary>
        public int DebugLostPercent { get { return this.voiceClient.DebugLostPercent; } set { this.voiceClient.DebugLostPercent = value; } }

        /// <summary>Iterates through copy of all local voices list.</summary>
        public IEnumerable<LocalVoice> LocalVoices { get { return this.voiceClient.LocalVoices; } }

        /// <summary>Iterates through copy of all local voices list of given channel.</summary>
        public IEnumerable<LocalVoice> LocalVoicesInChannel(int channelId) { return this.voiceClient.LocalVoicesInChannel(channelId); }

        /// <summary>Iterates through all remote voices infos.</summary>
        public IEnumerable<RemoteVoiceInfo> RemoteVoiceInfos { get { return voiceClient.RemoteVoiceInfos; } }

        /// <summary>Iterates through all local objects set by user in remote voices.</summary>
        public IEnumerable<object> RemoteVoiceLocalUserObjects { get { return voiceClient.RemoteVoiceLocalUserObjects; } }

        #region ClientBase

        public bool IsChannelJoined(int channelId) { return this.State == LoadBalancing.ClientState.Joined; }

        #endregion ClientBase

        /// <summary>
        /// If true, outgoing stream routed back to client via server same way as for remote client's streams.
        /// Can be swithed any time. OnRemoteVoiceInfoAction and OnRemoteVoiceRemoveAction are triggered if required.
        /// </summary>
        /// <remarks>
        /// For debug purposes only. 
        /// Room consistency is not guranteed if the property set to true at least once during join session.
        /// </remarks>
        public bool DebugEchoMode { 
            get {return debugEchoMode;}
            set
            {
                this.debugEchoMode = value;
                // need to update my voices in remote voice list if switched while joined
                if (this.State == LoadBalancing.ClientState.Joined)
                {
                    if (this.debugEchoMode)
                    {
                        // send to self - easiest way to setup speakers
                        this.voiceClient.sendChannelVoicesInfo(LoadBalancingFrontend.DefaultVoiceChannel, this.LocalPlayer.ID);
                    }
                    else
                    {
                        object[] content = new object[] { (byte)0, EventSubcode.DebugEchoRemoveMyVoices};
                        var opt = new LoadBalancing.RaiseEventOptions();
                        opt.TargetActors = new int[] { this.LocalPlayer.ID };
                        this.OpRaiseEvent((byte)EventCode.VoiceEvent, content, true, opt);
                    }
                }
            }
        }


        // let user code set actions which we occupy; call them in our actions
        /// <summary>Register a method to be called when an event got dispatched. Gets called at the end of OnEvent().</summary>
        /// <see cref="ExitGames.Client.Photon.LoadBalancing.LoadBalancingClient.OnEventAction"/>
        new public Action<EventData> OnEventAction { get; set; } // called by voice client action, so user still can use action

        // let user code set actions which we occupy; call them in our actions
        /// <summary>Register a method to be called when an event got dispatched. Gets called at the end of OnEvent().</summary>
        /// <see cref="ExitGames.Client.Photon.LoadBalancing.LoadBalancingClient.OnStateChangeAction"/>
        new public Action<LoadBalancing.ClientState> OnStateChangeAction { get; set; } // called by voice client action, so user still can use action
        
        /// <summary>Creates Client instance</summary>
        public LoadBalancingFrontend()
        {
            base.OnEventAction += onEventActionVoiceClient;
            base.OnStateChangeAction += onStateChangeVoiceClient;
            this.voiceClient = new VoiceClient(this);
        }

        /// <summary>
        /// This method dispatches all available incoming commands and then sends this client's outgoing commands.
        /// Call this method regularly (2..20 times a second).
        /// </summary>
        new public void Service()
        {
            base.Service();
            this.voiceClient.Service();
        }

        /// <summary>
        /// Creates new local voice (outgoing audio stream).
        /// </summary>
        /// <param name="audioStream">Object providing audio data for the outgoing stream.</param>
        /// <param name="voiceInfo">Outgoing audio stream parameters (should be set according to Opus encoder restrictions).</param>
        /// <returns>Outgoing stream handler.</returns>
        /// <remarks>
        /// audioStream.SamplingRate and voiceInfo.SamplingRate may do not match. Automatic resampling will occur in this case.
        /// </remarks>
        public LocalVoice CreateLocalVoice(IAudioStreamBase audioStream, VoiceInfo voiceInfo)
        {
            return this.voiceClient.CreateLocalVoice(audioStream, voiceInfo, LoadBalancingFrontend.DefaultVoiceChannel);
        }

        /// <summary>
        /// Change audio groups listended by client. Works only while joined to a voice room.
        /// </summary>
        /// <see cref="LocalVoice.AudioGroup"/>
        /// <see cref="SetGlobalAudioGroup(byte)"/>
        /// <remarks>
        /// Note the difference between passing null and byte[0]:
        ///   null won't add/remove any groups.
        ///   byte[0] will add/remove all (existing) groups.
        /// First, removing groups is executed. This way, you could leave all groups and join only the ones provided.
        /// </remarks>
        /// <param name="groupsToRemove">Groups to remove from listened. Null will not leave any. A byte[0] will remove all.</param>
        /// <param name="groupsToAdd">Groups to add to listened. Null will not add any. A byte[0] will add all current.</param>
        /// <returns>If request could be enqueued for sending</returns>
        public virtual bool ChangeAudioGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            return this.loadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
        }

        /// <summary>
        /// Set global audio group for this client. This call sets AudioGroup for existing local voices and for created later to given value.
        /// Client set as listening to this group only until ChangeAudioGroups called. This method can be called any time.
        /// </summary>
        /// <see cref="LocalVoice.AudioGroup"/>
        /// <see cref="ChangeAudioGroups(byte[], byte[])"/>
        public byte GlobalAudioGroup
        {
            get { return this.voiceClient.GlobalAudioGroup; }
            set
            {
                this.voiceClient.GlobalAudioGroup = value;
                if (this.State == LoadBalancing.ClientState.Joined)
                {
                    if (this.voiceClient.GlobalAudioGroup != 0)
                    {
                        this.loadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalAudioGroup });
                    }
                    else
                    {
                        this.loadBalancingPeer.OpChangeGroups(new byte[0], null);
                    }
                }                
            }
        }

        
        #region nonpublic

        private bool debugEchoMode;

        // send to others if playerId == 0 or to playerId only
        public void SendVoicesInfo(object content, int channelId, int targetPlayerId)
        {            
            var opt = new LoadBalancing.RaiseEventOptions();
            if (targetPlayerId != 0)
            {
                opt.TargetActors = new int[] { targetPlayerId };
            }
            else
            { // bradcast to others
                if (this.DebugEchoMode) // and to self as well if debugging
                {
                    opt.Receivers = LoadBalancing.ReceiverGroup.All;
                }
            }
            this.OpRaiseEvent((byte)EventCode.VoiceEvent, content, true, opt);
        }

        public void SendVoiceRemove(object content, int channelId)
        {
            var opt = new LoadBalancing.RaiseEventOptions();
            if (this.DebugEchoMode)
            {
                opt.Receivers = LoadBalancing.ReceiverGroup.All;
            }
            this.OpRaiseEvent((byte)EventCode.VoiceEvent, content, true, opt);
        }


        public void SendFrame(object content, int channelId, byte audioGroup)
        {                        
            var opt = new LoadBalancing.RaiseEventOptions();
            if (this.DebugEchoMode)
            {
                opt.Receivers = LoadBalancing.ReceiverGroup.All;
            }
            opt.InterestGroup = audioGroup;
            this.OpRaiseEvent((byte)EventCode.VoiceEvent, content, false, opt);
            this.loadBalancingPeer.SendOutgoingCommands();
        }

        public string ChannelIdStr(int channelId) { return null; }
        public string PlayerIdStr(int playerId) { return null; }
        public bool SupportsArraySegmentSerialization { get { return true; } }
        private void onEventActionVoiceClient(EventData ev)
        {
            int playerId;
            switch (ev.Code)
            {
                case (byte)LoadBalancing.EventCode.Join:
                    playerId = (int)ev[LoadBalancing.ParameterCode.ActorNr];
                    if (playerId == this.LocalPlayer.ID) 
                    {
                    }
                    else 
                    {
                        this.voiceClient.sendChannelVoicesInfo(LoadBalancingFrontend.DefaultVoiceChannel, playerId);// send to new joined only
                    }
                    break;
                case (byte)LoadBalancing.EventCode.Leave:
                    {
                        playerId = (int)ev[LoadBalancing.ParameterCode.ActorNr];
                        if (playerId == this.LocalPlayer.ID)
                        {
                            this.voiceClient.clearRemoteVoices();                            
                        }
                        else
                        {
                            onPlayerLeave(playerId);
                        }
                    }
                    break;                
                case (byte)EventCode.VoiceEvent:                    
                    // Single event code for all events to save codes for user.
                    // Payloads are arrays. If first array element is 0 than next is event subcode. Otherwise, the event is data frame with voiceId in 1st element.                    
                    this.voiceClient.onVoiceEvent(ev[(byte)LoadBalancing.ParameterCode.CustomEventContent], LoadBalancingFrontend.DefaultVoiceChannel, (int)ev[LoadBalancing.ParameterCode.ActorNr], this.LocalPlayer.ID);
                    break;
            }

            if (this.OnEventAction != null) this.OnEventAction(ev);
        }

        void onStateChangeVoiceClient(LoadBalancing.ClientState state)
        {
            switch (state)
            {
                case LoadBalancing.ClientState.Joined:
                    this.voiceClient.clearRemoteVoices();
                    this.voiceClient.sendChannelVoicesInfo(LoadBalancingFrontend.DefaultVoiceChannel, 0);// my join, broadcast
                    if (this.voiceClient.GlobalAudioGroup != 0)
                    {
                        this.loadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalAudioGroup });
                    }
                    break;
            }
            if (this.OnStateChangeAction != null) this.OnStateChangeAction(state);
        }
        private void onPlayerLeave(int playerId)
        {
            if (this.voiceClient.removePlayerVoices(LoadBalancingFrontend.DefaultVoiceChannel, playerId))
            {
                this.DebugReturn(DebugLevel.INFO, "[PV] Player " + playerId + " voices removed on leave");
            }            
            else
            {
                this.DebugReturn(DebugLevel.WARNING, "[PV] Voices of player " + playerId + " not found when trying to remove on player leave");
            }
        }
        #endregion

        public void Dispose()
        {
            this.voiceClient.Dispose();
        }
    }
}