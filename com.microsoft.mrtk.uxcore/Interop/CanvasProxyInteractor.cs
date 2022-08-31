// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple proxy interactor which will select + hover things on our behalf, for canvas input.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Canvas Proxy Interactor")]
    public class CanvasProxyInteractor : XRBaseInteractor, IProxyInteractor, IModeManagedInteractor
    {
        protected HashSet<IXRInteractable> validTargets = new HashSet<IXRInteractable>();

        protected IXRSelectInteractable manualSelectTarget;

        // We set this flag whenever we're cancelling an interaction. This will suppress
        // events (like OnClicked) on any StatefulInteractable.
        private bool isCancellingInteraction = false;

        /// <summary>
        /// The camera-depth of the last interaction ray; used to stabilize
        /// the cursor as it leaves the selected interactable.
        /// Reset when a new interactable is selected.
        /// </summary>
        private float currentDepth;

        /// <inheritdoc />
        public void StartHover(IXRHoverInteractable target)
        {
            StartHover(target, target.colliders[0].transform.position);
        }

        /// <inheritdoc />
        public void StartHover(IXRHoverInteractable target, Vector3 worldPosition)
        {
            if (target != null)
            {
                Vector3 cameraLocal = worldPosition - Camera.main.transform.position;
                cameraLocal = cameraLocal.normalized * currentDepth;
                transform.position = Camera.main.transform.position + cameraLocal;
                validTargets.Add(target);
            }
        }

        /// <inheritdoc />
        public void EndHover(IXRHoverInteractable target)
        {
            if (target != null)
            {
                validTargets.Remove(target);
            }
        }

        /// <inheritdoc />
        public void StartSelect(IXRSelectInteractable target)
        {
            StartSelect(target, target.colliders[0].transform.position);
        }

        /// <inheritdoc />
        public void StartSelect(IXRSelectInteractable target, Vector3 worldPosition)
        {
            if (interactionManager.IsRegistered(target))
            {
                // If we're already selecting something, end selection but suppress events.
                if (manualSelectTarget != null)
                {
                    isCancellingInteraction = true;
                    EndManualInteraction();
                    isCancellingInteraction = false;
                }

                transform.position = worldPosition;
                currentDepth = Vector3.Distance(Camera.main.transform.position, worldPosition);
                manualSelectTarget = target;
                StartManualInteraction(target);
            }
        }

        /// <inheritdoc />
        public void UpdateSelect(IXRSelectInteractable interactable, Vector3 worldPosition)
        {
            Vector3 cameraLocal = worldPosition - Camera.main.transform.position;
            cameraLocal = cameraLocal.normalized * currentDepth;
            transform.position = Camera.main.transform.position + cameraLocal;
        }

        /// <inheritdoc />
        public void EndSelect(IXRSelectInteractable target, bool suppressEvents = false)
        {
            if (manualSelectTarget == target)
            {
                manualSelectTarget = null;

                if (suppressEvents)
                {
                    isCancellingInteraction = true;
                }

                EndManualInteraction();

                if (suppressEvents)
                {
                    isCancellingInteraction = false;
                }
            }
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(validTargets);
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && interactable == manualSelectTarget;
        }

        /// <inheritdoc />
        public override bool isSelectActive
        {
            get
            {
                // If the base interactor class doesn't want to select, for
                // some reason. Shouldn't happen in most cases.
                if (!base.isSelectActive)
                    return false;

                // We use Start/EndManualInteraction to select our target.
                if (isPerformingManualInteraction)
                    return true;

                // No other way to select.
                return false;
            }
        }

        /// <inheritdoc />
        // We combine the base hoverActive with our flag for whether we're suppressing events.
        // Our interactors use isHoverActive = false to indicate interaction cancellation.
        public override bool isHoverActive => base.isHoverActive && !isCancellingInteraction;

        /// <inheritdoc />
        public GameObject GetModeManagedController() => gameObject;
    }
}
