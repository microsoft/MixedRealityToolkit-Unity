// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// The associated GameObject acts as a teleporter to a referenced respawn location.  
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Scripts/MRTK/Examples/TransportToRespawnLocation")]
    public class TransportToRespawnLocation : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The referenced game objects acts as a placeholder for the respawn location.")]
        private GameObject RespawnReference = null;

        [SerializeField]
        [Tooltip("Optional audio clip which is played when the target is respawned.")]
        private AudioClip AudioFX_OnRespawn = null;

        void OnTriggerEnter(Collider other)
        {
            if (RespawnReference != null)
            {
                other.gameObject.transform.position = RespawnReference.transform.position;
                AudioFeedbackPlayer.Instance.PlaySound(AudioFX_OnRespawn);
            }
        }
    }
}
