using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Marshalling;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc
{
    /// <summary>
    /// Peer event frontend
    /// </summary>
    [RequireComponent(typeof(Webrtc))]
    public class WebrtcPeerEvents : MonoBehaviour, IPeerImpl
    {
        /// <summary>
        /// Represents a peer event composed of a single string-literal
        /// </summary>
        [Serializable]
        public class WebrtcPeerStringEvent : UnityEvent<string>
        {
        }

        /// <summary>
        /// Represents a peer event composed of two string-literals
        /// </summary>
        [Serializable]
        public class WebrtcPeerString2Event : UnityEvent<string, string>
        {
        }

        /// <summary>
        /// Represents a peer event composed of a string-literal, an int, and another string-literal
        /// </summary>
        [Serializable]
        public class WebrtcPeerStringIntStringEvent : UnityEvent<string, int, string>
        {
        }

        /// <summary>
        /// Flag to log all errors to the Unity console automatically
        /// </summary>
        [Tooltip("This will log all errors to the Unity console")]
        public bool AutoLogErrors = true;

        /// <summary>
        /// Event that occurs when the peer is ready
        /// </summary>
        /// <remarks>
        /// This is the same as <see cref="Webrtc.OnInitialized"/> but is included here for convienience
        /// </remarks>
        [Tooltip("Event that occurs when the peer is ready")]
        public UnityEvent OnPeerReady = new UnityEvent();

        /// <summary>
        /// Event that occurs when data is ready from the data channel
        /// </summary>
        [Tooltip("Event that occurs when data is ready from the data channel")]
        public WebrtcPeerStringEvent OnDataFromDataChannelReady = new WebrtcPeerStringEvent();

        /// <summary>
        /// Event that occurs when a peer-specific failure happens
        /// </summary>
        [Tooltip("Event that occurs when a peer-specific failure happens")]
        public WebrtcPeerStringEvent OnFailure = new WebrtcPeerStringEvent();

        /// <summary>
        /// Event that occurs when an ice candidate is prepared
        /// </summary>
        [Tooltip("Event that occurs when an ice candidate is prepared")]
        public WebrtcPeerStringIntStringEvent OnIceCandiateReadyToSend = new WebrtcPeerStringIntStringEvent();

        /// <summary>
        /// Event that occurs when the data channel is ready
        /// </summary>
        [Tooltip("Event that occurs when the data channel is ready")]
        public UnityEvent OnLocalDataChannelReady = new UnityEvent();

        /// <summary>
        /// Event that occurs when an sdp message is prepared
        /// </summary>
        [Tooltip("Event that occurs when an sdp message is prepared")]
        public WebrtcPeerString2Event OnLocalSdpReadyToSend = new WebrtcPeerString2Event();

        /// <summary>
        /// Event that occurs when an sdp offer is prepared
        /// </summary>
        [Tooltip("Event that occurs when an sdp offer is prepared")]
        public WebrtcPeerStringEvent OnSdpOfferReadyToSend = new WebrtcPeerStringEvent();

        /// <summary>
        /// Event that occurs when an sdp answer is prepared
        /// </summary>
        [Tooltip("Event that occurs when an sdp answer is prepared")]
        public WebrtcPeerStringEvent OnSdpAnswerReadyToSend = new WebrtcPeerStringEvent();

        /// <summary>
        /// The underlying peer
        /// </summary>
        private IPeer peer = null;

        /// <summary>
        /// Internal queue used to marshal work back to the main unity thread
        /// </summary>
        private Queue<Action> MainThreadWorkloads = new Queue<Action>();

        /// <summary>
        /// Initialization handler
        /// 
        /// TODO(bengreenier): This could be auto-bound with editor script
        /// </summary>
        /// <remarks>
        /// This is expected to be bound to <see cref="Webrtc.OnInitialized"/> via the UnityEditor
        /// </remarks>
        public void OnInitialized()
        {
            // cache the peer.
            this.peer = this.GetComponent<Webrtc>().Peer;

            // setup the peer events.
            this.peer.DataFromDataChannelReady += Peer_DataFromDataChannelReady;
            this.peer.FailureMessage += Peer_FailureMessage;
            this.peer.IceCandiateReadytoSend += Peer_IceCandiateReadytoSend;
            this.peer.LocalDataChannelReady += Peer_LocalDataChannelReady;
            this.peer.LocalSdpReadytoSend += Peer_LocalSdpReadytoSend;

            // setup unity events for processing out additional pieces.
            this.OnLocalSdpReadyToSend.AddListener(new UnityAction<string, string>(OnLocalSdpReadyToSend_Listener));

            // if the caller demands auto-log behavior...
            if (AutoLogErrors)
            {
                // we bind a handler for that.
                this.OnFailure.AddListener(new UnityAction<string>(OnFailure_Listener));
            }

            this.OnPeerReady.Invoke();
        }

        /// <summary>
        /// Unity Engine Update() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
        /// </remarks>
        private void Update()
        {
            // check if there's work for us to execute.
            while (MainThreadWorkloads.Count > 0)
            {
                // get the work.
                var workload = MainThreadWorkloads.Dequeue();

                //execute it.
                workload();
            }
        }

        /// <summary>
        /// Unity Engine OnDestroy() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html
        /// </remarks>
        private void OnDestroy()
        {
            // cleanup, if necessary
            if (this.peer != null)
            {
                // cleanup the peer events.
                this.peer.DataFromDataChannelReady -= Peer_DataFromDataChannelReady;
                this.peer.FailureMessage -= Peer_FailureMessage;
                this.peer.IceCandiateReadytoSend -= Peer_IceCandiateReadytoSend;
                this.peer.LocalDataChannelReady -= Peer_LocalDataChannelReady;
                this.peer.LocalSdpReadytoSend -= Peer_LocalSdpReadytoSend;

                // cleanup the peer.
                this.peer = null;
            }
        }

        /// <summary>
        /// Internal handler for on-failure, if <see cref="AutoLogErrors"/> is <c>true</c>
        /// </summary>
        /// <param name="error">The error message</param>
        private void OnFailure_Listener(string error)
        {
            // we log the error to the Unity console error channel
            UnityEngine.Debug.LogError(error);
        }

        /// <summary>
        /// Internal handler for processing local sdp's into separate events for offers and answers
        /// </summary>
        /// <param name="type">message type</param>
        /// <param name="sdp">message</param>
        private void OnLocalSdpReadyToSend_Listener(string type, string sdp)
        {
            // if the type is offer...
            if (string.Equals(type, "offer", StringComparison.OrdinalIgnoreCase))
            {
                // emit an offer event.
                OnSdpOfferReadyToSend.Invoke(sdp);
            }
            // if the type is answer...
            else if (string.Equals(type, "answer", StringComparison.OrdinalIgnoreCase))
            {
                // emit an answer event.
                OnSdpAnswerReadyToSend.Invoke(sdp);
            }
        }

        /// <summary>
        /// We front the comms peer events (note: not video/audio events) so that UnityEvents
        /// and engine code have something they can bind to
        /// </summary>
        #region peer event handlers

        private void Peer_DataFromDataChannelReady(string s)
        {
            // execute when possible.
            MainThreadWorkloads.Enqueue(() =>
            {
                this.OnDataFromDataChannelReady.Invoke(s);
            });
        }

        private void Peer_FailureMessage(string msg)
        {
            // execute when possible.
            MainThreadWorkloads.Enqueue(() =>
            {
                this.OnFailure.Invoke(msg);
            });
        }

        private void Peer_IceCandiateReadytoSend(string candidate, int sdpMlineIndex, string sdpMid)
        {
            // execute when possible.
            MainThreadWorkloads.Enqueue(() =>
            {
                this.OnIceCandiateReadyToSend.Invoke(candidate, sdpMlineIndex, sdpMid);
            });
        }

        private void Peer_LocalDataChannelReady()
        {
            // execute when possible.
            MainThreadWorkloads.Enqueue(() =>
            {
                this.OnLocalDataChannelReady.Invoke();
            });
        }

        private void Peer_LocalSdpReadytoSend(string type, string sdp)
        {
            // execute when possible.
            MainThreadWorkloads.Enqueue(() =>
            {
                this.OnLocalSdpReadyToSend.Invoke(type, sdp);
            });
        }

        #endregion

        /// <summary>
        /// we front the <see cref="IPeer"/> interface for the underlying peer so that UnityEvents
        /// and engine code have something they can bind to to trigger the actual calls
        /// </summary>
        #region IPeer

        public void AddDataChannel()
        {
            if (peer != null)
            {
                peer.AddDataChannel();
            }
        }

        public void AddIceCandidate(string candidate, int sdpMlineindex, string sdpMid)
        {
            if (peer != null)
            {
                peer.AddIceCandidate(candidate, sdpMlineindex, sdpMid);
            }
        }

        public void AddStream(bool audioOnly)
        {
            if (peer != null)
            {
                peer.AddStream(audioOnly);
            }
        }

        public void ClosePeerConnection()
        {
            if (peer != null)
            {
                // this method needs to also mutate state in the parent controller.
                // TODO(bengreenier): should we remove cleanup from the IPeer interface?
                this.GetComponent<Webrtc>().Uninitialize();
                peer = null;
            }
        }

        public void CreateAnswer()
        {
            if (peer != null)
            {
                peer.CreateAnswer();
            }
        }

        public void CreateOffer()
        {
            if (peer != null)
            {
                peer.CreateOffer();
            }
        }

        public int GetUniqueId()
        {
            if (peer != null)
            {
                return peer.GetUniqueId();
            }
            else
            {
                return -1;
            }
        }

        public void SendDataViaDataChannel(string data)
        {
            if (peer != null)
            {
                peer.SendDataViaDataChannel(data);
            }
        }

        public void SetAudioControl(bool isMute, bool isRecord)
        {
            if (peer != null)
            {
                peer.SetAudioControl(isMute, isRecord);
            }
        }

        public void SetRemoteDescription(string type, string sdp)
        {
            if (peer != null)
            {
                peer.SetRemoteDescription(type, sdp);
            }
        }

        #endregion

        /// <summary>
        /// We provide a function Log() that matches the params of all exposed events so that one
        /// may easily wire UnityEvents to Log calls using <c>this.Log()</c> as a reference in editor
        /// </summary>
        #region log helpers

        public void Log()
        {
            UnityEngine.Debug.Log(new StackTrace(new Exception(), false).GetFrames()[0].GetMethod().Name);
        }

        public void Log(string arg1)
        {
            UnityEngine.Debug.Log(arg1);
        }

        public void Log(string arg1, string arg2)
        {
            UnityEngine.Debug.Log(string.Join(",", new string[] { arg1, arg2 }));
        }

        public void Log(string arg1, int arg2, string arg3)
        {
            UnityEngine.Debug.Log(string.Join(",", new string[] { arg1, arg2.ToString(), arg3 }));
        }

        #endregion
    }
}
