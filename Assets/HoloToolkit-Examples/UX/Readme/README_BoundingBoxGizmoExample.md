# Bounding Box & App Bar
![Bounding Box](/External/ReadMeImages/MRTK_AppBar_BoundingBox.jpg)
This example scene demonstrates how to make objects manipulatable using [BoundingBoxRig](/Assets/HoloToolkit/UX/Scripts/BoundingBoxes/BoundingBoxRig.cs) script. The bounding box is a standard interface for manipulating object in Windows Mixed Reality. Using gizmo on the corners and edges, you can scale or rotate the object. 
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
https://gfycat.com/TameQuaintGnu

When you select one of the gizmo, it is highlighted and other gizmos becomes invisible. This visual feedback helps the user to understand which handle is beining interacted with.


<img src="/External/ReadMeImages/MRTK_AppBar_BoundingBox_Interaction.jpg" width="600">

## Example scene ##
[BoundingBoxExample.unity](/Assets/HoloToolkit-Examples/UX/Scenes/BoundingBoxGizmoExample.unity)

<img src="/External/ReadMeImages/MRTK_AppBar_BoundingBox_ExampleScene.jpg" width="600">



## Structure of Bounding Box & App Bar ##
![Bounding Box](/External/ReadMeImages/MRTK_AppBar_BoundingBox_Structure.jpg)
### [BoundingBoxRig script](/Assets/HoloToolkit/UX/Scripts/BoundingBoxes/BoundingBoxRig.cs) ###
This script gets added as a Component to a GameObject. An object with this script as a Component will appear with an AppBar floating in front of it. The BoundingBoxRig script creates a rig of corner and mid-edge handles for scaling and rotating an object. The Adjust button in the AppBar turns on and off the rig. This script cooperates with the TwoHandedManipulation script.

You can also watch the activity of a bounding box rig by using the `IBoundingBoxStateHandler` interface, which enables you to see whether a rig is being activated or deactivated through the `OnBoundingBoxRigActivated` and `OnBoundingBoxRigDeactivated` hooks.


### [BoundingBoxGizmoHandle script](/Assets/HoloToolkit/UX/Scripts/BoundingBoxes/BoundingBoxGizmoHandle.cs) ###
This script is not used directly. It is already included in the BoundingBoxRig script. It takes care of creating the rotation and scale handles that are used to adjust the Target object.




### [AppBar](/Assets/HoloToolkit/UX/Scripts/AppBar/AppBar.cs) and [AppBarButton](/Assets/HoloToolkit/UX/Scripts/AppBar/AppBarButton.cs) script ###
AppBar uses [HolographicButton](/Assets/HoloToolkit/UX/Prefabs/Buttons/HolographicButton.prefab) as a template to build a button collection. In default, it includes Show, Hide, Adjust, Done and Remove buttons. Depending on the current mode - Default, Hidden and Manipulation - it controls the visiblity of each button. Based on the number of the buttons, it adjusts the width of the BackgroundBar component. AppBar script has the logic for following user's position in FollowBoundingBox() function.


## How to use Bounding Box & App Bar ##
You can enable Bounding Box and App Bar by simply assigning [BoundingBoxRig script](/Assets/HoloToolkit/UX/Scripts/BoundingBoxes/BoundingBoxRig.cs) to any GameObject. **BoundingBoxRig** script contains these items.
- **BoundingBoxBasic prefab** - Visualizes boundary wireframe 
- **AppBar prefab** - Constructs App Bar
- **BoundingBoxHandle material** - Default Material for the gizmo
- **BoundingBoxGrabbed material** - Material for the gizmo when it is being interacted

![BoundingBox Script Setup](/External/ReadMeImages/MRTK_AppBar_BoundingBox_ScriptSetup.jpg)
![BoundingBox Material Setup](/External/ReadMeImages/MRTK_AppBar_BoundingBox_Materials.jpg)

## Inspector Properties ##
The BoundingBoxRig exposes several properties in the Inspector. 'Flattening' lets you create a BoundingBoxRig that treats the object as if it were flat in one of its dimensions.

Customization Settings- let you specify custom materials for the Scale cubes and the Rotate spheres. Additionally, you can specify a Material that is used for any selected handle.

Behavior- These settings effect how the BoundingBoxRig Scales and Rotates the Target object. Rotation Type allows the object to be rotated in either World Coordinates, or Model Coordinates. You can also specify whether rotation occurs by moving the hand or controller, or by rotating a controller.

## Combining with HandDraggable script ##
Bounding Box with gizmo provides the interface for scaling and rotating object. To make the object movable, you can assign [HandDraggable script](/Assets/HoloToolkit/InputModule/Scripts/Utilities/Interactions/HandDraggable.cs) to the object. With HandDraggable script, you can grab the body of the object and move. You can move object in both normal mode and adjust mode since it does not require the gizmo interface.

## Combining with TwoHandManipulatable script ##
[TwoHandManipulatable script](/Assets/HoloToolkit/Input/Scripts/Utilities/Interactions/TwoHandManipulatable.cs) allows for an object to be movable, scalable, and rotatable with one or two hands. This script can be combined with BoudningBoxRig script, providing both options for manipulating objects. You can find this interaction behavior in the cliff house. Using two motion controllers and select buttons, you can move/rotate/scale any objects without entering adjust mode. Still you can enter adjust mode by pressing the button on the AppBar and use gizmo to scale/rotate the object. For more detailed information about TwoHandManipulatable script, please refer to the [README file](/Assets/HoloToolkit-Examples/Input/Readme/README_TwoHandManipulationTest.md).


## How to add custom buttons to the App Bar ##
You can add additional buttons to the App Bar for other actions. AppBar prefab provides convenient options in the Inspector panel. Find the AppBar prefab in the Project panel and select it. In the Inspector panel, expand **Buttons** section. By modifying the number of **Size** field, you will be able to see the Element populated in the Buttons section. You can specify details such as postion, name and icon texture name. These buttons are using [HolographicButton](/Assets/HoloToolkit/UX/Prefabs/Buttons/HolographicButton.prefab) prefab as a template. HolographicButton is based on [CompoundButton](/Assets/HoloToolkit/UX/Scripts/Buttons/CompoundButton.cs) script series. For more details about CompoundButton, please refer to the [README file of the InteractableObjectExample](/Assets/HoloToolkit-Examples/UX/Readme/README_InteractableObjectExample.md).

![AppBar Custom Button](/External/ReadMeImages/MRTK_AppBar_BoundingBox_CustomButtons.jpg)

