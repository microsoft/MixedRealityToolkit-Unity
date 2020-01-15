using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleProfileTest : MonoBehaviour
{
    private void Start()
    { }

    [SerializeField]
    MixedRealityToolkitConfigurationProfile profile0 = null;
    [SerializeField]
    MixedRealityToolkitConfigurationProfile profile1 = null;

    public void ToggleProfile()
    {
        MixedRealityToolkitConfigurationProfile newProfile = (MixedRealityToolkit.Instance.ActiveProfile == profile0) ? profile1 : profile0;
        Debug.Log($"Switching to {newProfile}");
        MixedRealityToolkit.Instance.ActiveProfile = newProfile;
    }
}
