This document describes how to prepare your work envionment to use the HoloToolkit-Unity in your project.

# 1. Setting up your development envionment
Be sure to enable Developer mode for Windows 10 via:

`Action Center -> All Settings -> Update & Security -> For Developers -> Enable Developer mode`

![Enable Developer Mode](/External/ReadMeImages/EnableDevModeWin10.PNG "Enable Developer Mode for Windows 10")

Be sure to download and install [Visual Studio 2017](https://www.visualstudio.com/vs/) and these required componenets:

![Visual Studio Components](/External/ReadMeImages/VisualStudioComponents.PNG)

If you haven't already installed Unity 3d on your machine, you should [download and install the latest version](https://unity3d.com/get-unity/download/archive) this project says it supports on the [main readme page](/README.md).

_Note: Be sure to include the Windows Store .NET scripting backend components._

![Unity Installer](/External/ReadMeImages/UnityInstaller.PNG "Unity Installer")

# 2. Download the HoloToolkit-Unity asset packages
You can download the latest unity package from [Releases](https://github.com/Microsoft/HoloToolkit-Unity/releases) folder.

### Using the source code
Optionally, If you'd like to build the HoloToolkit from the source, you'll need to clone the GitHub repository at https://github.com/Microsoft/HoloToolkit-Unity.git. If you've never cloned a repo before then you should consider using the GitHub desktop client, see https://desktop.github.com/.

### Preparing an asset package from the source code

Open the folder you just cloned in Unity.

Now, inside of Unity ensure you have the Assets folder selected in the project view, and export the package. **IMPORTANT:** Make sure you select the root Assets folder in the Project. It contains important .rsp files like csc, gmcs and smcs.

`Assets -> Export Package…`

# 3. Adding the HoloToolkit-Unity package in your project

Open or create your project in Unity.

`Assets -> Import Package -> Custom Package…` [Navigate to the package 
you have either downloaded or exported above].

- **NOTE**: If you've prepared the source code yourself, The HoloToolkit-Examples and HoloToolkit-Test folders (and all its content and subfolders) is optional when you import the custom package. You can uncheck it in the **Import Unity Package** window that shows all the contents of the package before performing the import.   

You should now have a `HoloToolkit` menu item.

_Note: This process should be repeated for the examples and test asset packages as well._

# 4. Preparing a Scene for Holographic Content
Create a new Scene: `File -> New Scene`

Remove the default `Main Camera` and `Directional Light` objects in the scene.

Add the `HoloLensCamera.prefab` (found under HoloToolkit/Input/Prefabs).

Add the `DefaultCursor.prefab` (found under HoloToolkit/Input/Prefabs/Cursor).

Create an empty object in your scene and make sure its transform is zeroed on the origin.
Rename it 'Managers'.

Add the `InputManager.prefab` (found under HoloToolkit/Input/Prefabs) as a child to your new 'Managers' Object.

Add an `Event System` to your scene by right click on 'Managers' object in your scene Hierarchy: `UI -> Event System`.

Optionally, if you wish to enable spatial mapping in your scene, you can add the `SpatialMapping.prefab` (found under HoloToolkit/SpatialMapping/Prefabs) to your 'Managers' object.  Be aware that you must also enable `Spatial Perception` Capabilities: `Edit/Project Settings/Player -> Inspector -> Publishing Settings/Capabilities`.

# 5. Building your project for HoloLens
 1. Be sure to plug in your HoloLens via usb.
 2. Navigate to the Build Window: `HoloToolkit -> Build Window`.
 3. Under `Deploy` be sure to fill out the device Username and Password fields.
 4. Under `Quick Options` press: `Build SLN, Build APPX, then Install`.

![Build Window](/External/ReadMeImages/BuildWindow.PNG)

Optionally, you can use the [default build steps if needed](https://docs.unity3d.com/Manual/windowsholographic-startup.html).

# 6. Deploying your HoloLens app using Visual Studio
 1. Navigate to the Build WInodw: `HoloToolkit -> Build Window`.
 2. Press `Open SLN`.
 3. Select **x86** in your build configuration options.
 4. In the debug toolbar, Select the emulator or the device that you're using.
 5. Run the app using the debug toolbar.
