using UnityEngine;
using UnityEngine.Events;

public class OnGazeEnterEvent : MonoBehaviour
{
    public UnityEvent Event;
    public void OnGazeEnter()
    {
        if (Event != null)
        {
            Event.Invoke();
        }
    }
}