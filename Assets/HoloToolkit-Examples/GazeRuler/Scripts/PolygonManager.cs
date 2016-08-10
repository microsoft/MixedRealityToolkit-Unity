using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;


/// <summary>
/// manager all geometries in the scene
/// </summary>
public class PolygonManager : Singleton<PolygonManager>, IGeometry, IPolygonClosable
{
    // save all geometries
    public Stack<Ploygon> Ploygons = new Stack<Ploygon>();
    public Ploygon CurrentPloygon;

    /// <summary>
    ///  handle new point users place
    /// </summary>
    /// <param name="LinePrefab"></param>
    /// <param name="PointPrefab"></param>
    /// <param name="TextPrefab"></param>
    public void AddPoint(GameObject LinePrefab, GameObject PointPrefab, GameObject TextPrefab)
    {
        var hitPoint = GazeManager.Instance.HitInfo.point;
        var point = (GameObject)Instantiate(PointPrefab, hitPoint, Quaternion.identity);
        var newPoint = new Point
        {
            Position = hitPoint,
            Root = point
        };
        if (CurrentPloygon.IsFinished)
        {
            CurrentPloygon = new Ploygon()
            {
                IsFinished = false,
                Root = new GameObject(),
                Points = new List<Vector3>()
            };

            CurrentPloygon.Points.Add(newPoint.Position);
            newPoint.Root.transform.parent = CurrentPloygon.Root.transform;
        }
        else
        {
            CurrentPloygon.Points.Add(newPoint.Position);
            newPoint.Root.transform.parent = CurrentPloygon.Root.transform;
            if (CurrentPloygon.Points.Count > 1)
            {
                var index = CurrentPloygon.Points.Count - 1;
                var centerPos = (CurrentPloygon.Points[index] + CurrentPloygon.Points[index - 1]) * 0.5f;
                var direction = CurrentPloygon.Points[index] - CurrentPloygon.Points[index - 1];
                var distance = Vector3.Distance(CurrentPloygon.Points[index], CurrentPloygon.Points[index - 1]);
                var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
                line.transform.localScale = new Vector3(distance, 0.005f, 0.005f);
                line.transform.Rotate(Vector3.down, 90f);
                line.transform.parent = CurrentPloygon.Root.transform;
            }

        }

    }

    /// <summary>
    /// finish current geometry
    /// </summary>
    /// <param name="LinePrefab"></param>
    /// <param name="TextPrefab"></param>
    public void ClosePloygon(GameObject LinePrefab, GameObject TextPrefab)
    {
        if (CurrentPloygon != null)
        {
            CurrentPloygon.IsFinished = true;
            var area = CalculatePloygonArea(CurrentPloygon);
            var index = CurrentPloygon.Points.Count - 1;
            var centerPos = (CurrentPloygon.Points[index] + CurrentPloygon.Points[0]) * 0.5f;
            var direction = CurrentPloygon.Points[index] - CurrentPloygon.Points[0];
            var distance = Vector3.Distance(CurrentPloygon.Points[index], CurrentPloygon.Points[0]);
            var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
            line.transform.localScale = new Vector3(distance, 0.005f, 0.005f);
            line.transform.Rotate(Vector3.down, 90f);
            line.transform.parent = CurrentPloygon.Root.transform;

            var vect = new Vector3(0, 0, 0);
            foreach (var point in CurrentPloygon.Points)
            {
                vect += point;
            }
            var centerPoint = vect / (index + 1);
            var direction1 = CurrentPloygon.Points[1] - CurrentPloygon.Points[0];
            var directionF = Vector3.Cross(direction, direction1);
            var tip = (GameObject)Instantiate(TextPrefab, centerPoint, Quaternion.LookRotation(directionF));//anchor.x + anchor.y + anchor.z < 0 ? -1 * anchor : anchor));

            // unit is ㎡
            tip.GetComponent<TextMesh>().text = area + "㎡";
            tip.transform.parent = CurrentPloygon.Root.transform;
            Ploygons.Push(CurrentPloygon);
        }
    }

    /// <summary>
    /// clear all geometries in the scene
    /// </summary>
    public void Clear()
    {
        if (Ploygons != null && Ploygons.Count > 0)
        {
            while (Ploygons.Count > 0)
            {
                var lastLine = Ploygons.Pop();
                Destroy(lastLine.Root);
            }
        }
    }

    // delete latest geometry
    public void Delete()
    {
        if (Ploygons != null && Ploygons.Count > 0)
        {
            var lastLine = Ploygons.Pop();
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
    float CalculateTriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
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
    /// <param name="ploygon"></param>
    /// <returns></returns>
    float CalculatePloygonArea(Ploygon ploygon)
    {
        var s = 0.0f;
        var i = 1;
        var n = ploygon.Points.Count;
        for (; i < n - 1; i++)
            s += CalculateTriangleArea(ploygon.Points[0], ploygon.Points[i], ploygon.Points[i + 1]);
        return 0.5f * Mathf.Abs(s);
    }

    // Use this for initialization
    void Start()
    {
        CurrentPloygon = new Ploygon()
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
        if (CurrentPloygon != null && !CurrentPloygon.IsFinished)
        {
            Destroy(CurrentPloygon.Root);
            CurrentPloygon = new Ploygon()
            {
                IsFinished = false,
                Root = new GameObject(),
                Points = new List<Vector3>()
            };
        }
    }
}


public class Ploygon
{
    public float Area { get; set; }

    public List<Vector3> Points { get; set; }

    public GameObject Root { get; set; }

    public bool IsFinished { get; set; }

}

