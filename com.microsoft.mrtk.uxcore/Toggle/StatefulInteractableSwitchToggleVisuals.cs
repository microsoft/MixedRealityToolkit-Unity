// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A visuals script to provide a visual layer on top of a
    /// <see cref="StatefulInteractable"/>.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Stateful Interactable Switch Toggle Visuals")]
    public class StatefulInteractableSwitchToggleVisuals : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        private StatefulInteractable statefulInteractable;

        [SerializeField]
        [Tooltip("The GameObject that contains the toggle thumb.")]
        private GameObject toggleRoot;

        [SerializeField]
        [Tooltip("The relative space from the switch's origin that is covered. Typically set to 0.5f")]
        private float toggleOffset = 0.5f;

        [Header("Easing")]
        [SerializeField]
        private float duration = 0.2f;

        [SerializeField]
        private AnimationCurve animationCurve;

        #endregion

        #region MonoBehaviours

        public void Awake()
        {
            // If the StatefulInteractable is null, 
            if (statefulInteractable == null)
            {
                statefulInteractable = GetComponent<StatefulInteractable>();
            }

            // Initializing the toggle state
            bool isToggled = statefulInteractable.IsToggled;

            if (isToggled)
            {
                toggleRoot.transform.localPosition = Vector3.right * toggleOffset;
            }
            else
            {
                toggleRoot.transform.localPosition = Vector3.left * toggleOffset;
            }

            lastToggleState = isToggled;
        }

        public void LateUpdate()
        {
            UpdateAllVisuals();
        }
        #endregion

        #region Visuals

        // Used to ensure we only update visuals when the toggle state changes
        private bool lastToggleState;

        // Used to animate the switch toggle based on the assignable easing properties
        private float animationTimer = float.MaxValue;

        private void UpdateAllVisuals()
        {
            bool isToggled = statefulInteractable.IsToggled;

            if (lastToggleState != isToggled)
            {
                animationTimer = 0.0f;
                lastToggleState = isToggled;
            }

            if (animationTimer < duration)
            {
                animationTimer += Time.deltaTime;
                if (isToggled)
                {
                    toggleRoot.transform.localPosition = Vector3.Lerp(Vector3.left * toggleOffset, Vector3.right * toggleOffset, animationCurve.Evaluate(animationTimer / duration));
                }
                else
                {
                    toggleRoot.transform.localPosition = Vector3.Lerp(Vector3.right * toggleOffset, Vector3.left * toggleOffset, animationCurve.Evaluate(animationTimer / duration));
                }
            }
        }


        private void OnDrawGizmos()
        {
            Vector3 toggleRelativeScale = toggleRoot.transform.lossyScale;
            float toggleGizmoScaleFactor = 0.001f;

            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(toggleRoot.transform.parent.position + toggleOffset * toggleRelativeScale.x * Vector3.left, toggleRelativeScale.magnitude * toggleGizmoScaleFactor);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(toggleRoot.transform.parent.position + toggleOffset * toggleRelativeScale.x * Vector3.right, toggleRelativeScale.magnitude * toggleGizmoScaleFactor);
        }

        #endregion
    }
}
