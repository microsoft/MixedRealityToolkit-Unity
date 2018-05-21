# Dialogs Example
<img src="/External/ReadMeImages/MRTK_Dialog.jpg" width="650">
Dialog is a transient UI element which appears when something happens that requires notification, approval, or additional information from the user. Dialogs block interactions with the app window until being explicitly dismissed. They often request some kind of action from the user. Dialog provides one or two button options. It works on both HoloLens(gaze and gesture) and Immersive headset(motion controllers).


![Dialogs on HoloLens and Immersive Headset](/External/ReadMeImages/MRTK_Dialog_Devices.jpg)

## Demo Video
https://gfycat.com/ConventionalDirtyKiskadee

## Dialog prefab and DialogShell script
Dialog prefab is located under [HoloToolkit/UX/Prefabs](/Assets/HoloToolkit/UX/Prefabs) folder and dynamically instantiated with Unity's Instantiate() function. This prefab contains **DialogShell** script and **Solver** scripts.

## How to display a Dialog control
You can find the code example in **LaunchDialogScript.cs** under SceneContent > SquareButton. It demonstrates the instantiation and response to a Dialog using the function Launch Dialog. The Dialog can be specified to have a single button, useful for communicating information to the user. The Dialog can also be configured to display two buttons, allowing the user to make a choice. The name of the selected button is returned to the script that opens the Dialog. 

An instance of the Dialog is created using: 
``` 
//fill dialogPrefab in the inspector:
[Serialize]
private Dialog dialogPrefab;

//then call:
Dialog dialog = Dialog.Open(dialogPrefab.gameObject, buttons, title, message);
```
The second argument to the Open function defines the caption or captions that the button(s) will have. 
They are identified by defining an enumerated value using Dialog.ButtonTypeEnum. 
To define two buttons, simply bitwise OR two enumerations:
 
**Dialog.ButtonTypeEnum.Yes | Dialog.ButtonTypeEnum.No**
 
The calling script can "listen" for the Dialog being closed by attaching an event Handler to the OnClosed Event of the Dialog.
 
In summary, the steps to create and launch a Dialog are:
In a LaunchFunction() called by a Coroutine:
1. Load Dialog Prefab into memory using Resources.Load("Dialog").
2. Create an instance of the Dialog Prefab using Dialog.Open(...). This also Opens and displays the Dialog.
3. Attach OnClosed handler to the dialog.OnClosed event.

## Properties
The Dialog will stay in the user's field of view while the user can use gaze or controller targeting to make a button selection. Tag-along and billboarding interaction is achieved by using **Solver scripts**. When you select Dialog.prefab, you will be able to find Solver scripts for the size and distance control. In **SolverConstantViewSize**, you can adjust **Target View Percent V** value to modify the size of the Dialog window.

You can find more detailed information about the [Solver System on this README page](/Assets/HoloToolkit-Examples/Utilities/Readme/README_SolverSystem.md).

<img src="/External/ReadMeImages/MRTK_Dialog_Inspector.jpg" width="450">

 
 
