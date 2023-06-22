﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Experimental
{
    /// <summary>
    /// A Unity component that is capable to routing child events to other child and parent target objects that contain
    /// a <see cref="IXRInteractableEventRouteTarget"/> component.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    [AddComponentMenu("MRTK/Core/Interactable Event Router")]
    public class InteractableEventRouter : MonoBehaviour
    {
        private IXRInteractable[] interactables = null;

        [SerializeReference]
        [InterfaceSelector(true)]
        [Experimental]
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

    /// <summary>
    /// This interface represents an object that relays
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
    /// events from interactables to game objects that contain <see cref="IXRInteractableEventRouteTarget"/> components.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IXRInteractableEventRoute
    {
        void OnEnabled(GameObject origin);

        void Register(IXRInteractable interactable);

        void Unregister(IXRInteractable interactable);
    }

    /// <summary>
    /// This interface represents a target for events transmitted by <see cref="IXRInteractableEventRoute"/>.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IXRInteractableEventRouteTarget : IXRInteractable
    {
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a parent of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRHoverInteractable.html">IXRHoverInteractable</see>.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IXRHoverInteractableParent : IXRInteractableEventRouteTarget
    {
        void OnChildHoverEntered(HoverEnterEventArgs args);

        void OnChildHoverExited(HoverExitEventArgs args);
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a child of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRHoverInteractable.html">IXRHoverInteractable</see>.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IXRHoverInteractableChild : IXRInteractableEventRouteTarget
    {
        void OnParentHoverEntered(HoverEnterEventArgs args);

        void OnParentHoverExited(HoverExitEventArgs args);
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a parent of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRSelectInteractable.html">IXRSelectInteractable</see>.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IXRSelectInteractableParent : IXRInteractableEventRouteTarget
    {
        void OnChildSelectEntered(SelectEnterEventArgs args);

        void OnChildSelectExited(SelectExitEventArgs args);
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a child of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRSelectInteractable.html">IXRSelectInteractable</see>.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IXRSelectInteractableChild : IXRInteractableEventRouteTarget
    {
        void OnParentSelectEntered(SelectEnterEventArgs args);

        void OnnParentSelectExited(SelectExitEventArgs args);
    }

    /// <summary>
    /// A <see cref="IXRInteractableEventRoute"/> that relays
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
    /// events from interactables to game objects that contain <see cref="IXRInteractableEventRouteTarget"/> components.
    /// </summary>
    /// <typeparam name="S">
    /// The specialized type of <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see> that will be registered.
    /// Only events originating from this type will be handled.
    /// </typeparam>
    /// <typeparam name="T">
    /// The specialized type of <see cref="IXRInteractableEventRouteTarget"/> that will receieve events originating from an interactable component.
    /// </typeparam>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public abstract class InteractableEventRoute<S, T> : IXRInteractableEventRoute
        where S : IXRInteractable
        where T : IXRInteractableEventRouteTarget
    {
        private T[] targets = null;

        /// <summary>
        /// Enable this event route by searching for <see cref="IXRInteractableEventRouteTarget"/>.
        /// </summary>
        /// <param name="origin">The game object that will be queied when searching for components that implement <see cref="IXRInteractableEventRouteTarget"/>.</param>
        public void OnEnabled(GameObject origin)
        {
            targets = GetTargets(origin);
        }


        /// <summary>
        /// Starts listening to events from an unregistered <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>.
        /// </summary>
        /// <param name="interactable">The interacble to register. Events will start being handled by this <see cref="IXRInteractableEventRoute"/>.</param>
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

        /// <summary>
        /// Stop listening to events from a registered <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>.
        /// </summary>
        /// <param name="interactable">The interacble to unregister. Events will no longer be handled by this <see cref="IXRInteractableEventRoute"/>.</param>
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

        /// <summary>
        /// Search for and return all <see cref="IXRInteractableEventRouteTarget"/> objects that should receive events from the
        /// registered <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
        /// objects.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        protected abstract T[] GetTargets(GameObject origin);

        /// <summary>
        /// Add event handlers that will handle events from the <paramref name="source"/> and direct them to the <paramref name="target"/>.
        /// </summary>
        /// <param name="source">
        /// The source of the interactable events.
        /// </param>
        /// <param name="target">
        /// The target to handle the interactable events.
        /// </param>
        protected abstract void Register(S source, T target);

        /// <summary>
        /// Remove event handlers that are handling events from the <paramref name="source"/> and directing them to the <paramref name="target"/>.
        /// </summary>
        /// <param name="source">
        /// The source of the interactable events.
        /// </param>
        /// <param name="target">
        /// The target to handle the interactable events.
        /// </param>
        protected abstract void Unregister(S source, T target);
    }

    /// <summary>
    /// A <see cref="InteractableEventRoute{S, T}"/> that targets game objects the are parents of interacbles.
    /// </summary>
    /// <typeparam name="S">
    /// The specialized type of <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see> that will be registered with this.
    /// Only events originating from this type will be handled.
    /// </typeparam>
    /// <typeparam name="T">
    /// The specialized type of <see cref="IXRInteractableEventRouteTarget"/> that will receieve the interacbles' events.
    /// </typeparam>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public abstract class InteractableParentEventRoute<S, T> : InteractableEventRoute<S, T>
        where S : IXRInteractable
        where T : IXRInteractableEventRouteTarget
    {
        /// <inheritdoc/>
        protected override T[] GetTargets(GameObject origin)
        {
            return origin.GetComponentsInParent<T>(includeInactive: true);
        }
    }


    /// <summary>
    /// A <see cref="InteractableEventRoute{S, T}"/> that targets game objects the are children of interacbles.
    /// </summary>
    /// <typeparam name="S">
    /// The specialized type of <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see> that will be registered with this.
    /// Only events originating from this type will be handled.
    /// </typeparam>
    /// <typeparam name="T">
    /// The specialized type of <see cref="IXRInteractableEventRouteTarget"/> that will receieve the interacbles' events.
    /// </typeparam>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public abstract class InteractableChildrenEventRoute<S, T> : InteractableEventRoute<S, T>
        where S : IXRInteractable
        where T : IXRInteractableEventRouteTarget
    {

        /// <inheritdoc/>
        protected override T[] GetTargets(GameObject origin)
        {
            return origin.GetComponentsInChildren<T>(includeInactive: true);
        }
    }

    /// <summary>
    /// A <see cref="InteractableParentEventRoute{S, T}"/> that retransmits hover events from
    /// child game objects up the heirarch, to the child game object's parents.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public sealed class BubbleChildHoverEvents : InteractableParentEventRoute<IXRHoverInteractable, IXRHoverInteractableParent>
    {
        /// <inheritdoc/>
        protected override void Register(IXRHoverInteractable source, IXRHoverInteractableParent target)
        {
            source.hoverEntered.AddListener(target.OnChildHoverEntered);
            source.hoverExited.AddListener(target.OnChildHoverExited);
        }

        /// <inheritdoc/>
        protected override void Unregister(IXRHoverInteractable source, IXRHoverInteractableParent target)
        {
            source.hoverEntered.RemoveListener(target.OnChildHoverEntered);
            source.hoverExited.RemoveListener(target.OnChildHoverExited);
        }
    }

    /// <summary>
    /// A <see cref="InteractableParentEventRoute{S, T}"/> that retransmits hover events from
    /// child game objects down the heirarch, to the child game object's children.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public sealed class TrickleChildHoverEvents : InteractableChildrenEventRoute<IXRHoverInteractable, IXRHoverInteractableChild>
    {
        /// <inheritdoc/>
        protected override void Register(IXRHoverInteractable source, IXRHoverInteractableChild target)
        {
            source.hoverEntered.AddListener(target.OnParentHoverEntered);
            source.hoverExited.AddListener(target.OnParentHoverExited);
        }

        /// <inheritdoc/>
        protected override void Unregister(IXRHoverInteractable source, IXRHoverInteractableChild target)
        {
            source.hoverEntered.RemoveListener(target.OnParentHoverEntered);
            source.hoverExited.RemoveListener(target.OnParentHoverExited);
        }
    }

    /// <summary>
    /// A <see cref="InteractableParentEventRoute{S, T}"/> that retransmits select events from
    /// child game objects up the heirarch, to the child game object's parents.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public sealed class BubbleChildSelectEvents : InteractableParentEventRoute<IXRSelectInteractable, IXRSelectInteractableParent>
    {
        /// <inheritdoc/>
        protected override void Register(IXRSelectInteractable source, IXRSelectInteractableParent target)
        {
            source.selectEntered.AddListener(target.OnChildSelectEntered);
            source.selectExited.AddListener(target.OnChildSelectExited);
        }

        /// <inheritdoc/>
        protected override void Unregister(IXRSelectInteractable source, IXRSelectInteractableParent target)
        {
            source.selectEntered.RemoveListener(target.OnChildSelectEntered);
            source.selectExited.RemoveListener(target.OnChildSelectExited);
        }
    }

    /// <summary>
    /// A <see cref="InteractableParentEventRoute{S, T}"/> that retransmits select events from
    /// child game objects down the heirarch, to the child game object's children.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public sealed class TrickleChildSelectEvents : InteractableChildrenEventRoute<IXRSelectInteractable, IXRSelectInteractableChild>
    {
        /// <inheritdoc/>
        protected override void Register(IXRSelectInteractable source, IXRSelectInteractableChild target)
        {
            source.selectEntered.AddListener(target.OnParentSelectEntered);
            source.selectExited.AddListener(target.OnnParentSelectExited);
        }

        /// <inheritdoc/>
        protected override void Unregister(IXRSelectInteractable source, IXRSelectInteractableChild target)
        {
            source.selectEntered.RemoveListener(target.OnParentSelectEntered);
            source.selectExited.RemoveListener(target.OnnParentSelectExited);
        }
    }
}
