
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;

namespace Microsoft.MixedReality.Toolkit.UI
{
    class BoundingBoxTarget
    {

        public BoundingBoxTarget(GameObject target)
        {
            this.Target = target;
        }

        public GameObject Target { get; private set; }

        public Transform transform
        {
            get
            {
                return Target.transform;
            }
        }

        /// <summary>
        /// The collider reference tracking the bounds utilized by this component during runtime
        /// </summary>
        public BoxCollider TargetBounds { get; set; }

        public bool HasChanged()
        {
            return Target.transform.hasChanged;
        }

        public void ClearChanged()
        {
            Target.transform.hasChanged = false;
        }


        
    }
}
