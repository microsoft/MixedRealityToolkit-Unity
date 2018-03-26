
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace  ARCA
{
	public class ShowRecordingControls : MonoBehaviour, IHoldHandler
	{
        [Tooltip("Recording controls container")]
		public GameObject RecordingControls;

        [Tooltip("Tap and hold time to show controls")]
        public float TimeToDisplayMenu = 1.0f;

		private float heldTimer;
		private bool holding = false;

		void Start()
		{
			InputManager.Instance.AddGlobalListener(gameObject);
		}

		void HoldEventStart()
		{
			holding = true;
		}

		void HoldEventEnd()
		{
			holding = false;
			heldTimer = 0.0f;
		}

		void HoldEventCancelled()
		{
			holding = false;
			heldTimer = 0.0f;
		}

		void ShowControls()
		{
			RecordingControls.SetActive(true);
		}

		void Update()
		{
			if(holding)
			{
				heldTimer += Time.deltaTime;
			}

			if(heldTimer > TimeToDisplayMenu)
			{
				ShowControls();
				heldTimer = 0.0f;
				holding = false;
			}
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
    }
}