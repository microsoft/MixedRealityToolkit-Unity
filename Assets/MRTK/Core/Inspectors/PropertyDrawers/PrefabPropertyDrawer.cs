// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom property drawer for <see cref="Microsoft.MixedReality.Toolkit.PrefabAttribute"/> decorated <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> values rendered in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(PrefabAttribute))]
    public class PrefabPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prefabAttribute = (PrefabAttribute)attribute;

            if (prefabAttribute != null &&
                property.propertyType == SerializedPropertyType.ObjectReference &&
                (property.objectReferenceValue is GameObject || property.objectReferenceValue == null))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(position, property);

                if (!EditorGUI.EndChangeCheck()) { return; }
                if (property.objectReferenceValue == null) { return; }

                if (PrefabUtility.GetPrefabAssetType(property.objectReferenceValue) == PrefabAssetType.NotAPrefab)
                {
                    property.objectReferenceValue = null;
                    Debug.LogWarning("Assigned GameObject must be a prefab.");
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use PrefabAttribute with GameObject fields only.");
            }
        }
    }
}