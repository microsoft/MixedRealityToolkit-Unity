﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Abstract class providing base functionality for data provider management in inspector. Useful for core systems that follow data provider access model.
    /// Designed to target ScriptableObject profile classes that configure services who support data providers. 
    /// These profile ScriptableObject classes should contain an array of IMixedRealityServiceConfigurations that configure a list of data providers for this service configuration
    /// </summary>
    public abstract class BaseDataProviderServiceInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        /// <summary>
        /// Container class used to store references to serialized properties on a <see cref="IMixedRealityServiceConfiguration"/> target
        /// </summary>
        protected class ServiceConfigurationProperties
        {
            internal SerializedProperty componentName;
            internal SerializedProperty componentType;
            internal SerializedProperty providerProfile;
            internal SerializedProperty runtimePlatform;
        }

        /// <summary>
        /// Allows implementations of a IMixedRealityDataProviderAccess system's inspector to provide custom data provider representations for data providers.
        /// </summary>
        /// <returns>SerializedProperty object that wraps references to array of <see cref="IMixedRealityServiceConfiguration"/> stored on the inspected target object.</returns>
        protected abstract SerializedProperty GetDataProviderConfigurationList();

        /// <summary>
        /// Builds <see cref="ServiceConfigurationProperties"/> container object with SerializedProperty references to associated properties on the supplied <see cref="IMixedRealityServiceConfiguration"/> reference
        /// </summary>
        /// <param name="providerEntry">SerializedProperty reference pointing to <see cref="IMixedRealityServiceConfiguration"/> instance</param>
        protected abstract ServiceConfigurationProperties GetDataProviderConfigurationProperties(SerializedProperty providerEntry);

        /// <summary>
        /// Returns direct <see cref="IMixedRealityServiceConfiguration"/> instance at provided index in target object's array of <see cref="IMixedRealityServiceConfiguration"/> configurations
        /// </summary>
        protected abstract IMixedRealityServiceConfiguration GetDataProviderConfiguration(int index);

        private SerializedProperty providerConfigurations;
        private List<bool> providerFoldouts = new List<bool>();

#if UNITY_2019
        private static readonly GUIContent GeneralProvidersLabel = new GUIContent("General Providers");
#endif // UNITY_2019

        private readonly XRPipelineUtility xrPipelineUtility = new XRPipelineUtility();
        private readonly List<SystemType> delayedDisplayProviders = new List<SystemType>();

        private static readonly GUIContent ComponentTypeLabel = new GUIContent("Type");
        private static readonly GUIContent SupportedPlatformsLabel = new GUIContent("Supported Platform(s)");

        private const string NewDataProvider = "New data provider";

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_2019
            xrPipelineUtility.Enable();
