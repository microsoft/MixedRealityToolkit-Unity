# Prototyping
This example shows some different components for rapidly prototyping basic interactions. They were built over several projects working with designers to help them build interactive click-throughs of their UX flows and test them on the HoloLens.

Using these components with some Interactive controls, a designer can put together a basic click-through in a few hours without any code.

## ClickThroughExample:
A simple click-through of three panels. The Main Menu allows naivation to two other panels (Placement and Lobby). The only interactive button in the Placement panel is the Cancel button which navigates back to Main Menu. The Lobby panel has a toggle button (Join A Session) which will show a set of available sessions. Selecting the button again will hide the sessions and show the Cancel button to navigate back to the Main Menu.

With these base elements we can build out a much deeper set of interactive wireframes that can be tested on the HoloLens.

## PanelLayout:
A designer typically creates wireframes using an art program like Adobe Photoshop or Illustrator. The PanelLayout scene is an example of creating similar wireframes in Unity using primitives. A designer could create artwork and export it as a texture and use this workflow to apply textures to primitives and size them correctly.

The panels in this example are more complex than a typical wireframe, but they are a good sample of what the layout scripts can do.

Once a couple panels are built, add some Interactives to Activate and Deactivate them and you have a click-through.

##### This example shows how to use the following layout components.

#### PanelTransformSize
(Used in the Transform Panel) <br/>
A primitive Cube is transformed into a panel and sized similar to a RectTransform using a Vector3 to represent the Width, Height and Depth values.

The sizes are the same as pixel values a designer would use in their application. In fact they can enter the pixel values of a texture to properly size a Cube or Quad to match the texture size and aspect ratio.

I work with a concept that 2048x2048 pixels will cover a 1x1 meter area in holographic space. I set up my templates and UI design to work in this ratio. That way if there is a 1024x720 panel in my design program, the same values can be used in Unity using these components.
[See this article on Medium for more and an Adobe Illustrator template](https://medium.com/microsoft-design/how-to-think-about-designing-3d-space-b88faf609df4)

#### PanelTransformPosition
(Used in the Transform Panel) <br/>
Sized primitive cubes are positioned based on a parent or anchor transform. Similar to Unity's RectTransforms anchoring system. The alignment will set the object's center point to the position indicated. The Offset values provide precise placement.

Combined with PanelTransformSize, the borders were sized and positioned.

#### PanelTransformSizeOffset
(Used in the Transform Panel) <br/>
This component provides an alternate approach to sizing and positioning a primitive using margins or buffers around the edges of the object. By default the primitive will take on the size of the anchor transform, by adding buffer values for top and left will reduce the height and width and push the position to the bottom right.

This is a good component for stacking primitives together to make interesting panel designs.

The main content area of the panel, with the gutters around it, as well as the header area was built with this component.

#### SizeToRectTransform
(Used in Rect-Transform Grid and Rect-Transform Layout Groups) <br/>
These component leverages Unity's RectTransform layout system. It's a powerful system with Grid, Horizontal and Vertical layout groups, but making it work with primitives and 3D objects can be tricky.

1. Add a Canvas to the scene
2. For best results, scale the RectTransform to 0.0005, 0.0005, 0.0005.
3. Create a child RectTransform and position and size as needed.
4. Place a regular Transform based 3D primitive inside this RectTransform
5. Add the SizeToRectTransform component
6. The primitive will automatically scale to match the size of the parent RectTransform

SizeToRectTransform also has a Depth value (which the RectTransform does not have).
This component can also be used on other RectTransforms for nesting and positioning multiple elements. See the examples mentioned above for more details.

## CycleArray:
Demonstrates a system for incrementing through an array of values, such as vectors, colors or textures.
Each cube has a different CycleArray that cycles through the property indicated (Colors, Scale, Rotation, and Position). Each air-tap assigns the next value in the array.

The CycleArray can go forward, backward or set explicitly by index. CycleArray can be extended to handle incrementing through any number of properties, even the visibility of GameObjects.

This example also demonstrates the ColorTransition, MoveToPostion, ScaleToValue and RotateToValue components used for animating the value transitions.

## ColorCycleSwitcher:
An example of combining the CycleArray and GestureInteractive (InteractiveElements) to produce an interesting control that handles air-taps and horizontal gestures.

Air-tap on the control to see the colors update or tap-hold-drag to skip forward or backward.

## PositionAnObject:
At some point a prototype will contain something that needs to be re-positioned. This is an example of air-tapping on a panel and using gaze to move and rotate it. Air-tap again anywhere in space to stop the movement. The panel does not need to be under the user's gaze to catch the second air-tap. The MoveWithObject component can be configured with a magnetism effect and other properties to create some interesting interactions.

## ScaleObjectByDistance:
The ScaleObjectByDistance is a panel with a button. Air-tap the button to start scaling. Move closer to the panel to scale it down, move away to scale it up. air-tap again to stop scaling. The user can be looking anywhere for the second air-tap.
