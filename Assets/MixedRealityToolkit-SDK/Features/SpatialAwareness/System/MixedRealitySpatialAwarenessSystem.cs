// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwareness;
//using Microsoft.MixedReality.Toolkit.Internal.EventDatum.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class MixedRealitySpatialAwarenessSystem : MixedRealityEventManager, IMixedRealitySpatialAwarenessSystem
{
    #region IMixedRealityManager Implmentation

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        InitializeInternal();
    }

    /// <summary>
    /// Performs initialization tasks for the SpatialAwarenessSystem.
    /// </summary>
    private void InitializeInternal()
    {
        // todo
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        InitializeInternal();
    }

    public override void Destroy()
    {
        // todo
    }

    #endregion IMixedRealityManager Implmentation

    #region IMixedRealityEventSystem Implmentation
    #endregion IMixedRealityEventSystem Implmentation

    #region IMixedRealityEventSystem Implmentation
    #endregion IMixedRealityEventSystem Implmentation

    #region IMixedRealitySpatialAwarenessSystem Implmentation

    /// <inheritdoc/>
    public bool AutoStart { get; set; } // todo

    /// <inheritdoc/>
    public Vector3 ObservationExtents { get; set; } // todo

    /// <inheritdoc/>
    public int PhysicsLayer { get; set; } // todo

    /// <inheritdoc/>
    public float UpdateInterval { get; set; } // todo

    /// <inheritdoc/>
    public bool IsObserverRunning { get; private set; }

    /// <inheritdoc/>
    public void StartObserver()
    {
        // todo
    }

    /// <inheritdoc/>
    public void StopObserver()
    {
        // todo
    }

    #region Mesh

    /// <inheritdoc/>
    public int TrianglesPerCubicMeter { get; set; } // todo

    /// <inheritdoc/>
    public bool RecalculateNormals { get; set; } // todo

    /// <inheritdoc/>
    public bool IsMeshDataSupported { get; private set; } // todo

    #endregion Mesh

    #region Surface

    /// <inheritdoc/>
    public float MinimumSurfaceArea { get; set; } // todo

    #endregion Surface

    #endregion IMixedRealitySpatialAwarenessSystem Implmentation
}
