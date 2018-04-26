using System;

namespace UnityEngine.XR.iOS
{
	public enum ARErrorCode : long
	{
		/** Unsupported session configuration. */
		ARErrorCodeUnsupportedConfiguration   = 100,

		/** A sensor required to run the session is not available. */
		ARErrorCodeSensorUnavailable          = 101,

		/** A sensor failed to provide the required input. */
		ARErrorCodeSensorFailed               = 102,

		/** World tracking has encountered a fatal error. */
		ARErrorCodeWorldTrackingFailed        = 200,

	}
}

