using UnityEngine;
using UnityEngine.Events;

public class OnSelectEvent : MonoBehaviour
{
    public UnityEvent Event;

    void Start()
    {
        // dummy Start function so we can use this.enabled
    }

    void OnSelect()
    {
        if (this.enabled == false) return;
        if (Event != null)
        {
            Event.Invoke();
        }
    }
}