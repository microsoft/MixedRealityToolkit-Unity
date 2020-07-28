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

            if (activePrecisionAffordance != null)
            {
                activePrecisionAffordance.SetAssociatedPointer(associatedPointer);
            }
        }

        protected override void ResetHandles()
        {
            base.ResetHandles();
            if(activePrecisionAffordance != null)
            {
                activePrecisionAffordance.DestroySelf();
            }
        }

        protected PrecisionRotationAffordance CreatePrecisionAffordance(Transform attachTarget, Transform objectRoot){
            if (precisionConfig.PrecisionWidgetPrefab == null || attachTarget == null)
            {
                return null;
            }

            PrecisionRotationAffordance affordance = GameObject.Instantiate(precisionConfig.PrecisionWidgetPrefab).GetComponent<PrecisionRotationAffordance>();
            
            // Precision affordances are centered on the parent object's origin.
            affordance.transform.position = objectRoot.position;

            affordance.transform.rotation = Quaternion.LookRotation(attachTarget.position - objectRoot.position, attachTarget.up);

            affordance.ManipulationScale = (attachTarget.position - objectRoot.position).magnitude + 0.05f;

            affordance.SetTrackingTarget(attachTarget, objectRoot, objectRoot.rotation * Quaternion.Inverse(affordance.transform.rotation));

            return affordance;
        }

    }
}

    
