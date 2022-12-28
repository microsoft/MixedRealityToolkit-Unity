// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MRTKBaseInteractable), true)]
    public class BaseInteractableEditor : XRBaseInteractableEditor
    {
        private SerializedProperty isGazeHovered;
        private SerializedProperty isGazePinchHovered;
        private SerializedProperty isRayHovered;
        private SerializedProperty isGrabHovered;
        private SerializedProperty isPokeHovered;
        private SerializedProperty isGazePinchSelected;
        private SerializedProperty isRaySelected;
        private SerializedProperty isGrabSelected;

        private SerializedProperty isPokeSelected;
        private SerializedProperty isActiveHovered;
        private SerializedProperty disabledInteractorTypes;

        private bool xriExpanded = false;
        private bool mrtkExpanded = false;

        private List<string> serializedPropertyNames = new List<string>();

        protected override void OnEnable()
        {
            base.OnEnable();

            isGazeHovered = SetUpProperty(nameof(isGazeHovered));
            isGazePinchHovered = SetUpProperty(nameof(isGazePinchHovered));
            isRayHovered = SetUpProperty(nameof(isRayHovered));
            isGrabHovered = SetUpProperty(nameof(isGrabHovered));
            isPokeHovered = SetUpProperty(nameof(isPokeHovered));
            isActiveHovered = SetUpProperty(nameof(isActiveHovered));

            isGazePinchSelected = SetUpProperty(nameof(isGazePinchSelected));
            isRaySelected = SetUpProperty(nameof(isRaySelected));
            isGrabSelected = SetUpProperty(nameof(isGrabSelected));
            isPokeSelected = SetUpProperty(nameof(isPokeSelected));

            disabledInteractorTypes = SetUpProperty(nameof(disabledInteractorTypes));
        }

        /// <summary>
        /// Sets up a serialized property based on the name of the backing serialized field.
        /// </summary>
        protected SerializedProperty SetUpProperty(string serializedFieldName)
        {
            serializedPropertyNames.Add(serializedFieldName);
            return serializedObject.FindProperty(serializedFieldName);
        }

        /// <summary>
        /// Sets up a serialized property based on the name of the autoproperty with an
        /// implicitly-defindd backing serialized field.
        /// </summary>
        /// <seealso cref="SetUpProperty(string)"/>
        /// <seealso cref="InspectorUIUtility.GetBackingField(string)"/>
        protected SerializedProperty SetUpAutoProp(string autoPropName)
        {
            string backingField = InspectorUIUtility.GetBackingField(autoPropName);
            return SetUpProperty(backingField);
        }

        protected override List<string> GetDerivedSerializedPropertyNames()
        {
            var propNames = base.GetDerivedSerializedPropertyNames();
            propNames.AddRange(serializedPropertyNames);
            return propNames;
        }

        static bool xriBaseFoldout = false;

        protected override void DrawProperties()
        {
            xriBaseFoldout = EditorGUILayout.Foldout(xriBaseFoldout, EditorGUIUtility.TrTempContent("Base XRI Settings"), true, EditorStyles.foldoutHeader);
            if (xriBaseFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    base.DrawProperties();
                }
            }
        }

        protected override void DrawInteractableEvents()
        {
            EditorGUILayout.PropertyField(disabledInteractorTypes);

            mrtkExpanded = EditorGUILayout.Foldout(mrtkExpanded, EditorGUIUtility.TrTempContent("MRTK Events"), true);

            if (mrtkExpanded)
            {
                using (new EditorGUI.IndentLevelScope()) { DrawMRTKInteractableFlags(); }
            }

            xriExpanded = EditorGUILayout.Foldout(xriExpanded, EditorGUIUtility.TrTempContent("XRI Interactable Events"), true);

            if (xriExpanded)
            {
                using (new EditorGUI.IndentLevelScope()) { base.DrawInteractableEventsNested(); }
            }
        }

        protected virtual void DrawMRTKInteractableFlags()
        {
            Color previousGUIColor = GUI.color;
            MRTKBaseInteractable interactable = target as MRTKBaseInteractable;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("MRTKBaseInteractable Hover Events", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                DrawTimedFlag(isGazeHovered, interactable.IsGazeHovered, previousGUIColor, Color.cyan);
                DrawTimedFlag(isGazePinchHovered, interactable.IsGazePinchHovered, previousGUIColor, Color.cyan);
                DrawTimedFlag(isRayHovered, interactable.IsRayHovered, previousGUIColor, Color.cyan);
                DrawTimedFlag(isGrabHovered, interactable.IsGrabHovered, previousGUIColor, Color.cyan);
                DrawTimedFlag(isPokeHovered, interactable.IsPokeHovered, previousGUIColor, Color.cyan);
                DrawTimedFlag(isActiveHovered, interactable.IsActiveHovered, previousGUIColor, Color.cyan);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("MRTKBaseInteractable Select Events", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                DrawTimedFlag(isGrabSelected, interactable.IsGrabSelected, previousGUIColor, Color.cyan);
                DrawTimedFlag(isRaySelected, interactable.IsRaySelected, previousGUIColor, Color.cyan);
                DrawTimedFlag(isGazePinchSelected, interactable.IsGazePinchSelected, previousGUIColor, Color.cyan);
                DrawTimedFlag(isPokeSelected, interactable.IsPokeSelected, previousGUIColor, Color.cyan);
            }
        }

        protected void DrawTimedFlag(SerializedProperty property, TimedFlag timedFlag, Color previousColor, Color activeColor)
        {
            GUI.color = previousColor;

            if (timedFlag.Active)
            {
                GUI.color = activeColor;
            }

            EditorGUILayout.PropertyField(property);

            GUI.color = previousColor;
        }
    }
}
