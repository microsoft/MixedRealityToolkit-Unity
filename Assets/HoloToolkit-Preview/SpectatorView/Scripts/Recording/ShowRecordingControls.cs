// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Controls the display of the recording UI.
    /// </summary>
    public class ShowRecordingControls : MonoBehaviour, IHoldHandler
    {
        /// <summary>
        /// Time the finger is being held down
        /// </summary>
        private float heldTimer;

        /// <summary>
        /// Is the user holding down a finger?
        /// </summary>
        private bool holding;

        /// <summary>
        /// Recording controls container
        /// </summary>
        [Tooltip("Recording controls container")]
        [SerializeField]
        private GameObject recordingControls;

        /// <summary>
        /// Tap and hold time to show controls
        /// </summary>
        [Tooltip("Tap and hold time to show controls")]
        [SerializeField]
        [Range(0.1f, 2.0f)]
        private float timeToDisplayMenu = 1.0f;

        /// <summary>
        /// Recording controls container
        /// </summary>
        public GameObject RecordingControls
        {
            get { return recordingControls; }
            set { recordingControls = value; }
        }

        /// <summary>
        /// Tap and hold time to show controls
        /// </summary>
        public float TimeToDisplayMenu
        {
            get { return timeToDisplayMenu; }
            set { timeToDisplayMenu = value; }
        }

        public void OnHoldStarted(HoldEventData eventData)
        {
            HoldEventStart();
        }

        public void OnHoldCompleted(HoldEventData eventData)
        {
            HoldEventEnd();
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
            HoldEventCancelled();
        }

        private void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        private void Update()
        {
            if (holding)
            {
                heldTimer += Time.deltaTime;
            }

            if (heldTimer > TimeToDisplayMenu)
            {
                ShowControls();
                heldTimer = 0.0f;
                holding = false;
            }
        }

        /// <summary>
        /// Holding starts
        /// </summary>
        private void HoldEventStart()
        {
            holding = true;
        }

        /// <summary>
        /// Holding stops
        /// </summary>
        private void HoldEventEnd()
        {
            holding = false;
            heldTimer = 0.0f;
        }

        /// <summary>
        /// Holding has been cancelled
        /// </summary>
        private void HoldEventCancelled()
        {
            holding = false;
            heldTimer = 0.0f;
        }

        /// <summary>
        /// Displays the recording UI controls
        /// </summary>
        private void ShowControls()
        {
            RecordingControls.SetActive(true);
        }
    }
}
