// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

/// <summary>
/// any geometry class inherit this interface should be closeable
/// </summary>
public interface IPolygonClosable
{
#if UNITY_WSA
    //finish special ploygon
    void ClosePloygon(GameObject LinePrefab, GameObject TextPrefab);
#endif
}

