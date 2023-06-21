// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A Unity component that is capable to routing child events to other child and parent target objects that contain
    /// a <see cref="IXRInteractableEventRouteTarget"/> component.
    /// </summary>
    [AddComponentMenu("MRTK/Core/Interactable Event Router")]
    public class InteractableEventRouter : MonoBehaviour
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
            if (eventRoutes == null)
            {
                return;
            }

            IXRInteractable[] interactables = GetComponentsInChildren<IXRActivateInteractable>(includeInactive: true);
            for (int i = 0; i < eventRoutes.Length; i++)
            {
                var eventRoute = eventRoutes[i];
                if (eventRoute != null)
                {
                    eventRoute.OnEnabled(gameObject);
                    for (int j = 0; j < interactables.Length; j++)
                    {
                        var interactable = interactables[j];
                        if (IsValidChild(interactable))
                        {
                            eventRoute.Register(interactables[j]);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Get if the given child interactable is valid. This will filter out references to this object.
        /// </summary>
        private bool IsValidChild(IXRInteractable interactable)
        {
            return interactable is MonoBehaviour behaviour && behaviour.gameObject != gameObject;
        }

        private void DisconnectSourcesFromTargets()
        {
            if (interactables == null || eventRoutes == null)
            {
                return;
            }

            for (int i = 0; i < eventRoutes.Length; i++)
            {
                var eventRoute = eventRoutes[i];
                if (eventRoute != null)
                {
                    for (int j = 0; j < interactables.Length; j++)
                    {
                        eventRoute.Unregister(interactables[j]);
                    }

                }
            }
        }

        /// <summary>
        /// Add the given event route type if not in the current set of routes.
        /// </summary>
        public void AddEventRoute<T>() where T : IXRInteractableEventRoute, new() 
        {
            bool add = true;
            if (eventRoutes != null)
            {
                for (int i = 0; eventRoutes.Length > i; i++)
                {
                    if (eventRoutes[i] is T)
                    {
                        add = false;
                        break;
                    }
                }
            }

            if (add)
            {
                if (eventRoutes == null)
                {
                    eventRoutes = new IXRInteractableEventRoute[1];
                }
                else
                {
                    Array.Resize(ref eventRoutes, eventRoutes.Length + 1);
                }

                eventRoutes[eventRoutes.Length - 1] = new T();
            }
        }

        /// <summary>
        /// Remove the given event route type if in the current set of routes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveEventRoute<T>() where T : IXRInteractableEventRoute, new()
        {           
            if (eventRoutes != null)
            {
                int remove;
                for (remove = 0; remove < eventRoutes.Length; remove++)
                {
                    if (eventRoutes[remove] is T)
                    {
                        break;
                    }
                }

                if (remove != eventRoutes.Length)
                {
                    for (int move = remove + 1; move < eventRoutes.Length; move++)
                    {
                        eventRoutes[move - 1] = eventRoutes[move];
                    }
                    Array.Resize(ref eventRoutes, eventRoutes.Length - 1);
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
        void OnChildHoverEntered(HoverEnterEventArgs args);

        void OnChildHoverExited(HoverExitEventArgs args);
    }

    public interface IXRHoverInteractableChild : IXRInteractableEventRouteTarget
    {
        void OnParentHoverEntered(HoverEnterEventArgs args);

        void OnParentHoverExited(HoverExitEventArgs args);
    }

    public interface IXRSelectInteractableParent : IXRInteractableEventRouteTarget
    {
        void OnChildSelectEntered(SelectEnterEventArgs args);

        void OnChildSelectExited(SelectExitEventArgs args);
    }

    public interface IXRSelectInteractableChild : IXRInteractableEventRouteTarget
    {
        void OnParentSelectEntered(SelectEnterEventArgs args);

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
