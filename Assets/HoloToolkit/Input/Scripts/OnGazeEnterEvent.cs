using UnityEngine;
using UnityEngine.Events;

public class OnGazeEnterEvent : MonoBehaviour
{
    public UnityEvent Event;

    void Start()
    {
        // dummy Start function so we can use this.enabled
    }

    void OnGazeEnter()
    {
        if (this.enabled == false) return;
        if (Event != null)
        {
            Event.Invoke();
        }
    }
}