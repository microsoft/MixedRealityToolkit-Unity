using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public class PlatformSwitcher : MonoBehaviour
    {
        /// <summary>
        /// Available platforms
        /// </summary>
        public enum Platform
        {
            HoloLens = 0,
            iOS,
            Android
        }
    }
}
