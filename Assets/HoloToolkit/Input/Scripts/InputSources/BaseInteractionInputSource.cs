using HoloToolkit.Unity.InputModule;
using UnityEngine;
#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

public abstract class BaseInteractionInputSource : BaseInputSource, IInteractionInputSource
{
#if UNITY_WSA
    public abstract bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind);

    public abstract bool TryGetPointerRotation(uint sourceId, out Quaternion rotation);

    public abstract bool TryGetGripPosition(uint sourceId, out Vector3 position);

    public abstract bool TryGetGripRotation(uint sourceId, out Quaternion rotation);

    public abstract bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position);

    public abstract bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position);

    public abstract bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedValue);

    public abstract bool TryGetGrasp(uint sourceId, out bool isPressed);

    public abstract bool TryGetMenu(uint sourceId, out bool isPressed);
#endif
}
