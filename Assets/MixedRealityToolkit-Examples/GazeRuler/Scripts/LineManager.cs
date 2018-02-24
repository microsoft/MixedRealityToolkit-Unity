// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.InputModule.Gaze;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.Examples.GazeRuler
{
    /// <summary>
    /// Manages all lines in the scene
    /// </summary>
    public class LineManager : Singleton<LineManager>, IGeometry
    {
        // save all lines in scene
        private Stack<Line> Lines = new Stack<Line>();

        private Point lastPoint;

        private const float defaultLineScale = 0.005f;

        // place point and lines
        public void AddPoint(GameObject LinePrefab, GameObject PointPrefab, GameObject TextPrefab)
        {

            Vector3 hitPoint = GazeManager.Instance.HitPosition;

            GameObject point = (GameObject)Instantiate(PointPrefab, hitPoint, Quaternion.identity);
            if (lastPoint != null && lastPoint.IsStart)
            {
                Vector3 centerPos = (lastPoint.Position + hitPoint) * 0.5f;
                Vector3 cameraPosition = CameraCache.Main.transform.position;
                Vector3 directionFromCamera = centerPos - cameraPosition;

                float distanceA = Vector3.Distance(lastPoint.Position, cameraPosition);
                float distanceB = Vector3.Distance(hitPoint, cameraPosition);

                Debug.Log("A: " + distanceA + ",B: " + distanceB);
                Vector3 direction;
                if (distanceB > distanceA || (distanceA > distanceB && distanceA - distanceB < 0.1))
                {
                    direction = hitPoint - lastPoint.Position;
                }
                else
                {
                    direction = lastPoint.Position - hitPoint;
                }

                float distance = Vector3.Distance(lastPoint.Position, hitPoint);
                GameObject line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
                line.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
                line.transform.Rotate(Vector3.down, 90f);

                Vector3 normalV = Vector3.Cross(direction, directionFromCamera);
                Vector3 normalF = Vector3.Cross(direction, normalV) * -1;
                GameObject tip = (GameObject)Instantiate(TextPrefab, centerPos, Quaternion.LookRotation(normalF));

                //unit is meter
                tip.transform.Translate(Vector3.up * 0.05f);
                tip.GetComponent<TextMesh>().text = distance + "m";

                GameObject root = new GameObject();
                lastPoint.Root.transform.parent = root.transform;
                line.transform.parent = root.transform;
                point.transform.parent = root.transform;
                tip.transform.parent = root.transform;

                Lines.Push(new Line
                {
                    Start = lastPoint.Position,
                    End = hitPoint,
                    Root = root,
                    Distance = distance
                });

                lastPoint = new Point
                {
                    Position = hitPoint,
                    Root = point,
                    IsStart = false
                };

            }
            else
            {
                lastPoint = new Point
                {
                    Position = hitPoint,
                    Root = point,
                    IsStart = true
                };
            }
        }


        // delete latest placed lines
        public void Delete()
        {
            if (Lines != null && Lines.Count > 0)
            {
                Line lastLine = Lines.Pop();
                Destroy(lastLine.Root);
            }

        }

        // delete all lines in the scene
        public void Clear()
        {
            if (Lines != null && Lines.Count > 0)
            {
                while (Lines.Count > 0)
                {
                    Line lastLine = Lines.Pop();
                    Destroy(lastLine.Root);
                }
            }
        }

        // reset current unfinished line
        public void Reset()
        {
            if (lastPoint != null && lastPoint.IsStart)
            {
                Destroy(lastPoint.Root);
                lastPoint = null;
            }
        }
    }


    public struct Line
    {
        public Vector3 Start { get; set; }

        public Vector3 End { get; set; }

        public GameObject Root { get; set; }

        public float Distance { get; set; }
    }
}