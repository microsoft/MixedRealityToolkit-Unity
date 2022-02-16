// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Provides helpers for rendering speech keyword dropdowns populated from the MRTK speech profile.
    /// </summary>
    public static class SpeechKeywordUtility
    {
        private static readonly GUIContent SpeechCommandsLabel = new GUIContent("Speech Command", "Speech keyword to use, pulled from MRTK/Input/Speech Commands Profile");

        /// <summary>
        /// Renders a dropdown with all keywords in the active <see cref="Input.MixedRealitySpeechCommandsProfile"/>.
        /// </summary>
        /// <param name="property">The string property representing the selected keyword.</param>
        public static void RenderKeywords(SerializedProperty property, Rect rect = default(Rect), GUIContent content = null) => RenderKeywordsExcept(null, property, rect, content);

        /// <summary>
        /// Renders a dropdown with all keywords in the active <see cref="Input.MixedRealitySpeechCommandsProfile"/> except those passed in.
        /// </summary>
        /// <param name="keywordsInUse">The keywords the component is currently using.</param>
        /// <param name="property">The string property representing the selected keyword.</param>
        public static void RenderKeywordsExcept(string[] keywordsInUse, SerializedProperty property, Rect rect = default(Rect), GUIContent content = null)
        {
            IEnumerable<string> availableKeywords = GetDistinctRegisteredKeywords();

            if (keywordsInUse != null && availableKeywords != null)
            {
                availableKeywords = availableKeywords.Except(keywordsInUse.Select(keyword => keyword));
            }

            bool validSpeechKeywords = availableKeywords != null && availableKeywords.Count() > 0;

            if (rect == default(Rect))
            {
                rect = EditorGUILayout.GetControlRect();
            }

            if (content == null)
            {
                content = SpeechCommandsLabel;
            }

            using (new EditorGUI.DisabledScope(!validSpeechKeywords))
            using (new EditorGUI.PropertyScope(rect, content, property))
            {
                string[] keywordOptions;
                if (validSpeechKeywords)
                {
                    if (string.IsNullOrWhiteSpace(property.stringValue) || availableKeywords.Contains(property.stringValue))
                    {
                        keywordOptions = availableKeywords.OrderBy(keyword => keyword).ToArray();
                    }
                    else
                    {
                        // if the currently selected option is not whitespace, ensure it's included in the dropdown
                        keywordOptions = availableKeywords.Concat(new[] { property.stringValue }).OrderBy(keyword => keyword).ToArray();
                    }
                }
                else
                {
                    keywordOptions = new string[] { "Missing Speech Commands" };
                }

                int previousSelection = Mathf.Clamp(ArrayUtility.IndexOf(keywordOptions, property.stringValue), 0, int.MaxValue);
                int currentSelection = EditorGUI.Popup(rect, content.text, previousSelection, keywordOptions);

                if (validSpeechKeywords && currentSelection != previousSelection)
                {
                    property.stringValue = currentSelection > 0 ? keywordOptions[currentSelection] : string.Empty; // Don't serialize the "(No Selection)" entry
                }
            }
        }

        /// <summary>
        /// Look for speech commands in the MRTK speech command profile.
        /// Adds a "(No Selection)" value at index zero and filters out duplicate values.
        /// </summary>
        public static IEnumerable<string> GetDistinctRegisteredKeywords()
        {
            if (!MixedRealityToolkit.ConfirmInitialized() ||
                !MixedRealityToolkit.Instance.HasActiveProfile ||
                !MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile == null ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands.Length == 0)
            {
                return null;
            }

            List<string> keywords = new List<string>
            {
                "(No Selection)"
            };

            Input.SpeechCommands[] speechCommands = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands;
            for (var i = 0; i < speechCommands.Length; i++)
            {
                keywords.Add(speechCommands[i].Keyword);
            }

            return keywords.Distinct();
        }
    }
}
