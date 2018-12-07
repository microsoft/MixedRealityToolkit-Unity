// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.NetworkingSystem;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityNetworkSystemProfile))]
    public class MixedRealityNetworkSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty registeredNetworkDataProviders;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            registeredNetworkDataProviders = serializedObject.FindProperty("registeredNetworkDataProviders");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Network System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Network System Profile helps developers configure networking messages no matter what platform you're building for.", MessageType.Info);

            CheckProfileLock(target);
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(registeredNetworkDataProviders, true);

            if (EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}