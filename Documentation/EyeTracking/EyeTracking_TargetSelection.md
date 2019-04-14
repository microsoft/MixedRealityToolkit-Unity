![MRTK](../../External/ReadMeImages/EyeTracking/mrtk_et_targetselect.png)

# Eye-Supported Target Selection
This page discusses different options for accessing eye gaze data and eye gaze specific events to select targets in MRTK. 
Eye Tracking allows for fast and effortless target selections using a combination of information about what a user is looking at with additional inputs such as 
_hand tracking_ and _voice commands_:
    - Look & Pinch (i.e., hold up your hand in front of you and pinch your thumb and index finger together)
    - Look & Say _"Select"_ (default voice command)
    - Look & Say _"Explode"_ or _"Pop"_ (custom voice commands)
    - Look & Bluetooth button

## Target Selection
### Use Generic Focus Handler 
If Eye Tracking is set up correctly (see [Basic MRTK Setup to use Eye Tracking](EyeTracking_BasicSetup.md)), enabling users to select 
holograms using their eyes is the same as for any other focus input (e.g., head gaze or hand ray).
This provides the great advantage of a flexible way to interact with your holograms by defining the main focus type in your MRTK Input Pointer Profile depending 
on your user's needs, while leaving your code untouched.
For example, this would enable to switch between head or eye gaze without changing a line of code. 
To detect when a hologram is focused at, use the _'IMixedRealityFocusHandler'_ interface that provides you with two interface members: _OnFocusEnter_ and 
_OnFocusExit_.

Here is a simple example from [ColorTap.cs](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.ColorTap) to change a hologram's 
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
For example, you can add the _IMixedRealityPointerHandler_ to react to simple pointer input. 
The _IMixedRealityPointerHandler_ interface requires you to implement the following three interface members: 
_OnPointerUp_, _OnPointerDown_, and _OnPointerClicked_.

The _MixedRealityInputAction_ is a configurable list of actions that you want to distinguish in your app and can be edited in the 
_MRTK Configuration Profile_ -> _Input System Profile_ -> _Input Actions Profile_. 

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



### Use Eye-Gaze-Specific _BaseEyeFocusHandler_
Given that eye gaze can be very different to other pointer inputs, you may want to make sure to only react to the focus if it is eye gaze.
Similar to the _FocusHandler_, the _BaseEyeFocusHandler_ is specific Eye Tracking.

Here is an example from [mrtk_eyes_02_TargetSelection.unity](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/mrtk_eyes_02_TargetSelection.unity
).
Having the [OnLookAt_Rotate.cs](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.OnLookAt_Rotate) attached, a GameObject will rotate while being looked at. 

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

The _BaseEyeFocusHandler_ provides more than only _OnEyeFocusStay_. Here is an overview of other events: 

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
For this purpose, you can simply attach the [EyeTrackingTarget](xref:Microsoft.MixedReality.Toolkit.Input.EyeTrackingTarget) script to a 
GameObject.

This has two advantages: 
1. You can make sure that the hologram is only reacting to the user's eye gaze.
2. Several Unity events have already been set up to make it fast and convenient to handle and reuse existing behaviors.

#### Example: Attentive Notifications
For example, in [mrtk_eyes_02_TargetSelection.unity](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/mrtk_eyes_02_TargetSelection.unity), 
you can find an example for _'smart attentive notifications'_ that react to your eye gaze. 
These are 3D text boxes that can be placed in the scene and that will smoothly enlarge and turn toward the user when being looked at to ease legibility.
While the user is reading the notification, the information keeps getting displayed crisp and clear. 
After reading it and looking away from the notification, the notification will automatically be dismissed and fades out.
To achieve all this, we created a few generic behavior scripts that are not specific to Eye Tracking at all such as:
- [FaceUser.cs](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.FaceUser)
- [ChangeSize.cs](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.ChangeSize)
- [BlendOut.cs](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.BlendOut)

