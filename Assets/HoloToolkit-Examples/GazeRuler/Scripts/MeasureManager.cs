using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System;

/// <summary>
/// manager all measure tools here
/// </summary>
public class MeasureManager : Singleton<MeasureManager>
{
    private IGeometry manager;
    public GeometryMode mode;

    // set up prefabs
    public GameObject LinePrefab;
    public GameObject PointPrefab;
    public GameObject ModeTipObject;
    public GameObject TextPrefab;

    void Start()
    {
        // inti measure mode
        switch (mode)
        {
            case GeometryMode.Ploygon:
                manager = PolygonManager.Instance;
                break;
            default:
                manager = LineManager.Instance;
                break;
        }
    }

    // place spatial point
    public void OnSelect()
    {
        manager.AddPoint(LinePrefab, PointPrefab, TextPrefab);
    }

    // delete latest line or geometry
    public void DeleteLine()
    {
        manager.Delete();
    }

    // delete all lines or geometry
    public void ClearAll()
    {
        manager.Clear();
    }

    // if current mode is geometry mode, try to finish geometry
    public void OnPloygonClose()
    {
        IPolygonClosable client = PolygonManager.Instance;
        client.ClosePloygon(LinePrefab, TextPrefab);
    }

    // change measure mode
    public void OnModeChange()
    {
        try
        {
            manager.Reset();
            if (mode == GeometryMode.Line)
            {
                mode = GeometryMode.Ploygon;
                manager = PolygonManager.Instance;
            }
            else
            {
                mode = GeometryMode.Line;
                manager = LineManager.Instance;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        ModeTipObject.SetActive(true);
    }
}

public class Point
{
    public Vector3 Position { get; set; }

    public GameObject Root { get; set; }
    public bool IsStart { get; set; }
}


public enum GeometryMode
{
    Line,
    Triangle,
    Rectangle,
    Cube,
    Ploygon
}