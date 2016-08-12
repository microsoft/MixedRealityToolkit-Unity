using UnityEngine;
using System.Collections;

public class GestureResponder : MonoBehaviour
{
    // Responds to the gesture manager's "TappedEvent"
    public void OnSelect()
    {
        PlaneTargetGroupPicker.Instance.PickNewTarget();
    }
}
