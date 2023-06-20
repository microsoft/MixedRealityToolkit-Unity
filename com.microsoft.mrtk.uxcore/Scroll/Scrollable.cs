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
    /// In order to recevie child select and hover event, this <see cref="Scrollable"/> object requires a <see cref="InteractableEventRouter"/> component be
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

        [Tooltip("The divisor to apply to the magnitude of interactor's dragged movement vector.")]
        [SerializeField]
        private float dragDivisor = 10.0f;

        /// <summary>
        /// The divisor to apply to the magnitude of interactor's dragged movement vector.
        /// </summary>
        public float DragDivisor
        {
            get => dragDivisor;
            set => dragDivisor = value;
        }

        readonly HashSetList<IXRInteractor> interactorsScrolling = new HashSetList<IXRInteractor>();
        readonly List<InteractorPosition> interactorPositions = new List<InteractorPosition>();
        List<IXRSelectInteractable> cancelableSelection;

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

                    displacementFromStart = (thisPoint - interactorPosition.startPoint).magnitude / dragDivisor;
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
                }


                if (displacementFromStart < 0.01f)
                {
                    velocity = Vector2.zero;
                }
                else
                {
                    velocity = dragDelta * (1.0f / Time.deltaTime);
                }

                if (scrollDelta.magnitude > 0.02f &&
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

                // velocity += ((dragDelta * (1.0f / Time.deltaTime) * Selectedness()) - velocity) * 0.1f;
                // velocity = newVelocity;             
                // scrollRect.velocity = new Vector2((dragDelta.x / contentWidth) * Selectedness(), (dragDelta.y / contentHeight) * Selectedness());
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// While in editor, verify that the sibling <see cref="InteractableEventRouter"/> has the neccessary event routes so
        /// child hover and select events are bubbled up to this component. Also, configure the scroll rect if not set.
        /// </summary>
        private void OnValidate()
        {
            var eventRouter = GetComponent<InteractableEventRouter>();
            if (eventRouter != null)
            {
                eventRouter.AddEventRoute<HoverParentEventRoute>();
                eventRouter.AddEventRoute<SelectParentEventRoute>();
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
                Debug.Log($"SCROLLABLE: Start scrolling with interactor ({interactor})");
                interactorPositions.Add(new InteractorPosition(manager, interactor));
            }
            else
            {
                Debug.Log($"SCROLLABLE: Already scrolling with interactor ({interactor})");
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
                        Debug.Log($"SCROLLABLE: Stop scrolling with interactor ({interactor})");
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
