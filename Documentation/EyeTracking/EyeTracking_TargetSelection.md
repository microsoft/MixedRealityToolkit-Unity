![MRTK](/External/ReadMeImages/EyeTracking/MRTK_ET_TargetSelect.png =750x450)

# Eye-Supported Target Selection

This page discusses different options for accessing eye gaze data and eye gaze specific events to select targets in MRTK. 
Eye Tracking allows for fast and effortless target selections using a combination of information about what a user is looking at with additional inputs such as 
*hand tracking* and *voice commands*:
    - Look & Pinch (i.e., hold up your hand in front of you and pinch your thumb and index finger together)
    - Look & Say *"Select"* (default voice command)
    - Look & Say *"Explode"* or *"Pop"* (custom voice commands)
    - Look & Bluetooth button

## Target Selection
### Use Generic Focus Handler 
If Eye Tracking is set up correctly (see [Basic MRTK Setup to use Eye Tracking](/Documentation/EyeTracking/EyeTracking_BasicSetup.md)), enabling users to select 
holograms using their eyes is the same as for any other focus input (e.g., head gaze or hand ray).
This provides the great advantage of a flexible way to interact with your holograms by defining the main focus type in your MRTK Input Pointer Profile depending 
on your user's needs, while leaving your code untouched.
For example, this would enable to switch between head or eye gaze without changing a line of code. 
To detect when a hologram is focused at, use the *'IMixedRealityFocusHandler'* interface that provides you with two interface members: *OnFocusEnter* and 
*OnFocusExit*.

Here is a simple example from [ColorTap.cs](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Demo_BasicSetup/Scripts/ColorTap.cs) to change a hologram's 
color when being looked at.

```csharp 
    public class ColorTap : MonoBehaviour, IMixedRealityFocusHandler
    {
        void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData)
        {
            material.color = color_OnHover;
        }

        void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
        {
            material.color = color_IdleState;
        }
        ...
    }
```

#### Selecting a Focused Hologram 
To select focused holograms, we can use Input Event Listeners to confirm a selection. 
For example, you can add the *IMixedRealityPointerHandler* to react to simple pointer input. 
The *IMixedRealityPointerHandler* interface requires you to implement the following three interface members: 
*OnPointerUp*, *OnPointerDown*, and *OnPointerClicked*.

The *MixedRealityInputAction* is a configurable list of actions that you want to distinguish in your app and can be edited in the 
*MRTK Configuration Profile* -> *Input System Profile* -> *Input Actions Profile*. 

```csharp 
   public class ColorTap : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityPointerHandler
    {
        // Allow for editing the type of select action in the Unity Editor.
        [SerializeField]
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None; 
        ...
      
        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.MixedRealityInputAction == selectAction)
            {
                material.color = color_OnHover;
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.MixedRealityInputAction == selectAction)
            {
                material.color = color_OnSelect;
            }
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }
    }
```



### Use Eye-Gaze-Specific *BaseEyeFocusHandler*
Given that eye gaze can be very different to other pointer inputs, you may want to make sure to only react to the focus if it is eye gaze.
Similar to the *FocusHandler*, the *BaseEyeFocusHandler* is specific Eye Tracking.

Here is an example from [mrtk_eyes_02_TargetSelection.unity](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/mrtk_eyes_02_TargetSelection.unity).
Having the [OnLookAt_Rotate.cs]() attached, a GameObject will rotate while being looked at. 

```csharp
    public class OnLookAt_Rotate : BaseEyeFocusHandler
    {
        ... 

        protected override void OnEyeFocusStay()
        {
            // Update target rotation
            RotateHitTarget();
        }

        /// <summary>
        /// Rotate game object based on specified rotation speed and Euler angles.
        /// </summary>
        private void RotateHitTarget()
        {
            transform.eulerAngles = transform.eulerAngles + RotateByEulerAngles * speed;
        }
    }
```

The *BaseEyeFocusHandler* provides more than only *'OnEyeFocusStay'*. Here is an overview of other events: 

```csharp 
        /// <summary>
        /// Triggered once the eye gaze ray *starts* intersecting with this target's collider.
        /// </summary>
        protected virtual void OnEyeFocusStart() { }

        /// <summary>
        /// Triggered *while* the eye gaze ray is intersecting with this target's collider.
        /// </summary>
        protected virtual void OnEyeFocusStay() { }

        /// <summary>
        /// Triggered once the eye gaze ray *stops* intersecting with this target's collider.
        /// </summary>
        protected virtual void OnEyeFocusStop() { }

        /// <summary>
        /// Triggered once the eye gaze ray has intersected with this target's collider for a specified amount of time.
        /// </summary>
        protected virtual void OnEyeFocusDwell() { }
```


### Use Eye-Gaze-Specific EyeTrackingTarget 
While handling focus events in code is great, another way is to handle this from within the Unity Editor.
For this purpose, you can simply attach the [EyeTrackingTarget](/Assets/MixedRealityToolkit.SDK/Features/Input/Handlers/EyeTrackingTarget.cs) script to a 
GameObject.

