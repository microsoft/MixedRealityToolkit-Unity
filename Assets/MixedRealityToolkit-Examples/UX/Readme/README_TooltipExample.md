# Tooltip Example
<img src="/External/ReadMeImages/MRTK_Tooltip.jpg">
This example scene demonstrates an implementation of the ToolTip user interface element. Tooltips are usually used to convey a hint or extra information upon closer inspection of an object. ToolTip can be used to explain button inputs on the motion controllers or to label objects in the physical environment.

## Demo Video
https://gfycat.com/ConventionalDirtyKiskadee

The example scene demonstrates two ways to display a Tooltip on an object.

## Directly adding to the scene and attaching to an object
A ToolTip can be added directly to the Hierarchy and targeted to an object. To use this method, Add a GameObject and a ToolTipPlated object to the Scene Hierarchy. In the ToolTIpPlated Inspector, Expand the Tool Tip (Script). Select a TipState and set other settings. Enter the ToolTip text in the Text field. Finally, expand the ToolTipConnector(Script). Drag the object that is to have the ToolTip from the Hierarchy into the field labelled Target. This attaches the ToolToolTip connector to the object. Finally, this use of ToolTipPlated assumes a ToolTip that is always showing or that is shown/hid in script by changing the TipState property of the ToolTip component.

 
## Dynamically spawning
A ToolTip can be dynamically added to an object at runtime as well as pre-set to show and hide on a Tap or focus. Simply add the ToolTipSpawner script to any GameObject. In the script's Inspector, you can set delays for appearing and disappearing. You can also set a lifetime so that the ToolTip when spawned, will disappear after a duration. You can also set style properties such as Background in the ToolTipSpawner script. This script is pre-populated with the ToiolTipPlated prefab. The GameObject to which the spawned ToolTip is anchored is determined by the object that has been dragged into the Anchor field in the ToolTipSpawner Inspector. This is usually set to the object that has the ToolTipSpawner script.

