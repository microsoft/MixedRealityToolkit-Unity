This document describes how to prepare the HoloToolkit sourcecode for use
in your project.

To get started either download the HoloToolkit-Unity Asset Package or grab a copy of this repository and download the entire project.

# 1a. Download a HoloToolkit-Unity Asset Package
You can download the latest unity package from [Releases](https://github.com/Microsoft/HoloToolkit-Unity/releases) folder

[Continue to step 2](/GettingStarted.md#2-using-holotoolkit-unity-in-your-project)

# 1b. Preparing the HoloToolkit-Unity Code

If you'd like to build the HoloToolkit from the source, you'll need to clone the GitHub repository at 
https://github.com/Microsoft/HoloToolkit-Unity.git. If you've never cloned a
repo before then you should consider using the GitHub desktop client, see 
https://desktop.github.com/.

# 1c. Preparing to use the HoloToolkit-Unity package

Open the folder you just cloned in Unity.

Now, inside of Unity ensure you have the Assets folder selected in the project view, and export the package. **IMPORTANT:** Make sure you select the root Assets folder in the Project. It contains important .rsp files like csc, gmcs and smcs.

`Assets -> Export Package…`

# 2. Using HoloToolkit-Unity in Your Project

Open or create your project in Unity.

`Assets -> Import Package -> Custom Package…` [Navigate to the package 
you have either downloaded or exported above]. **NOTE**: The HoloToolkit-Examples folder (and all its content and subfolders) is optional when you import the custom package. You can uncheck it in the **Import Unity Package** window that shows all the contents of the package before performing the import.   

You should now have a `HoloToolkit` menu item.

`HoloToolkit -> Configure -> Apply HoloLens Scene Settings`

# 3. Preparing a Scene for Holographic Content
Create a new Scene: `File -> New Scene`

Remove the default `Main Camera` and `Directional Light` objects in the scene.

Add the `HoloLensCamera.prefab` (found under HoloToolkit/Input/Prefabs).

Add the `DefaultCursor.prefab` (found under HoloToolkit/Input/Prefabs/Cursor).

Create an empty object in your scene and make sure its transform is zeroed on the origin.
Rename it 'Managers'.

Add the `InputManager.prefab` (found under HoloToolkit/Input/Prefabs) as a child to your new 'Managers' Object.

Add an `Event System` to your scene by right click on 'Managers' object in your scene Hierarchy: `UI -> Event System`.

Optionally, if you wish to enable spatial mapping in your scene, you can add the `SpatialMapping.prefab` (found under HoloToolkit/SpatialMapping/Prefabs) to your 'Managers' object.  Be aware that you must also enable `Spatial Perception` Capabilities: `Edit/Project Settings/Player -> Inspector -> Publishing Settings/Capabilities`.

# 4. Building Your Project for HoloLens

`HoloToolkit -> Build Window -> Build Visual Studio SLN`

`Open SLN`

Deploy to the emulator or device.

# 5. Deploying your HoloLens app using Visual Studio
 1. Select **x86** in your build configuration
 2. Select emulator or the device that you're using
 3. Run the app
