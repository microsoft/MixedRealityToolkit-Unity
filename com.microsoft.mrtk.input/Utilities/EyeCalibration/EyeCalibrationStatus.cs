// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
	/// <summary>
	/// Used to track the current eye calibration status. 
	/// </summary>
	public enum EyeCalibrationStatus
	{
		/// <summary>
		/// The eye calibration status is not defined. 
		/// </summary>
		Undefined,

		/// <summary>
		/// The eye calibration status could not be retrieved because this is an unsupported device.
		/// </summary>
		Unsupported,

		/// <summary>
		/// The eye calibration status could not be retrieved because eyes are not being tracked.
		/// This usually occurs when SpatialPointerPose's Eyes property is null. 
		/// </summary>
		NotTracked,

		/// <summary>
		/// The eye calibration status was retrieved and eyes are not calibrated. 
		/// </summary> 
		NotCalibrated,

		/// <summary>
		/// The eye calibration status was retrieved and eyes are calibrated. 
		/// </summary>
		Calibrated
	};
}
