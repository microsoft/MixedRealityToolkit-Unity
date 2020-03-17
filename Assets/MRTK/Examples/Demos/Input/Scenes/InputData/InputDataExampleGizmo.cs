using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Attach this component to a game object to synchronize the objects position
    /// and rotation to the given input type. If the input data is not available,
    /// the component will hide the object by disabling all renderers.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/InputDataExampleGizmo")]
    public class InputDataExampleGizmo : MonoBehaviour
    {
        public InputSourceType sourceType;
        public Handedness handedness;
        private bool isDataAvailable = true;

        private void SetIsDataAvailable(bool value)
        {
            if (value != isDataAvailable)
            {
                foreach (var item in GetComponentsInChildren<Renderer>())
                {
                    item.enabled = value;
                }
            }
            isDataAvailable = value;
        }
        public void Update()
        {
            Ray myRay;
            if(InputRayUtils.TryGetRay(sourceType, handedness, out myRay))
            {
                transform.localPosition= myRay.origin;
                transform.localRotation = Quaternion.LookRotation(myRay.direction, Vector3.up);
                SetIsDataAvailable(true);
            }
            else
            {
                SetIsDataAvailable(false);
            }
        }
    }
}

