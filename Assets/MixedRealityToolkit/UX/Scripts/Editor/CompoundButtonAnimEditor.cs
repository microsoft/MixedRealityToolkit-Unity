// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Inspectors.EditorScript;
using MixedRealityToolkit.UX.Buttons.Enums;
using MixedRealityToolkit.UX.Buttons.Utilities;
using UnityEngine;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(CompoundButtonAnim))]
    public class CompoundButtonAnimEditor : MRTKEditor
    {
        /// <summary>
        /// Draw a custom editor for AnimatorControllerActions to make them easier to edit
        /// </summary>
        protected override void DrawCustomFooter()
        {

            CompoundButtonAnim acb = (CompoundButtonAnim)target;
            Animator animator = acb.TargetAnimator;
            AnimatorControllerParameter[] animParams = null;

            // Validate the AnimButton controls - make sure there's one control for each button state
            ButtonStateEnum[] buttonStates = (ButtonStateEnum[])System.Enum.GetValues(typeof(ButtonStateEnum));
            if (acb.AnimActions == null || acb.AnimActions.Length != buttonStates.Length)
            {
                acb.AnimActions = new AnimatorControllerAction[buttonStates.Length];
            }

            // Don't allow user to change setup during play mode
            if (!Application.isPlaying && !string.IsNullOrEmpty(acb.gameObject.scene.name))
            {

                // Get the available animation parameters
                animParams = animator.parameters;

                for (int i = 0; i < buttonStates.Length; i++)
                {
                    acb.AnimActions[i].ButtonState = buttonStates[i];
                }

                // Now make sure all animation parameters are found
                for (int i = 0; i < acb.AnimActions.Length; i++)
                {
                    if (!string.IsNullOrEmpty(acb.AnimActions[i].ParamName))
                    {
                        bool invalidParam = true;
                        foreach (AnimatorControllerParameter animParam in animParams)
                        {
                            if (acb.AnimActions[i].ParamName == animParam.name)
                            {
                                // Update the type while we're here
                                invalidParam = false;
                                acb.AnimActions[i].ParamType = animParam.type;
                                break;
                            }
                        }

                        // If we didn't find it, mark it as invalid
                        acb.AnimActions[i].InvalidParam = invalidParam;
                    }
                }
            }

            UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            UnityEditor.EditorGUILayout.LabelField("Animation states:", UnityEditor.EditorStyles.miniBoldLabel);

            // Draw the editor for all the animation actions
            for (int i = 0; i < acb.AnimActions.Length; i++)
            {
                acb.AnimActions[i] = DrawAnimActionEditor(acb.AnimActions[i], animParams);
            }

            UnityEditor.EditorGUILayout.EndVertical();
        }

        AnimatorControllerAction DrawAnimActionEditor(AnimatorControllerAction action, AnimatorControllerParameter[] animParams)
        {
            bool actionIsEmpty = string.IsNullOrEmpty(action.ParamName);
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField(action.ButtonState.ToString(), GUILayout.MaxWidth(150f), GUILayout.MinWidth(150f));

            if (animParams != null && animParams.Length > 0)
            {
                // Show a dropdown
                string[] options = new string[animParams.Length + 1];
                options[0] = "(None)";
                int currentIndex = 0;
                for (int i = 0; i < animParams.Length; i++)
                {
                    options[i + 1] = animParams[i].name;
                    if (animParams[i].name == action.ParamName)
                    {
                        currentIndex = i + 1;
                    }
                }
                GUI.color = actionIsEmpty ? Color.gray : Color.white;
                int newIndex = UnityEditor.EditorGUILayout.Popup(currentIndex, options, GUILayout.MinWidth(150f), GUILayout.MaxWidth(150f));
                if (newIndex == 0)
                {
                    action.ParamName = string.Empty;
                }
                else
                {
                    action.ParamName = animParams[newIndex - 1].name;
                    action.ParamType = animParams[newIndex - 1].type;
                }
            }
            else
            {
                // Just show a label
                GUI.color = action.InvalidParam ? Color.yellow : Color.white;
                UnityEditor.EditorGUILayout.LabelField(actionIsEmpty ? "(None)" : action.ParamName, GUILayout.MinWidth(75f), GUILayout.MaxWidth(75f));
            }

            GUI.color = Color.white;

            if (!actionIsEmpty)
            {
                UnityEditor.EditorGUILayout.LabelField(action.ParamType.ToString(), UnityEditor.EditorStyles.miniLabel, GUILayout.MinWidth(75f), GUILayout.MaxWidth(75f));
                switch (action.ParamType)
                {
                    case AnimatorControllerParameterType.Bool:
                        action.BoolValue = UnityEditor.EditorGUILayout.Toggle(action.BoolValue);
                        break;

                    case AnimatorControllerParameterType.Float:
                        action.FloatValue = UnityEditor.EditorGUILayout.FloatField(action.FloatValue);
                        break;

                    case AnimatorControllerParameterType.Int:
                        action.IntValue = UnityEditor.EditorGUILayout.IntField(action.IntValue);
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        break;

                    default:
                        break;

                }
            }

            UnityEditor.EditorGUILayout.EndHorizontal();

            return action;
        }
    }
}