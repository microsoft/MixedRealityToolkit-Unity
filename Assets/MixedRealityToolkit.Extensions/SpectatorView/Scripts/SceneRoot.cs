using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView
{
    public class SceneRoot : MonoBehaviour
    {
        void Start()
        {
            var spectatorView = FindObjectOfType<SpectatorView>();
            if (spectatorView == null)
            {
                Debug.Log("Failed to find spectator view");
                return;
            }

            Debug.Log("Setting scene root: " + gameObject.name);
            spectatorView.SceneRoot = gameObject;
        }
    }
}
