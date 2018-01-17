// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.GazeRuler
{
    /// <summary>
    /// any geometry class inherit this interface should be closeable
    /// </summary>
    public interface IPolygonClosable
    {
        //finish special polygon
        void ClosePolygon(GameObject LinePrefab, GameObject TextPrefab);
    }
}