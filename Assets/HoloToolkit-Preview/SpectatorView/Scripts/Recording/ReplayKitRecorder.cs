// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.iOS;
using UnityEngine.Apple.ReplayKit;
#endif

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Records and replays screencaptures recorded from the iPhone
    /// </summary>
    public class ReplayKitRecorder : MonoBehaviour
    {
        /// <summary>
        /// Controls container gameObject
        /// </summary>
        [Tooltip("Controls container gameObject")]
        [SerializeField]
        private GameObject controls;

#pragma warning disable 0414
        /// <summary>
        /// Seconds to countdown before recording
        /// </summary>
        [Tooltip("Seconds to countdown before recording")]
        [SerializeField]
        private int countDownNumber = 3;
#pragma warning restore 0414

#if UNITY_IOS
        /// <summary>
        /// Is the component preparing for recording (Counting down)
        /// </summary>
        private bool preparingForRecording;
#endif

        /// <summary>
        /// Record button gameObject
        /// </summary>
        [Tooltip("Record button gameObject")]
        public GameObject RecordButton;

        /// <summary>
        /// Recording countdown button gameObject
        /// </summary>
        [Tooltip("Recording countdown button gameObject")]
        [SerializeField]
        private GameObject recordCountdownButton;

        /// <summary>
        /// Record countdown textfield
        /// </summary>
        [Tooltip("Record countdown textfield")]
        [SerializeField]
        private Text recordCountdownText;

#if UNITY_IOS
        /// <summary>
        /// Used to check whether the component is recording or not
        /// </summary>
        private bool recording = false;
#endif

        /// <summary>
        /// Replay (preview) button gameObject
        /// </summary>
        [Tooltip("Replay (preview) button gameObject")]
        [SerializeField]
        private GameObject replayButton;

        /// <summary>
        /// Stop button gameObject
        /// </summary>
        [Tooltip("Stop button gameObject")]
        [SerializeField]
        private GameObject stopButton;

        /// <summary>
        /// Controls container gameObject
        /// </summary>
        public GameObject Controls
        {
            get { return Controls; }
            set { Controls = value; }
        }

        /// <summary>
        /// Recording countdown button gameObject
        /// </summary>
        public GameObject RecordCountdownButton
        {
            get { return recordCountdownButton; }
            set { recordCountdownButton = value; }
        }

        /// <summary>
        /// Record countdown textfield
        /// </summary>
        public Text RecordCountdownText
        {
            get { return recordCountdownText; }
            set { recordCountdownText = value; }
        }

        /// <summary>
        /// Replay (preview) button gameObject
        /// </summary>
        public GameObject ReplayButton
        {
            get { return replayButton; }
            set { replayButton = value; }
        }

        /// <summary>
        /// Stop button gameObject
        /// </summary>
        public GameObject StopButton
        {
            get { return stopButton; }
            set { stopButton = value; }
        }

        private void Start()
        {
            RecordCountdownButton.SetActive(false);
        }

        private void Update()
        {
#if UNITY_IOS
            recording = ReplayKit.isRecording;

            if (recording)
            {
                StopButton.SetActive(true);
                RecordButton.SetActive(false);
            }
            else
            {
                StopButton.SetActive(false);
            }

            // Check if theres any available recorded video
            if (ReplayKit.recordingAvailable)
            {
                ReplayButton.SetActive(true);
            }
            else
            {
                ReplayButton.SetActive(false);
            }

            if (preparingForRecording)
            {
                RecordCountdownButton.SetActive(true);
            }
            else
            {
                RecordCountdownButton.SetActive(false);
                if (!recording)
                {
                    RecordButton.SetActive(true);
                }
            }
#endif
        }

        /// <summary>
        /// Sets up the components and variables to start recording
        /// </summary>
        public void PrepareForRecording()
        {
#if UNITY_IOS
            if (!ReplayKit.APIAvailable)
            {
                return;
            }

            preparingForRecording = true;
            RecordCountdownButton.SetActive(true);
            RecordButton.SetActive(false);
            Countdown();
            RecordCountdownButton.GetComponent<Animation>().Play();
#else
            Debug.LogWarning("Not impletmenting on the current platform");
#endif
        }

        /// <summary>
        /// Displays a countdown before recording. At the end of it, it starts recording
        /// </summary>
        public void Countdown()
        {
#if UNITY_IOS
            RecordCountdownText.text = countDownNumber.ToString();
            if (countDownNumber != 0)
            {
                countDownNumber--;
                Invoke("Countdown", 1f);
            }
            else
            {
                countDownNumber = 3;
                RecordCountdownButton.SetActive(false);
                preparingForRecording = false;
                StartRecording();
            }
#else
            Debug.LogWarning("Not implemented on the current platform");
#endif
        }

        /// <summary>
        /// Starts the recording process
        /// </summary>
        public void StartRecording()
        {
#if UNITY_IOS
            if (!ReplayKit.APIAvailable)
            {
                return;
            }

            if (!recording)
            {
                ReplayKit.StartRecording(true, true);
                Controls.SetActive(false);
            }
#else
            Debug.LogWarning("Not implemented on the current platform");
#endif
        }

        /// <summary>
        /// Stops the recording process
        /// </summary>
        public void StopRecording()
        {
#if UNITY_IOS
            if (recording)
            {
                ReplayKit.StopRecording();
                RecordButton.SetActive(true);
            }
#else
            Debug.LogWarning("Not implemented on the current platform");
#endif
        }

        /// <summary>
        /// Plays the last recorded video
        /// </summary>
        public void PlayPreview()
        {
#if UNITY_IOS
            ReplayKit.Preview();
#else
            Debug.LogWarning("Not implemented on the current platform");
#endif
        }
    }
}
