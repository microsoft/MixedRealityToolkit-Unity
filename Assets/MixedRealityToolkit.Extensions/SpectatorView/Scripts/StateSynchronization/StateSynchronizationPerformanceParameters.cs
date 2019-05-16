// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class StateSynchronizationPerformanceParameters : MonoBehaviour
    {
        public enum PollingFrequency
        {
            InheritFromParent = 0x0,
            UpdateOnceOnStart = 0x1,
            UpdateContinuously = 0x2
        }

        public enum FeatureInclusionType
        {
            InheritFromParent = 0x0,
            SynchronizeFeature = 0x1,
            DoNotSynchronizeFeature = 0x2,
        }

        [Serializable]
        internal class MaterialPropertyPollingFrequency
        {
            public string shaderName = null;
            public string propertyName = null;
            public PollingFrequency updateFrequency = PollingFrequency.UpdateContinuously;
        }

        [SerializeField]
        [Tooltip("Controls how frequently each GameObject checks for attached components that have a related ComponentBroadcaster.")]
        protected PollingFrequency checkForComponentBroadcasters = PollingFrequency.UpdateContinuously;

        [SerializeField]
        [Tooltip("Controls how frequently the shaderKeywords property on materials are checked for updates.")]
        protected PollingFrequency shaderKeywords = PollingFrequency.UpdateContinuously;

        [SerializeField]
        [Tooltip("Controls how frequently the renderQueue property on materials are checked for updates.")]
        protected PollingFrequency renderQueue = PollingFrequency.UpdateContinuously;

        [SerializeField]
        [Tooltip("Controls how frequently material properties are checked for updates by default. Specific material properties can be changed in the materialPropertyOverrides array.")]
        protected PollingFrequency materialProperties = PollingFrequency.UpdateContinuously;

        [SerializeField]
        [Tooltip("Overrides how frequently specific material properties are updated by shader and property name. Shader properties not listed here are updated at a frequency controlled by the materialProperties value.")]
        protected MaterialPropertyPollingFrequency[] materialPropertyOverrides = null;

        [SerializeField]
        [Tooltip("Controls whether or not MaterialPropertyBlocks on Renderers are synchronized.")]
        protected FeatureInclusionType materialPropertyBlocks = FeatureInclusionType.SynchronizeFeature;

        private static GameObject emptyParametersGameObject;
        private StateSynchronizationPerformanceParameters parentParameters;
        private Dictionary<MaterialPropertyKey, MaterialPropertyPollingFrequency> pollingFrequencyByMaterialProperty;

        private IDictionary<MaterialPropertyKey, MaterialPropertyPollingFrequency> PollingFrequencyByMaterialProperty
        {
            get
            {
                return pollingFrequencyByMaterialProperty ?? (pollingFrequencyByMaterialProperty = (materialPropertyOverrides ?? Array.Empty<MaterialPropertyPollingFrequency>()).ToDictionary(p => new MaterialPropertyKey(p.shaderName, p.propertyName)));
            }
        }

        private T GetInheritedProperty<T>(Func<StateSynchronizationPerformanceParameters, T> getter, T inhertedValue, T defaultValue)
        {
            StateSynchronizationPerformanceParameters parameters = this;
            while (parameters != null)
            {
                T pollingFrequency = getter(parameters);
                if (!Equals(pollingFrequency, PollingFrequency.InheritFromParent))
                {
                    return pollingFrequency;
                }

                parameters = parameters.parentParameters;
            }

            return defaultValue;
        }

        public PollingFrequency CheckForComponentBroadcasters
        {
            get { return GetInheritedProperty(p => p.checkForComponentBroadcasters, PollingFrequency.InheritFromParent, PollingFrequency.UpdateContinuously); }
        }

        public PollingFrequency ShaderKeywords
        {
            get { return GetInheritedProperty(p => p.shaderKeywords, PollingFrequency.InheritFromParent, PollingFrequency.UpdateContinuously); }
        }

        public PollingFrequency RenderQueue
        {
            get { return GetInheritedProperty(p => p.renderQueue, PollingFrequency.InheritFromParent, PollingFrequency.UpdateContinuously); }
        }

        public FeatureInclusionType MaterialPropertyBlocks
        {
            get { return GetInheritedProperty(p => p.materialPropertyBlocks, FeatureInclusionType.InheritFromParent, FeatureInclusionType.SynchronizeFeature); }
        }

        public bool ShouldUpdateMaterialProperty(MaterialPropertyAsset materialProperty)
        {
            if (materialProperty.propertyType == MaterialPropertyType.ShaderKeywords)
            {
                return ShaderKeywords == PollingFrequency.UpdateContinuously;
            }
            else if (materialProperty.propertyType == MaterialPropertyType.RenderQueue)
            {
                return RenderQueue == PollingFrequency.UpdateContinuously;
            }
            else
            {
                MaterialPropertyPollingFrequency pollingFrequency;
                if (PollingFrequencyByMaterialProperty.TryGetValue(new MaterialPropertyKey(materialProperty.ShaderName, materialProperty.propertyName), out pollingFrequency))
                {
                    switch (pollingFrequency.updateFrequency)
                    {
                        case PollingFrequency.UpdateContinuously:
                            return true;
                        case PollingFrequency.UpdateOnceOnStart:
                            return false;
                    }
                }

                // If we have a parent, check the parent to see if the parent has an explicit override list
                if (materialProperties == PollingFrequency.InheritFromParent && parentParameters != null)
                {
                    return parentParameters.ShouldUpdateMaterialProperty(materialProperty);
                }

                return materialProperties == PollingFrequency.UpdateContinuously;
            }
        }

        protected virtual void Awake()
        {
            UpdateParentParameters();
        }

        private void OnTransformParentChanged()
        {
            UpdateParentParameters();
        }

        private void UpdateParentParameters()
        {
            if (GetComponent<DefaultStateSynchronizationPerformanceParameters>() != null)
            {
                parentParameters = null;
            }
            else
            {
                if (transform.parent == null)
                {
                    parentParameters = DefaultStateSynchronizationPerformanceParameters.Instance;
                }
                else
                {
                    parentParameters = transform.parent.GetComponentInParent<StateSynchronizationPerformanceParameters>();
                    if (parentParameters == null)
                    {
                        parentParameters = DefaultStateSynchronizationPerformanceParameters.Instance;
                    }
                }
            }
        }

        public static StateSynchronizationPerformanceParameters CreateEmpty()
        {
            if (emptyParametersGameObject == null)
            {
                emptyParametersGameObject = new GameObject("EmptySychronizationPerformanceParameters");
            }
            return ComponentExtensions.EnsureComponent<StateSynchronizationPerformanceParameters>(emptyParametersGameObject);
        }
    }
}