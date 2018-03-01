# Bounding Box & App Bar
![Bounding Box](/External/ReadMeImages/MRTK_AppBar_BoundingBox.jpg)
This example scene demonstrates how to make objects manipulatable using [BoundingBox](/Assets/MixedRealityToolkit/UX/Scripts/BoundingBoxes/BoundingBoxRig.cs) script. The bounding box is a standard interface for manipulating object in Windows Mixed Reality. Using gizmo on the corners and edges, you can scale or rotate the object. 
App Bar provides the button for entering/exiting 'Adjust' mode to enable/disable Bounding Box. For more information please see ['App Bar and Bounding Box'](https://developer.microsoft.com/en-us/windows/mixed-reality/app_bar_and_bounding_box) on Windows Dev Center.

There are two modes in the Bounding Box: Normal mode and Adjust mode. 
### Normal mode ###
- Bounding Box is not visible. 
- App Bar shows 'Adjust' button

### Adjust mode ###
- Bounding Box and gizmo is visible
- App Bar shows 'Done' button

When you select one of the gizmo, it is highlighted and other gizmos becomes invisible. This visual feedback helps the user 


<img src="/External/ReadMeImages/MRTK_AppBar_BoundingBox_Interaction.jpg" width="600">

## How to use Bounding Box & App Bar ##
You can enable Bounding Box and App Bar by simply assigning [BoundingBoxRig script](/Assets/MixedRealityToolkit/UX/Scripts/BoundingBoxes/BoundingBoxRig.cs) to any GameObject. After assigning **BoundingBoxRig** script, you need to assign prefabs and materials. Below screenshot shows the items that need to be assigned.
- **BoundingBoxBasic prefab** 
- **AppBar prefab** 
- **BoundingBoxHandle material**
- **BoundingBoxGrabbed material**

![BoundingBox Script Setup](/External/ReadMeImages/MRTK_AppBar_BoundingBox_ScriptSetup.jpg)
![BoundingBox Material Setup](/External/ReadMeImages/MRTK_AppBar_BoundingBox_Materials.jpg)





App Bar provides collection of buttons to enter and exit adjust mode. In normal mode, Bounding Box is not visible. When you enter adjust mode, you can see the Bounding Box with gizmo interface. You can exit adjust mode by pressing 'Done' button. App Bar follows user's position for easier access. 
 
To create your own Interactable Object, you can combine different types of 'CompoundButton' scripts. It is designed to support various types of Interactable Object in flexible way.
