This document describes how to prepare the HoloToolkit sourcecode for use
in your project.

To get started either download the HoloToolkit-Unity Asset Package or grab a copy of this repository and download the entire project.

# 1a. Download a HoloToolkit-Unity Asset Package
You can download archived or daily builds of the HoloToolkit-Unity Asset Packages from the [HoloToolkit Archive](https://holotoolkit.download/) Rafael Rivera has so kindly put together for us.

Continue to step 2.

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

# 3. Preparing a Scene for Holographic Content

Remove the default camera in the project (the next step creates a camera
 customized for holographic development).

Add the `HoloLensCamera.prefab` (found under HoloToolkit/Input/Prefabs).

`HoloToolkit -> Configure -> Apply HoloLens Scene Settings`

`HoloToolkit -> Configure -> Apply HoloLens Project Settings`

# 4. Building Your Project for HoloLens

`HoloToolkit -> Build Window -> Build Visual Studio SLN`

`Open SLN`

Deploy to the emulator or device.

# 5. Deploying your HoloLens app using Visual Studio
 1. Select **x86** in your build configuration
 2. Select emulator or the device that you're using
 3. Run the app
