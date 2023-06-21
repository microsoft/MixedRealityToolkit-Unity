// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// An <see cref="Microsoft.MixedReality.Toolkit.IScrollable">IScrollable</see> that allows a
    /// <see href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-ScrollRect.html">ScrollRect</see> to be scrolled by
    /// Unity <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.3/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractor.html">IXRInteractors</see>.
    /// </summary>
    /// <remarks>
    /// In order to receive child select and hover event, this <see cref="Scrollable"/> object requires a <see cref="InteractableEventRouter"/> component be
    /// added to the Unity game object as well. 
    /// </remarks>
    [AddComponentMenu("MRTK/UX/Scrollable")]
    [RequireComponent(typeof(InteractableEventRouter))]
    public class Scrollable : MRTKBaseInteractable, IScrollable, IXRHoverInteractableParent, IXRSelectInteractableParent
    {
        [Tooltip("The scroll rect to scroll.")]
        [SerializeField]
        private ScrollRect scrollRect = null;

        /// <summary>
        /// The Unity <see href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-ScrollRect.html">ScrollRect</see> to scroll.
        /// </summary>
        public ScrollRect ScrollRect
        {
            get => scrollRect;
            set => scrollRect = value;
        }

        [Tooltip("The scale factor to apply to the magnitude of interactor's dragged movement vector, the vector from start to end. The dragged magnitude is scaled by this amount. If the product is 1 or greater, the interactor's delta movement is applied directly to the scroll movement. If less than one, the interactor's delta movement is scaled by the product.")]
        [SerializeField]
        private float deadZoneFactor = 0.1f;

        /// <summary>
        /// The scale factor to apply to the magnitude of interactor's dragged movement vector, the from vector start to end.
        /// </summary>
        /// <remarks>
        /// The dragged magnitude is scaled by this amount. If the product is 1 or greater, the interactor's delta
        /// movement is applied directly to the scroll movement. If less than one, the interactor's delta movement
        /// is scaled by the product.
        /// </remarks>
        public float DeadZoneFactor
        {
            get => deadZoneFactor;
            set => deadZoneFactor = value;
        }

        [Tooltip("The scroll distance at which to cancel any child interactable's selection.")]
        [SerializeField]
        private float cancelSelectDistance = 0.05f;

        /// <summary>
        /// The scroll distance at which to cancel any child interactable's selection.
        /// </summary>
        /// <remarks>
        /// After the 2D plane has been scrolled this total distance, and if there is an active child selection, the child
        /// selection will be canceled.
        /// </remarks>
        public float CancelSelectDistance
        {
            get => cancelSelectDistance;
            set => cancelSelectDistance = value;
        }

        readonly HashSetList<IXRInteractor> interactorsScrolling = new HashSetList<IXRInteractor>();
        readonly List<InteractorPosition> interactorPositions = new List<InteractorPosition>();
        List<IXRSelectInteractable> cancelableSelection;


        /// <summary>
        /// Get the transform that is backing this scrollable about.
        /// </summary>
        public Transform ScrollableTransform => scrollRect.transform;

        /// <summary>
        /// Get if the scrollable is currently scrolling
        /// </summary>
        public bool IsScrolling => interactorPositions.Count > 0;

        /// <summary>
        /// Get the interactor that is scrolling the transform
        /// </summary>
        public IXRInteractor ScrolllingInteractor
        {
            get
            {
                if (interactorPositions.Count > 0)
                {
                    return interactorPositions[0].interactor;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the anchor position at the start of the scroll
        /// </summary>
        public Vector3 ScrollingAnchorPosition
        {
            get
            {
                if (interactorPositions.Count > 0)
                {
                    var interactorPosition = interactorPositions[0];
                    if (!interactorPosition.startPoint.IsValidVector())
                    {
                        Debug.Log($"SCROLLABLE: ScrollingAnchorPosition using interactor attach point");
                        return scrollRect.transform.InverseTransformPoint(interactorPosition.interactor.GetAttachTransform(this).position);
                    }
                    else
                    {
                        return interactorPosition.startPoint;
                    }
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        private struct InteractorPosition
        {
            public XRInteractionManager manager;
            public IXRInteractor interactor;
            public Vector3 lastPoint;
            public Vector3 startPoint;
            public Vector2 scrollStart;

            public InteractorPosition(XRInteractionManager manager, IXRInteractor interactor)
            {
                this.manager = manager;
                this.interactor = interactor;
                lastPoint = Vector3.positiveInfinity;
                startPoint = Vector3.positiveInfinity;
                scrollStart = Vector2.positiveInfinity;
            }
        }        

        private Vector2 velocity;

     
        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                Vector2 dragDelta = Vector2.zero;
                float displacementFromStart = 0;
                IXRInteractor draggingInteractor = null;
                XRInteractionManager draggingManager = null;
                Vector2 scrollDelta = Vector2.zero;

                if (interactorPositions.Count > 0)
                {
                    var interactorPosition = interactorPositions[0];
                    //Debug.Log($"SCROLLABLE: dragging ({interactorPosition.interactor})");
                    var thisPoint = scrollRect.transform.InverseTransformPoint(interactorPosition.interactor.GetAttachTransform(this).position);

                    if (!interactorPosition.startPoint.IsValidVector() ||
                        !interactorPosition.lastPoint.IsValidVector())
                    {
                        interactorPosition.startPoint = thisPoint;
                        interactorPosition.lastPoint = thisPoint;
                        interactorPosition.scrollStart = new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
                    }

                    draggingInteractor = interactorPosition.interactor;
                    draggingManager = interactorPosition.manager;
                    dragDelta = thisPoint - interactorPosition.lastPoint;
                    interactorPosition.lastPoint = thisPoint;
                    interactorPositions[0] = interactorPosition;

                    displacementFromStart = (thisPoint - interactorPosition.startPoint).magnitude * deadZoneFactor;
                    dragDelta *= Mathf.Clamp01(displacementFromStart);


                    float contentHeight = scrollRect.content.rect.height - scrollRect.viewport.rect.height;
                    float contentWidth = scrollRect.content.rect.width - scrollRect.viewport.rect.width;

                    if (contentHeight > 0.0f)
                    {
                        scrollRect.verticalNormalizedPosition -= (dragDelta.y / contentHeight);
                    }

                    if (contentWidth > 0.0f)
                    {
                        scrollRect.horizontalNormalizedPosition -= (dragDelta.x / contentWidth);
                    }

                    scrollDelta = new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition) - interactorPosition.scrollStart;
                    if (scrollDelta.magnitude > cancelSelectDistance &&
                        draggingInteractor is IXRSelectInteractor selector &&
                        selector.hasSelection &&
                        (selector.interactablesSelected.Count > 1 || !selector.interactablesSelected.Contains(this)))
                    {
                        if (cancelableSelection == null)
                        {
                            cancelableSelection = new List<IXRSelectInteractable>(selector.interactablesSelected.Count);
                        }

                        IXRSelectInteractable thisInteractable = this;

                        foreach (var interactable in selector.interactablesSelected)
                        {
                            if (interactable != thisInteractable)
                            {
                                Debug.Log($"SCROLLABLE: Should cancel select on ({interactable})");
                                cancelableSelection.Add(interactable);
                            }
                        }

                        if (cancelableSelection.Count > 0)
                        {
                            Debug.Log($"SCROLLABLE: Calling SelectEnter ({thisInteractable})");
                            draggingManager.SelectEnter(selector, thisInteractable);

                            foreach (var interactable in cancelableSelection)
                            {
                                if (interactable.isSelected &&
                                    interactable.interactorsSelecting.Contains(selector))
                                {
                                    Debug.Log($"SCROLLABLE: Calling SelectCancel ({interactable})");
                                    draggingManager.SelectCancel(selector, interactable);
                                }
                            }
                        }

                        cancelableSelection.Clear();
                    }
                }


                if (displacementFromStart < 0.01f)
                {
                    velocity = Vector2.zero;
                }
                else
                {
                    velocity = dragDelta * (1.0f / Time.deltaTime);
                }


                // velocity += ((dragDelta * (1.0f / Time.deltaTime) * Selectedness()) - velocity) * 0.1f;
                // velocity = newVelocity;             
                // scrollRect.velocity = new Vector2((dragDelta.x / contentWidth) * Selectedness(), (dragDelta.y / contentHeight) * Selectedness());
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Verfiy that this component's dependencies are met, and then try to fix dependencies where broken.
        /// </summary>
        /// <remarks>
        /// While in editor, verify that the sibling <see cref="InteractableEventRouter"/> has the neccessary event routes so
        /// child hover and select events are bubbled up to this component.
        ///
        /// Also, to avoid this interactable from steal childern collider, this Unity interactor must have its collider manually
        /// configure. This function will attempt to configure this interactor's collider property, and log a warning if the
        /// collider configuration fails.
        /// 
        /// Finally, this will also configure the scroll rect if not set.
        /// </remarks>
        private void OnValidate()
        {
            var eventRouter = GetComponent<InteractableEventRouter>();
            if (eventRouter != null)
            {
                eventRouter.AddEventRoute<HoverParentEventRoute>();
                eventRouter.AddEventRoute<SelectParentEventRoute>();
            }

            if (colliders.Count == 0)
            {
                var collider = GetComponent<Collider>();
                if (collider == null)
                {
                    Debug.LogWarning($"The Scrollable, {name}, does not have its colliders configured. This may result in child interactors failling to function properly. Configure this Scrollable component's colliders to avoid failures.");
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

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            IncreaseHoverCount(args);
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            DecreaseHoverCount(args);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            IncreaseSelectCount(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            DecreaseSelectCount(args);
        }

        public void OnChildHoverEntered(HoverEnterEventArgs args)
        {
            IncreaseHoverCount(args);
        }

        public void OnChildHoverExited(HoverExitEventArgs args)
        {
            DecreaseHoverCount(args);
        }

        public void OnChildSelectEntered(SelectEnterEventArgs args)
        {
            IncreaseSelectCount(args);
        }

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
            if (interactorsScrolling.Add(interactor))
            {
                interactorPositions.Add(new InteractorPosition(manager, interactor));
            }
        }

        private void StopScrollingWithInteractor(IXRInteractor interactor)
        {
            if (!HasSelection(interactor) &&
                !HasPokeHover(interactor) &&
                interactorsScrolling.Contains(interactor))
            {
                for (int i = 0; i < interactorPositions.Count; i++)
                {
                    if (interactorPositions[i].interactor == interactor)
                    {
                        Debug.Log("SCROLLABLE: remove scrolling interactor");
                        interactorPositions.RemoveAt(i);
                        interactorsScrolling.Remove(interactor);
                        break;
                    }
                }

                if (interactorsScrolling.Count == 0)
                {
                    scrollRect.velocity = velocity;
                }
            }
        }

        /// <summary>
        /// Get if the given interactor has a selection.
        /// </summary>
        private bool HasSelection(IXRInteractor interactor)
        {
            return (interactor is IXRSelectInteractor selector) && selector.hasSelection;
        }

        /// <summary>
        /// Get if the given interactor is a poke interactor and is hovering an interacble.
        /// </summary>
        private bool HasPokeHover(IXRInteractor interactor)
        {
            return (interactor is IPokeInteractor poker) && poker.hasHover;
        }
    }
}
