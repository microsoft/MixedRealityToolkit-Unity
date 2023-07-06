// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
	/// <summary>
	/// Event arguments used when eye tracking status changes. 
	/// </summary>
	[Serializable]
	public struct EyeCalibrationStatusEventArgs
	{
		/// <summary>
		/// The eye tracking calibration status. 
		/// </summary>
		public EyeCalibrationStatus CalibratedStatus { get; set; }

		/// <summary>
		/// Constructor which sets CalibratedStatus.
		/// </summary>
		public EyeCalibrationStatusEventArgs(EyeCalibrationStatus calibratedStatus)
		{
			CalibratedStatus = calibratedStatus;
		}
	}

	/// <summary>
	/// Event fired whenever eye tracking status changes. Passes the new status as an argument.
	/// </summary>
	[Serializable]
	public class EyeCalibrationStatusEvent : UnityEvent<EyeCalibrationStatusEventArgs> { }
}