#endif // UNITY_2019

            providerConfigurations = GetDataProviderConfigurationList();

            if (providerFoldouts == null || providerFoldouts.Count != providerConfigurations.arraySize)
            {
                providerFoldouts = new List<bool>(new bool[providerConfigurations.arraySize]);
            }
        }

        /// <summary>
        /// Adds a new data provider profile entry (i.e <see cref="IMixedRealityServiceConfiguration"/>) to array list of target object
        /// Utilizes GetDataProviderConfigurationList() to get SerializedProperty object that represents array to insert against
        /// </summary>
        protected virtual void AddDataProvider()
        {
            providerConfigurations.InsertArrayElementAtIndex(providerConfigurations.arraySize);
            SerializedProperty provider = providerConfigurations.GetArrayElementAtIndex(providerConfigurations.arraySize - 1);

            ServiceConfigurationProperties providerProperties = GetDataProviderConfigurationProperties(provider);
            providerProperties.componentName.stringValue = $"{NewDataProvider} {providerConfigurations.arraySize - 1}";
            providerProperties.runtimePlatform.intValue = -1;
            providerProperties.providerProfile.objectReferenceValue = null;

            serializedObject.ApplyModifiedProperties();

            SystemType providerType = GetDataProviderConfiguration(providerConfigurations.arraySize - 1).ComponentType;
            providerType.Type = null;

            providerFoldouts.Add(false);
        }

        /// <summary>
        /// Removed given index item from <see cref="IMixedRealityServiceConfiguration"/> array list.
        /// Utilizes GetDataProviderConfigurationList() to get SerializedProperty object that represents array to delete against.
        /// </summary>
        protected virtual void RemoveDataProvider(int index)
        {
            providerConfigurations.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();

            providerFoldouts.RemoveAt(index);
        }

        /// <summary>
        /// Applies the given concrete data provider type properties to the provided <see cref="IMixedRealityServiceConfiguration"/> instance (as represented by <see cref="ServiceConfigurationProperties"/>).
        /// Requires <see cref="MixedRealityDataProviderAttribute"/> on concrete type class to pull initial values 
        /// that will be applied to the <see cref="ServiceConfigurationProperties"/> container SerializedProperties
        /// </summary>
        protected virtual void ApplyProviderConfiguration(Type dataProviderType, ServiceConfigurationProperties providerProperties)
        {
            if (dataProviderType != null)
            {
                if (MixedRealityExtensionServiceAttribute.Find(dataProviderType) is MixedRealityDataProviderAttribute providerAttribute)
                {
                    providerProperties.componentName.stringValue = !string.IsNullOrWhiteSpace(providerAttribute.Name) ? providerAttribute.Name : dataProviderType.Name;
                    providerProperties.providerProfile.objectReferenceValue = providerAttribute.DefaultProfile;
                    providerProperties.runtimePlatform.intValue = (int)providerAttribute.RuntimePlatforms;
                }
                else
                {
                    providerProperties.componentName.stringValue = dataProviderType.Name;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Render list of data provider configuration profiles in inspector. Use provided add and remove content labels for the insert/remove buttons
        /// Returns true if any property has changed in this render pass, false otherwise
        /// </summary>
        protected bool RenderDataProviderList(GUIContent addContentLabel, GUIContent removeContentLabel, string errorMsg, Type dataProviderProfileType = null)
        {
            bool changed = false;

            using (new EditorGUILayout.VerticalScope())
            {
                if (providerConfigurations == null || providerConfigurations.arraySize == 0)
                {
                    EditorGUILayout.HelpBox(errorMsg, MessageType.Info);
                }

                if (InspectorUIUtility.RenderIndentedButton(addContentLabel, EditorStyles.miniButton))
                {
                    AddDataProvider();
                    return true;
                }

#if UNITY_2019
                xrPipelineUtility.RenderXRPipelineTabs();
#endif // UNITY_2019

                delayedDisplayProviders.Clear();

                for (int i = 0; i < providerConfigurations.arraySize; i++)
                {
                    SystemType serviceType = GetDataProviderConfiguration(i).ComponentType;

                    if (serviceType.Type != null && MixedRealityExtensionServiceAttribute.Find(serviceType.Type) is MixedRealityDataProviderAttribute providerAttribute)
                    {
                        // Using == here to compare flags because we want to know if this is the only supported pipeline
                        // Providers that support multiple pipelines are rendered below the tabbed section
                        if (providerAttribute.SupportedUnityXRPipelines == xrPipelineUtility.SelectedPipeline)
                        {
                            changed |= RenderDataProviderEntry(i, removeContentLabel, serviceType, dataProviderProfileType);
                            delayedDisplayProviders.Add(null);
                        }
                        else if (providerAttribute.SupportedUnityXRPipelines == (SupportedUnityXRPipelines)(-1))
                        {
                            delayedDisplayProviders.Add(serviceType);
                        }
                        else
                        {
                            // Add null to ensure the delayedDisplayProviders list has an identical size to providerConfigurations.arraySize
                            // This is so we can iterate through without keeping track of i separately
                            delayedDisplayProviders.Add(null);
                        }
                    }
                    else
                    {
                        delayedDisplayProviders.Add(serviceType);
                    }
                }

#if UNITY_2019
                EditorGUILayout.LabelField(GeneralProvidersLabel, EditorStyles.boldLabel);
#endif // UNITY_2019

                for (int i = 0; i < delayedDisplayProviders.Count; i++)
                {
                    SystemType service = delayedDisplayProviders[i];
                    if (service != null)
                    {
                        changed |= RenderDataProviderEntry(i, removeContentLabel, service, dataProviderProfileType);
                    }
                }

                return changed;
            }
        }

        /// <summary>
        /// Renders properties of <see cref="IMixedRealityServiceConfiguration"/> instance at provided index in inspector.
        /// Also renders inspector view of data provider's profile object and its contents if applicable and foldout is expanded.
        /// </summary>
        private bool RenderDataProviderEntry(int index, GUIContent removeContent, SystemType serviceType, Type dataProviderProfileType = null)
        {
            bool changed = false;
            SerializedProperty provider = providerConfigurations.GetArrayElementAtIndex(index);
            ServiceConfigurationProperties providerProperties = GetDataProviderConfigurationProperties(provider);

            // Don't hide new data providers added via the UI, otherwise there's no easy way to change their type
            if (serviceType?.Type == null && !MixedRealityProjectPreferences.ShowNullDataProviders && !providerProperties.componentName.stringValue.StartsWith(NewDataProvider))
            {
                return false;
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (index < 0 || index >= providerFoldouts.Count) index = 0;
                    providerFoldouts[index] = EditorGUILayout.Foldout(providerFoldouts[index], providerProperties.componentName.stringValue, true);

                    if (GUILayout.Button(removeContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                    {
                        RemoveDataProvider(index);
                        return true;
                    }
                }

                if (providerFoldouts[index])
                {
                    using (var c = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUILayout.PropertyField(providerProperties.componentType, ComponentTypeLabel);
                        if (c.changed)
                        {
                            serializedObject.ApplyModifiedProperties();
                            ApplyProviderConfiguration(serviceType.Type, providerProperties);
                            return true;
                        }

                        EditorGUILayout.PropertyField(providerProperties.runtimePlatform, SupportedPlatformsLabel);
                        changed = c.changed;
                    }

                    changed |= RenderProfile(providerProperties.providerProfile, dataProviderProfileType, true, false, serviceType);

                    serializedObject.ApplyModifiedProperties();
                }

                if (IsProfileRequired(serviceType) &&
                    (providerProperties.providerProfile.objectReferenceValue == null))
                {
                    EditorGUILayout.HelpBox($"{providerProperties.componentName.stringValue} requires a profile.", MessageType.Warning);
                }
            }

            return changed;
        }
    }
}
