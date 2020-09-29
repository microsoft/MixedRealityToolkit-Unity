# App bar

![App bar](../Documentation/Images/AppBar/MRTK_AppBar_Main.png)

App bar is a UI component that is used together with the [bounds control](README_BoundsControl.md) script. It adds button controls to an object with the intent to manipulate it. Using the 'Adjust' button, the bounds control interface for an object can be de- / activated. The "Remove" button should remove the object from the scene.

## How to use app bar

Drag and drop `AppBar` (Assets/MRTK/SDK/Features/UX/Prefabs/AppBar/AppBar.prefab) into the scene hierarchy. In the inspector panel of the component, assign any object with a bounds control as the *Target Bounding Box* to add the app bar to it.

**Important:** The bounds control activation option for the target object should be 'Activate Manually'.

<img src="../Documentation/Images/AppBar/MRTK_AppBar_Setup1.png" width="450">

<img src="../Documentation/Images/AppBar/MRTK_AppBar_Setup2.png" width="450">
