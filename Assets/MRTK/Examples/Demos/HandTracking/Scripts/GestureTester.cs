// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [AddComponentMenu("Scripts/MRTK/Examples/GestureTester")]
    public class GestureTester : MonoBehaviour, IMixedRealityGestureHandler<Vector3>
    {
        [Header("Gesture indicators")]

        [SerializeField, FormerlySerializedAs("HoldIndicator")]
        private GameObject holdIndicator = null;

        [SerializeField, FormerlySerializedAs("ManipulationIndicator")]
        private GameObject manipulationIndicator = null;

        [SerializeField, FormerlySerializedAs("NavigationIndicator")]
        private GameObject navigationIndicator = null;

        [SerializeField, FormerlySerializedAs("SelectIndicator")]
        private GameObject selectIndicator = null;

        [Header("Gesture materials")]

        [SerializeField, FormerlySerializedAs("DefaultMaterial")]
        private Material defaultMaterial = null;

        [SerializeField, FormerlySerializedAs("HoldMaterial")]
        private Material holdMaterial = null;

        [SerializeField, FormerlySerializedAs("ManipulationMaterial")]
        private Material manipulationMaterial = null;

        [SerializeField, FormerlySerializedAs("NavigationMaterial")]
        private Material navigationMaterial = null;

        [SerializeField, FormerlySerializedAs("SelectMaterial")]
        private Material selectMaterial = null;

        [Header("Rails axis visuals")]

        [SerializeField, FormerlySerializedAs("RailsAxisX")]
        private GameObject railsAxisX = null;

        [SerializeField, FormerlySerializedAs("RailsAxisY")]
        private GameObject railsAxisY = null;

        [SerializeField, FormerlySerializedAs("RailsAxisZ")]
        private GameObject railsAxisZ = null;

        [Header("Mapped gesture input actions")]

        [SerializeField]
        private MixedRealityInputAction holdAction = MixedRealityInputAction.None;

        [SerializeField]
        private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;

        [SerializeField]
        private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;

        [SerializeField]
        private MixedRealityInputAction tapAction = MixedRealityInputAction.None;

        private void OnEnable()
        {
            HideRails();
        }

        public void OnGestureStarted(InputEventData eventData)
        {
            Debug.Log($"OnGestureStarted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

            MixedRealityInputAction action = eventData.MixedRealityInputAction;
            if (action == holdAction)
            {
                SetIndicator(holdIndicator, "Hold: started", holdMaterial);
            }
            else if (action == manipulationAction)
            {
                SetIndicator(manipulationIndicator, $"Manipulation: started {Vector3.zero}", manipulationMaterial, Vector3.zero);
            }
            else if (action == navigationAction)
            {
                SetIndicator(navigationIndicator, $"Navigation: started {Vector3.zero}", navigationMaterial, Vector3.zero);
                ShowRails(Vector3.zero);
            }

            SetIndicator(selectIndicator, "Select:", defaultMaterial);
        }

        public void OnGestureUpdated(InputEventData eventData)
        {
            Debug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

            MixedRealityInputAction action = eventData.MixedRealityInputAction;
            if (action == holdAction)
            {
                SetIndicator(holdIndicator, "Hold: updated", defaultMaterial);
            }
        }

        public void OnGestureUpdated(InputEventData<Vector3> eventData)
        {
            Debug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

            MixedRealityInputAction action = eventData.MixedRealityInputAction;
            if (action == manipulationAction)
            {
                SetIndicator(manipulationIndicator, $"Manipulation: updated {eventData.InputData}", manipulationMaterial, eventData.InputData);
            }
            else if (action == navigationAction)
            {
                SetIndicator(navigationIndicator, $"Navigation: updated {eventData.InputData}", navigationMaterial, eventData.InputData);
                ShowRails(eventData.InputData);
            }
        }

        public void OnGestureCompleted(InputEventData eventData)
        {
            Debug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

            MixedRealityInputAction action = eventData.MixedRealityInputAction;
            if (action == holdAction)
            {
                SetIndicator(holdIndicator, "Hold: completed", defaultMaterial);
            }
            else if (action == tapAction)
            {
                SetIndicator(selectIndicator, "Select: completed", selectMaterial);
            }
        }

        public void OnGestureCompleted(InputEventData<Vector3> eventData)
        {
            Debug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

            MixedRealityInputAction action = eventData.MixedRealityInputAction;
            if (action == manipulationAction)
            {
                SetIndicator(manipulationIndicator, $"Manipulation: completed {eventData.InputData}", defaultMaterial, eventData.InputData);
            }
            else if (action == navigationAction)
            {
                SetIndicator(navigationIndicator, $"Navigation: completed {eventData.InputData}", defaultMaterial, eventData.InputData);
                HideRails();
            }
        }

        public void OnGestureCanceled(InputEventData eventData)
        {
            Debug.Log($"OnGestureCanceled [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

            MixedRealityInputAction action = eventData.MixedRealityInputAction;
            if (action == holdAction)
            {
                SetIndicator(holdIndicator, "Hold: canceled", defaultMaterial);
            }
            else if (action == manipulationAction)
            {
                SetIndicator(manipulationIndicator, "Manipulation: canceled", defaultMaterial);
            }
            else if (action == navigationAction)
            {
                SetIndicator(navigationIndicator, "Navigation: canceled", defaultMaterial);
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
                var text = indicator.GetComponentInChildren<TextMeshPro>();
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
            var gestureProfile = CoreServices.InputSystem.InputSystemProfile.GesturesProfile;
            var useRails = gestureProfile.UseRailsNavigation;

            if (railsAxisX)
            {
                railsAxisX.SetActive(!useRails || position.x != 0.0f);
            }
            if (railsAxisY)
            {
                railsAxisY.SetActive(!useRails || position.y != 0.0f);
            }
            if (railsAxisZ)
            {
                railsAxisZ.SetActive(!useRails || position.z != 0.0f);
            }
        }

        private void HideRails()
        {
            if (railsAxisX)
            {
                railsAxisX.SetActive(false);
            }
            if (railsAxisY)
            {
                railsAxisY.SetActive(false);
            }
            if (railsAxisZ)
            {
                railsAxisZ.SetActive(false);
            }
        }
    }
}
