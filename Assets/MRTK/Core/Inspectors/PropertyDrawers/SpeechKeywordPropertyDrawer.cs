// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomPropertyDrawer(typeof(SpeechKeywordAttribute))]
    public class SpeechKeywordPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content) => SpeechKeywordUtility.RenderKeywords(property, rect, content);
    }
}