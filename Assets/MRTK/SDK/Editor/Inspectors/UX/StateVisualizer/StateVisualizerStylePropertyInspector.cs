using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    public static class StateVisualizerStylePropertyInspector
    {        
        public static void RenderStyleProperty(SerializedProperty styleProperty, StateVisualizer stateVisualizerInstance, int animationTargetIndex)
        {
            SerializedProperty stylePropertyName = styleProperty.FindPropertyRelative("stylePropertyName");
            SerializedProperty stateName = styleProperty.FindPropertyRelative("stateName");
            SerializedProperty targetObj = styleProperty.FindPropertyRelative("target");
            SerializedProperty animationCurve = styleProperty.FindPropertyRelative("animationCurve");

            GameObject targetGameObject = targetObj.objectReferenceValue as GameObject;

            if (targetGameObject != null)
            {
                if (!IsTargetObjectValid(targetGameObject))
                {
                    targetObj.objectReferenceValue = null;
                    Debug.LogError("The target object must be itself or a child object");
                }

                RenderScaleProperty(styleProperty, stateVisualizerInstance, stateName.stringValue, stylePropertyName.stringValue, animationTargetIndex);
            }
        }

        private static void RenderScaleProperty(SerializedProperty scaleProperty, StateVisualizer stateVisualizerInstance, string stateName, string stylePropertyName, int animationTargetIndex)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(scaleProperty, true);

                if (check.changed)
                {
                    stateVisualizerInstance.SetKeyFrames(stateName, animationTargetIndex, stylePropertyName);
                }
            }
        }

        public static void RemoveKeyFrames(StateVisualizer stateVisualizerInstance, string stateName, string stylePropertyName, int animationTargetIndex)
        {
            stateVisualizerInstance.RemoveKeyFrames(stateName, animationTargetIndex, stylePropertyName);
        }


        public static void CreateStylePropertyInstance(StateVisualizer stateVisualizerInstance, string stateName, string propertyName, int animationTargetIndex)
        {
            stateVisualizerInstance.CreateStylePropertyInstance(animationTargetIndex, propertyName, stateName);
        }

        // A target game object is one that is itself or a child of the root
        private static bool IsTargetObjectValid(GameObject targetObj)
        {
            Transform startTransform = targetObj.transform;
            Transform initialTransform = targetObj.transform;

            // If this game object has the State Visualizer attached 
            if (targetObj.GetComponent<StateVisualizer>() != null)
            {
                return true;
            }

            // If the current object is a root and does not have a parent 
            if (startTransform.parent != null)
            {
                // Traverse parents until the State Visualizer is found to determine if the current target is a valid child object
                while (startTransform.parent != initialTransform)
                {
                    if (startTransform.GetComponent<StateVisualizer>() != null)
                    {
                        return true;
                    }

                    startTransform = startTransform.parent;
                }
            }

            return false;
        }
    }
}
