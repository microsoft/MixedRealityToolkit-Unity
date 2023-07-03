// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Microsoft.MixedReality.Toolkit.Experimental
{
    /// <summary>
    /// A Unity component that is capable of routing child events to other child and parent target objects that contain
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
        private readonly HashSet<IXRInteractable> activeInteractables = new HashSet<IXRInteractable>();
        private readonly List<IXRInteractable> newInteractables = new List<IXRInteractable>();
        private readonly List<InteractableEventRouterChildSource> childSources = new List<InteractableEventRouterChildSource>();

        [SerializeReference]
        [InterfaceSelector(true)]
        [Experimental]
        IXRInteractableEventRoute[] eventRoutes = null;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            EnableEventRoutes();
            ConnectAllEventRoutesToInteractables();
            ConnectChildSources();
        }

        /// <summary>
        /// This function is called when the object becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            DisconnectChildSources();
            DisconnectAllEventRoutesFromKnownInteractables();
        }

        /// <summary>
        /// Callback sent to the object after a Transform children change occurs.
        /// </summary>
        protected virtual void OnTransformChildrenChanged()
        {
            Refresh();
        }

        /// <summary>
        /// Re-query for the list of child <see cref="IXRInteractableEventRouteTarget"/> components, and hook-up event handlers to these child objects.
        /// </summary>
        public void Refresh()
        {
            DisconnectChildSources();
            DisconnectAllEventRoutesFromKnownInteractables();
            EnableEventRoutes();
            ConnectAllEventRoutesToInteractables();
            ConnectChildSources();
        }

        /// <summary>
        /// Enable the current set of event routes.
        /// </summary>
        private void EnableEventRoutes()
        {
            if (eventRoutes != null)
            {
                for (int i = 0; i < eventRoutes.Length; i++)
                {
                    EnableEventRoute(eventRoutes[i]);
                }
            }
        }

        /// <summary>
        /// Enable a single event route.
        /// </summary>
        /// <param name="eventRoute">The event route to enable.</param>
        private void EnableEventRoute(IXRInteractableEventRoute eventRoute)
        {
            eventRoute.OnEnabled(gameObject);
        }

        /// <summary>
        /// Connect event handlers to <see cref="InteractableEventRouterChildSource"/> components in child game objects.
        /// </summary>
        private void ConnectChildSources()
        {
            GetComponentsInChildren(includeInactive: true, childSources);
            for (int i = 0; i < childSources.Count; i++)
            {
                childSources[i].ChildrenChanged.AddListener(ConnectAllEventRoutesToInteractables);
            }
        }

        /// <summary>
        /// Disconnect event handlers from <see cref="InteractableEventRouterChildSource"/> components in child game objects.
        /// </summary>
        private void DisconnectChildSources()
        {
            for (int i = 0; i < childSources.Count; i++)
            {
                childSources[i].ChildrenChanged.RemoveListener(ConnectAllEventRoutesToInteractables);
            }
        }

        /// <summary>
        /// Find all <see cref="IXRInteractableEventRouteTarget"/> components in child game objects, and register these components with
        /// the current set of <see cref="IXRInteractableEventRoute"/> objects.
        /// </summary>
        private void ConnectAllEventRoutesToInteractables()
        {
            GetComponentsInChildren(includeInactive: true, newInteractables);
            for (int i = 0; i < newInteractables.Count; i++)
            {
                var interactable = newInteractables[i];
                if (activeInteractables.Add(interactable) && IsValidChild(interactable) && eventRoutes != null)
                {
                    for (int j = 0; j < eventRoutes.Length; j++)
                    {
                        eventRoutes[j].Register(interactable);
                    }
                }
            }
        }


        /// <summary>
        /// Register the currently known <see cref="IXRInteractableEventRouteTarget"/> components with the specified <paramref name="eventRoute"/>.
        /// </summary>
        /// <param name="eventRoute">
        /// The known <see cref="IXRInteractableEventRouteTarget"/> components with be registered with this <see cref="IXRInteractableEventRoute"/> object. 
        /// </param>
        private void ConnectEventRouteToKnownInteractables(IXRInteractableEventRoute eventRoute)
        {
            if (eventRoute == null)
            {
                return;
            }

            foreach (var activeInteractable in activeInteractables)
            {
                eventRoute.Register(activeInteractable);
            }
        }

        /// <summary>
        /// Unregister the currently known <see cref="IXRInteractableEventRouteTarget"/> components with all the known <see cref="IXRInteractableEventRoute"/> objects.
        /// </summary>
        private void DisconnectAllEventRoutesFromKnownInteractables()
        {
            if (eventRoutes != null)
            {
                foreach (var interactable in activeInteractables)
                {
                    for (int j = 0; j < eventRoutes.Length; j++)
                    {
                        eventRoutes[j].Unregister(interactable);
                    }

                }
            }
            activeInteractables.Clear();
        }

        /// <summary>
        /// Unregister the currently known <see cref="IXRInteractableEventRouteTarget"/> components with the specified <paramref name="eventRoute"/>.
        /// </summary>
        /// <param name="eventRoute">
        /// The known <see cref="IXRInteractableEventRouteTarget"/> components with be unregistered with this <see cref="IXRInteractableEventRoute"/> object. 
        /// </param>
        private void DisconnectEventRouteFromKnownInteractables(IXRInteractableEventRoute eventRoute)
        {
            if (eventRoute == null)
            {
                return;
            }

            foreach (var activeInteractable in activeInteractables)
            {
                eventRoute.Unregister(activeInteractable);
            }
        }

        /// <summary>
        /// Determine if the given child interactable is valid. This will filter out references to this object, and block
        /// interactables that are being managed by another <see cref="InteractableEventRouter"/>.
        /// </summary>
        private bool IsValidChild(IXRInteractable interactable)
        {
            return interactable is MonoBehaviour behaviour &&
                behaviour.gameObject != gameObject;
        }

        /// <summary>
        /// Add the given event route type if not in the current set of routes.
        /// </summary>
        /// <typeparam name="T">The class type of the <see cref="IXRInteractableEventRoute"/> to add.</typeparam>
        public void AddEventRoute<T>() where T : IXRInteractableEventRoute, new() 
        {
            bool added = true;
            if (eventRoutes != null)
            {
                for (int i = 0; eventRoutes.Length > i; i++)
                {
                    if (eventRoutes[i] is T)
                    {
                        added = false;
                        break;
                    }
                }
            }

            if (added)
            {
                if (eventRoutes == null)
                {
                    eventRoutes = new IXRInteractableEventRoute[1];
                }
                else
                {
                    Array.Resize(ref eventRoutes, eventRoutes.Length + 1);
                }

                T newEventRoute = new T();
                eventRoutes[eventRoutes.Length - 1] = newEventRoute;

                if (Application.isPlaying && isActiveAndEnabled)
                {
                    EnableEventRoute(newEventRoute);
                    ConnectEventRouteToKnownInteractables(newEventRoute);
                }
            }
        }

        /// <summary>
        /// Remove the given event route type if in the current set of routes.
        /// </summary>
        /// <typeparam name="T">The class type of the <see cref="IXRInteractableEventRoute"/> to remove.</typeparam>
        public void RemoveEventRoute<T>() where T : IXRInteractableEventRoute, new()
        {
            if (eventRoutes != null)
            {
                int removeAt;
                T oldEventSource = default(T);
                for (removeAt = 0; removeAt < eventRoutes.Length; removeAt++)
                {
                    if (eventRoutes[removeAt] is T)
                    {
                        oldEventSource = (T)eventRoutes[removeAt];
                        break;
                    }
                }

                if (removeAt != eventRoutes.Length)
                {
                    for (int move = removeAt + 1; move < eventRoutes.Length; move++)
                    {
                        eventRoutes[move - 1] = eventRoutes[move];
                    }
                    Array.Resize(ref eventRoutes, eventRoutes.Length - 1);

                    if (Application.isPlaying && isActiveAndEnabled)
                    {
                        DisconnectEventRouteFromKnownInteractables(oldEventSource);
                    }
                }
            }
        }
    }

    /// <summary>
    /// This interface represents an object that relays
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
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
        /// <summary>
        /// Enable this event route by searching for <see cref="IXRInteractableEventRouteTarget"/>.
        /// </summary>
        /// <param name="origin">
        /// This game object will be queried for components that implement <see cref="IXRInteractableEventRouteTarget"/>.
        /// </param>
        void OnEnabled(GameObject origin);

        /// <summary>
        /// Starts listening to events from an unregistered 
        /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>.
        /// </summary>
        /// <param name="interactable">
        /// The interactable to register. Events will start being handled by this <see cref="IXRInteractableEventRoute"/>.
        /// </param>
        void Register(IXRInteractable interactable);

        /// <summary>
        /// Stop listening to events from a registered 
        /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>.
        /// </summary>
        /// <param name="interactable">
        /// The interactable to unregister. Events will no longer be handled by this <see cref="IXRInteractableEventRoute"/>.
        /// </param>
        void Unregister(IXRInteractable interactable);
    }

    /// <summary>
    /// This interface represents a target for events transmitted by <see cref="IXRInteractableEventRoute"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="InteractableEventRouter"/> class will search for classes that
    /// implement this interface.
    /// 
    /// 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IXRInteractableEventRouteTarget
    {
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a parent of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRHoverInteractable.html">IXRHoverInteractable</see>.
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
        /// <summary>
        /// When a child game object's interactable receives a "hover entered" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.HoverEnterEventArgs.html">HoverEnterEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnChildHoverEntered(HoverEnterEventArgs args);

        /// <summary>
        /// When a child game object's interactable receives a "hover exited" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.HoverExitEventArgs.html">HoverExitEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnChildHoverExited(HoverExitEventArgs args);
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a child of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRHoverInteractable.html">IXRHoverInteractable</see>.
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
        /// <summary>
        /// When a parent game object's interactable receives a "hover entered" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.HoverEnterEventArgs.html">HoverEnterEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnParentHoverEntered(HoverEnterEventArgs args);

        /// <summary>
        /// When a parent game object's interactable receives a "hover exited" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.HoverExitEventArgs.html">HoverExitEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnParentHoverExited(HoverExitEventArgs args);
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a parent of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRSelectInteractable.html">IXRSelectInteractable</see>.
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
        /// <summary>
        /// When a child game object's interactable receives a "select entered" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs.html">SelectEnterEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnChildSelectEntered(SelectEnterEventArgs args);

        /// <summary>
        /// When a child game object's interactable receives a "select exited" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs.html">SelectExitEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnChildSelectExited(SelectExitEventArgs args);
    }

    /// <summary>
    /// A specialized <see cref="IXRInteractableEventRouteTarget"/> that represents a child of a Unity
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRSelectInteractable.html">IXRSelectInteractable</see>.
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
        /// <summary>
        /// When a parent game object's interactable receives a "select entered" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs.html">SelectEnterEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnParentSelectEntered(SelectEnterEventArgs args);

        /// <summary>
        /// When a parent game object's interactable receives a "select exited" event, this function will be invoked.
        /// </summary>
        /// <param name="args">
        /// The Unity <see cref="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%401.0/api/UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs.html">SelectExitEventArgs</see>
        /// associated with the original interaction event.
        /// </param>
        void OnParentSelectExited(SelectExitEventArgs args);
    }

    /// <summary>
    /// A <see cref="IXRInteractableEventRoute"/> that relays
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
    /// events from interactables to game objects that contain <see cref="IXRInteractableEventRouteTarget"/> components.
    /// </summary>
    /// <typeparam name="S">
    /// The specialized type of <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see> that will be registered.
    /// Only events originating from this type will be handled.
    /// </typeparam>
    /// <typeparam name="T">
    /// The specialized type of <see cref="IXRInteractableEventRouteTarget"/> that will receive events originating from an interactable component.
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
        private List<T> targets = null;

        /// <inheritdoc/>
        public void OnEnabled(GameObject origin)
        {
            if (targets == null)
            {
                targets = new List<T>();
            }
            else
            {
                targets.Clear();
            }

            GetTargets(origin, targets);
            FilterTargets(origin, targets);
        }

        /// <inheritdoc/>
        public void Register(IXRInteractable interactable)
        {
            if (targets == null)
            {
                Debug.LogError("Unable to register an interactable with a `InteractableEventRoute`, since the `InteractableEventRoute` was never enabled.");
                return;
            }

            if (interactable is S source)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] is T target)
                    {
                        Register(source, target);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Unregister(IXRInteractable interactable)
        {
            if (targets == null)
            {
                return;
            }

            if (interactable is S source)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] is T target)
                    {
                        Unregister(source, target);
                    }
                }
            }
        }

        /// <summary>
        /// FIlter targets that are being targeted by other <see cref="InteractableEventRouter"/> objects.
        /// </summary>
        /// <param name="origin">The origin that is targeting <see cref="IXRInteractableEventRouteTarget"/>.</param>
        private void FilterTargets(GameObject origin, List<T> targets)
        {
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                var target = targets[i];
                if (target is MonoBehaviour behaviour)
                {
                    var router = behaviour.GetComponentInParent<InteractableEventRouter>();
                    if (router != null && router.gameObject != origin)
                    {
                        targets.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Search for and return all <see cref="IXRInteractableEventRouteTarget"/> objects that should receive events from the
        /// registered <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
        /// objects.
        /// </summary>
        /// <param name="origin">This game object will be queried for components that implement <see cref="IXRInteractableEventRouteTarget"/>.</param>
        /// <param name="targets">A list that should be filled with specialized <see cref="IXRInteractableEventRouteTarget"/> component objects.</param>
        protected abstract void GetTargets(GameObject origin, List<T> targets);

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
    /// A <see cref="InteractableEventRoute{S, T}"/> that targets child game objects the are parents of interactables.
    /// </summary>
    /// <typeparam name="S">
    /// The specialized type of <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see> 
    /// that will be registered with this. Only events originating from this type will be handled.
    /// </typeparam>
    /// <typeparam name="T">
    /// The specialized type of <see cref="IXRInteractableEventRouteTarget"/> that will receive interactable events.
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
        /// <summary>
        /// Search for and return all <see cref="IXRInteractableEventRouteTarget"/> objects that should receive events from the
        /// registered <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
        /// objects.
        /// </summary>
        /// <param name="origin">
        /// The game object whose children will be queried for components that implement <see cref="IXRInteractableEventRouteTarget"/>.
        /// </param>
        /// <param name="targets">A list that should be filled with specialized <see cref="IXRInteractableEventRouteTarget"/> component objects.</param>
        protected override void GetTargets(GameObject origin, List<T> targets)
        {
            origin.GetComponentsInChildren<T>(includeInactive: true, targets);
        }
    }


    /// <summary>
    /// A <see cref="InteractableEventRoute{S, T}"/> that targets child game objects the are children of interactables.
    /// </summary>
    /// <typeparam name="S">
    /// The specialized type of <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
    /// that will be registered with this. Only events originating from this type will be handled.
    /// </typeparam>
    /// <typeparam name="T">
    /// The specialized type of <see cref="IXRInteractableEventRouteTarget"/> that will receive interactables events.
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
        /// <summary>
        /// Search for and return all <see cref="IXRInteractableEventRouteTarget"/> objects that should receive events from the
        /// registered <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit%402.0/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractable.html">IXRInteractable</see>
        /// objects.
        /// </summary>
        /// <param name="origin">
        /// The game object whose children will be queried for components that implement <see cref="IXRInteractableEventRouteTarget"/>.
        /// </param>
        /// <param name="targets">A list that should be filled with specialized <see cref="IXRInteractableEventRouteTarget"/> component objects.</param>
        protected override void GetTargets(GameObject origin, List<T> targets)
        {
            origin.GetComponentsInChildren<T>(includeInactive: true, targets);
        }
    }

    /// <summary>
    /// A <see cref="InteractableParentEventRoute{S, T}"/> that retransmits hover events from
    /// child game objects up the hierarchy, to the child game object's parents.
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
    /// child game objects down the hierarchy, to the child game object's children.
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
    /// child game objects up the hierarchy, to the child game object's parents.
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
    /// child game objects down the hierarchy, to the child game object's children.
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
            source.selectExited.AddListener(target.OnParentSelectExited);
        }

        /// <inheritdoc/>
        protected override void Unregister(IXRSelectInteractable source, IXRSelectInteractableChild target)
        {
            source.selectEntered.RemoveListener(target.OnParentSelectEntered);
            source.selectExited.RemoveListener(target.OnParentSelectExited);
        }
    }
}
