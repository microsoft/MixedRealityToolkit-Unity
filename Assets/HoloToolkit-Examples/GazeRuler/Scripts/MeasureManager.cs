using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// manager all measure tools here
/// </summary>
public class MeasureManager : Singleton<MeasureManager>, IHoldHandler, IInputClickHandler
{
    private IGeometry manager;
    public GeometryMode Mode;

    // set up prefabs
    public GameObject LinePrefab;
    public GameObject PointPrefab;
    public GameObject ModeTipObject;
    public GameObject TextPrefab;

    void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);

        // inti measure mode
        switch (Mode)
        {
            case GeometryMode.Polygon:
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
    public void OnPolygonClose()
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
            if (Mode == GeometryMode.Line)
            {
                Mode = GeometryMode.Polygon;
                manager = PolygonManager.Instance;
            }
            else
            {
                Mode = GeometryMode.Line;
                manager = LineManager.Instance;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        ModeTipObject.SetActive(true);
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
        OnPolygonClose();
    }

    public void OnHoldCompleted(HoldEventData eventData)
    {
        // Nothing to do
    }

    public void OnHoldCanceled(HoldEventData eventData)
    {
        // Nothing to do
    }

    public void OnInputClicked(InputEventData eventData)
    {
        OnSelect();
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
    Polygon
}