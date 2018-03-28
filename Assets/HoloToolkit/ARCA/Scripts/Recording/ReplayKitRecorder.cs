// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
#if PLATFORM_IOS
using UnityEngine.iOS;
using UnityEngine.Apple.ReplayKit;
#endif
using UnityEngine.UI;

namespace HoloToolkit.ARCapture
{
    public class ReplayKitRecorder : MonoBehaviour
    {
        [Tooltip("Record button gameObject")]
		public GameObject RecordButton;

        [Tooltip("Recording countdown button gameObject")]
        public GameObject RecordCountdownButton;

        [Tooltip("Record countdown textfield")]
        public Text RecordCountdownText;

        [Tooltip("Stop button gameObject")]
        public GameObject StopButton;

        [Tooltip("Controls container gameObject")]
        public GameObject Controls;

        [Tooltip("Replay (preview) button gameObject")]
        public GameObject ReplayButton;

		private string lastError = "";
		private bool preparingForRecording = false;
		private bool recording = false;
		private int countDownNumber = 3;

		void Start()
		{
            RecordCountdownButton.SetActive(false);
		}

		void Update()
		{
            #if PLATFORM_IOS
			recording = ReplayKit.isRecording;

			if(recording)
			{
				StopButton.SetActive(true);
				RecordButton.SetActive(false);
			}
			else
			{
				StopButton.SetActive(false);
			}

			// Check if theres any available recorded video
			if(ReplayKit.recordingAvailable)
			{
				ReplayButton.SetActive(true);
			}
			else
			{
				ReplayButton.SetActive(false);
			}

			if(preparingForRecording)
			{
				RecordCountdownButton.SetActive(true);
			}
			else
			{
				RecordCountdownButton.SetActive(false);
				if(!recording)
				{
					RecordButton.SetActive(true);
				}
			}
        #endif
        }

        public void PrepareForRecording()
		{
            #if PLATFORM_IOS
			if(!ReplayKit.APIAvailable)
			{
				return;
			}

			preparingForRecording = true;
			RecordCountdownButton.SetActive(true);
			RecordButton.SetActive(false);
			Countdown();
			RecordCountdownButton.GetComponent<Animation>().Play();
            #endif
        }

        public void Countdown()
		{
			RecordCountdownText.text = countDownNumber.ToString();
			if(countDownNumber >= 1)
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
		}

		public void StartRecording()
		{
            #if PLATFORM_IOS
			if(!ReplayKit.APIAvailable)
            {
				return;
            }

			if(!recording)
			{
				ReplayKit.StartRecording(true, true);
				Controls.SetActive(false);
			}
            #endif
        }

        public void StopRecording()
		{
            #if PLATFORM_IOS
			if(recording)
			{
				ReplayKit.StopRecording();
				RecordButton.SetActive(true);
			}
            #endif
        }

        public void PlayPreview()
		{
			#if PLATFORM_IOS
			ReplayKit.Preview();
			#endif
		}
    }
}
