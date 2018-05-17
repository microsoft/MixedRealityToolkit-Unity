using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Collections.Generic;

namespace ARKit.Examples
{
    public class UnityPointCloudExample : MonoBehaviour
    {
        public uint numPointsToShow = 100;
        public GameObject PointCloudPrefab = null;
        private List<GameObject> pointCloudObjects;
        private Vector3[] m_PointCloudData;

        private void Start()
        {
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
            if (PointCloudPrefab != null)
            {
                pointCloudObjects = new List<GameObject>();
                for (int i = 0; i < numPointsToShow; i++)
                {
                    pointCloudObjects.Add(Instantiate(PointCloudPrefab));
                }
            }
        }

        public void ARFrameUpdated(UnityARCamera camera)
        {
            m_PointCloudData = camera.pointCloudData;
        }

        private void Update()
        {
            if (PointCloudPrefab != null && m_PointCloudData != null)
            {
                for (int count = 0; count < Math.Min(m_PointCloudData.Length, numPointsToShow); count++)
                {
                    Vector4 vert = m_PointCloudData[count];
                    GameObject point = pointCloudObjects[count];
                    point.transform.position = new Vector3(vert.x, vert.y, vert.z);
                }
            }
        }
    }
}