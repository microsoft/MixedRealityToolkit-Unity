# Building and Deploying MRTK
To run an app on device as a standalone app (for HoloLens, Android, iOS, etc.), the build and deploy step needs to be executed in the unity project. Building and deploying an app that uses MRTK is just like building and deploying any other Unity app. There are no MRTK-specific instructions. Read below for detailed steps on how to build and deploy a Unity app for HoloLens.  Learn more about building for other platforms at [Publishing Builds](https://docs.unity3d.com/Manual/PublishingBuilds.html).

### Building and deploying MRTK to HoloLens 1 and HoloLens 2 (UWP)
As mentioned above, building an app that uses MRTK is just like building any other UWP app, there's nothing MRTK specific. You can follow instructions at [building your application to device](https://docs.microsoft.com/en-us/windows/mixed-reality/mrlearning-base-ch1#build-your-application-to-your-device)  for more information on how to build and deploy your app to HoloLens.

**Tip:** When building for WMR, HoloLens 1, or HoloLens 2, it is recommended that your build settings "Target SDK Version"
and "Minimum Platform Version" look like they do in the picture below:

![](../Documentation/Images/getting_started/BuildWindow.png)

The other settings can be different (for example, Build Configuration/Architecture/Build Type and others can always
be changed inside the Visual Studio solution).

Make sure that you click on "Target SDK Version" and that it has the option "10.0.18362.0" - if this is missing,
it means you have to install [the latest Windows SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk).
