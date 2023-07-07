// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// An <see cref="Microsoft.MixedReality.Toolkit.IScrollable">IScrollable</see> that allows a
    /// <see href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-ScrollRect.html">ScrollRect</see> to be scrolled by
    /// Unity <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.3/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractor.html">IXRInteractors</see>.
    /// </summary>
    /// <remarks>
    /// In order to receive child select and hover event, this <see cref="Scrollable"/> object requires a <see cref="InteractableEventRouter"/> component be
    /// added to the Unity game object as well.
    /// 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    [AddComponentMenu("MRTK/UX/Scrollable")]
    [RequireComponent(typeof(InteractableEventRouter))]
    public class Scrollable : MRTKBaseInteractable, IScrollable, IXRHoverInteractableParent, IXRSelectInteractableParent
    {
        /// <summary>
        /// The current scrollVelocity of the scroll, to be applied once scrolling is complete.
        /// </summary>
        private Vector2 scrollVelocity;

        /// <summary>
        /// The scroller's goal position
        /// </summary>
        private Vector2 scrollGoal;

        /// <summary>
        /// The list of interactors states tracking the interactors' positions and dead zones
        /// </summary>
        private readonly OrderedDictionary states = new OrderedDictionary();

        /// <summary>
        /// A cache of interactables whose selection should be canceled.
        /// </summary>
        private List<IXRSelectInteractable> cancelableSelections;

        [Tooltip("The scroll rect to scroll.")]
        [SerializeField]
        [Experimental]
        private ScrollRect scrollRect = null;

        /// <summary>
        /// The Unity <see href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-ScrollRect.html">ScrollRect</see> to scroll.
        /// </summary>
        public ScrollRect ScrollRect
        {
            get => scrollRect;
            set => scrollRect = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float moveLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float MoveLerpTime
        {
            get => moveLerpTime;
            set => moveLerpTime = value;
        }

        [SerializeField]
        [Tooltip("Once the ScrollingInteractor moves this distance or more, this component will start changing scroll position of the ScrollRect.")]
        private float deadZone = 0.05f;

        /// <summary>
        /// Once the <see cref="ScrollingInteractor"/> moves this distance or more, this component will
        /// start changing scroll positions of the <see cref="ScrollRect"/>.
        /// </summary>
        /// <remarks>
        /// When the <see cref="ScrollingInteractor"/> moves this distance away from the starting point, the positions of <see cref="ScrollRect"/>
        /// will start being altered.
        ///
        /// If <see cref="ScrollingInteractor"/> implements <see cref="Microsoft.MixedReality.Toolkit.IPokeInteractor">IPokeInteractor</see>,
        /// the <see cref="PokeDeadZone"/> property will be used instead.
        /// </remarks>
        public float DeadZone
        {
            get => deadZone;
            set => deadZone = value;
        }

        [Tooltip("The scroll distance at which to cancel any child interactable's selection.")]
        [SerializeField]
        private float cancelSelectDistance = 0.06f;

        /// <summary>
        /// The scroll distance at which to cancel any child interactable's selection.
        /// </summary>
        /// <remarks>
        /// After the 2D plane has been scrolled this total distance, and if there is an active child selection, the child
        /// selection will be canceled.
        ///
        /// If <see cref="ScrollingInteractor"/> implements <see cref="Microsoft.MixedReality.Toolkit.IPokeInteractor">IPokeInteractor</see>, the <see cref="PokeCancelSelectDistance"/>
        /// property is used instead.
        /// </remarks>
        public float CancelSelectDistance
        {
            get => cancelSelectDistance;
            set => cancelSelectDistance = value;
        }

        [SerializeField]
        [Tooltip("Once the ScrollingInteractor moves this distance or more, this component will start changing scroll position of the ScrollRect. This only used if ScrollingInteractor implements IPokeInteractor.")]

        private float pokeDeadZone = 0.01f;

        /// <summary>
        /// Once the <see cref="ScrollingInteractor"/> moves this distance or more, this component will
        /// start changing scroll positions of the <see cref="ScrollRect"/>.
        /// </summary>
        /// <remarks>
        /// When the <see cref="ScrollingInteractor"/> moves this distance away from the starting point, the positions of <see cref="ScrollRect"/>
        /// will start being altered.
        ///
        /// If <see cref="ScrollingInteractor"/> does not implement <see cref="Microsoft.MixedReality.Toolkit.IPokeInteractor">IPokeInteractor</see>,
        /// the <see cref="DeadZone"/> property will be used instead.
        /// </remarks>
        public float PokeDeadZone
        {
            get => deadZone;
            set => deadZone = value;
        }

        [Tooltip("The scroll distance at which to cancel any child interactable's selection. This only used if ScrollingInteractor implements IPokeInteractor.")]
        [SerializeField]
        private float pokeCancelSelectDistance = 0.02f;

        /// <summary>
        /// The scroll distance at which to cancel any child interactable's selection.
        /// </summary>
        /// <remarks>
        /// After the 2D plane has been scrolled this total distance, and if there is an active child selection, the child
        /// selection will be canceled.
        ///
        /// If <see cref="ScrollingInteractor"/> does not implement <see cref="Microsoft.MixedReality.Toolkit.IPokeInteractor">IPokeInteractor</see>, the <see cref="CancelSelectDistance"/>
        /// property is used instead.
        /// </remarks>
        public float PokeCancelSelectDistance
        {
            get => pokeCancelSelectDistance;
            set => pokeCancelSelectDistance = value;
        }

        /// <inheritdoc />
        public Transform ScrollableTransform => scrollRect.transform;

        /// <inheritdoc />
        public bool IsScrolling
        {
            get
            {
                if (states.Count > 0)
                {
                    return GetCurrentScrollingInteractionData().IsScrolling;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public IXRInteractor ScrollingInteractor
        {
            get
            {
                if (states.Count > 0)
                {
                    return GetCurrentScrollingInteractionData().Interactor;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <inheritdoc />
        public Vector3 ScrollingLocalAnchorPosition
        {
            get
            {
                if (states.Count > 0)
                {
                    return scrollRect.transform.InverseTransformPoint(GetCurrentScrollingInteractionData().StartPosition);
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Verify that this component's dependencies are met, and then try to fix dependencies where broken.
        /// </summary>
        /// <remarks>
        /// While in editor, verify that the sibling <see cref="InteractableEventRouter"/> has the necessary event routes so
        /// child hover and select events are bubbled up to this component.
        ///
        /// Also, to avoid this interactable from consuming colliders belonging to child interactables, this Unity interactor must
        /// have its collider manually configured. This function will attempt to configure this interactor's collider property,
        /// and log a warning if the collider configuration fails.
        /// 
        /// Finally, this will also configure the scroll rect if not set.
        /// </remarks>
        private void OnValidate()
        {
            var eventRouter = GetComponent<InteractableEventRouter>();
            if (eventRouter != null)
            {
                eventRouter.AddEventRoute<BubbleChildHoverEvents>();
                eventRouter.AddEventRoute<BubbleChildSelectEvents>();
            }

            if (colliders.Count == 0)
            {
                var collider = GetComponent<Collider>();
                if (collider == null)
                {
                    Debug.LogWarning($"The Scrollable, {name}, does not have its colliders configured. This may result in child interactors being inaccessible by interaction managers. Configure this Scrollable component's colliders to avoid failures.");
                }
                else
                {
                    colliders.Add(collider);
                }
            }

            if (scrollRect == null)
            {
                scrollRect = GetComponent<ScrollRect>();
            }
        }
#endif

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic &&
                UpdateScrolling(out ScrollingInteractorData data))
            {
                CancelSelectionsIfNeeded(in data);
            }
        }

        /// <inheritdoc />
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            IncreaseHoverCount(args);
        }

        /// <inheritdoc />
        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            DecreaseHoverCount(args);
        }

        /// <inheritdoc />
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            IncreaseSelectCount(args);
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            DecreaseSelectCount(args);
        }

        /// <inheritdoc />
        public void OnChildHoverEntered(HoverEnterEventArgs args)
        {
            IncreaseHoverCount(args);
        }

        /// <inheritdoc />
        public void OnChildHoverExited(HoverExitEventArgs args)
        {
            DecreaseHoverCount(args);
        }

        /// <inheritdoc />
        public void OnChildSelectEntered(SelectEnterEventArgs args)
        {
            IncreaseSelectCount(args);
        }

        /// <inheritdoc/>
        public void OnChildSelectExited(SelectExitEventArgs args)
        {
            DecreaseSelectCount(args);
        }

        private void IncreaseHoverCount(HoverEnterEventArgs args)
        {
            if (args.interactorObject is IPokeInteractor)
            {
                StartScrollingWithInteractor(args.manager, args.interactorObject);
            }
        }

        private void DecreaseHoverCount(HoverExitEventArgs args)
        {
            if (args.interactorObject is IPokeInteractor)
            {
                StopScrollingWithInteractor(args.interactorObject);
            }
        }

        private void IncreaseSelectCount(SelectEnterEventArgs args)
        {
            if (!(args.interactorObject is IPokeInteractor))
            {
                StartScrollingWithInteractor(args.manager, args.interactorObject);
            }
        }

        private void DecreaseSelectCount(SelectExitEventArgs args)
        {
            if (!(args.interactorObject is IPokeInteractor))
            {
                StopScrollingWithInteractor(args.interactorObject);
            }
        }

        private void StartScrollingWithInteractor(XRInteractionManager manager, IXRInteractor interactor)
        {
            bool isPokeInteractor = interactor is IPokeInteractor;
            bool wasEmpty = states.Count == 0;
            states[interactor] = new ScrollingInteractorData(
                manager,
                interactor,
                scrollRect.transform,
                isPokeInteractor ? pokeDeadZone : deadZone,
                isPokeInteractor ? pokeCancelSelectDistance : cancelSelectDistance);

            // Initialize the scroll goal to the current scroll rect position
            if (wasEmpty)
            {
                scrollGoal = scrollRect.normalizedPosition;
            }
        }

        private void StopScrollingWithInteractor(IXRInteractor interactor)
        {
            if (!HasSelection(interactor) &&
                !HasPokeHover(interactor) &&
                states.Contains(interactor))
            {
                states.Remove(interactor);
                if (states.Count == 0)
                {
                    scrollRect.velocity = scrollVelocity;
                }
            }
        }

        /// <summary>
        /// Update the scrolling positions for the given data structure.
        /// </summary>
        private bool UpdateScrolling(out ScrollingInteractorData data)
        {
            if (states.Count == 0)
            {
                data = default;
                return false;
            }

            // Get the active set of scrolling data
            data = GetCurrentScrollingInteractionData();

            // Validate the interactor is hovering or selecting something.
            // If not, remove interactor's state and abort.
            if (!HasPokeHover(data.Interactor) && !HasSelection(data.Interactor))
            {
                states.Remove(data.Interactor);
                return false;
            }

            // Update the interactor's position
            data.UpdatePosition(data.Interactor.GetAttachTransform(this).position);

            // Determine if scrolling is active, and update drag delta
            bool isScrolling = data.IsScrolling || data.ScrollMovementSquareMagnitude > data.DeadZoneSquared;
            Vector2 dragDelta = isScrolling ? data.LocalPosition - data.LocalPreviousPosition : Vector2.zero;
            data.IsScrolling = isScrolling;

            // Update scrolling positions
            float contentWidth = scrollRect.content.rect.width - scrollRect.viewport.rect.width;
            if (contentWidth > 0.0f)
            {
                scrollGoal.x -= dragDelta.x / contentWidth;
                scrollRect.horizontalNormalizedPosition = Smoothing.SmoothTo(scrollRect.horizontalNormalizedPosition, scrollGoal.x, moveLerpTime, Time.deltaTime);
            }

            float contentHeight = scrollRect.content.rect.height - scrollRect.viewport.rect.height;
            if (contentHeight > 0.0f)
            {
                scrollGoal.y -= dragDelta.y / contentHeight;
                scrollRect.verticalNormalizedPosition = Smoothing.SmoothTo(scrollRect.verticalNormalizedPosition, scrollGoal.y, moveLerpTime, Time.deltaTime);
            }

            scrollVelocity = dragDelta * (1.0f / Time.deltaTime);
            UpdateCurrentScrollingInteractionData(data);
            return true;
        }

        /// <summary>
        /// Cancel selection on child interactables being selected by the given data's interactor.
        /// </summary>
        private void CancelSelectionsIfNeeded(in ScrollingInteractorData data)
        {
            var scrollerMovementSquared = data.ScrollMovementSquareMagnitude;
            if (scrollerMovementSquared > data.CancelSelectionDistanceSquared &&
                data.Interactor is IXRSelectInteractor selector &&
                IsSelectingChild(selector))
            {
                if (cancelableSelections == null)
                {
                    cancelableSelections = new List<IXRSelectInteractable>(selector.interactablesSelected.Count);
                }
                else
                {
                    cancelableSelections.Clear();
                }

                foreach (var interactable in selector.interactablesSelected)
                {
                    if (interactable != (IXRSelectInteractable)this)
                    {
                        cancelableSelections.Add(interactable);
                    }
                }

                if (cancelableSelections.Count > 0)
                {
                    data.Manager.SelectEnter(selector, this);
                    foreach (var interactable in cancelableSelections)
                    {
                        if (interactable.isSelected &&
                            interactable.interactorsSelecting.Contains(selector))
                        {
                            data.Manager.SelectCancel(selector, interactable);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the current ScrollingInteractorData.
        /// </summary>
        private ScrollingInteractorData GetCurrentScrollingInteractionData()
        {
            return (ScrollingInteractorData)states[0];
        }

        /// <summary>
        /// Update the current ScrollingInteractorData.
        /// </summary>
        private void UpdateCurrentScrollingInteractionData(in ScrollingInteractorData data)
        {
            states[0] = data;
        }

        /// <summary>
        /// Get if the given interactor has a selection.
        /// </summary>
        private bool HasSelection(IXRInteractor interactor)
        {
            return (interactor is IXRSelectInteractor selector) && selector.hasSelection;
        }

        /// <summary>
        /// Get if the given interactor is a poke interactor and is hovering an interactable.
        /// </summary>
        private bool HasPokeHover(IXRInteractor interactor)
        {
            return (interactor is IPokeInteractor poker) && poker.hasHover;
        }

        /// <summary>
        /// Get if the interactor is selecting a child interactor.
        /// </summary>
        private bool IsSelectingChild(IXRSelectInteractor interactor)
        {
            return interactor.hasSelection &&
                (interactor.interactablesSelected.Count > 1 || !interactor.interactablesSelected.Contains(this));
        }

        /// <summary>
        /// A set of data tracking an interactor's manager and position as it scrolls through the scroll region of a <see cref="Scrollable"/> component.
        /// </summary>
        private struct ScrollingInteractorData
        {
            /// <summary>
            /// Thw interactors manager
            /// </summary>
            public XRInteractionManager Manager { get; private set; }

            /// <summary>
            /// The interactor wanting to scroll
            /// </summary>
            public IXRInteractor Interactor { get; private set; }

            /// <summary>
            /// The scroll region the interactor is acting upon
            /// </summary>
            public Transform ScrollRegion { get; private set; }

            /// <summary>
            /// Get the interactor's previous position, relative to scroll regions coordinate space.
            /// </summary>
            public Vector3 LocalPreviousPosition { get; private set; }

            /// <summary>
            /// Get the interactor's previous position, relative to world space.
            /// </summary>
            public Vector3 PreviousPosition => ScrollRegion.TransformPoint(LocalPreviousPosition);

            /// <summary>
            /// Get the interactor's last sampled position, relative to world space.
            /// </summary>
            public Vector3 Position => ScrollRegion.TransformPoint(LocalPosition);

            /// <summary>
            /// Get the interactor's current position, relative to scroll regions coordinate space.
            /// </summary>
            public Vector3 LocalPosition { get; private set; }

            /// <summary>
            /// Get the interactor's current position, relative to world space.
            /// </summary>
            public Vector3 StartPosition => ScrollRegion.TransformPoint(LocalStartPosition);

            /// <summary>
            /// Get the interactor's starting position, relative to scroll regions coordinate space.
            /// </summary>
            public Vector3 LocalStartPosition { get; private set; }

            /// <summary>
            /// Get or set if the interactor has moving past the dead zone distance, and started scrolling
            /// </summary>
            public bool IsScrolling { get; set; }

            /// <summary>
            /// Get the dead zone distance for this interactor.
            /// </summary>
            public float DeadZoneSquared { get; private set; }

            /// <summary>
            /// Get the cancel selection distance for this interactor.
            /// </summary>
            public float CancelSelectionDistanceSquared { get; private set; }

            /// <summary>
            /// Get the move vector of the scroll.
            /// </summary>
            public Vector2 LocalScrollMovement => (LocalStartPosition - LocalPosition);

            /// <summary>
            /// Get total scroll movement along the scroll plane, at world scale.
            /// </summary>
            public float ScrollMovementSquareMagnitude => ScrollRegion.TransformVector(LocalScrollMovement).sqrMagnitude;

            private bool positionInitialized;

            public ScrollingInteractorData(
                XRInteractionManager manager,
                IXRInteractor interactor,
                Transform scrollRegion,
                float deadZone,
                float cancelSelectionDistance)
            {

                Manager = manager;
                Interactor = interactor;
                ScrollRegion = scrollRegion;
                LocalPosition = Vector3.zero;
                LocalStartPosition = Vector3.zero;
                LocalPreviousPosition = Vector3.zero;
                DeadZoneSquared = deadZone * deadZone;
                CancelSelectionDistanceSquared = cancelSelectionDistance * cancelSelectionDistance;
                IsScrolling = false;
                positionInitialized = false;
            }

            /// <summary>
            /// Set the interactor's scroll position, relative to world space.
            /// </summary>
            public void UpdatePosition(in Vector3 position)
            {
                LocalPreviousPosition = LocalPosition;
                LocalPosition = ScrollRegion.InverseTransformPoint(position);

                if (!positionInitialized)
                {
                    positionInitialized = true;
                    LocalPreviousPosition = LocalPosition;
                    LocalStartPosition = LocalPosition;
                }
            }
        }
    }
}