This has two advantages: 
1. You can make sure that the hologram is only reacting to the user's eye gaze.
2. Several Unity events have already been set up to make it fast and convenient to handle and reuse existing behaviors.

#### Example: Attentive Notifications
For example, in [mrtk_eyes_02_TargetSelection.unity](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/mrtk_eyes_02_TargetSelection.unity), 
you can find an example for *'smart attentive notifications'* that react to your eye gaze. 
These are 3D text boxes that can be placed in the scene and that will smoothly enlarge and turn toward the user when being looked at to ease legibility.
While the user is reading the notification, the information keeps getting displayed crisp and clear. 
After reading it and looking away from the notification, the notification will automatically be dismissed and fades out.
To achieve all this, we created a few generic behavior scripts that are not specific to Eye Tracking at all such as:
- [FaceUser.cs](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/General/Scripts/TargetBehaviors/FaceUser.cs)
- [ChangeSize.cs](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/General/Scripts/TargetBehaviors/ChangeSize.cs)
- [BlendOut.cs](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/General/Scripts/TargetBehaviors/BlendOut.cs)

The advantage of this approach is that the same scripts can be reused by various events. 
For example, a hologram may start facing the user based on a voice commands or after pressing virtual button. 
To trigger these events, you can simply reference the methods that should be executed in the [EyeTrackingTarget](/Assets/MixedRealityToolkit.SDK/Features/Input/Handlers/EyeTrackingTarget.cs) 
script that is attached to your GameObject.
For the example of the *'smart attentive notifications'*, the following happens:
- **OnLookAtStart()**: The notification starts to...
    - *FaceUser.Engage:* ... turn toward the user.
    - *ChangeSize.Engage:* ... increase in size *(up to a specified maximum scale)*.
    - *BlendOut.Engage:* ... starts to blend in more *(after being at a more subtle idle state)*.  

- **OnDwell()**: Informs the '*BlendOut*' script that the notification has been looked at sufficiently.

- **OnLookAway()**: The notification starts to...
    - *FaceUser.Disengage:* ... turn back to its original orientation.
    - *ChangeSize.Disengage:* ... decrease back to its original size.
    - *BlendOut.Disengage:* ... starts to blend out - If *'OnDwell()'* was triggered, blend out completely and destroy, otherwise back to its idle state.

**Design Consideration:**
The key to an enjoyable experience here is to carefully tune the speed of any of these behaviors to avoid causing discomfort by reacting to the user’s eye gaze too quickly all the time. 
Otherwise this can quickly feel extremely overwhelming.

![MRTK](/External/ReadMeImages/EyeTracking/mrtk_et_EyeTrackingTarget_Notification.jpg =750x450)


#### Example: Multimodal Gaze-Supported Target Selection
One event provided by the [EyeTrackingTarget](/Assets/MixedRealityToolkit.SDK/Features/Input/Handlers/EyeTrackingTarget.cs), yet not used by the 
*'Attentive Notifications'* is the *OnSelected()* event. 
Using the *EyeTrackingTarget*, you can specify what triggers the selection which will invoke the *'OnSelected()'* event. 
For example, the screenshot below is from 
[mrtk_eyes_02_TargetSelection.unity](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/mrtk_eyes_02_TargetSelection.unity).
It shows how the [EyeTrackingTarget](/Assets/MixedRealityToolkit.SDK/Features/Input/Handlers/EyeTrackingTarget.cs)
is set up for one of the gems that explodes when you select it.

![MRTK](/External/ReadMeImages/EyeTracking/mrtk_et_EyeTrackingTarget.jpg =750x450)

The *'OnSelected()'* event triggers the method *'TargetSelected'* in the 
[HitBehavior_DestroyOnSelect](\Assets\MixedRealityToolkit.Examples\Demos\EyeTracking\Demo_TargetSelections\Scripts\HitBehavior_DestroyOnSelect.cs) 
script attached to the gem GameObject.
The interesting part is *how* the selection is triggered. 
The [EyeTrackingTarget](/Assets/MixedRealityToolkit.SDK/Features/Input/Handlers/EyeTrackingTarget.cs)
allows for quickly assigning different ways to invoke a selection:

- *Pinch gesture*: Setting the 'Select Action' to 'Select' uses the default hand gesture to trigger the selection. This means that the user can simply raise their hand and pinch their thumb and index finger together to confirm the selection.

- Say *"Select"*: Use the default voice command *"Select"* for selecting a hologram.

- Say *"Explode"* or *"Pop"*: To use custom voice commands, you need to follow two steps: 
    1. Set up a custom action such as *"DestroyTarget"*
        - Navigate to *'Input System Profile'* -> *'Input Actions Profile'* 
        - Add new action

    2. Set up the voice commands that trigger this action such as *"Explode"* or *"Pop"* 
        - Navigate to *'Input System Profile'* -> *'Speech Commands Profile'*
        - Add new speech command and associate the action you just created 
        - Assign a *'KeyCode'* to allow for triggering the action via a button press


<br>

This should get you started in accessing Eye Tracking data in your MRTK Unity app! 

---
[Back to "Eye Tracking in the MixedRealityToolkit"](/Documentation/EyeTracking/EyeTracking_Main.md)







