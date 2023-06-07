using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    public class XRInteractableEventRouter : MonoBehaviour
    {
        private IXRInteractable[] interactables = null;

        [SerializeReference]
        [InterfaceSelector(true)]
        IXRInteractableEventRoute[] eventRoutes = null;

        private void OnEnable()
        {
            ConnectSourcesToTargets();
        }

        private void OnDisable()
        {
            DisconnectSourcesFromTargets();
        }

        private void OnTransformChildrenChanged()
        {
            DisconnectSourcesFromTargets();
            ConnectSourcesToTargets();
        }

        private void ConnectSourcesToTargets()
        {
            IXRInteractable[] interactables = GetComponentsInChildren<IXRActivateInteractable>();
            for (int i = 0; i < eventRoutes.Length; i++)
            {
                eventRoutes[i].OnEnabled(gameObject);
                for (int j = 0; j < interactables.Length; j++)
                {
                    eventRoutes[i].Register(interactables[j]);
                }
            }
        }

        private void DisconnectSourcesFromTargets()
        {
            if (interactables == null)
            {
                return;
            }

            for (int i = 0; i < eventRoutes.Length; i++)
            {
                for (int j = 0; j < interactables.Length; j++)
                {
                    eventRoutes[i].Unregister(interactables[j]);
                }
            }
        }
    }

    public interface IXRInteractableEventRoute
    {
        void OnEnabled(GameObject origin);

        void Register(IXRInteractable interactable);

        void Unregister(IXRInteractable interactable);
    }

    public interface IXRInteractableEventRouteTarget : IXRInteractable
    {
    }

    public interface IXRHoverInteractableParent : IXRInteractableEventRouteTarget
    {
        void OnChildHoverEntering(HoverEnterEventArgs args);

        void OnChildHoverEntered(HoverEnterEventArgs args);

        void OnChildHoverExiting(HoverExitEventArgs args);

        void OnChildHoverExited(HoverExitEventArgs args);
    }

    public interface IXRHoverInteractableChild : IXRInteractableEventRouteTarget
    {
        void OnParentHoverEntering(HoverEnterEventArgs args);

        void OnParentHoverEntered(HoverEnterEventArgs args);

        void OnParentHoverExiting(HoverExitEventArgs args);

        void OnParentHoverExited(HoverExitEventArgs args);
    }

    public interface IXRSelectInteractableParent : IXRInteractableEventRouteTarget
    {
        void OnChildSelectEntering(SelectEnterEventArgs args);

        void OnChildSelectEntered(SelectEnterEventArgs args);

        void OnChildSelectExiting(SelectExitEventArgs args);

        void OnChildSelectExited(SelectExitEventArgs args);
    }

    public interface IXRSelectInteractableChild : IXRInteractableEventRouteTarget
    {
        void OnParentSelectEntering(SelectEnterEventArgs args);

        void OnParentSelectEntered(SelectEnterEventArgs args);

        void OnnParentSelectExiting(SelectExitEventArgs args);

        void OnnParentSelectExited(SelectExitEventArgs args);
    }

    public abstract class XRInteractableEventRoute<S, T> : IXRInteractableEventRoute
        where S : IXRInteractable
        where T : IXRInteractableEventRouteTarget
    {
        private T[] targets = null;

        public void OnEnabled(GameObject origin)
        {
            targets = GetTargets(origin);
        }


        public void Register(IXRInteractable interactable)
        {
            if (interactable is S source)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] is T target)
                    {
                        Register(source, target);
                    }
                }
            }
        }

        public void Unregister(IXRInteractable interactable)
        {
            if (interactable is S source)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] is T target)
                    {
                        Unregister(source, target);
                    }
                }
            }
        }

        protected abstract T[] GetTargets(GameObject origin);

        protected abstract void Register(S source, T target);

        protected abstract void Unregister(S source, T target);
    }

    public abstract class XRParentInteractableEventRoute<S, T> : XRInteractableEventRoute<S, T>
        where S : IXRInteractable
        where T : IXRInteractableEventRouteTarget
    {

        protected override T[] GetTargets(GameObject origin)
        {
            return origin.GetComponentsInParent<T>(includeInactive: true);
        }
    }

    public abstract class XRChildInteractableEventRoute<S, T> : XRInteractableEventRoute<S, T>
        where S : IXRInteractable
        where T : IXRInteractableEventRouteTarget
    {

        protected override T[] GetTargets(GameObject origin)
        {
            return origin.GetComponentsInChildren<T>(includeInactive: true);
        }
    }

    public sealed class HoverParentEventRoute : XRParentInteractableEventRoute<IXRHoverInteractable, IXRHoverInteractableParent>
    {
        protected override void Register(IXRHoverInteractable source, IXRHoverInteractableParent target)
        {
            source.hoverEntered.AddListener(target.OnChildHoverEntered);
            source.hoverExited.AddListener(target.OnChildHoverExited);
        }

        protected override void Unregister(IXRHoverInteractable source, IXRHoverInteractableParent target)
        {
            source.hoverEntered.RemoveListener(target.OnChildHoverEntered);
            source.hoverExited.RemoveListener(target.OnChildHoverExited);
        }
    }

    public sealed class HoverChildEventRoute : XRChildInteractableEventRoute<IXRHoverInteractable, IXRHoverInteractableParent>
    {
        protected override void Register(IXRHoverInteractable source, IXRHoverInteractableParent target)
        {
            source.hoverEntered.AddListener(target.OnChildHoverEntered);
            source.hoverExited.AddListener(target.OnChildHoverExited);
        }

        protected override void Unregister(IXRHoverInteractable source, IXRHoverInteractableParent target)
        {
            source.hoverEntered.RemoveListener(target.OnChildHoverEntered);
            source.hoverExited.RemoveListener(target.OnChildHoverExited);
        }
    }

    public sealed class SelectParentEventRoute : XRParentInteractableEventRoute<IXRSelectInteractable, IXRSelectInteractableParent>
    {
        protected override void Register(IXRSelectInteractable source, IXRSelectInteractableParent target)
        {
            source.selectEntered.AddListener(target.OnChildSelectEntered);
            source.selectExited.AddListener(target.OnChildSelectExited);
        }

        protected override void Unregister(IXRSelectInteractable source, IXRSelectInteractableParent target)
        {
            source.selectEntered.RemoveListener(target.OnChildSelectEntered);
            source.selectExited.RemoveListener(target.OnChildSelectExited);
        }
    }

    public sealed class SelectChildEventRoute : XRChildInteractableEventRoute<IXRSelectInteractable, IXRSelectInteractableChild>
    {
        protected override void Register(IXRSelectInteractable source, IXRSelectInteractableChild target)
        {
            source.selectEntered.AddListener(target.OnParentSelectEntered);
            source.selectExited.AddListener(target.OnnParentSelectExited);
        }

        protected override void Unregister(IXRSelectInteractable source, IXRSelectInteractableChild target)
        {
            source.selectEntered.RemoveListener(target.OnParentSelectEntered);
            source.selectExited.RemoveListener(target.OnnParentSelectExited);
        }
    }
}
