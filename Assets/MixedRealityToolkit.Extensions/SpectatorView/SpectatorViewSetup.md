# Requirements
### HoloLens
1. Windows PC
2. HoloLens
3. [Visual Studio 2017](https://visualstudio.microsoft.com/vs/) installed on the PC
4. [Unity](https://unity3d.com/get-unity/download) installed on the PC
5. [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) Unity Package

### iOS
1. Mac
2. ARM64 iOS Device that supports [AR Kit](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/DeviceCompatibilityMatrix/DeviceCompatibilityMatrix.html)
3. [Unity](https://unity3d.com/get-unity/download) installed on the Mac
4. [XCode](https://developer.apple.com/xcode/) installed on the Mac
5. Obtain an [apple developer license](https://developer.apple.com/programs/enroll/)
6. [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) Unity Package
7. [ARKit XR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.arkit@1.0/manual/index.html) Unity Package

### Android
1. Windows PC or Mac
2. Android Device that supports [AR Core](https://developers.google.com/ar/discover/supported-devices)
3. [Android Studio](https://developer.android.com/studio)
4. [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) Unity Package
5. [ARCore XR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.arcore@1.0/manual/index.html) Unity Package

# Example Unity Scene
Spectator View currently has two example scenes. It may be worth compiling and running one of said scenes before attempting to setup your own Spectator View experience.

1) **HoloLensExampleMRTKScene.unity** - An example scene that is configured to use the core MRTK functionality
2) **HoloLensExampleScene.unity** - An example scene that has no dependencies on the core MRTK functionality
>>Note both scenes can be found at Assets/MixedRealityToolkit.Extensions/SpectatorView/Scenes/

# Setting Up New Projects

## HoloLens
1) Install the [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) Unity Package through Unity's package manager
2) Add the 'Spectator View - HoloLens' prefab (Assets/MixedRealityToolkit.Extensions/SpectatorView/Prefabs/Spectator View - HoloLens.prefab) to your unity scene.
3) Place all content in your scene under an empty parent GameObject
4) Add the [SceneRoot MonoBehaviour](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.SceneRoot) to your empty parent GameObject

## HoloLens 2
coming soon...

# Building
Spectator View contains a [PlatformSwitcherEditor](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor.PlatformSwitcherEditor) Unity editor component that can assist in switching between platforms. Toggling platforms with said component should apply any needed build configuration settings as well as any platform specific capabilities.

![Spectator View Platform Switcher](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/wikiFiles/Documentation/images/spectatorViewPlatformSwitcher.png)

## HoloLens
### Build the native plugin in Visual Studio
The HoloLens flavor of spectator view requires building/obtaining SpectatorViewPlugin.dll and all of its dependencies:
1) Clone https://github.com/Microsoft/MixedRealityToolkit
2) Checkout feature/spectatorView in the MixedRealityToolkit (not the MixedRealityToolkit-Unity repo)
3) Compile and obtain the associated opencv dependencies via vcpkg by following these [instructions](https://github.com/Microsoft/MixedRealityToolkit/tree/feature/spectatorView/SpectatorViewPlugin)
3) Build a x86 Release version of SpectatorViewPlugin in visual studio with [SpectatorViewPlugin.sln](https://github.com/Microsoft/MixedRealityToolkit/blob/feature/spectatorView/SpectatorViewPlugin/SpectatorViewPlugin/SpectatorViewPlugin.sln) after obtaining the opencv dependencies
4) Copy all output binaries into a Plugins\WSA\x86 directory in your unity assets folder (Typically the output binaries are dropped into MixedRealityToolkit\SpectatorViewPlugin\SpectatorViewPlugin\Release\SpectatorViewPlugin\ by visual studio, but this folder may vary between different local development setups\visual studio configurations)

### Build the unity scene
First, obtain the [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) Unity Package through Unity's package manager. Then, use the [PlatformSwitcherEditor](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor.PlatformSwitcherEditor) to specify the HoloLens platform, Spectator View shouldn't require any additional customization when compared to other HoloLens projects. To generate a visual studio solution with Unity use File -> Build Settings -> Build. Then open the generated solution in visual studio, compile and deploy.

![Spectator View HoloLens Build Settings](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/wikiFiles/Documentation/images/spectatorViewHoloLensBuildSettings.png)

## iOS
For iOS, you will need a Mac with both Unity and XCode installed. After opening your Unity project, Install the [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) and [ARKit XR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.arkit@1.0/manual/index.html) Unity Packages using Unity's Package Manager. Then, use the [PlatformSwitcherEditor](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor.PlatformSwitcherEditor) component to configure the project for iOS. Then select File -> Build Settings -> Build. You can then open the generated solution in XCode to compile and deploy.

![Spectator View iOS Build Settings](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/wikiFiles/Documentation/images/spectatorViewIOSBuildSettings.png)

Within XCode, you will need to configure a [signing certificate](https://developer.apple.com/support/code-signing/) for your application before compiling and deploying. You can do this by selecting your Unity generated XCode project and updating the signing information.

## Android
Unlike iOS, an Android Spectator View experience can be compiled and deployed entirely from a PC. However, for Spectator View's android recording component to work, you will need to export the project from Unity and compile/deploy the experience with Android Studio. Unity did not have an obvious choice for Android screen capture when beginning the Android Spectator View experience. A custom activity, [ScreenRecorderActivity.java](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/feature/spectatorView/Assets/MixedRealityToolkit.Extensions/ScreenRecording/Plugins/Android), was created to bridge this feature gap. For more information on setting up/using custom android activities, see [Unity's documentation](https://docs.unity3d.com/Manual/AndroidUnityPlayerActivity.html).

First, obtain the [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html) and [ARCore XR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.arcore@1.0/manual/index.html) Unity Packages using Unity's Package Manager. Then configure your project for Android, by again using the [PlatformSwitcherEditor](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor.PlatformSwitcherEditor). To build the android studio solution, select File -> Build Settings. Select the 'Export Project' option and press build. 

![Spectator View Android Build Settings](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/wikiFiles/Documentation/images/spectatorViewAndroidBuildSettings.png)

After selecting the Android platform with the platform switcher component, open Unity's build settings. Select in the android build menu to 'Export Project'. Then press the 'Export' button. This will generate an android solution that can be opened, compiled and deployed in Android Studio. However, when building in Android Studio, make sure that your AndroidManifest.xml declares ScreenRecorderActivity as the primary activity compared to the default UnityPlayerActivity. Also ensure that android.permission.RECORD_AUDIO and android.permission.WRITE_EXTERNAL_STORAGE have been added to your AndroidManifest.xml

![Spectator View Android Studio Manifest](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/wikiFiles/Documentation/images/spectatorViewAndroidStudioManifest.png)
