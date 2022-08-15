using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    public class InternalTestInteractor: IXRInteractor
    {
        public InteractionLayerMask interactionLayers => throw new NotImplementedException();

        public Transform transform => null;

        public event Action<InteractorRegisteredEventArgs> registered;
        public event Action<InteractorUnregisteredEventArgs> unregistered;

        public Transform GetAttachTransform(IXRInteractable interactable)
        {
            return transform;
        }

        public void GetValidTargets(List<IXRInteractable> targets)
        {
        }

        public void OnRegistered(InteractorRegisteredEventArgs args)
        {
        }

        public void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
        }

        public void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
        }

        public void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
        }

    }

    public class TestInteractor : MonoBehaviour
    {
        [SerializeField]
        XRInteractionManager m_InteractionManager;

        InternalTestInteractor myInteractor = new InternalTestInteractor();

        private void Awake()
        {
            m_InteractionManager.RegisterInteractor(myInteractor);
        }
    }
}
