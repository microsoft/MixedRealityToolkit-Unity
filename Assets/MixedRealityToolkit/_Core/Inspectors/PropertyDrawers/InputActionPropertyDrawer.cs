// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(MixedRealityInputAction))]
    public class InputActionPropertyDrawer : PropertyDrawer
    {
        private static MixedRealityInputActionsProfile profile = null;
        private static GUIContent[] actionLabels = { new GUIContent("Missing Input Action Profile") };
        private static int[] actionIds = { 0 };

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            if (!MixedRealityToolkit.IsInitialized || !MixedRealityToolkit.HasActiveProfile)
            {
                profile = null;
                actionLabels = new[] { new GUIContent("Missing Mixed Reality Toolkit") };
                actionIds = new[] { 0 };
            }

            if (profile == null ||
                (MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled &&
                 profile.InputActions != null &&
                 profile.InputActions != MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions))
            {
                profile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile;

                if (profile != null)
                {
                    actionLabels = profile.InputActions.Select(action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
                    actionIds = profile.InputActions.Select(action => (int)action.Id).Prepend(0).ToArray();
                }
                else
                {
                    actionLabels = new[] { new GUIContent("No input action profile found") };
                    actionIds = new[] { 0 };
                }
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                profile = null;
                actionLabels = new[] { new GUIContent("Input System Disabled") };
                actionIds = new[] { 0 };
            }

            var label = EditorGUI.BeginProperty(rect, content, property);
            var inputActionId = property.FindPropertyRelative("id");

            if (profile == null || actionLabels == null || actionIds == null)
            {
                GUI.enabled = false;
                EditorGUI.IntPopup(rect, label, inputActionId.intValue.ResetIfGreaterThan(0), actionLabels, actionIds);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                inputActionId.intValue = EditorGUI.IntPopup(rect, label, inputActionId.intValue.ResetIfGreaterThan(profile.InputActions.Length), actionLabels, actionIds);

                if (EditorGUI.EndChangeCheck())
                {
                    var description = property.FindPropertyRelative("description");
                    var axisConstraint = property.FindPropertyRelative("axisConstraint");

                    if (inputActionId.intValue > 0)
                    {
                        description.stringValue = profile.InputActions[inputActionId.intValue - 1].Description;
                        axisConstraint.intValue = (int)profile.InputActions[inputActionId.intValue - 1].AxisConstraint;
                    }
                    else
                    {
                        description.stringValue = "None";
                        axisConstraint.intValue = 0;
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
