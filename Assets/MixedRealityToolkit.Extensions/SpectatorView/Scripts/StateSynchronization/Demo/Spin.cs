using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class Spin : MonoBehaviour
    {
        void Update()
        {
            if (StateSynchronization.Instance.Role == Role.Broadcaster)
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 100 * Time.time, 0);
            }
        }
    }
}
