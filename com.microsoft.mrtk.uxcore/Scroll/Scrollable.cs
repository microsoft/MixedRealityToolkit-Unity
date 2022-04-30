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
        
        private Dictionary<IXRSelectInteractor, Vector2> touchPoints = new Dictionary<IXRSelectInteractor, Vector2>();

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                Vector2 dragDelta = Vector2.zero;

                foreach (var interactor in interactorsSelecting)
                {
                    Vector2 lastFrame = Vector2.zero;
                    Vector2 thisFrame = scrollRect.transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);

                    if (touchPoints.ContainsKey(interactor))
                    {
                        lastFrame = touchPoints[interactor];
                    }
                    else
                    {
                        lastFrame = thisFrame;
                    }

                    dragDelta += thisFrame - lastFrame;
                    touchPoints[interactor] = thisFrame;
                }

                float contentHeight = scrollRect.content.sizeDelta.y - scrollRect.viewport.rect.height;
                float contentWidth = scrollRect.content.sizeDelta.x - scrollRect.viewport.rect.width;

                scrollRect.verticalNormalizedPosition -= (dragDelta.y / contentHeight) * Selectedness();
                scrollRect.horizontalNormalizedPosition -= (dragDelta.x / contentWidth) * Selectedness();;
            }
        }

        protected override void OnSelectExited(XRBaseInteractor interactor)
        {
            base.OnSelectExited(interactor);
            touchPoints.Remove(interactor);
        }

    }
}