
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    internal interface ITestSpatialAwarenessDataProvider : IMixedRealityDataProvider, ITestService, IMixedRealitySpatialAwarenessObserver, IMixedRealitySpatialAwarenessMeshObserver
    {
    }

    /// <summary>
    /// Dummy test IMixedRealitySpatialAwarenessObserver implementation only used for testing
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        (SupportedPlatforms)(-1), // All platforms supported by Unity
        "Test Spatial Awareness Data Provider",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK")]
    public class TestSpatialAwarenessDataProvider : TestBaseDataProvider, ITestSpatialAwarenessDataProvider
    {
        public TestSpatialAwarenessDataProvider(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(spatialAwarenessSystem, name, priority, profile) { }

        public AutoStartBehavior StartupBehavior { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int DefaultPhysicsLayer => throw new System.NotImplementedException();
        public bool IsRunning => throw new System.NotImplementedException();
        public bool IsStationaryObserver { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public VolumeType ObserverVolumeType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Vector3 ObservationExtents { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Quaternion ObserverRotation { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Vector3 ObserverOrigin { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float UpdateInterval { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint SourceId => throw new System.NotImplementedException();
        public string SourceName => throw new System.NotImplementedException();
        public SpatialAwarenessMeshDisplayOptions DisplayOption { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public SpatialAwarenessMeshLevelOfDetail LevelOfDetail { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes => throw new System.NotImplementedException();
        public int MeshPhysicsLayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int MeshPhysicsLayerMask => throw new System.NotImplementedException();
        public bool RecalculateNormals { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int TrianglesPerCubicMeter { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Material OcclusionMaterial { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Material VisibleMaterial { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Resume() { throw new System.NotImplementedException(); }
        public void Suspend() { throw new System.NotImplementedException(); }
        public void ClearObservations() { throw new System.NotImplementedException(); }
        public new bool Equals(object x, object y) { throw new System.NotImplementedException(); }
        public int GetHashCode(object obj) { throw new System.NotImplementedException(); }
    }
}