// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Allows a <see cref="ScrollRect"/> to be scrolled by XRI interactors.
    /// </summary>
    public class Scrollable : MRTKBaseInteractable, IXRHoverInteractableParent, IXRSelectInteractableParent
    {

        [Tooltip("The scroll rect to scroll.")]
        [SerializeField]
        private ScrollRect scrollRect = null;

        /// <summary>
        /// The <see cref="ScrollRect"/> to scroll.
        /// </summary>
        public ScrollRect ScrollRect
        {
            get => scrollRect;
            set => scrollRect = value;
        }

        [Tooltip("The scroll rect to scroll.")]
        [SerializeField]
        private float deadzone = 10.0f;

        readonly HashSetList<IXRInteractor> interactorsScrolling = new HashSetList<IXRInteractor>();
        readonly List<InteractorPosition> interactorPositions = new List<InteractorPosition>();
        List<IXRSelectInteractable> cancelableSelection;
        List<Collider> disabledColliders;

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
                IXRInteractor draggingIntector = null;
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

                    draggingIntector = interactorPosition.interactor;
                    draggingManager = interactorPosition.manager;
                    dragDelta = thisPoint - interactorPosition.lastPoint;
                    interactorPosition.lastPoint = thisPoint;
                    interactorPositions[0] = interactorPosition;

                    displacementFromStart = (thisPoint - interactorPosition.startPoint).magnitude / deadzone;
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
                    draggingIntector is IXRSelectInteractor selector &&
                    selector.hasSelection &&
                    (selector.interactablesSelected.Count > 1 || !selector.interactablesSelected.Contains(this)))
                {
                    if (cancelableSelection == null)
                    {
                        cancelableSelection = new List<IXRSelectInteractable>(selector.interactablesSelected.Count);
                    }

                    if (disabledColliders == null)
                    {
                        disabledColliders = new List<Collider>(selector.interactablesSelected.Count);
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

                    GetComponentsInChildren(includeInactive: false, disabledColliders);
                    if (disabledColliders.Count > 0)
                    {
                        foreach (var c in disabledColliders)
                        {
                            if (c.gameObject != gameObject)
                            {
                                Debug.Log($"SCROLLABLE: Disabling collider ({c})");
                                c.enabled = false;
                            }
                        }
                    }

                    if (cancelableSelection.Count > 0)
                    {
                        selector.PreprocessInteractor(updatePhase);
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

                    if (disabledColliders.Count > 0)
                    {
                        foreach (var c in disabledColliders)
                        {
                           //c.enabled = true;
                        }
                    }

                    cancelableSelection.Clear();
                    disabledColliders.Clear();
                }

                // velocity += ((dragDelta * (1.0f / Time.deltaTime) * Selectedness()) - velocity) * 0.1f;
                // velocity = newVelocity;             
                // scrollRect.velocity = new Vector2((dragDelta.x / contentWidth) * Selectedness(), (dragDelta.y / contentHeight) * Selectedness());
            }
        }


        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            //Debug.Log("SCROLLABLE: Hover Entered");
            base.OnHoverEntered(args);
            IncreaseHoverCount(args);
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            //Debug.Log("SCROLLABLE: Hover Exitted");
            base.OnHoverExited(args);
            DescreaseHoverCount(args);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            Debug.Log("SCROLLABLE: Select Entered");
            base.OnSelectEntered(args);
            IncreaseSelectCount(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            Debug.Log("SCROLLABLE: Select Exitted");
            base.OnSelectExited(args);
            DecreaseSelectCount(args);
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            //Debug.Log("SCROLLABLE: Activated");
            base.OnActivated(args);
        }

        protected override void OnDeactivated(DeactivateEventArgs args)
        {
            //Debug.Log("SCROLLABLE: Deactived");
            base.OnDeactivated(args);
        }

        public void OnChildHoverEntered(HoverEnterEventArgs args)
        {
           // Debug.Log("SCROLLABLE: Child Hover Entered");
            this.IncreaseHoverCount(args);
        }

        public void OnChildHoverExited(HoverExitEventArgs args)
        {
            //Debug.Log("SCROLLABLE: Child Hover Exitted");
            this.DescreaseHoverCount(args);
        }

        public void OnChildSelectEntered(SelectEnterEventArgs args)
        {
            //Debug.Log("SCROLLABLE: Child Select Entered");
            IncreaseSelectCount(args);
        }

        public void OnChildSelectExited(SelectExitEventArgs args)
        {
            //Debug.Log("SCROLLABLE: Child Select Exitted");
            DecreaseSelectCount(args);
        }

        private void IncreaseHoverCount(HoverEnterEventArgs args)
        {
            if (args.interactorObject is IPokeInteractor)
            {
                StartScrollingWithInteractor(args.manager, args.interactorObject);
            }
        }

        private void DescreaseHoverCount(HoverExitEventArgs args)
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
            if (interactorsScrolling.Contains(interactor))
            {
                for (int i = 0; i < interactorPositions.Count; i++)
                {
                    if (interactorPositions[i].interactor == interactor &&
                        (!(interactor is IXRSelectInteractor selector) || !selector.hasSelection) &&
                        (!(interactor is IPokeInteractor poker) || !poker.hasHover))
                    {
                        Debug.Log($"SCROLLABLE: Stop scrolling with interactor ({interactor})");
                        interactorPositions.RemoveAt(i);
                        interactorsScrolling.Remove(interactor);
                        break;
                    }
                    else
                    {
                        Debug.Log($"SCROLLABLE: Can't stop scrolling with interactor ({interactor})");
                        if (interactor is IXRSelectInteractor s)
                        {
                            foreach (var si in s.interactablesSelected)
                            {
                                Debug.Log($"SCROLLABLE: --------> still selected ({si})");

                            }
                        }

                        if (interactor is IPokeInteractor p)
                        {

                            foreach (var si in p.interactablesHovered)
                            {
                                Debug.Log($"SCROLLABLE: --------> still hovered ({si})");

                            }
                        }
                    }
                }

                if (interactorsScrolling.Count == 0)
                {
                    scrollRect.velocity = velocity;
                }
            }
        }
    }
}