The advantage of this approach is that the same scripts can be reused by various events. 
For example, a hologram may start facing the user based on a voice commands or after pressing virtual button. 
To trigger these events, you can simply reference the methods that should be executed in the [EyeTrackingTarget](xref:Microsoft.MixedReality.Toolkit.Input.EyeTrackingTarget) 
script that is attached to your GameObject.
For the example of the _'smart attentive notifications'_, the following happens:
- **OnLookAtStart()**: The notification starts to...
    - *FaceUser.Engage:* ... turn toward the user.
    - *ChangeSize.Engage:* ... increase in size _(up to a specified maximum scale)_.
    - *BlendOut.Engage:* ... starts to blend in more _(after being at a more subtle idle state)_.  

- **OnDwell()**: Informs the _BlendOut_ script that the notification has been looked at sufficiently.

- **OnLookAway()**: The notification starts to...
    - *FaceUser.Disengage:* ... turn back to its original orientation.
    - *ChangeSize.Disengage:* ... decrease back to its original size.
    - *BlendOut.Disengage:* ... starts to blend out - If _OnDwell()_ was triggered, blend out completely and destroy, otherwise back to its idle state.

**Design Consideration:**
The key to an enjoyable experience here is to carefully tune the speed of any of these behaviors to avoid causing discomfort by reacting to the user’s eye gaze too quickly all the time. 
Otherwise this can quickly feel extremely overwhelming.

<img src="../../External/ReadMeImages/EyeTracking/mrtk_et_EyeTrackingTarget_Notification.jpg" width="750" alt="MRTK">

#### Example: Multimodal Gaze-Supported Target Selection
One event provided by the [EyeTrackingTarget](xref:Microsoft.MixedReality.Toolkit.Input.EyeTrackingTarget), yet not used by the 
_'Attentive Notifications'_ is the _OnSelected()_ event. 
Using the _EyeTrackingTarget_, you can specify what triggers the selection which will invoke the _OnSelected()_ event. 
For example, the screenshot below is from 
[mrtk_eyes_02_TargetSelection.unity](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/mrtk_eyes_02_TargetSelection.unity).
It shows how the [EyeTrackingTarget](xref:Microsoft.MixedReality.Toolkit.Input.EyeTrackingTarget)
is set up for one of the gems that explodes when you select it.

<img src="../../External/ReadMeImages/EyeTracking/mrtk_et_EyeTrackingTarget.jpg" width="750" alt="MRTK">

The _OnSelected()_ event triggers the method _'TargetSelected'_ in the 
[HitBehavior_DestroyOnSelect](xref:Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.HitBehavior_DestroyOnSelect) 
script attached to the gem GameObject.
The interesting part is _how_ the selection is triggered. 
The [EyeTrackingTarget](xref:Microsoft.MixedReality.Toolkit.Input.EyeTrackingTarget)
allows for quickly assigning different ways to invoke a selection:

- _Pinch gesture_: Setting the 'Select Action' to 'Select' uses the default hand gesture to trigger the selection. This means that the user can simply raise their hand and pinch their thumb and index finger together to confirm the selection.

- Say _"Select"_: Use the default voice command _"Select"_ for selecting a hologram.

- Say _"Explode"_ or _"Pop"_: To use custom voice commands, you need to follow two steps: 
    1. Set up a custom action such as _"DestroyTarget"_
        - Navigate to _'Input System Profile'_ -> _'Input Actions Profile'_ 
        - Add new action

    2. Set up the voice commands that trigger this action such as _"Explode"_ or _"Pop"_ 
        - Navigate to _'Input System Profile'_ -> _'Speech Commands Profile'_
        - Add new speech command and associate the action you just created 
        - Assign a _'KeyCode'_ to allow for triggering the action via a button press
<br>

This should get you started in accessing Eye Tracking data in your MRTK Unity app! 

---
[Back to "Eye Tracking in the MixedRealityToolkit"](EyeTracking_Main.md)
