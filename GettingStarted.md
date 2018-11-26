This document describes how to prepare your work environment to use MixedRealityToolkit-Unity in your Unity 3D project.

# 1. Check your Windows 10 version
It is recommended to be running the Windows 10 Fall Creators update for modern Windows Universal apps targeting Mixed Reality Headsets

This can be verified by running the "WinVer" application from the Windows Run command

`Windows Key + R -> WinVer`

Which will display a new window as follows:

![Windows Version dialog](/External/ReadMeImages/WindowsVersionFCU.png)

If you are not running the Windows 10 Fall Creators update, then you will need to Update your version of Windows.

# 2. Setting up your development environment
Be sure to enable Developer mode for Windows 10 via:

`Action Center -> All Settings -> Update & Security -> For Developers -> Enable Developer mode`

![Enable Developer Mode](/External/ReadMeImages/EnableDevModeWin10.PNG "Enable Developer Mode for Windows 10")

If you have not already, download and install [Visual Studio 2017](https://www.visualstudio.com/vs/) and these required components:

- Windows Universal Platform Development Workload
- Windows SDK 10.16299.10
- Visual Studio Tools for Unity
- msbuild
- Nuget Package Manager

![Visual Studio Components](/External/ReadMeImages/VisualStudioComponents.PNG)

You can install more components and UWP SDK's as you wish.

Make sure you are running the appropriate version of Unity 3D on your machine. You should [download and install the latest version](https://unity3d.com/get-unity/download/archive) this project says it supports on the [main readme page](/README.md).

[unity-release]:             https://unity3d.com/unity/qa/patch-releases/2017.2.1p2
[unity-version-badge]:       https://img.shields.io/badge/Unity%20Editor-2017.2.1p2-green.svg

> The Mixed Reality Toolkit now recommends the following Unity 3D version:
> [![Github Release][unity-version-badge]][unity-release] 

_Note: Be sure to include the Windows Store .NET scripting backend components._

![Unity Installer](/External/ReadMeImages/UnityInstaller.PNG "Unity Installer")

# 2. Download the MixedRealityToolkit-Unity asset packages
You can download the latest unity package from [Releases](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases) folder.

_Note: The latest release should work for both HoloLens and Windows Mixed Reality development._

[unity-release1]:                https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/latest
[mrtk-version-badge]:            https://img.shields.io/github/tag/microsoft/MixedRealityToolkit-unity.svg?style=flat-square&label=Latest%20Master%20Branch%20Release&colorB=007ec6
[![Github Release][mrtk-version-badge]][unity-release1]

### Using the source code
Optionally, If you'd like to build the Mixed Reality Toolkit from the source, you'll need to clone the GitHub repository from:

>  ## https://github.com/Microsoft/MixedRealityToolkit-Unity.git. 

If you've never cloned a repo before then you should consider using the GitHub desktop client, see https://desktop.github.com/ for more information.

> ### (optional) Preparing an asset package from the source code
> Open the folder you just cloned in Unity.
> Now, inside of Unity ensure you have the Assets folder selected in the project view, and export the package. 
>
> **IMPORTANT:** Make sure you select the root Assets folder in the Project. It contains important .rsp files like csc, gmcs and smcs.
>
> `Assets -> Export Package…`

# 3. Adding the MixedRealityToolkit-Unity package in your project

Open or create your project in Unity.

Then import the MRTK asset using `Assets -> Import Package -> Custom Package…` [Navigate to the package 
you have either downloaded or exported above].

> **NOTE**: If you've prepared the source code yourself, The HoloToolkit-Examples and HoloToolkit-Test folders (and all its content and subfolders) are optional when you import the custom package. You can uncheck those folders in the **Import Unity Package** window that shows all the contents of the package before performing the import.   

You should now have a `Mixed Reality Toolkit` menu item in the Unity editor.

_Note: This process should be repeated for the examples and test asset packages as well._

# 4. Preparing your project for Mixed Reality Content

Select the "Apply Mixed Reality Project Settings" option in the Unity Editor: 

` Mixed Reality Toolkit -> Configure -> Apply Mixed Reality Project Settings`

![MRTK Editor Project Settings](/External/ReadMeImages/MixedRealityProjectEditorOption.png)

Select all the required options for your Project type:

![MRTK Project Options](/External/ReadMeImages/MixedRealityProjectOptions.png)

Check:
* For Immersive headsets, check the **Target Occluded Devices** option
* For HoloLens, leave this option *Unchecked*

> **Note**
> If you enable the **Enable Xbox Controller Support** option, this will download the ProjectSettings Input file and replace your current version in your project and override any existing input settings, the old file will be renamed to "*.Old*".
> Else, you will have to setup all the MRTK input axis manually.

# 5. Preparing a Scene for Mixed Reality Content

Select the "Apply Mixed Reality Scene Settings" option in the Unity Editor: 

` Mixed Reality Toolkit -> Configure -> Apply Mixed Reality Scene Settings`

![MRTK Editor Scene Settings](/External/ReadMeImages/MixedRealityEditorSceneSettings.png)

Select all the required options for your Scene type:

![MRTK Scene options](/External/ReadMeImages/MixedRealitySceneOptions.png)

Alternatively, you can setup your scene manually as follows:

1. Create a new Scene: `File -> New Scene`

2. Remove the default `Main Camera` and `Directional Light` objects in the scene.

3. Add the `MixedRealityCameraParent.prefab` (found under HoloToolkit/Input/Prefabs). Check the configured options for the Parent and child camera meet your requirements.

4. Add the `InputManager.prefab` (found under HoloToolkit/Input/Prefabs), which will add a new **InputManager** Prefab which contains the all-important UI **EventSystem** object.

5. Add the `DefaultCursor.prefab` (found under HoloToolkit/Input/Prefabs/Cursor) and add that Object to the **InputManager** Cursor parameter (to avoid it being searched for on scene start) 

> **For Hololens**
> Optionally, if you wish to enable spatial mapping in your scene for HoloLens, you can add the `SpatialMapping.prefab` (found under HoloToolkit/SpatialMapping/Prefabs) to your 'Managers' object.  Be aware that you must also enable `Spatial Perception` Capabilities: `Edit/Project Settings/Player -> Inspector -> Publishing Settings/Capabilities`.

# 5 Building your project 
The MRTK provides you a quick and easy way to generate your Unity project from a custom window, which can be found under:

`Mixed Reality Toolkit -> Build Window`

![MRTK Build Window](/External/ReadMeImages/MixedRealityEditorBuildWindow.png)

This window offers many quick options to be able to:

* Build your UWP C# solution
* Build your project APPX
* (HoloLens) Deploy your project to a remote device

![Build Window](/External/ReadMeImages/BuildWindow.PNG)

_Note: You should always target the lastest Windows SDK in all builds._

## 5.1 Running your project for **Immersive Headsets**
Unity supports running your Immersive solution direct from the editor **BUT** Only when the Mixed Reality Portal is running.
> Unity may resolve this in the future and auto-start the portal, but it is better to have it running beforehand!

1. Start the MR Portal
2. Plug-in and wake up the headset or [start the Simulator](https://developer.microsoft.com/en-us/windows/mixed-reality/using_the_windows_mixed_reality_simulator)
3. Make sure the "Cliffhouse" is displayed in the Headset
4. Select the scene you want to run
5. Hit Play in the Unity Editor

Provided everything was good, you should now see your scene running in the headset/simulator.

Alternatively, either Use the Unity Build options or the MRTK Build window to generate the UWP package and run locally on your machine.

## 5.2 Running your project for **HoloLens**

 1. Be sure to plug in your HoloLens via usb.
 2. Open the above Build Window: `Mixed Reality Toolkit -> Build Window`.
 3. Under `Deploy` be sure to fill out the device **Username** and **Password** fields.
 4. In the `Quick Options` section, press the "`Build SLN, Build APPX, then Install`" button to deploy to HoloLens.

> Optionally, you can use the [default build steps if needed](https://docs.unity3d.com/Manual/windowsholographic-startup.html).

# 6. Deploying your project
Once you are ready to deploy to a platform and have built your UWP project, you need to get it on to the device to run, package and ship it to the store.

## 6.1 Deploying your **Immersive** app using Visual Studio

 1. Navigate to the Build Window: `Mixed Reality Toolkit -> Build Window`.
 2. Press `Open Project Solution` to open the project in Visual Studio.
 3. Select **x64** in your build configuration options.
 4. In the debug toolbar, select "Local Machine" as the target. (or remote device if you are pushing to another machine)
 5. Run the app by hitting play.

 See the [Windows Developer site](https://developer.microsoft.com/en-us/store/publish-apps) for publishing to the Microsoft Store.


## 6.2 Deploying your **HoloLens** app using Visual Studio

 1. Navigate to the Build Window: `Mixed Reality Toolkit -> Build Window`.
 2. Press `Open Project Solution` to open the project in Visual Studio.
 3. Select **x86** in your build configuration options.
 4. In the debug toolbar, select the emulator or the device that you're using.
 5. Run the app using the debug toolbar.
