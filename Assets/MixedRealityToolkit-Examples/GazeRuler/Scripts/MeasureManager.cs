// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.InputModule;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using System;
using UnityEngine;

namespace MixedRealityToolkit.Examples.GazeRuler
{
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

        private void Start()
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
            client.ClosePolygon(LinePrefab, TextPrefab);
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

    public void OnInputClicked(InputClickedEventData eventData)
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
}
