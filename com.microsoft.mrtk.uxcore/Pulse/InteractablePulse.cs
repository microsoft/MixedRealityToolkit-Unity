// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.GraphicsTools;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A script to enable pulse effect on <see cref="StatefulInteractable"/>.
    /// </summary>
    [RequireComponent(typeof(StatefulInteractable))]
    [AddComponentMenu("MRTK/UX/Interactable Pulse")]
    public class InteractablePulse : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The FrontPlatePulse component to perform the pulse effect.")]
        private FrontPlatePulse pulse;

        private StatefulInteractable interactable;

        private StatefulInteractable Interactable
        {
            get
            {
                if (interactable == null)
                {
                    interactable = GetComponent<StatefulInteractable>();
                }
                return interactable;
            }
        }

        protected void OnEnable()
        {
            Interactable.selectEntered.AddListener(OnSelectEntered);
        }

        protected void OnDisable()
        {
            Interactable.selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactorObject is IHandedInteractor handedInteractor)
            {
                pulse.Pulse(handedInteractor.GetAttachTransform(Interactable).position, handedInteractor.Handedness == Handedness.Left);
            }
        }
    }
}
