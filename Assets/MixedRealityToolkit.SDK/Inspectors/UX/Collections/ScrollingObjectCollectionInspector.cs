// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(ScrollingObjectCollection))]
    public class ScrollingObjectCollectionInspector : UnityEditor.Editor
    {
        private bool animateTransition = true;
        private int amountOfItemsToMoveBy = 0;
        private int amountOfRowsToMoveTo = 0;
        private int indexToMoveTo = 0;

        private SerializedProperty sorting;
        private SerializedProperty cellHeight;
        private SerializedProperty cellWidth;
        private SerializedProperty ignoreInactiveTransforms;

        private SerializedProperty scrollDirection;
        private SerializedProperty viewableArea;
        private SerializedProperty columns;
        private SerializedProperty useCameraPreRender;

        private SerializedProperty setUpAtRuntime;
        private SerializedProperty occlusionPositionPadding;
        private SerializedProperty occlusionScalePadding;
        private SerializedProperty dragTimeThreshold;
        private SerializedProperty handDeltaMagThreshold;
        private SerializedProperty snapListItems;

        private SerializedProperty velocityType;
        private SerializedProperty velocityMultiplier;
        private SerializedProperty velocityFalloff;
        private SerializedProperty animationCurve;
        private SerializedProperty animationLength;

        private SerializedProperty clickEvent;
        private SerializedProperty touchEvent;
        private SerializedProperty untouchEvent;
        private SerializedProperty momentumEvent;


        private bool showOverrideButtons = true;
        private bool showUnityEvents = false;

        private void OnEnable()
        {
            sorting = serializedObject.FindProperty("sortType");
            cellHeight = serializedObject.FindProperty("cellHeight");
            cellWidth = serializedObject.FindProperty("cellWidth");
            ignoreInactiveTransforms = serializedObject.FindProperty("ignoreInactiveTransforms");

            columns = serializedObject.FindProperty("columns");
            scrollDirection = serializedObject.FindProperty("scrollDirection");
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
            velocityFalloff = serializedObject.FindProperty("velocityFalloff");
            animationCurve = serializedObject.FindProperty("paginationCurve");
            animationLength = serializedObject.FindProperty("animationLength");

            clickEvent = serializedObject.FindProperty("ClickEvent");
            touchEvent = serializedObject.FindProperty("TouchStarted");
            untouchEvent = serializedObject.FindProperty("TouchEnded");
            momentumEvent = serializedObject.FindProperty("ListMomentumEnded");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ScrollingObjectCollection scrollContainer = (ScrollingObjectCollection)target;

            EditorGUILayout.LabelField("Collection Properties", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(sorting);
                EditorGUILayout.PropertyField(ignoreInactiveTransforms);

                EditorGUILayout.PropertyField(cellWidth);
                EditorGUILayout.PropertyField(cellHeight);
                EditorGUILayout.PropertyField(columns);
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Scrolling Properties", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {

                EditorGUILayout.PropertyField(scrollDirection);
                EditorGUILayout.PropertyField(viewableArea);
                EditorGUILayout.PropertyField(setUpAtRuntime);
                EditorGUILayout.PropertyField(useCameraPreRender);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(dragTimeThreshold);
                EditorGUILayout.PropertyField(handDeltaMagThreshold);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(occlusionPositionPadding);
                EditorGUILayout.PropertyField(occlusionScalePadding);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(animationCurve);
                EditorGUILayout.PropertyField(animationLength);

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

            if (GUILayout.Button("Update Collection"))
            {
                scrollContainer.UpdateCollection();
                EditorUtility.SetDirty(scrollContainer);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Velocity Properties", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(velocityType);
                EditorGUILayout.PropertyField(velocityMultiplier);
                EditorGUILayout.PropertyField(velocityFalloff);
            }

            EditorGUILayout.Space();

            showOverrideButtons = EditorGUILayout.Foldout(showOverrideButtons, "Scrolling debug buttons", true);

            EditorGUILayout.Space();

            if (showOverrideButtons)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    animateTransition = EditorGUILayout.Toggle("Animate", animateTransition);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Page Up"))
                    {
                        scrollContainer.PageBy(1);
                    }
                    if (GUILayout.Button("Page Down"))
                    {
                        scrollContainer.PageBy(-1);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    amountOfRowsToMoveTo = EditorGUILayout.IntField(amountOfRowsToMoveTo);
                    if (GUILayout.Button("Move By Rows"))
                    {
                        scrollContainer.MoveByRows(amountOfRowsToMoveTo, animateTransition);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    amountOfItemsToMoveBy = EditorGUILayout.IntField(amountOfItemsToMoveBy);
                    if (GUILayout.Button("Move By Items"))
                    {
                        scrollContainer.MoveByItems(amountOfItemsToMoveBy, animateTransition);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    indexToMoveTo = EditorGUILayout.IntField(indexToMoveTo);
                    if (GUILayout.Button("Move To Index"))
                    {
                        scrollContainer.MoveTo(indexToMoveTo, animateTransition);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
