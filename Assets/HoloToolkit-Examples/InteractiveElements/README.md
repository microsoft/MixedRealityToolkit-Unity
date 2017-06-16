# InteractiveElements Example
This example shows how to create 3D interactive elements that provide all the user feedback we expect from modern UI elements. This is not an official set of UI controls, but a baseline set of controls, a starting point for building truly immersive 3D UI that fits the theme and design language of our own Holographic applications.

The base components are extendable and easy to use for prototyping or UI development.

##### Their are three main components that make up these interactive elements:

1. Interactive - the base component that detects gaze, press, select and hold input from the user. The Interactive manages the 8 interactive states of a button, which include: Default, Focus, Press, Selected, FocusSelected, PressSelected, Disabled, and DisabledSelected. The Interactive requires a collider to receive input. All input is analyzed and categorized into one of the 8 interactive states then the basic input events are exposed to the Unity Inspector.
2. InteractiveWidget - components that reside on child elements of the Interactive and receives updates as each state changes. A button can have a Label widget for example, that updates the label's color or scale based on the interactive state.
3. InteractiveThemes - a collection of values related to the 8 interactive states. Some widgets (InteractiveThemeWidget) look for themes locally or globally to find values for providing visual feedback. Themes allow for centralizing visual attributes, similar to how CSS is used for web apps, but nowhere near as powerful.

Interactives also provide keyword support, for voice recognition.

## InteractiveButtonComponents:
The InteractiveButtonComponents showcase the control prefabs and their full range of interactive feedback and possible uses.

##### This sample includes:

#### Button:
The basic interactive for simple clicking to make things happen.

#### ToggleButton:
An extention of the Button that provides a selected state. When focused on the ToggleButton, say "toggle button" to active using voice commands.

#### RaidalButton:
ToggleButtons or an InteractiveToggle can be used to create a few different types of components like Tabs, Check Boxes and Radials.
RadialButtons normally are clicked once then become unresponsive, similar to a Tab, until another RadialButton is selected. This sample allows for deselecting for the sake of the demonstration.

#### CheckBox:
Very common component that can be found on many forms, such as confirming a condition before submitting data.

#### Toggle:
Similar to a light switch, this component is very explicit about its state and value.

When focused on the Toggle, say "on" or "off" to flip the control using voice commands.

#### RadialButton Set:
Demonstrates the proper use of a radial or tab system

At any time say "radial one", "radial two", or "radial three" to switch the selected radial using voice commands.

#### Loading Animation:
Though this is a non-interactive component, it does provide some very important feedback to the user and the type of motion is very expressive.

#### Slider:
Gestures are very common interactions for touch, this sample shows how to create a slider element in 3D space for the HoloLens. There are many considerations and approaches to handling gesture input, this example is built on a GestureInteractive that hopefully may simplify these types of interactions for creating more futuristic types of UI.

When focused on the Slider, say "min", "center" or "max" to update its value using voice commands.

## InteractiveExample:
The Interactive example demonstrates the difference between an Interactive and InteractiveToggle in a very bare-bones way. It also displays all the states literally with some example feedback.

## GestureInteractiveExample:
The GestureInteractive takes gesture information and translates it into data sets that are easier to translate into visual feedback or input values.

1. Direction - a vector that represents the hand's current position in relationship to the position where the gesture started.
2. Distance - the current distance from the hand's starting point as if the starting point was zero and every direction from there is a positive and negative value. This helps to determine if the hand is moving right or left, up or down and is very helpful when translating a 3D gesture into a local visual representation.
3. Percentage - based on a predefined MaxGestureDistance value that represents the distance we expect the hand to travel for a given interaction, the percentage lets us know how close the hand is to reaching that max distance. In the case of the slider, we could expect a user to drag their hand about a foot to go from a 0 value to the 100 percent value. We can increase the MaxGestureDistance to increase sensitivity.

The GestureInteractive requires a GestureInteractiveControl that receives the gesture state and these three values. The GestureInteractiveControl is a class that can be extended to build out any type of gesture based interaction. There are many different helper functions to handle many scenarios.

One of the most important functions is GetGestureData() which takes in a gesture distance and alignment vector to evaluate the user's hand movement in relation to the vector direction. This function returns a GestureInteractiveData set containing a Direction, Distance and Percentage related to the supplied vector. This allows for easily evaluating horizontal movement vs vertical movement for example. In this case I would call GetGestureData with a Vector3.up and call again with a Vector3.right. The one with the highest percentage is the predominate movement.

## SliderSamples:
A slider in 3D creates a lot of edge cases. Having a slider that billboards to always face the user requires one set of calculations, but if the slider is still and the user walks around the slider, this creates a whole new set of issues. Conceder that the user is facing forward and moves their hand left to right. Based on the forward facing direction, the movement is in a positive direction in world space. The exact same gesture produces a negative direction if the user turns around.

This sample also attempts to answer the question of what to do if the user is facing the slider at an angle. How would the user expect to move their hand? Still left to right across the body or at the same angle as the slider in relation ot the body as if they were grabbing the slider and pulling it along the rail.

##### The sample includes:

#### Billboarded:
The same example that is found in InteractiveButtonComponents to allow a comparison to other versions.

#### Billboarded Centered:
The slider can be centered with a negative value to the left and positive value to the right.

#### Control Aligned
This slider is not billboarded and the user can pull the slider along the rail. If the slider is at an angle from the user, the hand gesture should match the angle of the slider, pushing or pulling along the rail.

#### Camera Aligned
This slider is looking for a left or right movement perpendicular to the camera's forward.
