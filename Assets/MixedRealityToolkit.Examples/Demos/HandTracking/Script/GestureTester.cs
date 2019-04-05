// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class GestureTester : MonoBehaviour, IMixedRealityGestureHandler<Vector3>
{
    public GameObject HoldIndicator = null;
    public GameObject ManipulationIndicator = null;
    public GameObject NavigationIndicator = null;

    public Material DefaultMaterial = null;
    public Material HoldMaterial = null;
    public Material ManipulationMaterial = null;
    public Material NavigationMaterial = null;

    public GameObject RailsAxisX = null;
    public GameObject RailsAxisY = null;
    public GameObject RailsAxisZ = null;

    void OnEnable()
    {
        HideRails();
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log($"OnGestureStarted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Hold Action")
        {
            SetIndicator(HoldIndicator, "Hold: started", HoldMaterial);
        }
        else if (action == "Manipulate Action")
        {
            SetIndicator(ManipulationIndicator, $"Manipulation: started {Vector3.zero}", ManipulationMaterial, Vector3.zero);
        }
        else if (action == "Navigation Action")
        {
            SetIndicator(NavigationIndicator, $"Navigation: started {Vector3.zero}", NavigationMaterial, Vector3.zero);
            ShowRails(Vector3.zero);
        }
    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        Debug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Hold Action")
        {
            SetIndicator(HoldIndicator, "Hold: updated", DefaultMaterial);
        }
    }

    public void OnGestureUpdated(InputEventData<Vector3> eventData)
    {
        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Manipulate Action")
        {
            SetIndicator(ManipulationIndicator, $"Manipulation: updated {eventData.InputData}", ManipulationMaterial, eventData.InputData);
        }
        else if (action == "Navigation Action")
        {
            SetIndicator(NavigationIndicator, $"Navigation: updated {eventData.InputData}", NavigationMaterial, eventData.InputData);
            ShowRails(eventData.InputData);
        }
    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Hold Action")
        {
            SetIndicator(HoldIndicator, "Hold: completed", DefaultMaterial);
        }
    }

    public void OnGestureCompleted(InputEventData<Vector3> eventData)
    {
        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Manipulate Action")
        {
            SetIndicator(ManipulationIndicator, $"Manipulation: completed {eventData.InputData}", DefaultMaterial, eventData.InputData);
        }
        else if (action == "Navigation Action")
        {
            SetIndicator(NavigationIndicator, $"Navigation: completed {eventData.InputData}", DefaultMaterial, eventData.InputData);
            HideRails();
        }
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log($"OnGestureCanceled [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Hold Action")
        {
            SetIndicator(HoldIndicator, "Hold: canceled", DefaultMaterial);
        }
        else if (action == "Manipulate Action")
        {
            SetIndicator(ManipulationIndicator, "Manipulation: canceled", DefaultMaterial);
        }
        else if (action == "Navigation Action")
        {
            SetIndicator(NavigationIndicator, "Navigation: canceled", DefaultMaterial);
            HideRails();
        }
    }

    private void SetIndicator(GameObject indicator, string label, Material material)
    {
        if (indicator)
        {
            var renderer = indicator.GetComponentInChildren<Renderer>();
            if (material && renderer)
            {
                renderer.material = material;
            }
            var text = indicator.GetComponentInChildren<TextMesh>();
            if (text)
            {
                text.text = label;
            }
        }
    }

    private void SetIndicator(GameObject indicator, string label, Material material, Vector3 position)
    {
        SetIndicator(indicator, label, material);
        if (indicator)
        {
            indicator.transform.localPosition = position;
        }
    }

    private void ShowRails(Vector3 position)
    {
        var gestureProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.GesturesProfile;
        var useRails = gestureProfile.UseRailsNavigation;

        if (RailsAxisX)
        {
            RailsAxisX.SetActive(!useRails || position.x != 0.0f);
        }
        if (RailsAxisY)
        {
            RailsAxisY.SetActive(!useRails || position.y != 0.0f);
        }
        if (RailsAxisZ)
        {
            RailsAxisZ.SetActive(!useRails || position.z != 0.0f);
        }
    }

    private void HideRails()
    {
        if (RailsAxisX)
        {
            RailsAxisX.SetActive(false);
        }
        if (RailsAxisY)
        {
            RailsAxisY.SetActive(false);
        }
        if (RailsAxisZ)
        {
            RailsAxisZ.SetActive(false);
        }
    }
}
