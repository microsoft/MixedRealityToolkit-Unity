// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSceneUnderstandingSceneObjectConsumer : MonoBehaviour, ISceneUnderstandingSceneObjectConsumer
    {
        [PhysicsLayer]
        public int DefaultLayer;
        public string DefaultTag;

        [PhysicsLayer]
        public int WallLayer;
        public string WallTag;

        public void OnSpatialAwarenessSceneObjectCreated(SpatialAwarenessSceneObject sceneObject)
        {
            switch (sceneObject.SurfaceType)
            {
                case SpatialAwarenessSurfaceTypes.Wall:
                    if (WallTag != "")
                    {
                        gameObject.tag = WallTag;
                    }
                    gameObject.layer = WallLayer;
                    break;
                default:
                    if (DefaultTag != "")
                    {
                        gameObject.tag = DefaultTag;
                    }
                    gameObject.layer = DefaultLayer;
                    break;
            }

            if (sceneObject.Quads.Count > 0)
            {
                float sx = sceneObject.Quads[0].extents.x;
                float sy = sceneObject.Quads[0].extents.y;
                transform.localScale = new Vector3(sx, sy, .1f);
            }
            else
            {
                Debug.LogWarning("No quad data found - did you request it from the observer?");
            }
        }
    }
}
