// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.Input.Handlers
{
    [CustomEditor(typeof(PointerClickHandler))]
    public class PointerClickHandlerInspector : BaseInputHandlerInspector
    {
        private SerializedProperty pointerUpProperty;
        private SerializedProperty pointerDownProperty;
        private SerializedProperty pointerClickedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointerUpProperty = serializedObject.FindProperty("onPointerUpActionEvent");
            pointerDownProperty = serializedObject.FindProperty("onPointerDownActionEvent");
            pointerClickedProperty = serializedObject.FindProperty("onPointerClickedActionEvent");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured()) { return; }

            serializedObject.Update();
            EditorGUILayout.PropertyField(pointerUpProperty, true);
            EditorGUILayout.PropertyField(pointerDownProperty, true);
            EditorGUILayout.PropertyField(pointerClickedProperty, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}