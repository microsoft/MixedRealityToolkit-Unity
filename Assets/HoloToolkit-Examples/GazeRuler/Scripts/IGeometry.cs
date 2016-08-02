using UnityEngine;
using System.Collections;

/// <summary>
/// interface for geometry class
/// </summary>
public interface IGeometry
{
    void AddPoint(GameObject LinePrefab, GameObject PointPrefab, GameObject TextPrefab);

    void Delete();

    void Clear();

    void Reset();
}