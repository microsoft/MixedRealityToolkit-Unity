using UnityEngine;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Services.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;

public class InputTest : InputSystemGlobalListener, IMixedRealityInputHandler, IMixedRealityInputHandler<Vector2>
{
    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        Debug.Log($"{ToString(eventData.MixedRealityInputAction)} , Input: {eventData.InputData}");
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log(ToString(eventData.MixedRealityInputAction));
    }

    public void OnInputPressed(InputEventData<float> eventData)
    {
        Debug.Log(ToString(eventData.MixedRealityInputAction));
    }

    public void OnInputUp(InputEventData eventData){ }

    public void OnPositionInputChanged(InputEventData<Vector2> eventData) { }

    private string ToString(MixedRealityInputAction action)
    {
        return $"Description: {action.Description}, ID: {action.Id}, Constraint: {action.AxisConstraint}";
    }
}
