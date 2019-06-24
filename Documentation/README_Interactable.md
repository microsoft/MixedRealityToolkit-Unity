# Interactable #

![Interactable](../Documentation/Images/Interactable/InteractableExamples.png)

With Interactable script, you can make any object interactable with differentiated visual state. For example, you can change color of the object on focus or make it bigger on pressed state. Since you can have multiple themes that control different parts of the object, you can achieve sophisticated visual states including shader property changes. In fact, most [interaction example scenes](README_HandInteractionExamples.md) revolve around interactions based on Interactables. 

## How to use interactables ##

Simply add the [`Interactable.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Interactable.cs) component to a GameObject.

![Interactable](../Documentation/Images/Interactable/InteractableInspector_basicSteps.png)

1. A collider must exist on the GameObject with the interactable or the child of the interactable for it to receive input.
2. Use the *OnClick* event to make something happen.
3. Add visual feedback by linking a target to a profile and assigning a theme.

Interactable features can be extended using external components like `PhysicalPressEventRouter` which enables press events to drive some state changes in the interactable.

## Input settings ##

The basic features allow for button style interactions, such as pointer focus and clicks, that maps to interaction states to drive themes which are setup through the interactable profile. Controller or hand focus, down, up and click (both near and far) are handled. Functionality can be extended using external scripts that can set state manually.

<img src="../Documentation/Images/Interactable/InputFeatures_short.png" width="450">

**Input Actions**
Select the action, from the input configuration or controller mapping profile, that the interactable should react to.
See [Overview of the input system in MRTK](./Input/Overview.md) for more on how input actions are setup and intended to be used in the application.

**Enabled**
Sets the interactables enabled state, which will disable some input handling and update the themes to reflect the current state which is disabled.

This is different from disabling input all together (using *Enable Input*). It means that a specific button that would normally be interactive will be disabled and has a visual look and feel to denote its disabled state. A typical example of this would be a submit button waiting for all the required input fields to be completed.

**IsGlobal**
Focus is not required to detect input actions, default behavior is false.

**Voice Commands**
A voice command to trigger an OnClick event. This will also trigger a quick state change to drive any themes visuals.
 
Note: Make sure there is a unique voice command on each button. The voice recognizer is global (even if the interactable is not) and will not register the same voice command twice; in this case an error will be thrown.

**Requires Gaze (Only available when the voice command field has a value)**
The voice command requires the interactable to have focus to listen for the voice command. There are several ways to use voice commands to trigger an interactable, be careful not to have multiple objects with the same voice command or there will be conflicts. Using the MRTK voice recognition profile or online speech service are other ways to enable voice commands.

**Public Properties**
ClickCount - a read only value that tracks how many clicks have occured.


## Profiles and Themes ##

The profile will define how button content will be linked to and manipulated by themes, based on state changes.

<img src="../Documentation/Images/Interactable/Profiles_noTarget.png" width="450">

Themes work a lot like materials. They are scriptable objects that contain a list of data that will be assigned to an object based on the current state. Like materials, they can be edited individually in the project panel or through the interactable profile. Editing a theme through an interactable will update its settings for all other interactables using that theme. Themes can be extended to control any aspect of a GameObject with a few basic themes provided that can change color, scale, position or an combination of the three.

<img src="../Documentation/Images/Interactable/DefaultTheme_button.png" width="450">

A default theme will be provided whenever an target object is added to a profile. It is not advised to edit the default theme, like in the case MRTK is updated, the theme could get overridden. A "Create Theme" button is provided whenever the default theme is used to make it easier to create a new themes. 

<img src="../Documentation/Images/Interactable/DefaultTheme_values.png" width="450">

*Example of the Default Theme*

<img src="../Documentation/Images/Interactable/Theme.png" width="450">

*Example of a Color Theme*

The best way to save a profile of a button, with all the themes and targets setup, is to create a prefab of your button.
Note that themes that manipulate mesh objects (color or shader themes) are able to detect the shader properties in the material assigned to the target object. A drop down list of shader properties will define how the values of the theme are applied and is a convenience of this ability. Conflicts can arise if the same theme is used on objects that do not share the same material shader setting. Best practice is to create a separate theme for objects with different shaders; this is not an issue when using the same color theme on a text object and a mesh object, because all the shader properties are ignored on text objects.


### Creating Toggles
Toggle or multi-step buttons can be created in the Profile using the Dimensions field. The idea is that each set of states can have multiple dimensions and in this case, when the Dimensions value is increased, slots for additional themes are provided for each Target in the Profile. This allows for a Normal Theme and a Toggled Theme to be used depending if the Interactable is toggled or not. 

<img src="../Documentation/Images/Interactable/Profile_toggle.png" width="450">

With dimensions being a numeric value, the options for adding themes or steps is endless. An example of a multi-step button with 3 dimensions is one that controls speed. We may only want to have the option for 3 values, Fast (1x), Faster (2x) or Fastest (3x). Dimensions are used to control the text or texture of the button for each individual speed setting, using 3 different themes for each of them. Developers can assess the *DimensionIndex* to determine which dimension is currently active.

```
//Access the current DimensionIndex
GetDimensionIndex();

//Set the DimensionIndex - toggled
SetDimensionIndex(1);

//Set the DimensinIndex - Untoggled
SetDimensionIndex(0);
```

Every click event will advance the DimensionIndex which will increase until the set Dimensions value is reached then cycle or reset to 0. A good example of working with Dimensions with code is the InteractiveToggleCollection found in the InteractableExamples demo scene on the RadialSet object.

<img src="../Documentation/Images/Interactable/InteractableToggleCollection.png" width="450">

**See the Events section to learn about Toggle Events.**

## Events
You can use Interactable to detect input events other than just OnClick. The Events feature provides a way to enable functionality to extend a button, but not really visual or needed to provide feedback based on state changes.

<img src="../Documentation/Images/Interactable/Events.png" width="450">

**At the bottom of the Interactable component, click the "Add Event" button to reveal additional event options.** A drop down menu contains the current list of supported events like toggle, hold or double tap. The idea of these events is to monitor Interactable state changes and define patterns to detect. When a pattern is detected, an action can be triggered through the inspector or directly in code.

<img src="../Documentation/Images/Interactable/Event_audioClip.png" width="450">

*Example of audio clip to play on click. There is an audio theme for playing audio clips for each state change, like focus*

<img src="../Documentation/Images/Interactable/Event_toggle.png" width="450">

*Example of Toggle events*

<img src="../Documentation/Images/Interactable/Event_hold.png" width="450">

*Example of a hold event*

<img src="../Documentation/Images/Interactable/Event_onClickEffect.png" width="450">

*Example of a modified OnClick event to spawn a gameObject on click*

Events can be placed on an object to monitor a separate interactable. Use `InteractableReceiver` for a single event (from the list) or `InteractableReceiverList` for a list of events similar to the interactable event list.

<img src="../Documentation/Images/Interactable/InteractableReceiver.png" width="450">

*Example of InteractableReceiver existing on a separate gameObject from the Interactable, referencing the Interactable for event and state updates*

"Search Scope" provides a preferred path to search for an Interactable if one is not explicitly assigned.

## States ##
States are a list of terms that can be used to define interactions phases, like press or observed.

<img src="../Documentation/Images/Interactable/DefaultStates.png" width="450">

Interactable states provide two major roles:
* Establish a list of states that are relevant for the Interactable. This list will be displayed in the themes and can also be referenced by the events.
* Controls how different interaction phases are ranked into states. For instance, a press state is also in a focused state, but the InteractableStates class will define it is a press state based on the ranking preferences setup in the State ScriptableObject.

<img src="../Documentation/Images/Interactable/StatesScriptableObject.png" width="450">

The InteractableStates State Model will handle any state list with a layered ranking system, starting with the most isolated state and ending with the state that could contain all other states.

The DefaultInteractableStates list contains 4 states:

- **Default**: Nothing is happening, this is the most isolated base state. If anything does happen, it should over rule this state.
- **Focus**: The object is being pointed at. This is a single state, no other states are currently set, but it will out rank Default.
- **Press**: The object is being pointed at and a button or hand is pressing. The Press state out ranks Default and Focus. This state will also get set as a fallback to Physcial Press.
- **Disabled**: The button should not be interactive and visual feedback will let the user know for some reason this button is not usable at this time. In theory, the disabled state could contain all other states, but when Enabled is turned off, the Disabled state trumps all other states.

A bit value (#) is assigned to the state depending on the order in the list.

There are currently 17 states total that you can used to drive themes, though some are meant to be driven by other components. Here's a list of those with built-in functionality.
- Default, Focus, Pressed and Disabled are mentioned above
- Visited: the Interactable has been clicked.
- Toggled: The button is in a toggled state or Dimension idex is an odd number.
- Gesture: The hand or controller was pressed and has moved from the original position.
- VoiceCommand: The internal voice command was used, or if using global voice commands, set this manually.
- PhyscialTouch: A touch input is currently detected, use NearInteractionTouchable to enable.
- Grab: A hand is currently grabbing in the bounds of the object, use NearInteractionGrabbable to enable

States have corresponding properties and Methods in the Interactable, like SetFocus(bool focus) or HasFocus.


## Extending themes ##
Extend `InteractableThemeBase` to create a new theme that will show up in the theme property drop-down list. Themes can be created to control anything based on state changes. We could have a custom component on a GameObject that is driven by a custom theme with the values for each state being set in the inspector.

Setup the configuration of the theme settings in the constructor.

``` csharp
public NewCustomTheme()
{
    Types = new Type[] { typeof(Transform) };
    Name = "Custom Theme Scale";
    ThemeProperties.Add(
        new InteractableThemeProperty()
        {
            Name = "Scale",
            Type = InteractableThemePropertyValueTypes.Vector3,
            Values = new List<InteractableThemePropertyValue>(),
            Default = new InteractableThemePropertyValue() { Vector3 = Vector3.one}
        });
}
```

- **Name**: The display name of the property, this will display next to the property field under each state title.
- **Types**: Allow filtering based on components on the object. In this case, this theme will show up in the list when assigned to an object with a Transform.
- **Name**: The name that will show up in the inspector.
- **ThemeProperties**: A list of properties that theme will store to be used when the state changes.

Each Theme Property has a name, type (defining the fields to display for each state), a set of values for each state and a default value for the fields. The state fields can also be hidden in the inspector, if the theme does not require them to be visible.

``` csharp
    // custom settings will appear in the configuration section of the theme inspector
    // expose properties to allow more customization to the theme
    CustomSettings = new List<InteractableCustomSetting>()
    {
        new InteractableCustomSetting()
        {
            Name = "ScaleMagnifier",
            Type = InteractableThemePropertyValueTypes.Vector3,
            Value = new InteractableThemePropertyValue() { Vector3 = Vector3.one }
        }
    };

```

Override Init to run any startup code, that needs references to the Host GameObject.

``` csharp
public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
{
    InteractableThemePropertyValue start = new InteractableThemePropertyValue();
    start.Vector3 = Host.transform.localScale;
    return start;
}
```

GetProperty should grab the current property of the Host. This will be used for animation later. Property is provided in case the current value depends on a cached property value. In the example below on the current scale.

``` csharp
public override void SetValue(InteractableThemeProperty property, int index, float percentage)
{
    Host.transform.localScale = Vector3.Lerp(property.StartValue.Vector3, property.Values[index].Vector3, percentage);
}
```

The SetValue function is used to set the property value based on the current state.
- **Property**: A list of property values per state, set through the theme inspector.
- **Index**: Correlates to the current state.
- **Percentage**: A float value between 0 and 1. It will be the eased value if an Animation curve is used.

Custom Settings added to the new class will also be displayed in the inspector. If there are properties that need to be exposed in the inspector but do not need to be based on states, they should be added to the Custom Settings list.

### Extending shader themes

If creating a custom interactable theme to modify shader properties, it is advised to extend the `InteractableShaderTheme` class. Please see the `InteractableColorTheme` class as an example to extend the `InteractableShaderTheme` class.

Regardless of whether extending the `InteractableShaderTheme` class or not, it is highly recommended to set shader properties 1) via [**MaterialPropertyBlock**](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) and 2) using the shader properties integer keys instead of string keys. Unity assigns integer keys for all shader properties in a project at runtime.See [**Shader.PropertyToID**](https://docs.unity3d.com/ScriptReference/Shader.PropertyToID.html) for more details on. The integer key for an interactable property can be accessed via  `InteractableThemeProperty.GetShaderPropertyId()`. 

Both of these two steps will help with performance. [**MaterialPropertyBlock**](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) will ensure a new material instance is not created for every object modifying a shader property and integer shader property IDs will eliminate the string-to-int lookup step performed by Unity when using string keys (i.e "_Color") on set/get functions.

``` csharp
public override void SetValue(InteractableThemeProperty property, int index, float percentage)
{
     renderer.GetPropertyBlock(propertyBlock);

     int propId = property.GetShaderPropertyId();
     
     Color newColor = Color.Lerp(property.StartValue.Color, property.Values[index].Color, percentage);
     block.SetColor(propId, color);
             
     renderer.SetPropertyBlock(propertyBlock);
 }
```

## Extending events ##
Like Themes, events can be extended to detect any state pattern or to expose functionality. 

Custom events can be created and used in two main ways:
* Extend `ReceiverBase` to create a custom event that will show up in the dropdown list of event types. A Unity event is provided by default, but additional Unity events can be added or the event can be set to hide Unity events. This functionality allows a designer to work with an engineer on a project to create a custom event that the designer or implementer to setup in the editor.
* Extend `ReceiverBaseMonoBehavior` to create a completely custom event component that can reside on the interactable or another object. The `ReceiverBaseMonoBehavior` will reference the interactable to detect state changes. This approach is the most direct for engineers that do not want to work through the inspector.

### Example for extending `ReceiverBase` ###
`MixedRealityToolkit.Examples` contains an example extension of ReceiverBase to display status information about the interactable.

``` csharp
public CustomInteractablesReceiver(UnityEvent ev) : base(ev)
{
    Name = "CustomEvent";
    HideUnityEvents = true; // hides Unity events in the receiver - meant to be code only
}
```

*OnUpdate* is an abstract method that can be used to detect patterns. Here is an example of accessing all the states of the interactable. Though there is a definite state that is defined with `state.CurrentState()`, the state object has a reference to all other states and their values.

``` csharp
public override void OnUpdate(InteractableStates state, Interactable source)
{
    if (state.CurrentState() != lastState)
    {
        // the state has changed, do something new
        /*
            bool hasDown = state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value > 0;
            bool focused = state.GetState(InteractableStates.InteractableStateEnum.Focus).Value > 0;
            bool isDisabled = state.GetState(InteractableStates.InteractableStateEnum.Disabled).Value > 0;
            bool hasInteractive = state.GetState(InteractableStates.InteractableStateEnum.Interactive).Value > 0;
            bool hasObservation = state.GetState(InteractableStates.InteractableStateEnum.Observation).Value > 0;
            bool hasObservationTargeted = state.GetState(InteractableStates.InteractableStateEnum.ObservationTargeted).Value > 0;
            bool isTargeted = state.GetState(InteractableStates.InteractableStateEnum.Targeted).Value > 0;
            bool isToggled = state.GetState(InteractableStates.InteractableStateEnum.Toggled).Value > 0;
            bool isVisited = state.GetState(InteractableStates.InteractableStateEnum.Visited).Value > 0;
            bool isDefault = state.GetState(InteractableStates.InteractableStateEnum.Default).Value > 0;
            bool hasGesture = state.GetState(InteractableStates.InteractableStateEnum.Gesture).Value > 0;
            bool hasGestureMax = state.GetState(InteractableStates.InteractableStateEnum.GestureMax).Value > 0;
            bool hasCollision = state.GetState(InteractableStates.InteractableStateEnum.Collision).Value > 0;
            bool hasPhysicalTouch = state.GetState(InteractableStates.InteractableStateEnum.PhysicalTouch).Value > 0;
            bool hasCustom = state.GetState(InteractableStates.InteractableStateEnum.Custom).Value > 0;
            or:
            bool hasFocus = source.HasFocus;
            bool hasPress = source.HasPress;
        */
        lastState = state.CurrentState();
        SetOutput();
    }
}
```

Two interactable event methods are also available if driving functionality from *OnClick* or *OnVoiceCommand* is needed.

``` csharp
public virtual void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
{
    // voice command called
}  
public virtual void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
{
    // click called
}
```

ReceiverBase scripts use InspectorField attributes to expose custom properties in the inspector. Here's an example of Vector3 a custom property with tooltip and label information.

``` csharp
 [InspectorField(Label = "Offset Position", Tooltip = "Spawn the prefab relative to the Interactive position", Type = InspectorField.FieldTypes.Vector3)]
        public Vector3 EffectOffset = Vector3.zero;
```

## Extending states ##
The functionality of how states are ranked can be overridden by extending `InteractableStates` class. Override the `CompareStates` method to manually control the ranking.

``` csharp
public override State CompareStates()
{
    int bit = GetBit();
    currentState = stateList[0];
    for (int i = stateList.Count - 1; i > -1; i--)
    {
        if (bit >= stateList[i].Bit)
        {
            currentState = stateList[i];
            break;
        }
    }
    return currentState;
}
```

## See also
- [MRTK Standard Shader](README_MRTKStandardShader.md)