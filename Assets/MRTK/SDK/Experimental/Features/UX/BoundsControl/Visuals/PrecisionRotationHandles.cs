// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities.Experimental;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Allows more advanced rotation widget spawning for BoundsControl.
    /// Inteded for use with variable precision rotation widgets.
    /// </summary>
    public class PrecisionRotationHandles : RotationHandles
    {

        protected PrecisionRotationHandlesConfiguration precisionConfig;

        protected PrecisionRotationAffordance activePrecisionAffordance;

        internal PrecisionRotationHandles(PrecisionRotationHandlesConfiguration configuration) : base(configuration)
        {
            precisionConfig = configuration;
        }

        ~PrecisionRotationHandles()
        {
            config.handlesChanged.RemoveListener(HandlesChanged);
            config.colliderTypeChanged.RemoveListener(UpdateColliderType);
        }

        internal override void SetHighlighted(Transform handleToHighlight, IMixedRealityPointer associatedPointer = null)
        {
            base.SetHighlighted(handleToHighlight);

            if (handles.Contains(handleToHighlight))
            {
                // Create a precision affordance mounted onto the active handle's visual
                activePrecisionAffordance = CreatePrecisionAffordance(handleToHighlight.GetChild(0), handleToHighlight.parent);
            }

            // If we have an active precision affordance/widget, we set the associated pointer.
            if (activePrecisionAffordance != null)
            {
                activePrecisionAffordance.SetAssociatedPointer(associatedPointer);
            }
        }

        protected override void ResetHandles()
        {
            base.ResetHandles();

            // If we have an active precision affordance,
            // we destroy it on a timer (and let it perform
            // the deflation/fade out transition.)
            if(activePrecisionAffordance != null)
            {
                activePrecisionAffordance.DestroySelf();
            }
        }

        protected PrecisionRotationAffordance CreatePrecisionAffordance(Transform attachTarget, Transform objectRoot){

            // If no precision widget is specified, we won't create one.
            if (precisionConfig.PrecisionWidgetPrefab == null || attachTarget == null)
            {
                return null;
            }

            // Create the precision affordance from the specified precision widget prefab.
            PrecisionRotationAffordance affordance = GameObject.Instantiate(precisionConfig.PrecisionWidgetPrefab).GetComponent<PrecisionRotationAffordance>();
            
            // Precision affordances are centered on the parent object's origin.
            affordance.transform.position = objectRoot.position;

            // The precision affordance is initialized to a look-rotation away from the center of the object, pointing
            // in the direction of attachTarget, with the same up-vector as the attachTarget.
            affordance.transform.rotation = Quaternion.LookRotation(attachTarget.position - objectRoot.position, attachTarget.up);

            // Initialize the affordance's tracking target.
            affordance.SetTrackingTarget(attachTarget, objectRoot, objectRoot.rotation * Quaternion.Inverse(affordance.transform.rotation));

            return affordance;
        }

    }
}

    
