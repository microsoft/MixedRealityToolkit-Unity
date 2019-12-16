using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

public interface IWindowsMixedRealitySceneUnderstanding
{
    bool TryGetPlaneValidationMask(SpatialAwarenessSceneObject.Quad quad, ushort textureWidth, ushort textureHeight, out byte[] mask);
    bool TryGetBestPlacementPosition(SpatialAwarenessSceneObject.Quad quad, Vector2 objExtents, out Vector2 placementPosOnPlane);
}
