// ----------------------------------------------------------------------------
// <copyright file="Highlighter.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Class that highlights the Photon Voice features by toggling isometric view 
// icons for the two components Recorder and Speaker.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Demos.DemoPunVoice {

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Canvas))]
    public class Highlighter : MonoBehaviour {
        private Canvas canvas;

        [SerializeField]
        private PhotonVoiceRecorder recorder;

        [SerializeField]
        private PhotonVoiceSpeaker speaker;

        [SerializeField]
        private Image recorderSprite;

        [SerializeField]
        private Image speakerSprite;

        [SerializeField]
        private Text bufferLagText;

        private bool showSpeakerLag;

        private void OnEnable() {
            ChangePOV.CameraChanged += ChangePOV_CameraChanged;
            VoiceDemoUI.DebugToggled += VoiceDemoUI_DebugToggled;
        }

        private void OnDisable() {
            ChangePOV.CameraChanged -= ChangePOV_CameraChanged;
            VoiceDemoUI.DebugToggled -= VoiceDemoUI_DebugToggled;
        }

        private void VoiceDemoUI_DebugToggled(bool debugMode) {
            showSpeakerLag = debugMode;
        }

        private void ChangePOV_CameraChanged(Camera camera) {
            canvas.worldCamera = camera;
        }

        private void Awake() {
            canvas = GetComponent<Canvas>();
            if (canvas != null && canvas.worldCamera == null) { canvas.worldCamera = Camera.main; }
        }


        // Update is called once per frame
        private void Update() {
            recorderSprite.enabled = recorder != null && recorder.IsTransmitting &&
                    PhotonVoiceNetwork.ClientState == Client.Photon.LoadBalancing.ClientState.Joined;
            speakerSprite.enabled = speaker != null && speaker.IsPlaying &&
                    PhotonVoiceNetwork.ClientState == Client.Photon.LoadBalancing.ClientState.Joined;
            bufferLagText.enabled = showSpeakerLag && speaker.IsPlaying && speaker.IsVoiceLinked;
            bufferLagText.text = string.Format("{0}", speaker.CurrentBufferLag);
        }

        private void LateUpdate() {
            if (canvas == null || canvas.worldCamera == null) { return; } // should not happen, throw error
            transform.rotation = Quaternion.Euler(0f, canvas.worldCamera.transform.eulerAngles.y, 0f); //canvas.worldCamera.transform.rotation;
        }
    }
}