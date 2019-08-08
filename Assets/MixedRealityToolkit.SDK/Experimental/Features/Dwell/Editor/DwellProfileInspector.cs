// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    [CustomEditor(typeof(DwellProfile))]
    public class DwellProfileInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(this.serializedObject, "timeToAllowDwellResume");

            DrawConditionParameter("timeToAllowDwellResume", "allowDwellResume");

            this.serializedObject.ApplyModifiedProperties();
        }

        public void DrawConditionParameter(string propertyToDraw, string conditionalProperty)
        {
            var allowDwllResume = serializedObject.FindProperty(conditionalProperty);
            if (allowDwllResume.boolValue)
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty(propertyToDraw));
            }

        }
    }
}