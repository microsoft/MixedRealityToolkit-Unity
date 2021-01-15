using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Animations;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    [RequireComponent(typeof(ParentConstraint))]
    public class DemoSceneUnderstandingStuffController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var source = new ConstraintSource
            {
                sourceTransform = CameraCache.Main.transform,
                weight = 1.0f
            };
            GetComponent<ParentConstraint>().AddSource(source);
        }
    }
}
