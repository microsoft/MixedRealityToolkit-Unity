using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class GestureResponder : MonoBehaviour, IInputHandler
{
    private void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);
    }

    public void OnInputUp(InputEventData eventData)
    {
        // Nothing to do
    }

    public void OnInputDown(InputEventData eventData)
    {
        // Nothing to do
    }

    public void OnInputClicked(InputEventData eventData)
    {
        PlaneTargetGroupPicker.Instance.PickNewTarget();
    }
}
