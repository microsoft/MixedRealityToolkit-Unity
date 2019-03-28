using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class FollowMeToggle : MonoBehaviour
{
    private Orbital orbital;

    private void Start()
    {
        // Get Orbital Solver component
        orbital = GetComponent<Orbital>();
    }

    public void ToggleFollowMeBehavior()
    {
        if(orbital != null)
        {
            // Toggle Orbital Solver component
            // You can tweak the detailed positioning behavior such as offset, lerping time, orientation type in the Inspector panel
            orbital.enabled = !orbital.enabled;
        }
        
    }
}
