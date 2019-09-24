// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class UIMaterialInstance
    {
        // this set ensures that we do not end up creating multiple copies of materials for every theme targetting the same instance
        private static HashSet<int> targetInstances = new HashSet<int>();

        private Material material = null;
        private Graphic targetGraphic;

        public UIMaterialInstance(Graphic targetObject)
        {
            targetGraphic = targetObject;
            material = targetGraphic.material;
        }

        public void TryCreateMaterialCopy()
        {
            if (!targetInstances.Contains(targetGraphic.GetInstanceID()))
            {
                targetInstances.Add(targetGraphic.GetInstanceID());
                targetGraphic.material = new Material(targetGraphic.material);
                material = targetGraphic.material;
            }
        }

        public Color GetColor(int propertyId)
        {
            return material.GetColor(propertyId);
        }

        public float GetFloat(int propertyId)
        {
            return material.GetFloat(propertyId);
        }

        public void SetColor(int propertyId, Color value)
        {
            material.SetColor(propertyId, value);
        }

        public void SetFloat(int propertyId, float value)
        {
            material.SetFloat(propertyId, value);
        }
    }
}
