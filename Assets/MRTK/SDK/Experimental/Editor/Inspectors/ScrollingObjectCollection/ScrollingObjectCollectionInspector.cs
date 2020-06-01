// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Inspectors
{
    [CustomEditor(typeof(ScrollingObjectCollection))]
    public class ScrollingObjectCollectionInspector : UnityEditor.Editor
    {
        private bool animateTransition = true;
        private int amountOfItemsToMoveBy = 1;
        private int amountOfLinesToMoveTo = 1;
        private int indexToMoveTo = 1;

        private SerializedProperty sorting;
        private SerializedProperty cellHeight;
        private SerializedProperty cellWidth;

        private SerializedProperty canScroll;
        private SerializedProperty scrollDirection;
        private SerializedProperty useNearScrollBoundary;
        private SerializedProperty viewableArea;
        private SerializedProperty tiers;
        private SerializedProperty useCameraPreRender;

        private SerializedProperty setUpAtRuntime;
        private SerializedProperty occlusionPositionPadding;
        private SerializedProperty occlusionScalePadding;
        private SerializedProperty dragTimeThreshold;
        private SerializedProperty handDeltaMagThreshold;
        private SerializedProperty snapListItems;

        private SerializedProperty velocityType;
        private SerializedProperty velocityMultiplier;
        private SerializedProperty velocityDampen;
        private SerializedProperty bounceMultiplier;
        private SerializedProperty animationCurve;
        private SerializedProperty animationLength;

        private SerializedProperty clickEvent;
        private SerializedProperty touchEvent;
        private SerializedProperty untouchEvent;
        private SerializedProperty momentumEvent;
        private SerializedProperty disableClippedItems;

        private bool showOverrideButtons = true;
        private bool showUnityEvents = false;

        // Serialized properties purely for inspector visualization
        private SerializedProperty nodeList;
        private SerializedProperty releaseDistance;

        private Shader MRTKtmp;

        private void OnEnable()
        {
            sorting = serializedObject.FindProperty("sortType");
            cellHeight = serializedObject.FindProperty("cellHeight");
            cellWidth = serializedObject.FindProperty("cellWidth");

            tiers = serializedObject.FindProperty("tiers");
            canScroll = serializedObject.FindProperty("canScroll");
            scrollDirection = serializedObject.FindProperty("scrollDirection");
            useNearScrollBoundary = serializedObject.FindProperty("useNearScrollBoundary");
            viewableArea = serializedObject.FindProperty("viewableArea");
            setUpAtRuntime = serializedObject.FindProperty("setUpAtRuntime");
            useCameraPreRender = serializedObject.FindProperty("useOnPreRender");

            occlusionPositionPadding = serializedObject.FindProperty("occlusionPositionPadding");
            occlusionScalePadding = serializedObject.FindProperty("occlusionScalePadding");

            dragTimeThreshold = serializedObject.FindProperty("dragTimeThreshold");
            handDeltaMagThreshold = serializedObject.FindProperty("handDeltaMagThreshold");

            snapListItems = serializedObject.FindProperty("snapListItems");
            velocityType = serializedObject.FindProperty("typeOfVelocity");
            velocityMultiplier = serializedObject.FindProperty("velocityMultiplier");
            velocityDampen = serializedObject.FindProperty("velocityDampen");
            bounceMultiplier = serializedObject.FindProperty("bounceMultiplier");
            animationCurve = serializedObject.FindProperty("paginationCurve");
            animationLength = serializedObject.FindProperty("animationLength");

            clickEvent = serializedObject.FindProperty("ClickEvent");
            touchEvent = serializedObject.FindProperty("TouchStarted");
            untouchEvent = serializedObject.FindProperty("TouchEnded");
            momentumEvent = serializedObject.FindProperty("ListMomentumEnded");

            disableClippedItems = serializedObject.FindProperty("disableClippedItems");
            // Serialized properties for visualization
            nodeList = serializedObject.FindProperty("nodeList");
            releaseDistance = serializedObject.FindProperty("releaseDistance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            ScrollingObjectCollection scrollContainer = (ScrollingObjectCollection)target;

            EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(canScroll);
                EditorGUILayout.PropertyField(scrollDirection);
                EditorGUILayout.PropertyField(setUpAtRuntime);
                EditorGUILayout.PropertyField(useCameraPreRender);
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Collection Properties", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(sorting);
                EditorGUILayout.PropertyField(cellWidth);
                EditorGUILayout.PropertyField(cellHeight);
                EditorGUILayout.PropertyField(tiers);
                EditorGUILayout.PropertyField(viewableArea);
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Update Collection"))
            {
                scrollContainer.UpdateCollection();
                EditorUtility.SetDirty(scrollContainer);
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("In order for a ScrollableObjectCollection to work properly with PressableButton, ReleaseOnTouchEnd must be inactive.", MessageType.Info);

            if (GUILayout.Button("Set Up PressableButtons"))
            {
                PressableButton[] pBs = scrollContainer.transform.GetComponentsInChildren<PressableButton>();
                foreach (PressableButton p in pBs)
                {
                    p.ReleaseOnTouchEnd = false;
                }

                PhysicalPressEventRouter[] routers = scrollContainer.transform.GetComponentsInChildren<PhysicalPressEventRouter>();
                foreach (PhysicalPressEventRouter r in routers)
                {
                    r.InteractableOnClick = PhysicalPressEventRouter.PhysicalPressEventBehavior.EventOnClickCompletion;
                }
            }

            EditorGUILayout.LabelField("Scrolling Properties", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(dragTimeThreshold);
                EditorGUILayout.PropertyField(handDeltaMagThreshold);
                EditorGUILayout.PropertyField(useNearScrollBoundary);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(occlusionPositionPadding);
                EditorGUILayout.PropertyField(occlusionScalePadding);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(animationCurve);
                EditorGUILayout.PropertyField(animationLength);
                EditorGUILayout.PropertyField(disableClippedItems);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Velocity Properties", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(velocityType);

                if (velocityType.enumValueIndex <= 1)
                {
                    EditorGUILayout.PropertyField(velocityMultiplier);
                    EditorGUILayout.PropertyField(velocityDampen);
                    EditorGUILayout.PropertyField(bounceMultiplier);
                }
            }

            EditorGUILayout.Space();

            showUnityEvents = EditorGUILayout.Foldout(showUnityEvents, "Unity Events: ", true);

            if (showUnityEvents)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(clickEvent);
                    EditorGUILayout.PropertyField(touchEvent);
                    EditorGUILayout.PropertyField(untouchEvent);
                    EditorGUILayout.PropertyField(momentumEvent);
                }
            }

            EditorGUILayout.Space();

            showOverrideButtons = EditorGUILayout.Foldout(showOverrideButtons, "Scrolling debug buttons", true);

            EditorGUILayout.Space();

            if (showOverrideButtons)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    animateTransition = EditorGUILayout.Toggle("Animate", animateTransition);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Page Up"))
                        {
                            scrollContainer.PageBy(1, animateTransition);
                        }
                        if (GUILayout.Button("Page Down"))
                        {
                            scrollContainer.PageBy(-1, animateTransition);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {

                        amountOfLinesToMoveTo = EditorGUILayout.IntField(amountOfLinesToMoveTo);
                        if (GUILayout.Button("Move By Tiers"))
                        {
                            scrollContainer.MoveByLines(amountOfLinesToMoveTo, animateTransition);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {

                        amountOfItemsToMoveBy = EditorGUILayout.IntField(amountOfItemsToMoveBy);
                        if (GUILayout.Button("Move By Items"))
                        {
                            scrollContainer.MoveByItems(amountOfItemsToMoveBy, animateTransition);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {

                        indexToMoveTo = EditorGUILayout.IntField(indexToMoveTo);
                        if (GUILayout.Button("Move To Index"))
                        {
                            scrollContainer.MoveTo(indexToMoveTo, animateTransition);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < nodeList.arraySize; i++)
                {
                    var node = nodeList.GetArrayElementAtIndex(i);
                    Transform nodeTransform = node.FindPropertyRelative("Transform").objectReferenceValue as Transform;
                    if (nodeTransform == null) { continue; }

                    if (!CheckForStandardShader(nodeTransform.GetComponentsInChildren<Renderer>()))
                    {
                        Debug.LogWarning(nodeTransform.name + " has a renderer that is not using " + StandardShaderUtility.MrtkStandardShaderName + ". This will result in unexpected results with ScrollingObjectCollection");
                    }
                }
            }

        }

        [DrawGizmo(GizmoType.Selected)]
        private void OnSceneGUI()
        {
            ScrollingObjectCollection scrollContainer = (ScrollingObjectCollection)target;
            MRTKtmp = Shader.Find("Mixed Reality Toolkit/TextMeshPro");

            if (scrollContainer.ClippingObject == null) { return; }

            if (Event.current.type == EventType.Repaint)
            {
                DisplayTouchPlane(scrollContainer);

                // Display the item number on the list items
                for (int i = 0; i <= nodeList.arraySize - 1; i++)
                {
                    var node = nodeList.GetArrayElementAtIndex(i);
                    Transform nodeTransform = node.FindPropertyRelative("Transform").objectReferenceValue as Transform;

                    if (nodeTransform == null) { continue; }

                    Vector3 cp = nodeTransform.position;

                    UnityEditor.Handles.Label(cp, new GUIContent(i.ToString()));
                }
            }
        }

        /// <summary>
        /// Displays the touch plane used for scrolling (and releasing).
        /// </summary>
        private void DisplayTouchPlane(ScrollingObjectCollection container)
        {
            Color arrowColor = Color.cyan;
            Vector3 center;
            if (container.ClippingObject == null)
            {
                return;
            }

            var scrollContainer = (ScrollingObjectCollection)target;
            // now that its running lets show the press plane so users have feedback about touch
            center = scrollContainer.transform.TransformPoint(Vector3.forward * -1.0f * releaseDistance.floatValue);

            UnityEditor.Handles.color = arrowColor;

            float arrowSize = UnityEditor.HandleUtility.GetHandleSize(center) * 0.75f;

            container.transform.rotation.ToAngleAxis(out float ang, out Vector3 currRotAxis);

            Vector3 rightDelta = container.transform.right * container.ClippingObject.transform.localScale.x;
            Vector3 upDelta = container.transform.up * container.ClippingObject.transform.localScale.y;

            Quaternion rot = Quaternion.LookRotation(container.transform.forward * -1.0f, container.transform.up);
            UnityEditor.Handles.ArrowHandleCap(0, center + (rightDelta * 0.5f) - (upDelta * 0.5f), rot, arrowSize, EventType.Repaint);

            Vector3[] points = new Vector3[4];
            points[0] = center;
            points[1] = center + rightDelta;
            points[2] = center + rightDelta - upDelta;
            points[3] = center - upDelta;

            UnityEditor.Handles.DrawSolidRectangleWithOutline(points, new Color(0.85f, 1.0f, 1.0f, 0.1f), arrowColor);
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            UnityEditor.Handles.Label(center + (rightDelta * 0.5f) - (upDelta * 0.5f), new GUIContent("touch plane", "The plane which the finger will need to cross in order for the touch to be calculated as a scroll or release."), labelStyle);
        }

        /// <summary>
        /// Simple check for the use of the standard shader.
        /// </summary>
        /// <param name="rends"><see cref="Renderer[]"/> to check for the MRTK standard shader.</param>
        /// <returns>true when render is using the MRTK standard shader.</returns>
        private bool CheckForStandardShader(Renderer[] rends)
        {
            foreach (Renderer rend in rends)
            {
                if (!StandardShaderUtility.IsUsingMrtkStandardShader(rend.sharedMaterial) && rend.sharedMaterial.shader != MRTKtmp)
                {
                    return false;
                }
            }
            return true;
        }

    }
}