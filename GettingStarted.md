This document describes how to prepare the HoloToolkit sourcecode for use
in your project.

# Preparing the HoloToolkit-Unity Code

We'll build the HoloToolkit from source since it's a reasonable assumption 
that you want the latest and greatest if you are reading this document. To
do this you need to clone the GitHUb repository at 
https://github.com/Microsoft/HoloToolkit-Unity.git. If you've never cloned a
repo before then you should consider using the GitHub desktop client, see 
https://desktop.github.com/

# Preparing to use the HoloToolkit-Unity package

Open the folder you just cloned in Unity

Now, inside of Unity ensure you have the Assets folder selected in the project view, and export the package

`Assets -> Export Package…`

# Using HoloToolkit-Unity in Your Project

Open or create your project in Unity

`Assets -> Import Package -> Custom Package…` [Navigate to the package 
you exported above]

You should now have a `HoloToolkit` menu item.

# Preparing a Scene for Holographic Content

Remove the default camera in the project (the next step creates a camera
 customized for holographic development)

Add the `Main Camera.prefab` (found under HoloToolkit/Utilities/Prefabs) 

You will probably want to add `ManualCameraControl.cs` (found under
 HoloToolkit/Utilities/Scripts) to the 
`Main Camera`. This allows the user to manually control
 the camera when in the Unity player.

`HoloToolkit -> Configure -> Apply HoloLens Scene Settings`

`HoloToolkit -> Configure -> Apply HoloLens Project Settings`

# Building Your Project for HoloLens

`HoloToolkit -> Build Window -> Build Visual Studio SLN`

`Open SLN`

Deploy to the emulator or device/



