// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Examples.GazeRuler
{
    /// <summary>
    /// manager all geometries in the scene
    /// </summary>
    public class PolygonManager : Singleton<PolygonManager>, IGeometry, IPolygonClosable
    {
        // save all geometries
        public Stack<Polygon> Polygons = new Stack<Polygon>();
        public Polygon CurrentPolygon;

        /// <summary>
        ///  handle new point users place
        /// </summary>
        /// <param name="LinePrefab"></param>
        /// <param name="PointPrefab"></param>
        /// <param name="TextPrefab"></param>
        public void AddPoint(GameObject LinePrefab, GameObject PointPrefab, GameObject TextPrefab)
        {
            var hitPoint = GazeManager.Instance.HitPosition;
            var point = (GameObject)Instantiate(PointPrefab, hitPoint, Quaternion.identity);
            var newPoint = new Point
            {
                Position = hitPoint,
                Root = point
            };
            if (CurrentPolygon.IsFinished)
            {
                CurrentPolygon = new Polygon()
                {
                    IsFinished = false,
                    Root = new GameObject(),
                    Points = new List<Vector3>()
                };

                CurrentPolygon.Points.Add(newPoint.Position);
                newPoint.Root.transform.parent = CurrentPolygon.Root.transform;
            }
            else
            {
                CurrentPolygon.Points.Add(newPoint.Position);
                newPoint.Root.transform.parent = CurrentPolygon.Root.transform;
                if (CurrentPolygon.Points.Count > 1)
                {
                    var index = CurrentPolygon.Points.Count - 1;
                    var centerPos = (CurrentPolygon.Points[index] + CurrentPolygon.Points[index - 1]) * 0.5f;
                    var direction = CurrentPolygon.Points[index] - CurrentPolygon.Points[index - 1];
                    var distance = Vector3.Distance(CurrentPolygon.Points[index], CurrentPolygon.Points[index - 1]);
                    var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
                    line.transform.localScale = new Vector3(distance, 0.005f, 0.005f);
                    line.transform.Rotate(Vector3.down, 90f);
                    line.transform.parent = CurrentPolygon.Root.transform;
                }

            }

        }

        /// <summary>
        /// finish current geometry
        /// </summary>
        /// <param name="LinePrefab"></param>
        /// <param name="TextPrefab"></param>
        public void ClosePolygon(GameObject LinePrefab, GameObject TextPrefab)
        {
            if (CurrentPolygon != null)
            {
                CurrentPolygon.IsFinished = true;
                var area = CalculatePolygonArea(CurrentPolygon);
                var index = CurrentPolygon.Points.Count - 1;
                var centerPos = (CurrentPolygon.Points[index] + CurrentPolygon.Points[0]) * 0.5f;
                var direction = CurrentPolygon.Points[index] - CurrentPolygon.Points[0];
                var distance = Vector3.Distance(CurrentPolygon.Points[index], CurrentPolygon.Points[0]);
                var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
                line.transform.localScale = new Vector3(distance, 0.005f, 0.005f);
                line.transform.Rotate(Vector3.down, 90f);
                line.transform.parent = CurrentPolygon.Root.transform;

                var vect = new Vector3(0, 0, 0);
                foreach (var point in CurrentPolygon.Points)
                {
                    vect += point;
                }
                var centerPoint = vect / (index + 1);
                var direction1 = CurrentPolygon.Points[1] - CurrentPolygon.Points[0];
                var directionF = Vector3.Cross(direction, direction1);
                var tip = (GameObject)Instantiate(TextPrefab, centerPoint, Quaternion.LookRotation(directionF));//anchor.x + anchor.y + anchor.z < 0 ? -1 * anchor : anchor));

                // unit is ㎡
                tip.GetComponent<TextMesh>().text = area + "㎡";
                tip.transform.parent = CurrentPolygon.Root.transform;
                Polygons.Push(CurrentPolygon);
            }
        }

        /// <summary>
        /// clear all geometries in the scene
        /// </summary>
        public void Clear()
        {
            if (Polygons != null && Polygons.Count > 0)
            {
                while (Polygons.Count > 0)
                {
                    var lastLine = Polygons.Pop();
                    Destroy(lastLine.Root);
                }
            }
        }

        // delete latest geometry
        public void Delete()
        {
            if (Polygons != null && Polygons.Count > 0)
            {
                var lastLine = Polygons.Pop();
                Destroy(lastLine.Root);
            }
        }

        /// <summary>
        /// Calculate an area of triangle
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        private float CalculateTriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var a = Vector3.Distance(p1, p2);
            var b = Vector3.Distance(p1, p3);
            var c = Vector3.Distance(p3, p2);
            var p = (a + b + c) / 2f;
            var s = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));

            return s;
        }
        /// <summary>
        /// Calculate an area of geometry
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private float CalculatePolygonArea(Polygon polygon)
        {
            var s = 0.0f;
            var i = 1;
            var n = polygon.Points.Count;
            for (; i < n - 1; i++)
                s += CalculateTriangleArea(polygon.Points[0], polygon.Points[i], polygon.Points[i + 1]);
            return 0.5f * Mathf.Abs(s);
        }

        // Use this for initialization
        private void Start()
        {
            CurrentPolygon = new Polygon()
            {
                IsFinished = false,
                Root = new GameObject(),
                Points = new List<Vector3>()
            };
        }


        /// <summary>
        /// reset current unfinished geometry
        /// </summary>
        public void Reset()
        {
            if (CurrentPolygon != null && !CurrentPolygon.IsFinished)
            {
                Destroy(CurrentPolygon.Root);
                CurrentPolygon = new Polygon()
                {
                    IsFinished = false,
                    Root = new GameObject(),
                    Points = new List<Vector3>()
                };
            }
        }
    }


    public class Polygon
    {
        public float Area { get; set; }

        public List<Vector3> Points { get; set; }

        public GameObject Root { get; set; }

        public bool IsFinished { get; set; }

    }
}