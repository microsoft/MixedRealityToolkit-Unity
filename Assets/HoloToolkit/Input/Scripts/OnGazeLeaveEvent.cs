using UnityEngine;
using UnityEngine.Events;

public class OnGazeLeaveEvent : MonoBehaviour
{
    public UnityEvent Event;

    void Start()
    {
        // dummy Start function so we can use this.enabled
    }

    void OnGazeLeave()
    {
        if (this.enabled == false) return;
        if (Event != null)
        {
            Event.Invoke();
        }
    }
}