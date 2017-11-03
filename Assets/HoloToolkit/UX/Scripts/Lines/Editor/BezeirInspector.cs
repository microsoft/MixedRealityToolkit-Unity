//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEditor;
using UnityEngine;

namespace MRTK.UX
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Bezeir))]
    public class BezeirInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
        }

        void OnSceneGUI()
        {
            Bezeir b = (Bezeir)target;

            Handles.color = Color.cyan;

            Vector3 start = b.GetPoint(0);
            Vector3 mid1 = b.GetPoint(1);
            Vector3 mid2 = b.GetPoint(2);
            Vector3 end = b.GetPoint(3);

            mid1 = Handles.FreeMoveHandle(mid1, Quaternion.identity, 0.02f, Vector3.zero, Handles.CircleHandleCap);
            Handles.Label(mid1, "Mid 1");

            mid2 = Handles.FreeMoveHandle(mid2, Quaternion.identity, 0.02f, Vector3.zero, Handles.CircleHandleCap);
            Handles.Label(mid2, "Mid 2");

            start = Handles.FreeMoveHandle(start, Quaternion.identity, 0.05f, Vector3.zero, Handles.SphereHandleCap);
            Handles.Label(start, "Start");

            end = Handles.FreeMoveHandle(end, Quaternion.identity, 0.05f, Vector3.zero, Handles.SphereHandleCap);
            Handles.Label(end, "End"); 

            b.SetPoint(0, start);
            b.SetPoint(1, mid1);
            b.SetPoint(2, mid2);
            b.SetPoint(3, end);

            Handles.color = Color.white;
            Vector3 lastPos = b.GetPoint(0f);
            Vector3 currentPos = Vector3.zero;

            for (int i = 1; i < 10; i++)
            {
                float normalizedDistance = (1f / (10 - 1)) * i;
                currentPos = b.GetPoint(normalizedDistance);
                Handles.DrawDottedLine(lastPos, currentPos, 5f);
                lastPos = currentPos;
            }
        }
    }
}
