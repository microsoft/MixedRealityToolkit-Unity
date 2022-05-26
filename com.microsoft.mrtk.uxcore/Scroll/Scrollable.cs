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
    public class Scrollable : PressableButton
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

                Debug.Log(displacementFromStart.ToString("F3"));

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

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            Vector2 thisFrame = scrollRect.transform.InverseTransformPoint(args.interactorObject.GetAttachTransform(this).position);

            touchPoints[args.interactorObject] = thisFrame;

            // if (interactorsSelecting.Count == 1)
            // {
                startNormalizedPosition = new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
                startTouchPoint = thisFrame;
            // }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (touchPoints.ContainsKey(args.interactorObject))
            {
                // Vector2 lastFrame = touchPoints[interactor];
                // Vector2 thisFrame = scrollRect.transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);

                scrollRect.velocity = velocity;
            }
            
            touchPoints.Remove(args.interactorObject);
        }

    }
}