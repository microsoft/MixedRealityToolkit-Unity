using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SurfacePulse
{
	[AddComponentMenu("Scripts/MRTK/SDK/SurfacePulse")]
	public class SurfacePulse : MonoBehaviour
	{
		[Tooltip("Shader parameter name to drive the pulse radius")]
		public string ParamName = "_Pulse_";

		[Tooltip("Shader parameter name to set the pulse origin, in local space")]
		public string OriginParamName = "_Pulse_Origin_";

		[Tooltip("How long in seconds the pulse should animate")]
		public float PulseDuration = 5f;

		[Tooltip("How long to wait in seconds between pulses, when pulsing is active")]
		public float PulseRepeatDelay = 5f;

		[Tooltip("Minimum time to wait between each pulse")]
		public float PulseRepeatMinDelay = 1f;

		[Tooltip("Automatically begin repeated pulsing")]
		public bool bAutoStart = false;

		[Tooltip("Automatically set pulse origin to the main camera location")]
		public bool bOriginFollowCamera = false;

		[Tooltip("The material to animate")]
		public Material SurfaceMat;

		// Internal state
		Coroutine RepeatPulseCoroutine;

		float pulseStartedTime;
		bool repeatingPulse;
		bool cancelPulse;


		// Reset the material property when exiting play mode so it won't be changed on disk
#if UNITY_EDITOR

		SurfacePulse()
		{
			EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
		}

		void HandleOnPlayModeChanged(PlayModeStateChange change)
		{
			// This method is run whenever the playmode state is changed.
			if (!EditorApplication.isPlaying)
			{
				// do stuff when the editor is paused.
				ResetPulseMaterial();
			}
		}

#endif //UNITY_EDITOR

		private void OnDestroy()
		{
			ResetPulseMaterial();
		}

		private void Start()
		{
			if (bAutoStart)
			{
				StartPulsing();
			}
		}

		private void Update()
		{
			if (bOriginFollowCamera)
			{
				SetLocalOrigin(CameraCache.Main.transform.position);
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////
		// Material control
		/////////////////////////////////////////////////////////////////////////////////////////
		public void SetLocalOrigin(Vector3 origin)
		{
			SurfaceMat.SetVector(OriginParamName, origin);
		}

		public void ResetPulseMaterial()
		{
			ApplyPulseRadiusToMaterial(0);
		}

		/////////////////////////////////////////////////////////////////////////////////////////
		// Pulse control
		/////////////////////////////////////////////////////////////////////////////////////////
		public void PulseOnce()
		{
			cancelPulse = false;
			StartCoroutine(CoSinglePulse());
		}

		public void StartPulsing()
		{
			repeatingPulse = true;
			cancelPulse = false;
			if (RepeatPulseCoroutine == null)
			{
				RepeatPulseCoroutine = StartCoroutine(CoRepeatPulse());
			}
		}

		public void StopPulsing(bool bFinishCurrentPulse = true)
		{
			repeatingPulse = false;
			if (!bFinishCurrentPulse)
			{
				cancelPulse = true;
				ApplyPulseRadiusToMaterial(0);
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////
		// Implementation
		/////////////////////////////////////////////////////////////////////////////////////////
		IEnumerator CoSinglePulse()
		{
			yield return CoWaitForRepeatDelay();
			if (!cancelPulse)
			{
				yield return CoAnimatePulse();
			}
		}

		IEnumerator CoRepeatPulse()
		{
			while (repeatingPulse && !cancelPulse)
			{
				yield return CoSinglePulse();
			}

			RepeatPulseCoroutine = null;
		}

		private IEnumerator CoAnimatePulse()
		{
			pulseStartedTime = Time.time;
			float t = 0;
			while (t < PulseDuration && !cancelPulse)
			{
				t += Time.deltaTime;
				ApplyPulseRadiusToMaterial(t / PulseDuration);
				yield return null;
			}
		}

		IEnumerator CoWaitForRepeatDelay()
		{
			// Wait for minimum time between pulses starting
			if (pulseStartedTime > 0)
			{
				float timeSincePulseStarted = Time.time - pulseStartedTime;
				float delayTime = PulseRepeatMinDelay - timeSincePulseStarted;
				if (delayTime > 0)
				{
					yield return new WaitForSeconds(delayTime);
				}
			}
		}

		void ApplyPulseRadiusToMaterial(float radius)
		{
			SurfaceMat.SetFloat(ParamName, radius);
		}
	}
}
