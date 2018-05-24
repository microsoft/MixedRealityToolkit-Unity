using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxHelper : MonoBehaviour
{
    private List<Vector3> rawBoundingCorners = new List<Vector3>();
    private List<Vector3> worldBoundingCorners = new List<Vector3>();
    private GameObject targetObject;
    private bool rawBoundingCornersObtained = false;

    public void UpdateNonAABoundingBoxCornerPositions(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
    {
        if (target != targetObject || rawBoundingCornersObtained == false)
        {
            GetRawBBCorners(target, ignoreLayers);
        }

        if (target == targetObject && rawBoundingCornersObtained)
        {
            boundsPoints.Clear();
            for (int i = 0; i < rawBoundingCorners.Count; ++i)
            {
                boundsPoints.Add( target.transform.localToWorldMatrix.MultiplyPoint(rawBoundingCorners[i]) );
            }

            worldBoundingCorners.Clear();
            worldBoundingCorners.AddRange(boundsPoints);
        }
    }

    public void GetRawBBCorners(GameObject target, LayerMask ignoreLayers)
    {
        targetObject = target;

        GameObject clone = GameObject.Instantiate(targetObject);
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.position = Vector3.zero;
        clone.transform.localScale = new Vector3(1, 1, 1);
        Renderer[] renderers = clone.GetComponentsInChildren<Renderer>();
        rawBoundingCorners.Clear();
        rawBoundingCornersObtained = false;
        for (int i = 0; i < renderers.Length; ++i)
        {
            var rendererObj = renderers[i];
            if (ignoreLayers == (1 << rendererObj.gameObject.layer | ignoreLayers))
            {
                continue;
            }
            Vector3[] corners = null;
            rendererObj.bounds.GetCornerPositionsFromRendererBounds(ref corners);
            rawBoundingCorners.AddRange(corners);
        }

        GameObject.Destroy(clone);

        if (rawBoundingCorners != null && rawBoundingCorners.Count >= 4)
        {
            rawBoundingCornersObtained = true;
        }
    }

    public static void GetNonAABoundingBoxCornerPositions(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
    {
        GameObject clone = GameObject.Instantiate(target);
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.position = Vector3.zero;
        clone.transform.localScale = new Vector3(1, 1, 1);
        Renderer[] renderers = clone.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; ++i)
        {
            var rendererObj = renderers[i];
            if (ignoreLayers == (1 << rendererObj.gameObject.layer | ignoreLayers))
            {
                continue;
            }
            Vector3[] corners = null;
            rendererObj.bounds.GetCornerPositionsFromRendererBounds(ref corners);
            boundsPoints.AddRange(corners);
        }

        for (int i = 0; i < boundsPoints.Count; ++i)
        {
            boundsPoints[i] = target.transform.localToWorldMatrix.MultiplyPoint(boundsPoints[i]);
        }
        GameObject.Destroy(clone);
    }

    public int[] GetFaceIndices(int index)
    {
        switch (index)
        {
            case 0:
                return new int[] { 0, 1, 3, 2 };
            case 1:
                return new int[] { 1, 5, 7, 3 };
            case 2:
                return new int[] { 5, 4, 6, 7 };
            case 3:
                return new int[] { 4, 0, 2, 6 };
            case 4:
                return new int[] { 6, 2, 3, 7 };
            case 5:
                return new int[] { 1, 0, 4, 5};
        }

        return new int[0];
    }

    public Vector3[] GetFaceEdgeMidpoints(int index)
    {
        Vector3[] corners = GetFaceCorners(index);
        Vector3[] midpoints = new Vector3[4];
        midpoints[0] = (corners[0] + corners[1]) * 0.5f;
        midpoints[1] = (corners[1] + corners[2]) * 0.5f;
        midpoints[2] = (corners[2] + corners[3]) * 0.5f;
        midpoints[3] = (corners[3] + corners[0]) * 0.5f;

        return midpoints;
    }
    public Vector3 GetFaceNormal(int index)
    {
        int[] face = GetFaceIndices(index);

        if (face.Length == 4)
        {
            Vector3 ab = (worldBoundingCorners[face[1]] - worldBoundingCorners[face[0]]).normalized;
            Vector3 ac = (worldBoundingCorners[face[2]] - worldBoundingCorners[face[0]]).normalized;
            return Vector3.Cross(ab, ac).normalized;
        }

        return Vector3.zero;
    }
    public Vector3 GetFaceCentroid(int index)
    {
        int[] faceIndices = GetFaceIndices(index);

        if (faceIndices.Length == 4)
        {
            return (worldBoundingCorners[faceIndices[0]] + 
                    worldBoundingCorners[faceIndices[1]] + 
                    worldBoundingCorners[faceIndices[2]] + 
                    worldBoundingCorners[faceIndices[3]]) * 0.25f;
        }

        return Vector3.zero;
    }

    public Vector3 GetFaceBottomCentroid(int index)
    {
        Vector3[] edgeCentroids = GetFaceEdgeMidpoints(index);

        Vector3 leastYPoint = edgeCentroids[0];
        for (int i = 1; i < 4; ++i)
        {
            leastYPoint = edgeCentroids[i].y < leastYPoint.y ? edgeCentroids[i] : leastYPoint;
        }
        return leastYPoint;
    }

 

    public Vector3[] GetFaceCorners(int index)
    {
        int[] faceIndices = GetFaceIndices(index);

        if (faceIndices.Length == 4)
        {
            Vector3[] face = new Vector3[4];
            face[0] = worldBoundingCorners[faceIndices[0]];
            face[1] = worldBoundingCorners[faceIndices[1]];
            face[2] = worldBoundingCorners[faceIndices[2]];
            face[3] = worldBoundingCorners[faceIndices[3]];
            return face;
        }

        return new Vector3[0];
    }

    public int GetIndexOfForwardFace(Vector3 lookAtPoint)
    {
        int highestDotIndex = -1;
        float hightestDotValue = float.MinValue;
        for (int i = 0; i < 6; ++i)
        {
            Vector3 a = (lookAtPoint - GetFaceCentroid(i) ).normalized;
            Vector3 b = GetFaceNormal(i);
            float dot = Vector3.Dot(a, b);
            if (hightestDotValue < dot)
            {
                hightestDotValue = dot;
                highestDotIndex = i;
            }
        }
        return highestDotIndex;
    }
}
