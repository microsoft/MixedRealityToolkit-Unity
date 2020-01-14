# Building and deploying MRTK

To run an app on device as a standalone app (for HoloLens, Android, iOS, etc.), the build and deploy step needs to be executed in the unity project. Building and deploying an app that uses MRTK is just like building and deploying any other Unity app. There are no MRTK-specific instructions. Read below for detailed steps on how to build and deploy a Unity app for HoloLens.  Learn more about building for other platforms at [Publishing Builds](https://docs.unity3d.com/Manual/PublishingBuilds.html).

## Building and deploying MRTK to HoloLens 1 and HoloLens 2 (UWP)

Instructions on how to build and deploy for HoloLens 1 and HoloLens 2 (UWP) can be found at [building your application to device](https://docs.microsoft.com/windows/mixed-reality/mrlearning-base-ch1#build-your-application-to-your-device) .

**Tip:** When building for WMR, HoloLens 1, or HoloLens 2, it is recommended that the build settings "Target SDK Version"
and "Minimum Platform Version" look like they do in the picture below:

![Build window](../Documentation/Images/getting_started/BuildWindow.png)

The other settings can be different (for example, Build Configuration/Architecture/Build Type and others can always
be changed inside the Visual Studio solution).

Make sure that the "Target SDK Version" dropdown includes the option "10.0.18362.0" - if this is missing,
[the latest Windows SDK](https://developer.microsoft.com/windows/downloads/windows-10-sdk) needs to be installed.
