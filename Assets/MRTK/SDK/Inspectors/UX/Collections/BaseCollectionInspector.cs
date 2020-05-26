// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(BaseObjectCollection), true)]
    public class BaseCollectionInspector : UnityEditor.Editor
    {
        private SerializedProperty ignoreInactiveTransforms;
        private SerializedProperty sortType;

        protected virtual void OnEnable()
        {
            ignoreInactiveTransforms = serializedObject.FindProperty("ignoreInactiveTransforms");
            sortType = serializedObject.FindProperty("sortType");
        }

        sealed public override void OnInspectorGUI()
        {
            if (target != null)
            {
                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(ignoreInactiveTransforms);
            EditorGUILayout.PropertyField(sortType);
            OnInspectorGUIInsertion();
            serializedObject.ApplyModifiedProperties();

            // Place the button at the bottom
            BaseObjectCollection collection = (BaseObjectCollection)target;
            if (GUILayout.Button("Update Collection"))
            {
                collection.UpdateCollection();
            }
        }

        protected virtual void OnInspectorGUIInsertion() { }
    }
}