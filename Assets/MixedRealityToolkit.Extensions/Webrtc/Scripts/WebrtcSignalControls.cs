// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Signaling;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC
{
    /// <summary>
    /// This is a sample implementation of signalling behaviours.
    /// </summary>
    public class WebrtcSignalControls : MonoBehaviour
    {
        /// <summary>
        /// The id of the <see cref="PlayerPrefs"/> key that we cache the last connected target id under
        /// </summary>
        private const string kLastTargetId = "lastTargetId";

        /// <summary>
        /// HACK(bengreenier): This is to make it clear to callers that a sibling ISignaler must be attached
        /// </summary>
        [Header("Runtime dependency on: sibling ISignaler")]

        /// <summary>
        /// The peer event frontend instance that we will control
        /// </summary>
        [Tooltip("The peer event frontend instance to control")]
        public WebrtcPeerEvents PeerEventsInstance;
        
        /// <summary>
        /// The text field in which we display the device name
        /// </summary>
        [Tooltip("The text field in which we display the device name")]
        public Text DeviceNameLabel;

        /// <summary>
        /// The text input field in which we accept the target device name
        /// </summary>
        [Tooltip("The text input field in which we accept the target device name")]
        public InputField TargetIdField;

        /// <summary>
        /// The button that generates an offer to a given target
        /// </summary>
        [Tooltip("The button that generates an offer to a given target")]
        public Button CreateOfferButton;


        /// <summary>
        /// The text field in which we display the signaler status
        /// </summary>
        [Tooltip("The text field in which we display the signaler status")]
        public Text SignalerStatusLabel;

        /// <summary>
        /// The sibling signaler component that is found at runtime
        /// </summary>
        private ISignaler signaler;

        /// <summary>
        /// Unity Engine Awake() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
        /// </remarks>
        private void Awake()
        {
            signaler = GetComponent<ISignaler>();

            if (signaler == null)
            {
                throw new InvalidOperationException("sibling ISignaler not found");
            }

            signaler.OnConnect += () =>
            {
                SignalerStatusLabel.text = "Signaler Status: Connected";
            };

            signaler.OnDisconnect += () =>
            {
                SignalerStatusLabel.text = "Signaler Status: Disconnected";
            };
        }

        /// <summary>
        /// Unity Engine Start() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
        /// </remarks>
        private void Start()
        {
            if (PeerEventsInstance == null)
            {
                throw new ArgumentNullException("PeerEventsInstance");
            }

            // show device label
            DeviceNameLabel.text = SystemInfo.deviceUniqueIdentifier;

            // if playerprefs has a last target id, autofill the field
            if (PlayerPrefs.HasKey(kLastTargetId))
            {
                TargetIdField.text = PlayerPrefs.GetString(kLastTargetId);
            }

            // bind our handler for creating the offer
            CreateOfferButton.onClick.AddListener(() =>
            {
                // create offer if we were given a real targetId
                if (TargetIdField.text.Length > 0)
                {
                    // cache the targetId in PlayerPrefs so we can autofill it in the future
                    PlayerPrefs.SetString(kLastTargetId, TargetIdField.text);

                    PeerEventsInstance.CreateOffer();
                }
            });

            // bind our handler so when the peer is ready, we can start local av
            PeerEventsInstance.OnPeerReady.AddListener(() =>
            {
                PeerEventsInstance.AddStream(audioOnly: false);
            });

            // bind our handler so when an offer is ready we can write it to signalling
            PeerEventsInstance.OnSdpOfferReadyToSend.AddListener((string offer) =>
            {
                signaler.SendMessageAsync(new SignalerMessage()
                {
                    MessageType = SignalerMessage.WireMessageType.Offer,
                    Data = offer,
                    TargetId = TargetIdField.text
                });
            });

            // bind our handler so when an answer is ready we can write it to signalling
            PeerEventsInstance.OnSdpAnswerReadyToSend.AddListener((string answer) =>
            {
                signaler.SendMessageAsync(new SignalerMessage()
                {
                    MessageType = SignalerMessage.WireMessageType.Answer,
                    Data = answer,
                    TargetId = TargetIdField.text
                });
            });

            // bind our handler so when an ice message is ready we can to signalling
            PeerEventsInstance.OnIceCandiateReadyToSend.AddListener((string candidate, int sdpMlineindex, string sdpMid) =>
            {
                signaler.SendMessageAsync(new SignalerMessage()
                {
                    MessageType = SignalerMessage.WireMessageType.Ice,
                    Data = candidate + "|" + sdpMlineindex + "|" + sdpMid,
                    IceDataSeparator = "|",
                    TargetId = TargetIdField.text
                });
            });

            signaler.OnMessage += (SignalerMessage msg) =>
            {
                // depending on what type of message we get, we'll handle it differently
                // this is the "glue" that allows two peers to establish a connection.
                switch (msg.MessageType)
                {
                    case SignalerMessage.WireMessageType.Offer:
                        PeerEventsInstance.SetRemoteDescription("offer", msg.Data);
                        // if we get an offer, we immediately send an answer
                        PeerEventsInstance.CreateAnswer();
                        break;
                    case SignalerMessage.WireMessageType.Answer:
                        PeerEventsInstance.SetRemoteDescription("answer", msg.Data);
                        break;
                    case SignalerMessage.WireMessageType.Ice:
                        // this "parts" protocol is defined above, in PeerEventsInstance.OnIceCandiateReadyToSend listener
                        var parts = msg.Data.Split(new string[] { msg.IceDataSeparator }, StringSplitOptions.RemoveEmptyEntries);
                        PeerEventsInstance.AddIceCandidate(parts[0], int.Parse(parts[1]), parts[2]);
                        break;
                    case SignalerMessage.WireMessageType.SetPeer:
                        // this allows a remote peer to set our text target peer id
                        // it is primarily useful when one device does not support keyboard input
                        //
                        // note: when running this sample on hololens (for example) we may use postman or a similar
                        // tool to use this message type to set the target peer. This is NOT a production-quality solution.
                        TargetIdField.text = msg.Data;
                        break;
                    default:
                        Debug.Log("Unknown message: " + msg.MessageType + ": " + msg.Data);
                        break;
                }
            };

            signaler.OnFailure += (Exception ex) =>
            {
                Debug.Log(ex.Message);
            };
        }
    }
}
