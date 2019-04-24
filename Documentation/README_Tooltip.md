# Tooltip #

![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Main.png)

This example scene demonstrates an implementation of the ToolTip user interface element. Tool tips are usually used to convey a hint or extra information upon closer inspection of an object. Tool tips can be used to explain button inputs on the motion controllers or to label objects in the physical environment.

## Demo video ##

The [example scene](https://gfycat.com/WarmOblongBilby) demonstrates two ways to display a Tooltip on an object.

## How to use a tool tip ##
A tool tip can be added directly to the hierarchy and targeted to an object.

To use this method simply add a game object and one of the [tool tip prefabs](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Tooltips) to the scene hierarchy. In the prefab's inspector panel, expand the *Tool Tip* (script). Select a tip state and configure the tool tip.  Enter the respective text for the tool tip in the text field. Expand the *ToolTipConnector* (Script) and drag the object that is to have the tool tip from the hierarchy into the field labelled *Target*. This attaches the tool tip to the object. 

This use assumes a tool tip that is always showing or that is shown / hidden via script by changing the tip state property of the tool tip component.
 
## Dynamically spawning tooltips ##
A tooltip can be dynamically added to an object at runtime as well as pre-set to show and hide on a tap or focus. Simply add the [`ToolTipSpawner`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Tooltips/ToolTipSpawner.cs) script to any game object. Delays for appearing and disappearing can be set in the scripts inspector as well as a lifetime so that the tooltip will disappear after a set duration. Tooltips also feature style properties such as background visuals in the spawner script. By default the tooltip will be anchored to the object with the spawner script. This can be changed by assigning a GameObject to the anchor field.

## Tooltips on motion controllers ##
Tooltips can also be assigned to motion controllers, for example to explain the assigned actions of buttons. The example scene below also includes two tooltip groups on the bottom. These are layed out to match position of the buttons on the motion controllers. When motion controllers are detected, these tooltips will be attached automatically to the controllers, using the [`AttachToController.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/Utilities/Solvers/AttachToController.cs) script.

## Example scene ##
In the [example scene files](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/UX/Tooltips/Scenes), you will be able to find various examples of tooltips. First group on the left demonstrates the static tooltips examples that are always visible. In the center, you can see the example of using multiple tooltips on a single object. Each tooltip has different child object as a target object which works as an anchor. The group on the right shows the examples of dynamically spawning tooltips.

<img src="../Documentation/Images/ManipulationHandler/MRTK_Manipulation_Main.png" width="600">
