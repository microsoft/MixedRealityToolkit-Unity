using UnityEngine;
using UnityEngine.Events;

public class OnSelectEvent : MonoBehaviour
{
    public UnityEvent Event;
    public void OnSelect()
    {
        if (Event != null)
        {
            Event.Invoke();
        }
    }
}