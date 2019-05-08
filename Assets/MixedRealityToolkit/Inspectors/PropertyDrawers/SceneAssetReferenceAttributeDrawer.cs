using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Draws an object field as a scene asset reference.
    /// This enables fields to store references to scene assets (which is an editor-only object) as unity objects (which work in both editor and runtime)
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneAssetReferenceAttribute))]
    public class SceneAssetReferenceAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, new GUIContent(property.name), property);
            UnityEngine.Object newObject = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(SceneAsset), false);
            if (property.objectReferenceValue != newObject)
            {
                property.objectReferenceValue = newObject;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            EditorGUI.EndProperty();
        }
    }
}