using Microsoft.MixedReality.Toolkit.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="PrefabAttribute"/> decorated <see cref="GameObject"/> values rendered in the inspector.
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