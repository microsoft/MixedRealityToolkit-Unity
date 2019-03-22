// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.UI.Core;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Capture;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC
{
    /// <summary>
    /// High-level wrapper for Unity Webrtc functionality
    /// </summary>
    public class Webrtc : MonoBehaviour
    {
        /// <summary>
        /// Different Ice server types
        /// </summary>
        public enum IceType
        {
            /// <summary>
            /// Indicates there is no Ice information
            /// </summary>
            /// <remarks>
            /// Under normal use, this should not be used
            /// </remarks>
            None = 0,

            /// <summary>
            /// Indicates Ice information is of type STUN
            /// </summary>
            /// <remarks>
            /// https://en.wikipedia.org/wiki/STUN
            /// </remarks>
            Stun,

            /// <summary>
            /// Indicates Ice information is of type TURN
            /// </summary>
            /// <remarks>
            /// https://en.wikipedia.org/wiki/Traversal_Using_Relays_around_NAT
            /// </remarks>
            Turn
        }

        /// <summary>
        /// Represents an Ice server in a simple way that allows configuration from the unity inspector
        /// </summary>
        [Serializable]
        public struct ConfigurableIceServer
        {
            /// <summary>
            /// The type of the server
            /// </summary>
            public IceType Type;

            /// <summary>
            /// The unqualified uri of the server
            /// </summary>
            /// <remarks>
            /// You should not prefix this with "stun:" or "turn:"
            /// </remarks>
            public string Uri;

            /// <summary>
            /// Convert the server to the representation the underlying libraries use
            /// </summary>
            /// <returns>stringified server information</returns>
            public override string ToString()
            {
                return string.Format("{0}: {1}", Type.ToString().ToLower(), Uri);
            }
        }

        /// <summary>
        /// A UnityEvent that represents error events
        /// </summary>
        [Serializable]
        public class WebrtcErrorEvent : UnityEvent<string>
        {
        }

        /// <summary>
        /// Retrieves the underlying peer
        /// </summary>
        /// <remarks>
        /// If <see cref="OnInitialized"/> has not fired, this will be <c>null</c>
        /// </remarks>
        public IPeer Peer
        {
            get;
            protected set;
        }

        /// <summary>
        /// Flag to initialize Webrtc on <see cref="Start"/>
        /// </summary>
        [Tooltip("This will initialize Webrtc on Start()")]
        public bool AutoInitialize = true;

        /// <summary>
        /// Flag to log all errors to the Unity console automatically
        /// </summary>
        [Tooltip("This will log all errors to the Unity console")]
        public bool AutoLogErrors = true;

        /// <summary>
        /// Set of ice servers
        /// </summary>
        [Tooltip("(Optional) Set of Ice servers")]
        public List<ConfigurableIceServer> IceServers = new List<ConfigurableIceServer>()
        {
            new ConfigurableIceServer()
            {
                Type = IceType.Stun,
                Uri = "stun.l.google.com:19302"
            }
        };

        [Tooltip("(Optional) The Webrtc username for Ice connections")]
        public string IceUsername;

        [Tooltip("(Optional) The Webrtc credential for Ice connections")]
        public string IceCredential;

        /// <summary>
        /// Event that occurs when Webrtc is fully initialized
        /// </summary>
        [Tooltip("Event that occurs when Webrtc is fully initialized")]
        public UnityEvent OnInitialized = new UnityEvent();

        /// <summary>
        /// Event that occurs when a Webrtc error occurs
        /// </summary>
        [Tooltip("Event that occurs when a Webrtc error occurs")]
        public WebrtcErrorEvent OnError = new WebrtcErrorEvent();

        /// <summary>
        /// Internal queue used to marshal work back to the main unity thread
        /// </summary>
        private Queue<Action> MainThreadWorkloads = new Queue<Action>();

        /// <summary>
        /// Unity Engine Start() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
        /// </remarks>
        private void Start()
        {
            // if the caller demands auto-log behavior...
            if (AutoLogErrors)
            {
                // we bind a handler for that, as well.
                OnError.AddListener(new UnityAction<string>(OnError_Listener));
            }

            // if the caller demands auto-initialize behavior...
            if (AutoInitialize)
            {
                // we initialize.
                InitializeAsync();
            }
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
            // try to cleanup, if we get destroyed.
            Uninitialize();
        }

        /// <summary>
        /// Initialize the underlying Webrtc libraries
        /// </summary>
        /// <remarks>
        /// This function is asynchronous, to monitor it's status bind a handler to OnInitialized and OnError
        /// </remarks>
        public void InitializeAsync()
        {
            // if the peer is already set, we refuse to initialize again.
            // Note: for multi-peer scenarios, use multiple Webrtc components.
            if (Peer != null)
            {
                return;
            }

#if UNITY_ANDROID
            AndroidJavaClass systemClass = new AndroidJavaClass("java.lang.System");
            string libname = "jingle_peerconnection_so";
            systemClass.CallStatic("loadLibrary", new object[1] { libname });
            Debug.Log("loadLibrary loaded : " + libname);

            /*
            * Below is equivalent of this java code:
            * PeerConnectionFactory.InitializationOptions.Builder builder = 
            *   PeerConnectionFactory.InitializationOptions.builder(UnityPlayer.currentActivity);
            * PeerConnectionFactory.InitializationOptions options = 
            *   builder.createInitializationOptions();
            * PeerConnectionFactory.initialize(options);
            */

            AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass webrtcClass = new AndroidJavaClass("org.webrtc.PeerConnectionFactory");
            AndroidJavaClass initOptionsClass = new AndroidJavaClass("org.webrtc.PeerConnectionFactory$InitializationOptions");
            AndroidJavaObject builder = initOptionsClass.CallStatic<AndroidJavaObject>("builder", new object[1] { activity });
            AndroidJavaObject options = builder.Call<AndroidJavaObject>("createInitializationOptions");

            if (webrtcClass != null)
            {
                webrtcClass.CallStatic("initialize", new object[1] { options });
            }
#endif

#if UNITY_WSA && !UNITY_EDITOR
            if (UnityEngine.WSA.Application.RunningOnUIThread())
            {
#endif
            RequestAccessAndInitAsync();
#if UNITY_WSA && !UNITY_EDITOR
            }
            else
            {
                UnityEngine.WSA.Application.InvokeOnUIThread(RequestAccessAndInitAsync, waitUntilDone: true);
            }
#endif
        }

        /// <summary>
        /// Uninitialize the underlying webrtc libraries, effectively cleaning up the allocated peer
        /// </summary>
        /// <remarks>
        /// <see cref="Peer"/> will be <c>null</c> afterward
        /// </remarks>
        public void Uninitialize()
        {
            if (Peer != null)
            {
                Peer.ClosePeerConnection();
                Peer = null;
            }
        }

        /// <summary>
        /// Internal helper to ensure device access and continue initialization
        /// </summary>
        private void RequestAccessAndInitAsync()
        {
#if UNITY_WSA && !UNITY_EDITOR
        RequestAccessAsync().AsTask().ContinueWith(antecedent =>
        {
            if (antecedent.Result)
            {
#endif
            InitializePlugin();
#if UNITY_WSA && !UNITY_EDITOR
            }
            else
            {
                // fire the error handler at the next possible moment.
                MainThreadWorkloads.Enqueue(() =>
                {
                    OnError.Invoke("Audio/Video access failure: " + antecedent.Exception.Message);
                });
            }
        });
#endif
        }

#if UNITY_WSA && !UNITY_EDITOR
        /// <summary>
        /// Internal WSA-specific handler for securing device access
        /// </summary>
        private IAsyncOperation<bool> RequestAccessAsync()
        {
            MediaCapture mediaAccessRequester = new MediaCapture();
            MediaCaptureInitializationSettings mediaSettings =
                new MediaCaptureInitializationSettings();
            mediaSettings.AudioDeviceId = "";
            mediaSettings.VideoDeviceId = "";
            mediaSettings.StreamingCaptureMode =
                Windows.Media.Capture.StreamingCaptureMode.AudioAndVideo;
            mediaSettings.PhotoCaptureSource =
                Windows.Media.Capture.PhotoCaptureSource.VideoPreview;
            Task initTask = mediaAccessRequester.InitializeAsync(mediaSettings).AsTask();
            return initTask.ContinueWith(initResult => {
                if (initResult.Exception != null)
                {
                    throw initResult.Exception;
                }
                return true;
            }).AsAsyncOperation<bool>();
        }
#endif

        /// <summary>
        /// Internal handler to actually initialize the underlying library wrapper
        /// </summary>
        private void InitializePlugin()
        {
            try
            {
                // allocate the peer.
                this.Peer = new NativePeer(IceServers.Select(i => i.ToString()).ToList(), IceUsername, IceCredential);

                // enable audio and playback.
                this.Peer.SetAudioControl(isMute: false, isRecord: true);

                // notify all observers that we are ready at the next possible moment.
                MainThreadWorkloads.Enqueue(() =>
                {
                    OnInitialized.Invoke();
                });

            }
            catch (Exception ex)
            {
                // if the above fails, notify all error observers at the next possible moment.
                MainThreadWorkloads.Enqueue(() =>
                {
                    OnError.Invoke("Unable to initialize underlying library: " + ex.Message);
                });
            }
        }

        /// <summary>
        /// Internal handler for on-error, if <see cref="AutoLogErrors"/> is <c>true</c>
        /// </summary>
        /// <param name="error">The error message</param>
        private void OnError_Listener(string error)
        {
            // we log the error to the Unity console error channel
            UnityEngine.Debug.LogError(error);
        }
    }
}
