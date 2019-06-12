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
        private int amountOfLinesToMoveTo = 0;
        private int indexToMoveTo = 0;

        private SerializedProperty sorting;
        private SerializedProperty cellHeight;
        private SerializedProperty cellWidth;

        private SerializedProperty scrollDirection;
        private SerializedProperty viewableArea;
        private SerializedProperty collectionForward;
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
        private SerializedProperty velocityFalloff;
        private SerializedProperty animationCurve;
        private SerializedProperty animationLength;

        private SerializedProperty clickEvent;
        private SerializedProperty touchEvent;
        private SerializedProperty untouchEvent;
        private SerializedProperty momentumEvent;

        private bool showOverrideButtons = true;
        private bool showUnityEvents = false;

        //Serialized properties purely for inspector visualization
        private SerializedProperty pressPlane;

        private Shader MRTKstd;
        private Shader MRTKtmp;

        private void OnEnable()
        {
            sorting = serializedObject.FindProperty("sortType");
            cellHeight = serializedObject.FindProperty("cellHeight");
            cellWidth = serializedObject.FindProperty("cellWidth");

            tiers = serializedObject.FindProperty("tiers");
            scrollDirection = serializedObject.FindProperty("scrollDirection");
            collectionForward = serializedObject.FindProperty("collectionForward");
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


            //serialized properties for vizualisation
            pressPlane = serializedObject.FindProperty("thresholdPoint");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ScrollingObjectCollection scrollContainer = (ScrollingObjectCollection)target;

            EditorGUILayout.LabelField("Collection Properties", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(sorting);
                EditorGUILayout.PropertyField(cellWidth);
                EditorGUILayout.PropertyField(cellHeight);
                EditorGUILayout.PropertyField(tiers);
                EditorGUILayout.PropertyField(viewableArea);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(scrollDirection);
                EditorGUILayout.PropertyField(collectionForward);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(setUpAtRuntime);
                EditorGUILayout.PropertyField(useCameraPreRender);
                EditorGUILayout.Space();

            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Update Collection"))
            {
                scrollContainer.UpdateCollection();
                EditorUtility.SetDirty(scrollContainer);
            }

            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Scrolling Properties", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {                
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

            EditorGUILayout.LabelField("Velocity Properties", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(velocityType);
                EditorGUILayout.PropertyField(velocityMultiplier);
                EditorGUILayout.PropertyField(velocityFalloff);
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
                    amountOfLinesToMoveTo = EditorGUILayout.IntField(amountOfLinesToMoveTo);
                    if (GUILayout.Button("Move By Tiers"))
                    {
                        scrollContainer.MoveByLines(amountOfLinesToMoveTo, animateTransition);
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

        [DrawGizmo(GizmoType.Selected)]
        private void OnSceneGUI()
        {
            ScrollingObjectCollection scrollContainer = (ScrollingObjectCollection)target;

            MRTKstd = Shader.Find("Mixed Reality Toolkit/Standard");
            MRTKtmp = Shader.Find("Mixed Reality Toolkit/TextMeshPro");

            if (scrollContainer.ClippingObject == null) { return; }

            if (Event.current.type == EventType.Repaint)
            {
                Color arrowColor = Color.cyan;
                Vector3 center;
                if (scrollContainer.ClippingObject != null)
                {
                    center = scrollContainer.ClippingObject.transform.position;
                    center.z = scrollContainer.ClippingObject.transform.position.z + scrollContainer.ClippingObject.transform.localScale.z * (ScrollingObjectCollection.AxisOrientationToDirection(scrollContainer.CollectionForward).z * 0.5f);
                }
                else
                {
                    center = scrollContainer.transform.position;
                    arrowColor = Color.yellow;
                }

                if (Application.isPlaying)
                {
                    //now that its running lets show the press plane so users have feedback about touch
                    center.z = pressPlane.vector3Value.z;
                }

                UnityEditor.Handles.color = arrowColor;

                float arrowSize = UnityEditor.HandleUtility.GetHandleSize(center) * 0.75f;
                UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(scrollContainer.transform.rotation * ScrollingObjectCollection.AxisOrientationToDirection(scrollContainer.CollectionForward)), arrowSize, EventType.Repaint);

                Vector3 rightDelta = scrollContainer.transform.localToWorldMatrix.MultiplyVector(scrollContainer.transform.right * scrollContainer.ClippingObject.transform.localScale.x * 0.5f);
                Vector3 upDelta = scrollContainer.transform.localToWorldMatrix.MultiplyVector(scrollContainer.transform.up * scrollContainer.ClippingObject.transform.localScale.y * 0.5f);

                Vector3[] points = new Vector3[4];
                points[0] = center + rightDelta + upDelta;
                points[1] = center - rightDelta + upDelta;
                points[2] = center - rightDelta - upDelta;
                points[3] = center + rightDelta - upDelta;

                UnityEditor.Handles.DrawSolidRectangleWithOutline(points, Color.clear, arrowColor);
                UnityEditor.Handles.Label(center + new Vector3(-0.003f, 0.003f, 0.0f), new GUIContent("touch plane", "The plane which the finger will need to cross in order for the touch to be calculated as a scroll"));

                //Display the item number on the list items
                for (int i = 0; i < scrollContainer.CollectionNodes.Count; i++)
                {
                    ObjectCollectionNode node = scrollContainer.CollectionNodes[i];

                    if(node.Transform == null) { continue; }

                    if (!CheckForStandardShader(node.GameObject.GetComponentsInChildren<Renderer>()))
                    {
                        Debug.LogWarning(node.GameObject.name + " has a renderer that is not using " + MRTKstd.ToString() + ". This will result in unexpected results with ScrollingObjectCollection");
                    }

                    Vector3 cp = node.Transform.position;
                    cp.z = center.z;
                    if(scrollContainer.IsItemVisible(i))
                    {
                        UnityEditor.Handles.Label(cp, new GUIContent(i.ToString()));
                    }
                }
            }
        }

        private bool CheckForStandardShader(Renderer[] rends)
        {
            foreach (Renderer rend in rends)
            {
                if(rend.sharedMaterial.shader != MRTKstd && rend.sharedMaterial.shader != MRTKtmp)
                {
                    return false;
                }
            }
            return true;
        }


    }
}
