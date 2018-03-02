# Bounding Box & App Bar
![Bounding Box](/External/ReadMeImages/MRTK_AppBar_BoundingBox.jpg)
This example scene demonstrates how to make objects manipulatable using [BoundingBox](/Assets/MixedRealityToolkit/UX/Scripts/BoundingBoxes/BoundingBoxRig.cs) script. The bounding box is a standard interface for manipulating object in Windows Mixed Reality. Using gizmo on the corners and edges, you can scale or rotate the object. 
App Bar provides the button for entering/exiting 'Adjust' mode to enable/disable Bounding Box. For more information please see ['App Bar and Bounding Box'](https://developer.microsoft.com/en-us/windows/mixed-reality/app_bar_and_bounding_box) on Windows Dev Center.

## Interaction behavior ##
There are two modes in the Bounding Box: Normal mode and Adjust mode. 
### Normal mode ###
- Bounding Box is not visible. 
- App Bar shows 'Adjust' button

### Adjust mode ###
- Bounding Box and gizmo is visible
- App Bar shows 'Done' button

### Demo video ###
https://gfycat.com/EmptyLoathsomeGrouper

When you select one of the gizmo, it is highlighted and other gizmos becomes invisible. This visual feedback helps the user to understand which handle is beining interacted with.


<img src="/External/ReadMeImages/MRTK_AppBar_BoundingBox_Interaction.jpg" width="600">

## Structure of Bounding Box & App Bar ##
![Bounding Box](/External/ReadMeImages/MRTK_AppBar_BoundingBox_Structure.jpg)
### BoundingBoxRig script ###
Description here.
### BoundingBoxGizmoHandle script ###
Description here.
### AppBar and AppBarButton script ###
Description here.



## How to use Bounding Box & App Bar ##
You can enable Bounding Box and App Bar by simply assigning [BoundingBoxRig script](/Assets/MixedRealityToolkit/UX/Scripts/BoundingBoxes/BoundingBoxRig.cs) to any GameObject. **BoundingBoxRig** script contains these items.
- **BoundingBoxBasic prefab** - Visualizes boundary wireframe 
- **AppBar prefab** - Constructs App Bar
- **BoundingBoxHandle material** - Default Material for the gizmo
- **BoundingBoxGrabbed material** - Material for the gizmo when it is being interacted

![BoundingBox Script Setup](/External/ReadMeImages/MRTK_AppBar_BoundingBox_ScriptSetup.jpg)
![BoundingBox Material Setup](/External/ReadMeImages/MRTK_AppBar_BoundingBox_Materials.jpg)


## Combining with HandDraggable script ##
Bounding Box with gizmo provides the interface for scaling and rotating object. To make the object movable, you can assign [HandDraggable script](/Assets/MixedRealityToolkit/InputModule/Scripts/Utilities/Interactions/HandDraggable.cs) to the object. With HandDraggable script, you can grab the body of the object and move. You can move object in both normal mode and adjust mode since it does not require the gizmo interface.

## Combining with TwoHandManipulatable script ##
[TwoHandManipulatable script](/Assets/MixedRealityToolkit/InputModule/Scripts/Utilities/Interactions/TwoHandManipulatable.cs) allows for an object to be movable, scalable, and rotatable with one or two hands. This script can be combined with BoudningBoxRig script, providing both options for manipulating objects. You can find this interaction behavior in the cliff house. Using two motion controllers and select buttons, you can move/rotate/scale any objects without entering adjust mode. Still you can enter adjust mode by pressing the button on the AppBar and use gizmo to scale/rotate the object. For more detailed information about TwoHandManipulatable script, please refer to the [README file](/Assets/MixedRealityToolkit-Examples/Input/Readme/README_TwoHandManipulationTest.md).


## How to add custom buttons to the App Bar ##
You can add additional buttons to the App Bar for other actions. AppBar prefab provides convenient options in the Inspector panel. Find the AppBar prefab in the Project panel and select it. In the Inspector panel, expand **Buttons** section. By modifying the number of **Size** field, you will be able to see the Element populated in the Buttons section. You can specify details such as postion, name and icon texture name. These buttons are based on CompoundButton

![AppBar Custom Button](/External/ReadMeImages/MRTK_AppBar_BoundingBox_CustomButtons.jpg)



App Bar provides collection of buttons to enter and exit adjust mode. In normal mode, Bounding Box is not visible. When you enter adjust mode, you can see the Bounding Box with gizmo interface. You can exit adjust mode by pressing 'Done' button. App Bar follows user's position for easier access. 
 
To create your own Interactable Object, you can combine different types of 'CompoundButton' scripts. It is designed to support various types of Interactable Object in flexible way.
