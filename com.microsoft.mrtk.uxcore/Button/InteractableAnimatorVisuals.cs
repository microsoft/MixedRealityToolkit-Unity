// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// This visual driver sets various float parameters on the specified <see cref="Animator"/> based on the
    /// <see cref="StatefulInteractable"/>'s state. This is a temporary solution that will be mostly subsumed
    /// by the StateVisualizer architecture.
    /// </summary>
    [RequireComponent(typeof(StatefulInteractable))]
    [AddComponentMenu("MRTK/UX/Interactable Animator Visuals")]
    internal class InteractableAnimatorVisuals : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Interactable whose events and interaction state are forwarded to the Animator.")]
        private StatefulInteractable interactable;

        /// <summary>
        /// The Interactable whose events and interaction state are forwarded to the Animator.
        /// </summary>
        public StatefulInteractable Interactable
        {
            get
            {
                if (interactable == null)
                {
                    interactable = GetComponent<StatefulInteractable>();
                }
                return interactable;
            }
            set => interactable = value;
        }

        [SerializeField]
        [Tooltip("The Animator component to which this driver will feed interactable state.")]
        private Animator targetAnimator;

        /// <summary>
        /// The Animator component to feed interactable state to.
        /// </summary>
        public Animator TargetAnimator
        {
            get
            {
                if (targetAnimator == null)
                {
                    targetAnimator = GetComponent<Animator>();
                }
                return targetAnimator;
            }
            set => targetAnimator = value;
        }

        protected virtual void Update()
        {
            if (Interactable != null && TargetAnimator != null)
            {
                TargetAnimator.SetFloat("Selected", Interactable.Selectedness());
                TargetAnimator.SetFloat("ActiveFocus", Interactable.IsActiveHovered ? 1 : 0);
                TargetAnimator.SetFloat("PassiveFocus", Interactable.isHovered ? 1 : 0);
            }
        }
    }
}

