using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SurfacePulse
{
	public class HandPulseLogic : MonoBehaviour, IMixedRealityPointerHandler
	{
		public SurfacePulse Pulse;

		public bool bPulseOnLookAtPalms;
		public bool bPulseOnPinch;

		public float PalmFacingTime = 0.25f;
		float PalmFacingTimer = 0;

		public Vector3 PulseOriginPalms = new Vector3(0.5f, 0.5f, 0);
		public Vector3 PulseOriginFingertips = new Vector3(0, 1f, 0);

		// Start is called before the first frame update
		void Start()
		{
			MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>().RegisterHandler<IMixedRealityPointerHandler>(this);
		}

		private void OnDestroy()
		{
			MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>().UnregisterHandler<IMixedRealityPointerHandler>(this);
		}

		// Update is called once per frame
		void Update()
		{
			if (bPulseOnLookAtPalms)
			{
				if (IsAPalmFacingCamera())
				{
					if (PalmFacingTimer >= 0)
					{
						PalmFacingTimer += Time.deltaTime;
						if (PalmFacingTimer > PalmFacingTime)
						{
							PulsePalms();
							PalmFacingTimer = -1;
						}
					}
				}
				else
				{
					PalmFacingTimer = 0;
				}

			}

		}

		void PulsePalms()
		{
			Pulse.SetLocalOrigin(PulseOriginPalms);
			Pulse.PulseOnce();
		}

		void PulseFingerTips()
		{
			Pulse.SetLocalOrigin(PulseOriginFingertips);
			Pulse.PulseOnce();
		}

		private static bool IsAPalmFacingCamera()
		{
			foreach (IMixedRealityController c in CoreServices.InputSystem.DetectedControllers)
			{
				if (c.ControllerHandedness.IsMatch(Handedness.Both))
				{
					MixedRealityPose palmPose;
					var jointedHand = c as IMixedRealityHand;

					if ((jointedHand != null) && jointedHand.TryGetJoint(TrackedHandJoint.Palm, out palmPose))
					{
						if (Vector3.Dot(palmPose.Up, CameraCache.Main.transform.forward) > 0.0f)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		// IMixedRealityPointerHandler
		/// <summary>
		/// When a pointer down event is raised, this method is used to pass along the event data to the input handler.
		/// </summary>
		void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
		{
			if (bPulseOnPinch)
			{
				PulseFingerTips();
			}
		}

		/// <summary>
		/// Called every frame a pointer is down. Can be used to implement drag-like behaviors.
		/// </summary>
		void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData)
		{
		}

		/// <summary>
		/// When a pointer up event is raised, this method is used to pass along the event data to the input handler.
		/// </summary>
		void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
		{
		}

		/// <summary>
		/// When a pointer clicked event is raised, this method is used to pass along the event data to the input handler.
		/// </summary>
		void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
		{
		}
	}
}
