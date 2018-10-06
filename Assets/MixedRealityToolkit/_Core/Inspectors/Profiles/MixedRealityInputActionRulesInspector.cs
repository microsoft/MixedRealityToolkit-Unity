// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Inspectors;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityInputActionRulesProfile))]
    public class MixedRealityInputActionRulesInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent RuleAddButtonContent = new GUIContent("+ Add a New Rule Definition");
        private static readonly GUIContent RuleMinusButtonContent = new GUIContent("-", "Remove Rule Definition");
        private static readonly GUIContent BaseActionContent = new GUIContent("Base Input Action", "The Action that will raise new actions based on the criteria met");
        private static readonly GUIContent RuleActionContent = new GUIContent("Rule Input Action", "The Action that will be raised when the criteria is met");
        private static readonly GUIContent CriteriaContent = new GUIContent("Action Criteria", "The Criteria that must be met in order to raise the new Action");

        private SerializedProperty inputActionRulesDigital;
        private SerializedProperty inputActionRulesSingleAxis;
        private SerializedProperty inputActionRulesDualAxis;
        private SerializedProperty inputActionRulesVectorAxis;
        private SerializedProperty inputActionRulesQuaternionAxis;
        private SerializedProperty inputActionRulesPoseAxis;

        private int[] baseActionIds;
        private string[] baseActionLabels;
        private static int[] ruleActionIds = new int[0];
        private static string[] ruleActionLabels = new string[0];

        private int selectedBaseActionId = 0;
        private int selectedRuleActionId = 0;

        private static MixedRealityInputAction currentBaseAction = MixedRealityInputAction.None;
        private static MixedRealityInputAction currentRuleAction = MixedRealityInputAction.None;

        private bool currentBoolCriteria;
        private float currentSingleAxisCriteria;
        private Vector2 currentDualAxisCriteria;
        private Vector3 currentVectorCriteria;
        private Quaternion currentQuaternionCriteria;
        private MixedRealityPose currentPoseCriteria;

        private bool[] digitalFoldouts;
        private bool[] singleAxisFoldouts;
        private bool[] dualAxisFoldouts;
        private bool[] vectorFoldouts;
        private bool[] quaternionFoldouts;
        private bool[] poseFoldouts;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false) ||
                !MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled ||
                 MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                return;
            }

            inputActionRulesDigital = serializedObject.FindProperty("inputActionRulesDigital");
            inputActionRulesSingleAxis = serializedObject.FindProperty("inputActionRulesSingleAxis");
            inputActionRulesDualAxis = serializedObject.FindProperty("inputActionRulesDualAxis");
            inputActionRulesVectorAxis = serializedObject.FindProperty("inputActionRulesVectorAxis");
            inputActionRulesQuaternionAxis = serializedObject.FindProperty("inputActionRulesQuaternionAxis");
            inputActionRulesPoseAxis = serializedObject.FindProperty("inputActionRulesPoseAxis");

            baseActionLabels = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions
                .Select(action => action.Description)
                .Prepend("None").ToArray();

            baseActionIds = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            Reset();
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();

            if (!CheckMixedRealityManager() ||
                !MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled ||
                 MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                if (GUILayout.Button("Back to Configuration Profile"))
                {
                    Selection.activeObject = MixedRealityManager.Instance.ActiveProfile;
                }

                return;
            }

            if (GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input Action Rules Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Input Action Rules help define alternative Actions that will be raised based on specific criteria.\n\n" +
                                    "You can create new rules by assigning a base Input Action below, then assigning the criteria you'd like to meet. When the criteria is met, the Rule's Action will be raised with the criteria value.", MessageType.Info);

            EditorGUILayout.Space();

            var isGuiLocked = !(MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile);
            GUI.enabled = isGuiLocked;

            serializedObject.Update();

            selectedBaseActionId = RenderBaseInputAction(selectedBaseActionId, out currentBaseAction);
            GUI.enabled = isGuiLocked && currentBaseAction != MixedRealityInputAction.None;
            RenderCriteriaField(currentBaseAction);

            if (selectedBaseActionId == selectedRuleActionId)
            {
                selectedRuleActionId = 0;
            }

            selectedRuleActionId = RenderRuleInputAction(selectedRuleActionId, out currentRuleAction);

            EditorGUILayout.Space();

            GUI.enabled = isGuiLocked &&
                          currentBaseAction != MixedRealityInputAction.None &&
                          currentRuleAction != MixedRealityInputAction.None &&
                          currentBaseAction.AxisConstraint != AxisType.None &&
                          currentBaseAction.AxisConstraint != AxisType.Raw;

            if (GUILayout.Button(RuleAddButtonContent, EditorStyles.miniButton))
            {
                AddRule();
                Reset();
            }

            GUI.enabled = isGuiLocked;

            EditorGUILayout.Space();

            var isWideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            RenderList(inputActionRulesDigital, digitalFoldouts);
            RenderList(inputActionRulesSingleAxis, singleAxisFoldouts);
            RenderList(inputActionRulesDualAxis, dualAxisFoldouts);
            RenderList(inputActionRulesVectorAxis, vectorFoldouts);
            RenderList(inputActionRulesQuaternionAxis, quaternionFoldouts);
            RenderList(inputActionRulesPoseAxis, poseFoldouts);

            EditorGUIUtility.wideMode = isWideMode;
            serializedObject.ApplyModifiedProperties();
        }

        private void Reset()
        {
            selectedBaseActionId = 0;
            selectedRuleActionId = 0;
            currentBaseAction = MixedRealityInputAction.None;
            currentRuleAction = MixedRealityInputAction.None;
            currentBoolCriteria = false;
            currentSingleAxisCriteria = 0f;
            currentDualAxisCriteria = Vector2.zero;
            currentVectorCriteria = Vector3.zero;
            currentQuaternionCriteria = Quaternion.identity;
            currentPoseCriteria = MixedRealityPose.ZeroIdentity;

            digitalFoldouts = new bool[inputActionRulesDigital.arraySize];
            singleAxisFoldouts = new bool[inputActionRulesSingleAxis.arraySize];
            dualAxisFoldouts = new bool[inputActionRulesDualAxis.arraySize];
            vectorFoldouts = new bool[inputActionRulesVectorAxis.arraySize];
            quaternionFoldouts = new bool[inputActionRulesQuaternionAxis.arraySize];
            poseFoldouts = new bool[inputActionRulesPoseAxis.arraySize];
        }

        private static void GetCompatibleActions(MixedRealityInputAction baseAction)
        {
            ruleActionLabels = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == baseAction.AxisConstraint && inputAction.Id != baseAction.Id)
                .Select(action => action.Description)
                .Prepend("None").ToArray();

            ruleActionIds = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == baseAction.AxisConstraint && inputAction.Id != baseAction.Id)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();
        }

        private void RenderCriteriaField(MixedRealityInputAction action)
        {
            if (action != MixedRealityInputAction.None)
            {
                switch (action.AxisConstraint)
                {
                    default:
                    case AxisType.None:
                        EditorGUILayout.HelpBox("Base rule must have an axis constraint.", MessageType.Warning);
                        break;
                    case AxisType.Raw:
                        EditorGUILayout.HelpBox("Base rule's axis constraint is Raw. It's not possible to set this value in the inspector.", MessageType.Warning);
                        break;
                    case AxisType.Digital:
                        currentBoolCriteria = EditorGUILayout.Toggle(CriteriaContent, currentBoolCriteria);
                        break;
                    case AxisType.SingleAxis:
                        currentSingleAxisCriteria = EditorGUILayout.FloatField(CriteriaContent, currentSingleAxisCriteria);
                        break;
                    case AxisType.DualAxis:
                        currentDualAxisCriteria = EditorGUILayout.Vector2Field(CriteriaContent, currentDualAxisCriteria);
                        break;
                    case AxisType.ThreeDofPosition:
                        currentVectorCriteria = EditorGUILayout.Vector3Field(CriteriaContent, currentVectorCriteria);
                        break;
                    case AxisType.ThreeDofRotation:
                        currentQuaternionCriteria.eulerAngles = EditorGUILayout.Vector3Field(CriteriaContent, currentQuaternionCriteria.eulerAngles);
                        break;
                    case AxisType.SixDof:
                        EditorGUILayout.LabelField(CriteriaContent);
                        EditorGUI.indentLevel++;
                        currentPoseCriteria.Position = EditorGUILayout.Vector3Field("Position", currentPoseCriteria.Position);
                        Quaternion rotation = currentPoseCriteria.Rotation;
                        rotation.eulerAngles = EditorGUILayout.Vector3Field("Rotation", rotation.eulerAngles);
                        currentPoseCriteria.Rotation = rotation;
                        EditorGUI.indentLevel--;
                        break;
                }
            }
        }

        private void AddRule()
        {
            SerializedProperty rule;
            switch (currentBaseAction.AxisConstraint)
            {
                case AxisType.Digital:
                    inputActionRulesDigital.arraySize += 1;
                    rule = inputActionRulesDigital.GetArrayElementAtIndex(inputActionRulesDigital.arraySize - 1);
                    rule.FindPropertyRelative("criteria").boolValue = currentBoolCriteria;
                    break;
                case AxisType.SingleAxis:
                    inputActionRulesSingleAxis.arraySize += 1;
                    rule = inputActionRulesSingleAxis.GetArrayElementAtIndex(inputActionRulesSingleAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").floatValue = currentSingleAxisCriteria;
                    break;
                case AxisType.DualAxis:
                    inputActionRulesDualAxis.arraySize += 1;
                    rule = inputActionRulesDualAxis.GetArrayElementAtIndex(inputActionRulesDualAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").vector2Value = currentDualAxisCriteria;
                    break;
                case AxisType.ThreeDofPosition:
                    inputActionRulesVectorAxis.arraySize += 1;
                    rule = inputActionRulesVectorAxis.GetArrayElementAtIndex(inputActionRulesVectorAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").vector3Value = currentVectorCriteria;
                    break;
                case AxisType.ThreeDofRotation:
                    inputActionRulesQuaternionAxis.arraySize += 1;
                    rule = inputActionRulesQuaternionAxis.GetArrayElementAtIndex(inputActionRulesQuaternionAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").quaternionValue = currentQuaternionCriteria;
                    break;
                case AxisType.SixDof:
                    inputActionRulesPoseAxis.arraySize += 1;
                    rule = inputActionRulesPoseAxis.GetArrayElementAtIndex(inputActionRulesPoseAxis.arraySize - 1);
                    var criteria = rule.FindPropertyRelative("criteria");
                    criteria.FindPropertyRelative("position").vector3Value = currentPoseCriteria.Position;
                    criteria.FindPropertyRelative("rotation").quaternionValue = currentPoseCriteria.Rotation;
                    break;
                default:
                    Debug.LogError("Invalid Axis Constraint!");
                    return;
            }

            var baseAction = rule.FindPropertyRelative("baseAction");
            var baseActionId = baseAction.FindPropertyRelative("id");
            var baseActionDescription = baseAction.FindPropertyRelative("description");
            var baseActionConstraint = baseAction.FindPropertyRelative("axisConstraint");

            baseActionId.intValue = (int)currentBaseAction.Id;
            baseActionDescription.stringValue = currentBaseAction.Description;
            baseActionConstraint.intValue = (int)currentBaseAction.AxisConstraint;

            var ruleAction = rule.FindPropertyRelative("ruleAction");
            var ruleActionId = ruleAction.FindPropertyRelative("id");
            var ruleActionDescription = ruleAction.FindPropertyRelative("description");
            var ruleActionConstraint = ruleAction.FindPropertyRelative("axisConstraint");

            ruleActionId.intValue = (int)currentRuleAction.Id;
            ruleActionDescription.stringValue = currentRuleAction.Description;
            ruleActionConstraint.intValue = (int)currentRuleAction.AxisConstraint;
        }

        private int RenderBaseInputAction(int baseActionId, out MixedRealityInputAction action)
        {
            action = MixedRealityInputAction.None;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(BaseActionContent);
            EditorGUI.BeginChangeCheck();
            baseActionId = EditorGUILayout.IntPopup(baseActionId, baseActionLabels, baseActionIds);

            for (int i = 0; i < MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions.Length; i++)
            {
                if (baseActionId == (int)MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions[i].Id)
                {
                    action = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions[i];
                }
            }

            if (action != MixedRealityInputAction.None)
            {
                GetCompatibleActions(action);
            }

            EditorGUILayout.EndHorizontal();
            return baseActionId;
        }

        private int RenderRuleInputAction(int ruleActionId, out MixedRealityInputAction action)
        {
            action = MixedRealityInputAction.None;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(RuleActionContent);
            EditorGUI.BeginChangeCheck();
            ruleActionId = EditorGUILayout.IntPopup(ruleActionId, ruleActionLabels, ruleActionIds);

            for (int i = 0; i < MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions.Length; i++)
            {
                if (ruleActionId == (int)MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions[i].Id)
                {
                    action = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions[i];
                }
            }

            EditorGUILayout.EndHorizontal();
            return ruleActionId;
        }

        private void RenderList(SerializedProperty list, bool[] foldouts)
        {
            for (int i = 0; i < list?.arraySize; i++)
            {
                var rule = list.GetArrayElementAtIndex(i);
                var criteria = rule.FindPropertyRelative("criteria");

                var baseAction = rule.FindPropertyRelative("baseAction");
                var baseActionId = baseAction.FindPropertyRelative("id");
                var baseActionDescription = baseAction.FindPropertyRelative("description");
                var baseActionConstraint = baseAction.FindPropertyRelative("axisConstraint");

                var ruleAction = rule.FindPropertyRelative("ruleAction");
                var ruleActionId = ruleAction.FindPropertyRelative("id");
                var ruleActionDescription = ruleAction.FindPropertyRelative("description");
                var ruleActionConstraint = ruleAction.FindPropertyRelative("axisConstraint");

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], new GUIContent($"{baseActionDescription.stringValue} -> {ruleActionDescription.stringValue}"), true);

                if (GUILayout.Button(RuleMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (foldouts[i])
                {
                    EditorGUI.indentLevel++;

                    MixedRealityInputAction newBaseAction;
                    baseActionId.intValue = RenderBaseInputAction(baseActionId.intValue, out newBaseAction);
                    baseActionDescription.stringValue = newBaseAction.Description;
                    baseActionConstraint.intValue = (int)newBaseAction.AxisConstraint;

                    if (baseActionId.intValue == ruleActionId.intValue || newBaseAction == MixedRealityInputAction.None)
                    {
                        ruleActionId.intValue = (int)MixedRealityInputAction.None.Id;
                        ruleActionDescription.stringValue = MixedRealityInputAction.None.Description;
                        ruleActionConstraint.intValue = (int)MixedRealityInputAction.None.AxisConstraint;
                    }

                    EditorGUILayout.PropertyField(criteria);

                    MixedRealityInputAction newRuleAction;
                    ruleActionId.intValue = RenderRuleInputAction(ruleActionId.intValue, out newRuleAction);
                    ruleActionDescription.stringValue = newRuleAction.Description;
                    ruleActionConstraint.intValue = (int)newRuleAction.AxisConstraint;
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }
    }
}