// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
	/// <summary>
	/// Changes the VR viewport to correlate to the requested quality, trying to maintain a steady framerate by reducing the amount of pixels rendered.
	/// Uses the AdaptiveQuality component to respond to quality change events.
	/// At MaxQualityLevel, the viewport will be set to 1.0 and will linearly drop of to MinViewportSize at MinQualityLevel
	/// </summary>

	public class AdaptiveViewport : MonoBehaviour
	{
		[SerializeField]
		private int MaxQualityLevel = 5;
		[SerializeField]
		private int MinQualityLevel = -5;
		[SerializeField]
		private float MinViewportSize = 0.5f;
		[SerializeField]
		private AdaptiveQuality qualityController;

		public float CurrentScale { get; private set; }

		void OnEnable()
		{
			CurrentScale = 1.0f;

			Debug.Assert(qualityController != null, "The AdpativeViewport needs a connection to a AdaptiveQuality component.");

			//Register our callback to the AdaptiveQuality component
			if (qualityController)
			{
				qualityController.QualityChanged += QualityChangedEvent;
				SetScaleFromQuality(qualityController.QualityLevel);
			}
		}

		void OnDisable()
		{
			if (qualityController)
			{
				qualityController.QualityChanged -= QualityChangedEvent;
			}
			UnityEngine.VR.VRSettings.renderViewportScale = 1.0f;
		}

		protected void OnPreCull()
		{
			UnityEngine.VR.VRSettings.renderViewportScale = CurrentScale;
		}

		public void QualityChangedEvent(int newQuality, int previousQuality)
		{
			SetScaleFromQuality(newQuality);
		}

		private void SetScaleFromQuality(int quality)
		{
			//Clamp the quality to our min and max
			int clampedQuality = Math.Min(MaxQualityLevel, Math.Max(MinQualityLevel, quality));

			//Calculate our new scale value based on quality
			float lerpVal = Mathf.InverseLerp(MinQualityLevel, MaxQualityLevel, clampedQuality);
			CurrentScale = Mathf.Lerp(MinViewportSize, 1.0f, lerpVal);
		}
	}
}
