using Pixie.AppSystems.TimeSync;
using UnityEngine;

namespace Pixie.Demos
{
    public class DeviceTimeColors : MonoBehaviour
    {
        [SerializeField]
        private Gradient loopColor;
        [SerializeField]
        private float loopLength = 3f;

        private void Update()
        {
            float normalizedLoop = Mathf.Repeat(NetworkTime.Time, loopLength) / loopLength;
            GetComponent<Renderer>().material.color = loopColor.Evaluate(normalizedLoop);
        }
    }
}