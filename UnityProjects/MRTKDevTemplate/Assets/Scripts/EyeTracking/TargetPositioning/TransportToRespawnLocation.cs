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
        private GameObject _respawnReference = null;

        [SerializeField]
        [Tooltip("Optional audio clip which is played when the target is respawned.")]
        private AudioClip _audioFXOnRespawn = null;

        private AudioSource _audioSource = null;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (_respawnReference != null)
            {
                other.gameObject.transform.position = _respawnReference.transform.position;
                _audioSource.PlayOneShot(_audioFXOnRespawn);
            }
        }
    }
}
