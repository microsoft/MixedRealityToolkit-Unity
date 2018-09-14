// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SDK.Utilities.Solvers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.Utilities.Solvers
{
    [CustomEditor(typeof(InBetween))]
    public class InBetweenEditor : Editor
    {
        private SerializedProperty transformTargetProperty;

        private static readonly string[] fieldsToExclude = new string[] { "m_Script" };

        private void OnEnable()
        {
            transformTargetProperty = serializedObject.FindProperty("secondTransformOverride");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(transformTargetProperty);

            DrawPropertiesExcluding(serializedObject, fieldsToExclude);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
