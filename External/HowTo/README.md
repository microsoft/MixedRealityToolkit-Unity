# How to ...
This readme is intended to document any questions developers might have around how to achieve certain things using the HoloToolkit.

## How to migrate from the old SendMessage based input model to the new one?
Ensure you have the released version or latest HoloToolkit-Unity packages imported into your project.

###Focus aka Gaze
Previously, you attached a GazeManager script onto your Managers object.
You might also have used the old MainCamera.prefab.

In the new input model:
1. For the camera prefab, you can use the MixedRealityCamera.prefab or HoloLensCamera.prefab.
2. Next, create a Managers gameobject in your Hierarchy.
3. Make the InputManager prefab a child of the Managers object.

On the InputManager prefab you will see everything is wired up for getting started.
It has the updated GazeManager.cs along with GazeStabilizer.
It also has the stabilization plane being set using the StabilizationPlaneModifier.cs.
FocusManager.cs script helps make the GazeManager.cs more extensible for VR devices.

**Refer to test scene: Assets\HoloToolkit\Input\Tests\Scenes\InputTapTest.unity**

###Gestures
Previously, you attached the GestureManager script to your Managers object.

In the new input model:
1. Once you have made the InputManager prefab a child of the Managers object.

Expand the InputManager prefab and you will notice EditorHandsInput which handles hands input in the editor Play mode.
GesturesInput gameobject has GesturesInput script which is the replacement for GestureManager and uses the GestureRecognizer.
GamepadInput.cs is the script added to handle the Xbox gamepad and maps A to air tap, hold. It also maps pressed A and left joystick to navigation events.

###How to handle the tap gesture?
You might have had the OnSelect function or some other method name responding to the user's tap gesture.

1. You can keep using the same .cs file.
2. In that MonoBehavior, implement the interface called IInputClickHandler. This will get raised when a tap is detected by GesturesInput.
3. The event data inside the OnInputClicked event handler will provide with the necessary information needed for example tap count etc.

**Refer to test scene: Assets\HoloToolkit\Input\Tests\Scenes\InputTapTest.unity**

###How to handle the navigation gesture?

1. In your MonoBehavior, implement the interface called INavigationHandler. These events will get raised for navigation events.
2. The event data inside the event handler will provide with navigation offset to take your action.

**Refer to test scene: Assets\HoloToolkit\Input\Tests\Scenes\InputNavigationRotateTest.unity**