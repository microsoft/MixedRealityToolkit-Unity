# Accessing input state in MRTK

It's possible to directly query the state of all inputs in MRTK by iterating over the controllers attached to the input sources. MRTK also provides convenience methods for accessing the position and rotation of the eyes, hands, head, and motion controller.

See the InputDataExample scene for an example of querying input both via iterating over controllers, and by using the [`InputRayUtils`](xref:Microsoft.MixedReality.Toolkit.Input.InputRayUtils) class.

## Example: Access position, rotation of head, hands, eyes in MRTK

MRTK's [`InputRayUtils`](xref:Microsoft.MixedReality.Toolkit.Input.InputRayUtils) class provides convenience methods for accessing the hand ray, head ray, eye gaze ray, and motion controller rays.

```c#
// Get the head ray
var headRay = InputRayUtils.GetHeadGazeRay();

// Get the right hand ray
Ray rightHandRay;
if(InputRayUtils.TryGetHandRay(Handedness.right, rightHandRay))
{
    // Right hand ray is available
}
else
{
    // Right hand ray is not available
}
```

## Example: Access position, rotation of all 6DOF controllers active in scene

```c#
foreach(var controller in CoreServices.InputSystem.DetectedControllers)
{
    // Interactions for a controller is the list of inputs that this controller exposes
    foreach(MixedRealityInteractionMapping inputMapping in controller.Interactions)
    {
        // 6DOF controllers support the "SpatialPointer" type (pointing direction)
        // or "GripPointer" type (direction of the 6DOF controller)
        if (inputMapping.InputType == DeviceInputType.SpatialPointer)
        {
            Debug.Log("spatial pointer PositionData: " + inputMapping.PositionData);
            Debug.Log("spatial pointer RotationData: " + inputMapping.RotationData);
        }

        if (inputMapping.InputType == DeviceInputType.SpatialGrip)
        {
            Debug.Log("spatial grip PositionData: " + inputMapping.PositionData);
            Debug.Log("spatial grip RotationData: " + inputMapping.RotationData);
        }
    }
}
```

## See also

- [InputEvents](InputEvents.md)
- [Pointers](Pointers.md)
- [HandTracking](HandTracking.md)
