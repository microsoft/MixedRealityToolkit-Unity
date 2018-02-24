# Interactable Objects & Receiver
![Interactable Objects](/External/ReadMeImages/MRTK_InteractableObject.jpg)
This example scene demonstrates how to make objects interactable using [CompoundButton](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/CompoundButton.cs) script scries. With CompoundButton script, you can make any object interactable with differentiated visual state for the HoloLens' gesture input or immersive headset's motion controller input state. For more information please see ['Interactable Object'](https://developer.microsoft.com/en-us/windows/mixed-reality/interactable_object) on Windows Dev Center.

The scene also demonstrates how to use [Receiver](/Assets/MixedRealityToolkit/UX/Scripts/Receivers/InteractionReceiver.cs) to handle events comging from multiple objects, in a single script. This is especially useful when you have multiple buttons or interactable objects in a scene. 
 
In the scene [InteractableObject_Examples.unity](/Assets/MixedRealityToolkit-Examples/UX/Scenes/InteractableObjectExample.unity), you will be able to find various combinations of 'CompoundButton' scripts. To create your own Interactable Object, you can combine different types of 'CompoundButton' scripts. It is designed to support various types of Interactable Object in flexible way.

### [Compound Button](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/CompoundButton.cs) ###
This is the base of the button component. You will need this script to build any types of Interactable Objects.

![Compound Button](/External/ReadMeImages/MRTK_CompoundButton_Inspector.jpg)

### [Compound Button Mesh](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/Utilities/CompoundButtonMesh.cs) ###
Use this script to use various types of custom mesh. You can use your own 3D models imported from 3D modeling software. Using this script, you can easily change the scale, offset of the mesh or material properties such as color for the different input interaction states. To create an Interactable Object using script, it is recommended to create an empty GameObject as a container and put the 3D mesh model under it as child component. This will prevent unexpected behavior from different scaling or offset values.

![Compound Button](/External/ReadMeImages/MRTK_CompoundButtonMesh_Inspector.jpg)

### [Compound Button Icon](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/Utilities/CompoundButtonIcon.cs) ###
Using this scripts, you can use Texture 2D assets to display icons. To assgin your custom icon texture, expand **DefaultButtonIconProfileTexture**. You will be able to find empty slots for the Texture 2D asset. Once you assign Texture 2D asset, you can select the icon using the drop down on the bottom of the profile section.

![Compound Button](/External/ReadMeImages/MRTK_CompoundButtonIcon_Inspector.jpg)
![Compound Button Icon Texture](/External/ReadMeImages/MRTK_CompoundButtonIconTexture.jpg)

### [Compound Button Text](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/Utilities/CompoundButtonText.cs) ###
This scripts helps you manage a TextMesh component to display text on your button. This script can be used in conjunction with a CompoundButtonSpeech component to automatically link your button to spoken keywords.

![Compound Button](/External/ReadMeImages/MRTK_CompoundButtonText_Inspector.jpg)

### [Compound Button Sound](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/Utilities/CompoundButtonSounds.cs) ###
Use this script to add audio feedback for the different input interaction states.

![Compound Button](/External/ReadMeImages/MRTK_CompoundButtonSound_Inspector.jpg)

### [Compound Button Anim](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/Utilities/CompoundButtonAnim.cs) ###
This is the base of the button component. You will need this script to build any types of Interactable Objects.

![Compound Button](/External/ReadMeImages/MRTK_CompoundButtonAnim_Inspector.jpg)

### [Compound Button Speech](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/Utilities/CompoundButtonSpeech.cs) ###
Use this script to automatically register keywords for your button in the Speech Manager (This script is experimental and still being tested).

![Compound Button](/External/ReadMeImages/MRTK_CompoundButtonSpeech_Inspector.jpg)

### [Compound Button Toggle](/Assets/MixedRealityToolkit/UX/Scripts/Buttons/Utilities/CompoundButtonToggle.cs) ###
Use this script to add toggle on/off state.

## Receiver ##
![Receiver](/External/ReadMeImages/MRTK_Receiver.jpg)

With Receiver, you can handle events from multiple objects in a single script. To use it, simply create a script and inherit from [InteractionReceiver](/Assets/MixedRealityToolkit/UX/Scripts/Receivers/InteractionReceiver.cs). In this example scene, you cand find the example of using Receiver in [ButtonReceiverExample script](/Assets/MixedRealityToolkit-Examples/UX/Scripts/ButtonReceiverExample.cs). The script uses Receiver to display the object name and event type. You can use switch case statement with **obj.name** to do specific action for the object that triggered the event.

<pre>
protected override void InputDown(GameObject obj, InputEventData eventData) {
    Debug.Log(obj.name + " : InputDown");
    txt.text = obj.name + " : InputDown";

    switch(obj.name)
    {
        case "ButtonBalloon":
            // Do something when balloon is pressed
            break;

        case "ButtonCoffeeCup":
            // Do something when coffee cup is pressed
            break;

        default:
            break;
    }
}
</pre>




## Interactable Object Examples ##

### Holographic button ###

![Holographic button](/External/ReadMeImages/MRTK_InteractableObject_HolographicButton.jpg)

This is an example of Holographic button used in the Start menu and App Bar. This example uses Unity's Animation Controller and Animation Clips. This prefab is also available under MixedRealityToolkit's main folder: [MixedRealityToolkit/UX/Prefab](/Assets/MixedRealityToolkit/UX/Prefabs/Buttons/SquareButton.prefab)

### Mesh button ###

![Mesh button](/External/ReadMeImages/MRTK_InteractableObject_MeshButton.jpg)

These are the examples using primitives and imported 3D meshes as Interactable Objects. You can easily assign different scale, offset and colors to respond to different input interaction states.


### Traditional button ###

![Traditional button](/External/ReadMeImages/MRTK_InteractableObject_TraditionalButton.jpg)

This example shows a traditional 2D style button with some dimension. Each input state has a slightly different depth and animation properties.
 

### Other examples ###

![Other examples](/External/ReadMeImages/MRTK_InteractableObject_PushButton.jpg)

![Other examples](/External/ReadMeImages/MRTK_InteractableObject_RealLifeObject.jpg)

With HoloLens, you can leverage physical space. Imagine a holographic push button on the physical wall. Or how about a coffee cup on the real table? Using 3D models imported from modeling software, we can create Interactable Object that resembles real life object. Since it is digital object, we can add magical interactions to it.
