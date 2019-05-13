// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class DefaultSynchronizationPerformanceParameters : SynchronizationPerformanceParameters
    {
        private static DefaultSynchronizationPerformanceParameters _Instance;
        private readonly ConditionalWeakTable<Material, MaterialMutationMonitor> mutationMonitor = new ConditionalWeakTable<Material, MaterialMutationMonitor>();
        private HashSet<MaterialPropertyKey> continuouslyUpdatedMaterialProperties = new HashSet<MaterialPropertyKey>();

        public static bool IsInitialized
        {
            get
            {
                return _Instance != null;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _Instance = this;
        }

        protected virtual void OnDestroy()
        {
            _Instance = null;
        }

        public static DefaultSynchronizationPerformanceParameters Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<DefaultSynchronizationPerformanceParameters>();
                }
                return _Instance;
            }
        }

        public void NotifyMaterialMutated(Material material, string propertyName)
        {
            if (mutationMonitor.GetOrCreateValue(material).NotifyMaterialMutated(propertyName) == MutationType.Continuous)
            {
                continuouslyUpdatedMaterialProperties.Add(new MaterialPropertyKey
                {
                    PropertyName = propertyName,
                    ShaderName = material.shader.name
                });
            }
        }

        public IEnumerable<MaterialPropertyKey> ContinuouslyUpdatedMaterialProperties
        {
            get
            {
                return continuouslyUpdatedMaterialProperties;
            }
        }

        public void ConfigureDynamicMaterialProperties(IEnumerable<MaterialPropertyKey> dynamicProperties)
        {
            materialProperties = PollingFrequency.UpdateOnceOnStart;

            IEnumerable<MaterialPropertyKey> existingKeys = Array.Empty<MaterialPropertyKey>();
            if (materialPropertyOverrides != null)
            {
                existingKeys = materialPropertyOverrides.Select(materialPropertyOverride => new MaterialPropertyKey(materialPropertyOverride.shaderName, materialPropertyOverride.propertyName));
            }

            materialPropertyOverrides = dynamicProperties
                .Concat(existingKeys)
                .Distinct()
                .OrderBy(k => k.ShaderName)
                .ThenBy(k => k.PropertyName)
                .Select(key => new MaterialPropertyPollingFrequency
                {
                    shaderName = key.ShaderName,
                    propertyName = key.PropertyName,
                    updateFrequency = PollingFrequency.UpdateContinuously
                }).ToArray();
        }

        private class MaterialMutationMonitor
        {
            private HashSet<string> mutatedProperties = new HashSet<string>();

            public MutationType NotifyMaterialMutated(string propertyName)
            {
                if (mutatedProperties.Contains(propertyName))
                {
                    return MutationType.Continuous;
                }
                else
                {
                    mutatedProperties.Add(propertyName);
                    return MutationType.OneTime;
                }
            }
        }

        private enum MutationType
        {
            OneTime,
            Continuous
        }
    }
}