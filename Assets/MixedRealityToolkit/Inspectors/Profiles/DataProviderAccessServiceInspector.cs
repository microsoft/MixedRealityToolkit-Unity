// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Abstract class providing base functionality for data provider management in inspector. Useful for core systems that follow dataprovider access model.
    /// </summary>
    public abstract class BaseDataProviderServiceInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        protected class ServiceConfigurationProperties
        {
            public SerializedProperty Name;
            public SerializedProperty Type;
            public SerializedProperty providerProfile;
            public SerializedProperty runtimePlatform;
        }

        protected abstract SerializedProperty GetDataProviderConfigurationList();
        protected abstract ServiceConfigurationProperties GetDataProviderConfigurationProperties(SerializedProperty providerEntry);
        protected abstract IMixedRealityServiceConfiguration GetDataProviderConfiguration(int index);

        private SerializedProperty providerConfigurations;
        private List<bool> providerFoldouts = new List<bool>();

        private static readonly GUIContent ComponentTypeLabel = new GUIContent("Type");
        private static readonly GUIContent SupportedPlatformsLabel = new GUIContent("Supported Platform(s)");

        protected override void OnEnable()
        {
            base.OnEnable();

            providerConfigurations = GetDataProviderConfigurationList();

            if (providerFoldouts == null || providerFoldouts.Count != providerConfigurations.arraySize)
            {
                providerFoldouts = new List<bool>(new bool[providerConfigurations.arraySize]);
            }
        }

        protected virtual void AddDataProvider()
        {
            providerConfigurations.InsertArrayElementAtIndex(providerConfigurations.arraySize);
            SerializedProperty provider = providerConfigurations.GetArrayElementAtIndex(providerConfigurations.arraySize - 1);

            var providerProperties = GetDataProviderConfigurationProperties(provider);
            providerProperties.Name.stringValue = $"New data provider {providerConfigurations.arraySize - 1}";
            providerProperties.runtimePlatform.intValue = -1;
            providerProperties.providerProfile.objectReferenceValue = null;

            serializedObject.ApplyModifiedProperties();

            var providerType = GetDataProviderConfiguration(providerConfigurations.arraySize - 1).ComponentType;
            providerType.Type = null;

            providerFoldouts.Add(false);
        }

        protected virtual void RemoveDataProvider(int index)
        {
            providerConfigurations.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();

            providerFoldouts.RemoveAt(index);
        }

        protected virtual void ApplyProviderConfiguration(Type dataProviderType, ServiceConfigurationProperties providerProperties)
        {
            if (dataProviderType != null)
            {
                MixedRealityDataProviderAttribute providerAttribute = MixedRealityDataProviderAttribute.Find(dataProviderType) as MixedRealityDataProviderAttribute;
                if (providerAttribute != null)
                {
                    providerProperties.Name.stringValue = !string.IsNullOrWhiteSpace(providerAttribute.Name) ? providerAttribute.Name : dataProviderType.Name;
                    providerProperties.providerProfile.objectReferenceValue = providerAttribute.DefaultProfile;
                    providerProperties.runtimePlatform.intValue = (int)providerAttribute.RuntimePlatforms;
                }
                else
                {
                    providerProperties.Name.stringValue = dataProviderType.Name;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

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

                for (int i = 0; i < providerConfigurations.arraySize; i++)
                {
                    changed |= RenderDataProviderEntry(i, removeContentLabel, dataProviderProfileType);
                }

                return changed;
            }
        }

        protected bool RenderDataProviderEntry(int index, GUIContent removeContent, System.Type dataProviderProfileType = null)
        {
            bool changed = false;
            SerializedProperty provider = providerConfigurations.GetArrayElementAtIndex(index);
            ServiceConfigurationProperties providerProperties = GetDataProviderConfigurationProperties(provider);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    providerFoldouts[index] = EditorGUILayout.Foldout(providerFoldouts[index], providerProperties.Name.stringValue, true);

                    if (GUILayout.Button(removeContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                    {
                        RemoveDataProvider(index);
                        return true;
                    }
                }

                if (providerFoldouts[index])
                {
                    var serviceType = GetDataProviderConfiguration(index).ComponentType;

                    using (var c = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUILayout.PropertyField(providerProperties.Type, ComponentTypeLabel);
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
            }

            return changed;
        }
    }
}
