// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Facades
{
    [CustomEditor(typeof(ServiceFacade))]
    [InitializeOnLoad]
    public class ServiceFacadeEditor : UnityEditor.Editor
    {
        static ServiceFacadeEditor()
        {
            // Register this on startup so we can update whether a facade inspector is updated or not
            SceneView.onSceneGUIDelegate += DrawSceneGUI;
        }

        private static Dictionary<Type, Type> inspectorTypeLookup = new Dictionary<Type, Type>();
        private static Dictionary<Type, IMixedRealityServiceInspector> inspectorInstanceLookup = new Dictionary<Type, IMixedRealityServiceInspector>();
        private static bool initializedServiceInspectorLookup = false;

        Color proHeaderColor = (Color)new Color32(56, 56, 56, 255);
        Color defaultHeaderColor = (Color)new Color32(194, 194, 194, 255);

        protected override void OnHeaderGUI()
        {
            ServiceFacade facade = (ServiceFacade)target;

            var rect = EditorGUILayout.GetControlRect(false, 0f);
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y -= rect.height;
            rect.x = 48;
            rect.xMax -= rect.x * 2f;

            EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? proHeaderColor : defaultHeaderColor);

            string header = facade.name;
            if (string.IsNullOrEmpty(header))
                header = target.ToString();

            EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
        }

        public override void OnInspectorGUI()
        {
            OnHeaderGUI();

            ServiceFacade facade = (ServiceFacade)target;

            if (facade.Service == null)
                return;

            if (!MixedRealityToolkit.Instance.HasActiveProfile)
                return;

            if (MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile == null)
                return;

            if (!initializedServiceInspectorLookup)
                InitializeServiceInspectorLookup();

            // If we have a doc link, put that first
            DocLinkAttribute docLink = facade.ServiceType.GetCustomAttribute<DocLinkAttribute>();
            if (docLink != null)
            {
                if (GUILayout.Button("Click to view documentation", EditorStyles.miniButton))
                {
                    Application.OpenURL(docLink.URL);
                }
                EditorGUILayout.Space();
            }

            bool drawProfile = true;
            bool drawInspector = false;

            // Find and draw the custom inspector
            IMixedRealityServiceInspector inspectorInstance;
            if (GetServiceInspectorInstance(facade.Service.GetType(), out inspectorInstance))
            {
                drawInspector = true;
                drawProfile = inspectorInstance.DrawProfileField;
            }

            if (drawProfile)
            {
                // Draw the base profile stuff
                if (typeof(BaseExtensionService).IsAssignableFrom(facade.ServiceType))
                {
                    // If this is an extension service, see if it uses a profile
                    MixedRealityServiceConfiguration[] serviceConfigs = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile.Configurations;
                    for (int serviceIndex = 0; serviceIndex < serviceConfigs.Length; serviceIndex++)
                    {
                        MixedRealityServiceConfiguration serviceConfig = serviceConfigs[serviceIndex];
                        if (serviceConfig.ComponentType.Type.IsAssignableFrom(facade.ServiceType) && serviceConfig.ConfigurationProfile != null)
                        {
                            // We found the service that this type uses - draw the profile
                            SerializedObject serviceConfigObject = new SerializedObject(MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile);
                            SerializedProperty serviceConfigArray = serviceConfigObject.FindProperty("configurations");
                            SerializedProperty serviceConfigProp = serviceConfigArray.GetArrayElementAtIndex(serviceIndex);
                            SerializedProperty serviceProfileProp = serviceConfigProp.FindPropertyRelative("configurationProfile");
                            BaseMixedRealityProfileInspector.RenderProfile(serviceProfileProp, null, false, facade.ServiceType);
                            EditorGUILayout.Space();
                            drawProfile = true;
                            break;
                        }
                    }
                }
                else
                {
                    SerializedObject activeProfileObject = new SerializedObject(MixedRealityToolkit.Instance.ActiveProfile);
                    // Would be nice to handle this using some other method
                    // Would be nice to handle this with a lookup instead
                    if (typeof(IMixedRealityInputSystem).IsAssignableFrom(facade.ServiceType))
                    {
                        SerializedProperty serviceProfileProp = activeProfileObject.FindProperty("inputSystemProfile");
                        BaseMixedRealityProfileInspector.RenderProfile(serviceProfileProp, null, false, facade.ServiceType);
                        drawProfile = true;
                    }
                    else if (typeof(IMixedRealityBoundarySystem).IsAssignableFrom(facade.ServiceType))
                    {
                        SerializedProperty serviceProfileProp = activeProfileObject.FindProperty("boundaryVisualizationProfile");
                        BaseMixedRealityProfileInspector.RenderProfile(serviceProfileProp, null, false, facade.ServiceType);
                        drawProfile = true;
                    }
                    else if (typeof(IMixedRealityDiagnosticsSystem).IsAssignableFrom(facade.ServiceType))
                    {
                        SerializedProperty serviceProfileProp = activeProfileObject.FindProperty("diagnosticsSystemProfile");
                        BaseMixedRealityProfileInspector.RenderProfile(serviceProfileProp, null, false, facade.ServiceType);
                        drawProfile = true;
                    }
                }
            }

            if (drawInspector)
            {               
                // If we have a custom inspector, draw that now
                inspectorInstance.DrawInspectorGUI(facade.Service);
            }

            if (!drawProfile & !drawInspector)
            {
                // If we haven't drawn a profile and we don't have an inspector, draw a label so people aren't confused
                EditorGUILayout.LabelField("No inspector has been defined for this service type.", EditorStyles.miniLabel);
                EditorGUILayout.HelpBox("You can define an inspector for this facade by creating a class with a MixedRealityServiceInspector attribute.", MessageType.Info);
            }

        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Active)]
        private static void DrawGizmos(ServiceFacade facade, GizmoType type)
        {
            if (facade.Service == null)
                return;

            if (!MixedRealityToolkit.Instance.HasActiveProfile)
                return;

            if (MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile == null)
                return;

            if (!initializedServiceInspectorLookup)
                InitializeServiceInspectorLookup();

            // Find and draw the custom inspector
            IMixedRealityServiceInspector inspectorInstance;
            if (!GetServiceInspectorInstance(facade.Service.GetType(), out inspectorInstance))
                return;

            // If we've implemented a facade inspector, draw gizmos now
            inspectorInstance.DrawGizmos(facade.Service);
        }

        private static void DrawSceneGUI(SceneView sceneView)
        {
            if (!MixedRealityToolkit.Instance.HasActiveProfile)
                return;

            if (MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile == null)
                return;

            foreach (KeyValuePair<Type,Type> inspectorTypePair in inspectorTypeLookup)
            {
                // Find the facade associated with this service
                ServiceFacade facade;
                // If no facade exists for this service type, move on
                if (!ServiceFacade.FacadeLookup.TryGetValue(inspectorTypePair.Key, out facade) || facade.Destroyed || facade == null)
                    continue;

                IMixedRealityServiceInspector inspectorInstance;
                if (!GetServiceInspectorInstance(inspectorTypePair.Key, out inspectorInstance))
                    continue;

                if (Selection.Contains(facade) || inspectorInstance.AlwaysDrawSceneGUI)
                    inspectorInstance.DrawSceneGUI(facade.Service, sceneView);
            }
        }
        
        private static bool GetServiceInspectorInstance(Type serviceType, out IMixedRealityServiceInspector inspectorInstance)
        {
            inspectorInstance = null;

            Type inspectorType;
            if (inspectorTypeLookup.TryGetValue(serviceType, out inspectorType))
            {
                if (!inspectorInstanceLookup.TryGetValue(inspectorType, out inspectorInstance))
                {
                    // If an instance of the class doesn't exist yet, create it now
                    try
                    {
                        inspectorInstance = (IMixedRealityServiceInspector)Activator.CreateInstance(inspectorType);
                        inspectorInstanceLookup.Add(inspectorType, inspectorInstance);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Couldn't create instance of inspector type " + inspectorType);
                        Debug.LogException(e);
                    }
                }
                else
                {
                    return true;
                }
            }

            return inspectorInstance != null;
        }

        private static void InitializeServiceInspectorLookup()
        {
            inspectorTypeLookup.Clear();

            var typesWithMyAttribute =
                 from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                 from classType in assembly.GetTypes()
                 let attribute = classType.GetCustomAttribute<MixedRealityServiceInspectorAttribute>(true)
                 where attribute != null
                 select new { ClassType = classType, Attribute = attribute };

            foreach (var result in typesWithMyAttribute)
            {
                inspectorTypeLookup.Add(result.Attribute.ServiceType, result.ClassType);
            }

            initializedServiceInspectorLookup = true;
        }
    }
}