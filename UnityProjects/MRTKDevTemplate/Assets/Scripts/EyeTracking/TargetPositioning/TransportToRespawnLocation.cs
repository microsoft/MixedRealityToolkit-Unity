// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// The associated GameObject acts as a teleporter to a referenced respawn location when
    /// a GameObject enters the attached collider volume.  
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Scripts/MRTK/Examples/TransportToRespawnLocation")]
    public class TransportToRespawnLocation : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The referenced game objects acts as a placeholder for the respawn location.")]
        private GameObject respawnReference = null;

        [SerializeField]
        [Tooltip("Optional audio clip which is played when the target is respawned.")]
        private AudioClip audioFXOnRespawn = null;

        private AudioSource audioSource = null;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (respawnReference != null)
            {
                other.gameObject.transform.position = respawnReference.transform.position;
                audioSource.PlayOneShot(audioFXOnRespawn);
            }
        }
    }
}
