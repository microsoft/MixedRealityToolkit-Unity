This document describes how to prepare the HoloToolkit sourcecode for use
in your project.

# 1. Download a HoloToolkit-Unity Asset Package
You can download archived or daily builds of the HoloToolkit-Unity Asset Packages from the [HoloToolkit Archive](https://holotoolkit.download/) Rafael Rivera has so kindly put together for us.

Once downloaded you can skip down to step 4.

# 2. Preparing the HoloToolkit-Unity Code

If you'd like to build the HoloToolkit from the source, you'll need to clone the GitHUb repository at 
https://github.com/Microsoft/HoloToolkit-Unity.git. If you've never cloned a
repo before then you should consider using the GitHub desktop client, see 
https://desktop.github.com/.

# 3. Preparing to use the HoloToolkit-Unity package

Open the folder you just cloned in Unity.

Now, inside of Unity ensure you have the Assets folder selected in the project view, and export the package. **IMPORTANT:** Make sure you select the root Assets folder in the Project. It contains important .rsp files like csc, gmcs and smcs.

`Assets -> Export Package…`

# 4. Using HoloToolkit-Unity in Your Project

Open or create your project in Unity.

`Assets -> Import Package -> Custom Package…` [Navigate to the package 
you have either downloaded or exported above]. **NOTE**: The HoloToolkit-Examples folder (and all its content and subfolders) is optional when you import the custom package. You can uncheck it in the **Import Unity Package** window that shows all the contents of the package before performing the import.   

You should now have a `HoloToolkit` menu item.

# 5. Preparing a Scene for Holographic Content

Remove the default camera in the project (the next step creates a camera
 customized for holographic development).

Add the `Main Camera.prefab` (found under HoloToolkit/Utilities/Prefabs).

You will probably want to add `ManualCameraControl.cs` (found under
 HoloToolkit/Utilities/Scripts) to the 
`Main Camera`. This allows the user to manually control
 the camera when in the Unity player.

`HoloToolkit -> Configure -> Apply HoloLens Scene Settings`

`HoloToolkit -> Configure -> Apply HoloLens Project Settings`

# 6. Building Your Project for HoloLens

`HoloToolkit -> Build Window -> Build Visual Studio SLN`

`Open SLN`

Deploy to the emulator or device.

# 7. Deploying your HoloLens app using Visual Studio
 1. Select **x86** in your build configuration
 2. Select emulator or the device that you're using
 3. Run the app



