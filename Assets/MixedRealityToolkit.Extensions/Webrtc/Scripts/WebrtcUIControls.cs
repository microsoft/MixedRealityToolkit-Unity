using System;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc
{
    /// <summary>
    /// This SHOULD NOT BE USED FOR PRODUCTION.
    /// 
    /// This is a sample implementation of signalling behaviours driven by ui interactions.
    /// It is designed to behave similarly to https://webrtc.github.io/samples/src/content/peerconnection/munge-sdp/
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class WebrtcUIControls : MonoBehaviour
    {
        /// <summary>
        /// The peer event frontend instance that we will control
        /// </summary>
        [Tooltip("The peer event frontend instance to control")]
        public WebrtcPeerEvents peerEventsInstance;

        /// <summary>
        /// The button that enables local streams to be activated
        /// </summary>
        /// <remarks>
        /// This effectively enables local av
        /// </remarks>
        [Tooltip("The button that enables local stream to be activated")]
        public Button addLocalStream;

        /// <summary>
        /// The button that creates an sdp offer
        /// </summary>
        [Tooltip("The button that creates an sdp offer")]
        public Button createOffer;

        /// <summary>
        /// The text field that will display the sdp offer
        /// </summary>
        [Tooltip("The text field that will display the sdp offer")]
        public InputField offerText;

        /// <summary>
        /// The button that creates an sdp answer
        /// </summary>
        /// <remarks>
        /// A value should be set in <see cref="offerText"/> before calling
        /// </remarks>
        [Tooltip("The button that creates an sdp answer")]
        public Button createAnswer;

        /// <summary>
        /// The text field that will display the sdp offer
        /// </summary>
        [Tooltip("The text field that will display the sdp offer")]
        public InputField answerText;

        /// <summary>
        /// The text field that will display local trickle-ice candidates as they are tested
        /// </summary>
        [Tooltip("The text field that will display local trickle-ice candidates as they are tested")]
        public InputField localCandidates;

        /// <summary>
        /// The text field that accepts remote ice candidates
        /// </summary>
        /// <remarks>
        /// Needed for network negotiations
        /// </remarks>
        [Tooltip("The text field that accepts remote ice candidates")]
        public InputField remoteCandidates;

        /// <summary>
        /// The button that submits the data entered in <see cref="remoteCandidates"/>
        /// </summary>
        [Tooltip("The button that submits the data entered in remoteCandidates")]
        public Button submitRemoteCandidates;

        /// <summary>
        /// Unity Engine Start() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
        /// </remarks>
        private void Start()
        {
            if (peerEventsInstance == null)
            {
                throw new ArgumentNullException("peerEventsInstance");
            }

            // ensure there is no data in our text fields to start
            answerText.text = "";
            offerText.text = "";
            localCandidates.text = "";
            remoteCandidates.text = "";

            // bind our handler for enabling the local stream
            addLocalStream.onClick.AddListener(() =>
            {
                peerEventsInstance.AddStream(audioOnly: false);
            });

            // bind our handler for creating offers
            createOffer.onClick.AddListener(() =>
            {
                peerEventsInstance.CreateOffer();
            });

            // bind our handler for when sdp offers are produced
            peerEventsInstance.OnSdpOfferReadyToSend.AddListener((string offer) =>
            {
                // display it in the given inputfield
                offerText.text = offer;
            });

            // bind our handler for creating answers
            createAnswer.onClick.AddListener(() =>
            {
                // answers depend on offers - so we first set the remote offer description
                if (offerText.text.Length > 0)
                {
                    peerEventsInstance.SetRemoteDescription("offer", offerText.text);
                }

                // then we create the offer
                peerEventsInstance.CreateAnswer();
            });

            // bind our handler for when sdp answers are produced
            peerEventsInstance.OnSdpAnswerReadyToSend.AddListener((string answer) =>
            {
                // display it in the given inputfield
                answerText.text = answer;
            });

            // bind our handler for when an ice candidate is produced
            peerEventsInstance.OnIceCandiateReadyToSend.AddListener((string candidate, int sdpMlineindex, string sdpMid) =>
            {
                // define "protocol" for candidate serialization
                // the reciever must know how to parse this format as well
                localCandidates.text += candidate + "|" + sdpMlineindex + "|" + sdpMid + "\r\n";
            });

            // bind our handler for when remote ice candidates are input and submitted
            submitRemoteCandidates.onClick.AddListener(() =>
            {
                var lines = remoteCandidates.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    // note: this "protocol" is defined above, see `localCandidates.text +=`
                    var entry = line.Split('|');
                    var candidate = entry[0];
                    var sdpMlineindex = int.Parse(entry[1]);
                    var sdpMid = entry[2];

                    // add the candidate to the underlying peer
                    peerEventsInstance.AddIceCandidate(candidate, sdpMlineindex, sdpMid);
                }
            });
        }
    }
}
