using Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityDiagnosticsProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {

        private SerializedProperty showCpu;
        private SerializedProperty showFps;
        private SerializedProperty showMemory;
        private SerializedProperty visible;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            showCpu = serializedObject.FindProperty("showCpu");
            showFps = serializedObject.FindProperty("showFps");
            showMemory = serializedObject.FindProperty("showMemory");
            visible = serializedObject.FindProperty("visible");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityManager())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityManager.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Boundary Visualization Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Boundary visualizations can help users stay oriented and comfortable in the experience.", MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(visible);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(showCpu);
            EditorGUILayout.PropertyField(showFps);
            EditorGUILayout.PropertyField(showMemory);
        }
    }
}
