// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Allows a <see cref="ScrollRect"/> to be scrolled by XRI interactors.
    /// </summary>
    public class Scrollable : PressableButton, IXRHoverInteractableParent, IXRSelectInteractableParent
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
        
        private Dictionary<IXRInteractor, Vector2> touchPoints = new Dictionary<IXRInteractor, Vector2>();

        private Vector2 velocity;

        private Vector2 startNormalizedPosition;

        private Vector2 sprungNormalizedPosition;
        private Vector2 startTouchPoint;

        private bool isDead;

        private int hoverCounts = 0;
        private int selectCounts = 0;

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                Vector2 dragDelta = Vector2.zero;
                Vector2 thisFrame = startTouchPoint;

                foreach (var interactor in interactorsSelecting)
                {
                    Vector2 lastFrame = Vector2.zero;
                    thisFrame = scrollRect.transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);

                    if (touchPoints.ContainsKey(interactor))
                    {
                        lastFrame = touchPoints[interactor];
                    }
                    else
                    {
                        lastFrame = thisFrame;
                    }

                    dragDelta = thisFrame - lastFrame;
                    touchPoints[interactor] = thisFrame;
                }

                float displacementFromStart = (thisFrame - startTouchPoint).magnitude / deadzone;

                //Debug.Log(displacementFromStart.ToString("F3"));

                dragDelta *= Mathf.Clamp01(displacementFromStart);
    

                float contentHeight = scrollRect.content.rect.height - scrollRect.viewport.rect.height;
                float contentWidth = scrollRect.content.rect.width - scrollRect.viewport.rect.width;

                if (contentHeight > 0.0f)
                    scrollRect.verticalNormalizedPosition -= (dragDelta.y / contentHeight);

                if (contentWidth > 0.0f)
                    scrollRect.horizontalNormalizedPosition -= (dragDelta.x / contentWidth);

                
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

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            Debug.Log("SCROLLABLE: Hover Entered");
            base.OnHoverEntered(args);
            this.IncreaseHoverCount(args);
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            Debug.Log("SCROLLABLE: Hover Exitted");
            base.OnHoverExited(args);
            this.DescreaseHoverCount(args);
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
        }

        public void OnChildHoverEntering(HoverEnterEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Hover Entering");
        }

        public void OnChildHoverEntered(HoverEnterEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Hover Entered");
            this.IncreaseHoverCount(args);
        }

        public void OnChildHoverExiting(HoverExitEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Hover Exitting");
        }

        public void OnChildHoverExited(HoverExitEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Hover Exitted");
            this.DescreaseHoverCount(args);
        }


        public void OnChildSelectEntering(SelectEnterEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Select Entering");
        }

        public void OnChildSelectEntered(SelectEnterEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Select Entered");
            IncreaseSelectCount(args);
        }

        public void OnChildSelectExiting(SelectExitEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Select Exitting");
        }

        public void OnChildSelectExited(SelectExitEventArgs args)
        {
            Debug.Log("SCROLLABLE: Child Select Exitted");
            DecreaseSelectCount(args);
        }

        private void IncreaseHoverCount(HoverEnterEventArgs args)
        {
            hoverCounts++;
            if (hoverCounts == 1)
            {
            }
        }

        private void DescreaseHoverCount(HoverExitEventArgs args)
        {
            hoverCounts--;
            if (hoverCounts <= 0)
            {
                hoverCounts = 0;
            }
        }

        private void IncreaseSelectCount(SelectEnterEventArgs args)
        {
            selectCounts++;
            if (selectCounts == 1)
            {
                StartScrolling(args.interactorObject);
            }
        }


        private void DecreaseSelectCount(SelectExitEventArgs args)
        {
            selectCounts--;
            if (selectCounts <= 0)
            {
                hoverCounts = 0;
                StopScrolling(args.interactorObject);
            }
        }

        private void StartScrolling(IXRInteractor interactor)
        {
            Vector2 thisFrame = scrollRect.transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);

            touchPoints[interactor] = thisFrame;

            // if (interactorsSelecting.Count == 1)
            // {
            startNormalizedPosition = new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
            startTouchPoint = thisFrame;
            // }

            this.interactionManager.RegisterInteractable((IXRInteractable)this);
        }

        private void StopScrolling(IXRInteractor interactor)
        {

            if (touchPoints.ContainsKey(interactor))
            {
                // Vector2 lastFrame = touchPoints[interactor];
                // Vector2 thisFrame = scrollRect.transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);

                scrollRect.velocity = velocity;
            }

            touchPoints.Remove(interactor);

            this.interactionManager.UnregisterInteractable((IXRInteractable)this);
        }
    }
}
