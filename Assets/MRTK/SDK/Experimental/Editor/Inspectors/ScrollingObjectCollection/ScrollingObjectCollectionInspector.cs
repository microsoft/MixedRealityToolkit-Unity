// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Inspectors
{
    [CustomEditor(typeof(ScrollingObjectCollection))]
    public class ScrollingObjectCollectionInspector : UnityEditor.Editor
    {
        private bool animateTransition = true;
        private int amountOfTiersToMove = 1;
        private int amountOfPagesToMove = 1;
        private int indexToMoveTo = 1;

        private SerializedProperty sorting;
        private SerializedProperty cellHeight;
        private SerializedProperty cellWidth;

        private SerializedProperty canScroll;
        private SerializedProperty scrollDirection;
        private SerializedProperty viewableArea;
        private SerializedProperty itemsPerTier;
        private SerializedProperty useCameraPreRender;

        private SerializedProperty setupAtRuntime;
        private SerializedProperty occlusionPositionPadding;
        private SerializedProperty occlusionScalePadding;
        private SerializedProperty handDeltaMagThreshold;

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

        private bool showDebugOptions = true;
        private bool showUnityEvents = false;

        // Serialized properties purely for inspector visualization
        private SerializedProperty nodeList;
        private SerializedProperty releaseThresholdFront;
        private SerializedProperty releaseThresholdBack;
        private SerializedProperty releaseThresholdLeftRight;
        private SerializedProperty releaseThresholdTopBottom;
        private SerializedProperty frontPlaneDistance;
        private Shader MRTKtmp;

        private ScrollingObjectCollection scrollView;

        private static bool visibleDebugPlanes = false;

        private Color touchPlaneColor = Color.cyan;
        private Color releasePlaneColor = Color.magenta;
        private static GUIStyle labelStyle;
        private Vector3 mouseScreenPosition;
        private float smallestMouseHandleDistance;

        private const string TouchPlaneDescription = "Touch plane";
        private const string LeftReleasePlaneDescription = "Left Release Plane";
        private const string RightReleasePlaneDescription = "Right Release Plane";
        private const string TopReleasePlaneDescription = "Top release Plane";
        private const string BottomReleasePlaneDescription = "Bottom Release Plane";
        private const string BackReleasePlaneDescription = "Back release Plane";
        private const string FrontReleasePlaneDescription = "Front Release Plane";

        private void OnEnable()
        {
            sorting = serializedObject.FindProperty("sortType");
            cellHeight = serializedObject.FindProperty("cellHeight");
            cellWidth = serializedObject.FindProperty("cellWidth");

            itemsPerTier = serializedObject.FindProperty("itemsPerTier");
            canScroll = serializedObject.FindProperty("canScroll");
            scrollDirection = serializedObject.FindProperty("scrollDirection");
            viewableArea = serializedObject.FindProperty("viewableArea");
            setupAtRuntime = serializedObject.FindProperty("setupAtRuntime");
            useCameraPreRender = serializedObject.FindProperty("useOnPreRender");

            occlusionPositionPadding = serializedObject.FindProperty("occlusionPositionPadding");
            occlusionScalePadding = serializedObject.FindProperty("occlusionScalePadding");

            handDeltaMagThreshold = serializedObject.FindProperty("handDeltaMagThreshold");

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

            // Serialized properties for visualization
            nodeList = serializedObject.FindProperty("nodeList");
            releaseThresholdFront = serializedObject.FindProperty("releaseThresholdFront");
            releaseThresholdBack = serializedObject.FindProperty("releaseThresholdBack");
            releaseThresholdLeftRight = serializedObject.FindProperty("releaseThresholdLeftRight");
            releaseThresholdTopBottom = serializedObject.FindProperty("releaseThresholdTopBottom");
            frontPlaneDistance = serializedObject.FindProperty("frontPlaneDistance");

            scrollView = (ScrollingObjectCollection)target;

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.white;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            ScrollingObjectCollection scrollContainer = (ScrollingObjectCollection)target;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(canScroll);
                EditorGUILayout.PropertyField(scrollDirection);
                EditorGUILayout.PropertyField(setupAtRuntime);
                EditorGUILayout.PropertyField(useCameraPreRender);
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Collection Properties", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(sorting);
                EditorGUILayout.PropertyField(cellWidth);
                EditorGUILayout.PropertyField(cellHeight);
                EditorGUILayout.PropertyField(itemsPerTier);
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

            EditorGUILayout.LabelField("Scrolling Properties", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(handDeltaMagThreshold);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(occlusionPositionPadding);
                EditorGUILayout.PropertyField(occlusionScalePadding);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(animationCurve);
                EditorGUILayout.PropertyField(animationLength);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(releaseThresholdFront);
                EditorGUILayout.PropertyField(releaseThresholdBack);
                EditorGUILayout.PropertyField(releaseThresholdLeftRight);
                EditorGUILayout.PropertyField(releaseThresholdTopBottom);
                EditorGUILayout.PropertyField(frontPlaneDistance);
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

            showUnityEvents = EditorGUILayout.Foldout(showUnityEvents, "Unity Events", true);

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

            showDebugOptions = EditorGUILayout.Foldout(showDebugOptions, "Debug Options", true);

            if (showDebugOptions)
            {
                using (new EditorGUI.DisabledGroupScope(EditorApplication.isPlaying))
                {
                    visibleDebugPlanes = EditorGUILayout.Toggle("Show Event Planes", visibleDebugPlanes);
                }

                EditorGUILayout.Space();

                using (new EditorGUI.DisabledGroupScope(!EditorApplication.isPlaying))
                {

                    using (new EditorGUI.IndentLevelScope())
                    {
                        animateTransition = EditorGUILayout.Toggle("Animate", animateTransition);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            amountOfPagesToMove = EditorGUILayout.IntField(amountOfPagesToMove);
                            if (GUILayout.Button("Move By Pages"))
                            {
                                scrollContainer.MoveByPages(amountOfPagesToMove, animateTransition);
                            }
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            amountOfTiersToMove = EditorGUILayout.IntField(amountOfTiersToMove);
                            if (GUILayout.Button("Move By Tiers"))
                            {
                                scrollContainer.MoveByTiers(amountOfTiersToMove, animateTransition);
                            }
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            indexToMoveTo = EditorGUILayout.IntField(indexToMoveTo);
                            if (GUILayout.Button("Move To Index"))
                            {
                                scrollContainer.MoveToIndex(indexToMoveTo, animateTransition);
                            }
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
            MRTKtmp = Shader.Find("Mixed Reality Toolkit/TextMeshPro");

            if (scrollView.ClippingObject == null) { return; }

            if (Event.current.type == EventType.Repaint)
            {
                if (visibleDebugPlanes)
                {
                    GetMouseViewportPosition();

                    smallestMouseHandleDistance = float.PositiveInfinity;

                    DrawAllDebugPlanes();
                }

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
        /// Draws the touch plane used for scroll engage detection.
        /// </summary>
        private void DrawTouchPlane()
        {
            Vector3 planeLocalPosition = Vector3.forward * frontPlaneDistance.floatValue * -1.0f;
            Vector3 widthHalfExtent = scrollView.transform.right * scrollView.ClippingObject.transform.lossyScale.x / 2;
            Vector3 heightHalfExtent = scrollView.transform.up * scrollView.ClippingObject.transform.lossyScale.y / 2;

            Quaternion handleRotation = Quaternion.LookRotation(scrollView.transform.forward * -1.0f);

            DrawPlaneAndHandle(planeLocalPosition, widthHalfExtent, heightHalfExtent, handleRotation, touchPlaneColor, TouchPlaneDescription);
        }

        /// <summary>
        /// Draws the front release plane used for scroll engage release detection.
        /// </summary>
        private void DrawFrontReleasePlane()
        {
            Vector3 planeLocalPosition = Vector3.forward * releaseThresholdFront.floatValue * -1.0f;
            Vector3 widthHalfExtent = scrollView.transform.right * scrollView.ClippingObject.transform.lossyScale.x / 2;
            Vector3 heightHalfExtent = scrollView.transform.up * scrollView.ClippingObject.transform.lossyScale.y / 2;

            Quaternion handleRotation = Quaternion.LookRotation(scrollView.transform.forward);

            DrawPlaneAndHandle(planeLocalPosition, widthHalfExtent, heightHalfExtent, handleRotation, releasePlaneColor, FrontReleasePlaneDescription);
        }

        /// <summary>
        /// Draws the back release plane used for scroll engage release detection.
        /// </summary>
        private void DrawBackReleasePlane()
        {
            Vector3 planeLocalPosition = Vector3.forward * releaseThresholdBack.floatValue;
            Vector3 widthHalfExtent = scrollView.transform.right * scrollView.ClippingObject.transform.lossyScale.x / 2;
            Vector3 heightHalfExtent = scrollView.transform.up * scrollView.ClippingObject.transform.lossyScale.y / 2;

            Quaternion handleRotation = Quaternion.LookRotation(scrollView.transform.forward * -1.0f);

            DrawPlaneAndHandle(planeLocalPosition, widthHalfExtent, heightHalfExtent, handleRotation, releasePlaneColor, BackReleasePlaneDescription);
        }

        /// <summary>
        /// Draws the right release plane used for scroll engage release detection.
        /// </summary>
        private void DrawRightReleasePlane()
        {
            Vector3 planeLocalPosition = Vector3.right * releaseThresholdLeftRight.floatValue;
            Vector3 widthHalfExtent = scrollView.transform.forward * scrollView.ClippingObject.transform.lossyScale.z / 2;
            Vector3 heightHalfExtent = scrollView.transform.up * scrollView.ClippingObject.transform.lossyScale.y / 2;

            Quaternion handleRotation = Quaternion.LookRotation(scrollView.transform.right * -1.0f);

            DrawPlaneAndHandle(planeLocalPosition, widthHalfExtent, heightHalfExtent, handleRotation, releasePlaneColor, RightReleasePlaneDescription);
        }

        /// <summary>
        /// Draws the left release plane used for scroll engage release detection.
        /// </summary>
        private void DrawLeftReleasePlane()
        {
            Vector3 planeLocalPosition = Vector3.right * releaseThresholdLeftRight.floatValue * -1.0f;
            Vector3 widthHalfExtent = scrollView.transform.forward * scrollView.ClippingObject.transform.lossyScale.z / 2;
            Vector3 heightHalfExtent = scrollView.transform.up * scrollView.ClippingObject.transform.lossyScale.y / 2;

            Quaternion handleRotation = Quaternion.LookRotation(scrollView.transform.right);

            DrawPlaneAndHandle(planeLocalPosition, widthHalfExtent, heightHalfExtent, handleRotation, releasePlaneColor, LeftReleasePlaneDescription);
        }

        /// <summary>
        /// Draws the top release plane used for scroll engage release detection.
        /// </summary>
        private void DrawTopReleasePlane()
        {
            Vector3 planeLocalPosition = Vector3.up * releaseThresholdTopBottom.floatValue;
            Vector3 widthHalfExtent = scrollView.transform.right * scrollView.ClippingObject.transform.lossyScale.x / 2;
            Vector3 heightHalfExtent = scrollView.transform.forward * scrollView.ClippingObject.transform.lossyScale.z / 2;

            Quaternion handleRotation = Quaternion.LookRotation(scrollView.transform.up * -1.0f);

            DrawPlaneAndHandle(planeLocalPosition, widthHalfExtent, heightHalfExtent, handleRotation, releasePlaneColor, TopReleasePlaneDescription);
        }

        /// <summary>
        /// Draws the top release plane used for scroll engage release detection.
        /// </summary>
        private void DrawBottomReleasePlane()
        {
            Vector3 planeLocalPosition = Vector3.up * releaseThresholdTopBottom.floatValue * -1.0f;
            Vector3 widthHalfExtent = scrollView.transform.right * scrollView.ClippingObject.transform.lossyScale.x / 2;
            Vector3 heightHalfExtent = scrollView.transform.forward * scrollView.ClippingObject.transform.lossyScale.z / 2;

            Quaternion handleRotation = Quaternion.LookRotation(scrollView.transform.up);

            DrawPlaneAndHandle(planeLocalPosition, widthHalfExtent, heightHalfExtent, handleRotation, releasePlaneColor, BottomReleasePlaneDescription);
        }

        /// <summary>
        /// Draws the scroll interaction debug planes.
        /// </summary>
        private void DrawPlaneAndHandle(Vector3 planeLocalPosition, Vector3 widthHalfExtent, Vector3 hightHalfExtent, Quaternion handleRotation, Color color, string labelText)
        {
            Vector3 planePosition = scrollView.ClippingObject.transform.position + scrollView.ClippingObject.transform.TransformDirection(planeLocalPosition);

            Vector3[] points = new Vector3[4];
            points[0] = planePosition - widthHalfExtent + hightHalfExtent;
            points[1] = planePosition + widthHalfExtent + hightHalfExtent;
            points[2] = planePosition + widthHalfExtent - hightHalfExtent;
            points[3] = planePosition - widthHalfExtent - hightHalfExtent;

            float handleSize = HandleUtility.GetHandleSize(planePosition) * 0.30f;

            // Draw handle and plane
            Handles.color = color;
            Handles.ArrowHandleCap(0, planePosition, handleRotation, handleSize, EventType.Repaint);
            Handles.DrawSolidRectangleWithOutline(points, Color.Lerp(color, Color.clear, 0.5f), color);

            // Draw label if handle has smallest vieport distance from the mouse pointer
            Vector3 handleViewportPosition = SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(planePosition);
            handleViewportPosition = SceneView.currentDrawingSceneView.camera.ScreenToViewportPoint(handleViewportPosition);

            float mouseHandleDistance = Vector2.Distance(mouseScreenPosition, handleViewportPosition);

            if (mouseHandleDistance < handleSize * 10.0f && mouseHandleDistance < smallestMouseHandleDistance)
            {
                smallestMouseHandleDistance = mouseHandleDistance;

                Handles.Label(planePosition + (Vector3.up * handleSize * 3.0f), new GUIContent(labelText), labelStyle);
                Handles.DrawDottedLine(planePosition + (Vector3.up * handleSize * 2.0f) + (Vector3.right * handleSize), planePosition, 5f);
            }
        }

        private void DrawAllDebugPlanes()
        {
            DrawTouchPlane();
            DrawFrontReleasePlane();
            DrawBackReleasePlane();
            DrawRightReleasePlane();
            DrawLeftReleasePlane();
            DrawTopReleasePlane();
            DrawBottomReleasePlane();
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

        private void GetMouseViewportPosition()
        {
            mouseScreenPosition = HandleUtility.GUIPointToScreenPixelCoordinate(Event.current.mousePosition);
            mouseScreenPosition = SceneView.currentDrawingSceneView.camera.ScreenToViewportPoint(mouseScreenPosition);
        }
    }
}