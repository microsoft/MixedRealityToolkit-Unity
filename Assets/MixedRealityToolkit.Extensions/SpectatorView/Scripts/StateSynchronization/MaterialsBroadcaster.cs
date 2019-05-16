// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MaterialsBroadcaster
    {
        private Material[] cachedMaterials;
        private List<Dictionary<string, object>> previousValues = new List<Dictionary<string, object>>();
        private MaterialPropertyAsset[][] cachedMaterialPropertyAccessors;

        /// <summary>
        /// Asking for the sharedMaterial or sharedMaterials is expensive, so ensure this is only requested once per frame.
        /// </summary>
        private void UpdateCachedMaterials(Material[] materials, out bool areMaterialsDifferent)
        {
            if (cachedMaterials == null || materials == null || !materials.SequenceEqual(cachedMaterials))
            {
                cachedMaterials = materials;
                cachedMaterialPropertyAccessors = null;
                areMaterialsDifferent = true;

                ClearPreviousValuesCache();
            }
            else
            {
                areMaterialsDifferent = false;
            }
        }

        private void ClearPreviousValuesCache()
        {
            int length = cachedMaterials?.Length ?? 0;
            for (int i = 0; i < length; i++)
            {
                if (i < previousValues.Count)
                {
                    previousValues[i].Clear();
                }
                else
                {
                    previousValues.Add(new Dictionary<string, object>());
                }
            }
            for (int i = previousValues.Count - 1; i >= length; i++)
            {
                previousValues.RemoveAt(i);
            }
        }


        private MaterialPropertyAsset[] GetCachedMaterialProperties(int materialIndex, Material[] materials)
        {
            if (cachedMaterialPropertyAccessors == null)
            {
                cachedMaterialPropertyAccessors = new MaterialPropertyAsset[materials.Length][];
            }
            if (cachedMaterialPropertyAccessors[materialIndex] == null)
            {
                cachedMaterialPropertyAccessors[materialIndex] = AssetService.Instance.GetMaterialProperties(materials[materialIndex].shader.name).ToArray();
            }

            return cachedMaterialPropertyAccessors[materialIndex];
        }

        public void UpdateMaterials(Renderer renderer, StateSynchronizationPerformanceParameters performanceParameters, Material[] materials, out bool areMaterialsDifferent)
        {
            UpdateCachedMaterials(materials, out areMaterialsDifferent);

            if (renderer != null && performanceParameters.MaterialPropertyBlocks == StateSynchronizationPerformanceParameters.FeatureInclusionType.SynchronizeFeature)
            {
                using (StateSynchronizationPerformanceMonitor.Instance.MeasureScope(StateSynchronizationPerformanceFeature.MaterialPropertyBlockUpdate))
                {
                    renderer.UpdateCachedPropertyBlock();
                }
            }
        }

        public void SendMaterialPropertyChanges(IEnumerable<SocketEndpoint> endpoints, Renderer renderer, StateSynchronizationPerformanceParameters performanceParameters, Action<BinaryWriter> writeHeader, Func<MaterialPropertyAsset, bool> shouldSynchronizeMaterialProperty)
        {
            Renderer usedRenderer = performanceParameters.MaterialPropertyBlocks == StateSynchronizationPerformanceParameters.FeatureInclusionType.SynchronizeFeature ? renderer : null;
            using (StateSynchronizationPerformanceMonitor.Instance.MeasureScope(StateSynchronizationPerformanceFeature.MaterialPropertyUpdate))
            {
                for (int i = 0; i < cachedMaterials.Length; i++)
                {
                    if (cachedMaterials[i] != null)
                    {
                        foreach (MaterialPropertyAsset propertyAccessor in GetCachedMaterialProperties(i, cachedMaterials))
                        {
                            if (shouldSynchronizeMaterialProperty(propertyAccessor) && performanceParameters.ShouldUpdateMaterialProperty(propertyAccessor))
                            {
                                object newValue = propertyAccessor.GetValue(usedRenderer, cachedMaterials[i]);
                                object oldValue;
                                if (!previousValues[i].TryGetValue(propertyAccessor.propertyName, out oldValue) || !AreMaterialValuesEqual(oldValue, newValue))
                                {
                                    previousValues[i][propertyAccessor.propertyName] = newValue;
                                    SendMaterialPropertyChange(endpoints, usedRenderer, i, propertyAccessor, writeHeader);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool AreMaterialValuesEqual(object oldValue, object newValue)
        {
            string[] oldValueStringArray = oldValue as string[];
            string[] newValueStringArray = newValue as string[];

            if (oldValueStringArray != null && newValueStringArray != null)
            {
                return Enumerable.SequenceEqual(oldValueStringArray, newValueStringArray);
            }
            else
            {
                return Equals(oldValue, newValue);
            }
        }


        public void SendMaterials(BinaryWriter message, Renderer renderer, Func<MaterialPropertyAsset, bool> shouldSynchronizeMaterialProperty)
        {
            using (StateSynchronizationPerformanceMonitor.Instance.MeasureScope(StateSynchronizationPerformanceFeature.MaterialPropertyUpdate))
            {
                Material[] materials = cachedMaterials;
                int materialCount = materials?.Length ?? 0;
                message.Write(materialCount);

                for (int i = 0; i < materialCount; i++)
                {
                    Material material = materials[i];

                    if (material == null)
                    {
                        message.Write(string.Empty);
                    }
                    else
                    {
                        message.Write(material.shader.name);
                        message.Write(material.name);

                        MaterialPropertyAsset[] materialProperties = AssetService.Instance.GetMaterialProperties(material.shader.name).Where(shouldSynchronizeMaterialProperty).ToArray();
                        message.Write(materialProperties.Length);
                        foreach (MaterialPropertyAsset materialProperty in materialProperties)
                        {
                            previousValues[i][materialProperty.propertyName] = materialProperty.GetValue(renderer, material);
                            materialProperty.Write(message, renderer, material);
                        }
                    }
                }
            }
        }


        public void SendMaterialPropertyChange(IEnumerable<SocketEndpoint> endpoints, Renderer renderer, int materialIndex, MaterialPropertyAsset propertyAccessor, Action<BinaryWriter> writeHeader)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                writeHeader(message);
                message.Write(materialIndex);
                propertyAccessor.Write(message, renderer, cachedMaterials[materialIndex]);

                message.Flush();
                StateSynchronizationSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }
    }
}
