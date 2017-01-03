// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

/// <summary>
/// interface for geometry class
/// </summary>
public interface IGeometry
{
#if UNITY_WSA
    void AddPoint(GameObject LinePrefab, GameObject PointPrefab, GameObject TextPrefab);

    void Delete();

    void Clear();

    void Reset();
#endif
}

