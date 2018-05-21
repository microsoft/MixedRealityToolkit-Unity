using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UnityPointCloudExample : MonoBehaviour
{
    public uint numPointsToShow = 100;
    public GameObject PointCloudPrefab = null;
    private List<GameObject> pointCloudObjects;
    private Vector3[] m_PointCloudData;

    public void Start()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
        if (PointCloudPrefab != null)
        {
            pointCloudObjects = new List<GameObject> ();
            for (int i =0; i < numPointsToShow; i++)
            {
                pointCloudObjects.Add (Instantiate (PointCloudPrefab));
            }
        }
    }

    public void ARFrameUpdated(UnityARCamera camera)
    {
        m_PointCloudData = camera.pointCloudData;
    }

    public void Update()
    {
        if (PointCloudPrefab != null && m_PointCloudData != null)
        {
            for (int count = 0; count < Math.Min (m_PointCloudData.Length, numPointsToShow); count++)
            {
                Vector4 vert = m_PointCloudData [count];
                GameObject point = pointCloudObjects [count];
                point.transform.position = new Vector3(vert.x, vert.y, vert.z);
            }
        }
    }
}