using UnityEngine;
using UnityEngine.Events;

public class OnGazeLeaveEvent : MonoBehaviour
{
    public UnityEvent Event;
    public void OnGazeLeave()
    {
        if (Event != null)
        {
            Event.Invoke();
        }
    }
}