using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.WorldAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class WorldAnchorSpatialCoordinate : TransformSpatialCoordinate<string>
    {
        private WorldAnchor worldAnchor;

        public WorldAnchorSpatialCoordinate(string anchorName)
            : base(anchorName)
        {
            WorldAnchorManager.Instance.AttachAnchor(gameObject, anchorName);
        }

        public WorldAnchorSpatialCoordinate(string anchorName, Vector3 position, Quaternion rotation)
            : base(anchorName)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            WorldAnchorManager.Instance.AttachAnchor(gameObject, anchorName);
        }

        public override LocatedState State
        {
            get
            {
                if (worldAnchor == null)
                {
                    worldAnchor = gameObject.GetComponent<WorldAnchor>();
                }

                if (worldAnchor != null && worldAnchor.isLocated)
                {
                    return LocatedState.Tracking;
                }
                else
                {
                    return LocatedState.Resolved;
                }
            }
        }

        public void RemoveAnchor()
        {
            WorldAnchorManager.Instance.RemoveAnchor(gameObject);
        }
    }
}