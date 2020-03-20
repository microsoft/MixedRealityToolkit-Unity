# Accessing eye tracking data in your Unity script

This article assumes one has understanding for setting up eye tracking in an MRTK scene (see [Basic MRTK setup to use eye tracking](EyeTracking_BasicSetup.md)).
Accessing eye tracking data in a MonoBehaviour script is easy! Simply use *CoreServices.InputSystem.EyeGazeProvider*.

## IMixedRealityEyeGazeProvider

Eye tracking configuration in MRTK is configured via the [`IMixedRealityEyeGazeProvider`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityEyeGazeProvider) interface. Using [CoreServices.InputSystem.EyeGazeProvider](EyeTracking_EyeGazeProvider.md) provides the default gaze provider implementation registered in the toolkit at runtime.
Useful properties of the `EyeGazeProvider` is outlined below.

- **IsEyeTrackingEnabled**:
True if user has selected to use eye tracking for gaze.

- **IsEyeCalibrationValid**:
Indicates whether the user's eye tracking calibration is valid or not.
It returns 'null', if the value has not yet received data from the eye tracking system.
It may be invalid, because the user skipped the eye tracking calibration.

- **IsEyeTrackingEnabledAndValid**:
Indicates whether the current eye tracking data is currently been used for gaze.

- **IsEyeTrackingDataValid**:
True if eye tracking data is available.
It may be unavailable due to exceeded timeout (should be robust to the user blinking though) or lack of tracking hardware or permissions.
Check out our [Missing eye calibration notification sample](EyeTracking_IsUserCalibrated.md) that explains how to detect whether a user is eye calibrated and to show an appropriate notification.

- **GazeOrigin**:
Origin of the gaze ray.
Please note that this will return the *head* gaze origin if 'IsEyeGazeValid' is false.

- **GazeDirection**:
Direction of the gaze ray.
This will return the *head* gaze direction if 'IsEyeGazeValid' is false.

- **HitInfo**, **HitPosition**, **HitNormal**, etc.:
Information about the currently gazed at target.
Again, if `IsEyeGazeValid` is false, this will be based on the user's *head* gaze.

## Examples for using CoreServices.InputSystem.EyeGazeProvider

Here is an example from the [FollowEyeGaze.cs](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.FollowEyeGaze):

- Get the point of a hologram that the user is looking at:

```c#
// Show the object at the hit position of the user's eye gaze ray with the target.
gameObject.transform.position = CoreServices.InputSystem.EyeGazeProvider.HitPosition;
```

- Showing a visual asset at a fixed distance from where the user is currently looking:

```c#
// If no target is hit, show the object at a default distance along the gaze ray.
gameObject.transform.position =
CoreServices.InputSystem.EyeGazeProvider.GazeOrigin +
CoreServices.InputSystem.EyeGazeProvider.GazeDirection.normalized * defaultDistanceInMeters;
```

## See also

- [MRTK Eye Tracking Overview](EyeTracking_Main.md)
- [MRTK Eye Tracking setup](EyeTracking_BasicSetup.md)
- [MRTK Eye Tracking Calibration](EyeTracking_IsUserCalibrated.md)
- [HoloLens 2 Eye Tracking Documentation](https://docs.microsoft.com/windows/mixed-reality/eye-tracking)
