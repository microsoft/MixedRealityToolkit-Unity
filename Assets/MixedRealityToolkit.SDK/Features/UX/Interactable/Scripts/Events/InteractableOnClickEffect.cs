// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A basic receiver for detecting clicks
    /// </summary>
    public class InteractableOnClickEffect : ReceiverBase
    {
        [InspectorField(Label = "Effect Prefab", Tooltip = "The effect prefab, should destroy itself", Type = InspectorField.FieldTypes.GameObject)]
        public GameObject EffectPrefab;

        [InspectorField(Label = "Offset Position", Tooltip = "Spawn the prefab relative to the Interactive position", Type = InspectorField.FieldTypes.Vector3)]
        public Vector3 EffectOffset = Vector3.zero;

        public InteractableOnClickEffect(UnityEvent ev): base(ev)
        {
            Name = "OnClick";
            HideUnityEvents = true;
        }

        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            // using onClick
        }

        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            if(EffectPrefab != null)
            {
                GameObject effect = GameObject.Instantiate(EffectPrefab);
                effect.transform.position = Host.transform.position + EffectOffset;
            }
        }
    }
}
