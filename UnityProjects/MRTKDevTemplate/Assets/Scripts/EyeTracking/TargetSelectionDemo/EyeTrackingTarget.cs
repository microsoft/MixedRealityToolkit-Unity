// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    using System.Collections;

    /// <summary>
    /// TODO:
    /// </summary>
    public class EyeTrackingTarget : MonoBehaviour
    {

        [Tooltip("Visual effect (e.g., particle explosion or animation) that is played when a target is selected.")]
        [SerializeField]
        private GameObject _visualFxOnHit = null;

        [Tooltip("Audio clip that is played when a target is selected.")]
        [SerializeField]
        private AudioClip _audioFxCorrectTarget = null;

        [Tooltip("Audio clip that is played when a wrong target is selected.")]
        [SerializeField]
        private AudioClip _audioFxIncorrectTarget = null;

        [Tooltip("Manually indicate whether this is an incorrect target.")]
        [SerializeField]
        private bool _isValidTarget = true;

        [Tooltip("Euler angles by which the object should be rotated by.")]
        [SerializeField]
        private Vector3 _rotateByEulerAngles = Vector3.zero;

        [Tooltip("Rotation speed factor.")]
        [SerializeField]
        private float _speed = 1f;

        /// <summary>
        /// Coroutine that plays when the game object is hovered over.
        /// </summary>
        private Coroutine _rotationCoroutine;

        /// <summary>
        /// Internal audio source associated with the game object.
        /// </summary>
        private AudioSource _audioSource;

        private void Awake()
        {
            SetUpAudio();
        }

        public void OnGazeHoverEntered()
        {
            _rotationCoroutine = StartCoroutine(RotateTarget());
        }

        public void OnGazeHoverExited()
        {
            StopCoroutine(_rotationCoroutine);
        }

        public void OnTargetSelected()
        {
            if (!_isValidTarget)
            {
                PlayAudioOnHit(_audioFxIncorrectTarget);
                return;
            }

            // Play audio clip
            float audiocliplength = PlayAudioOnHit(_audioFxCorrectTarget);

            // Play animation
            float animationLength = PlayAnimationOnHit();

            // Destroy target
            gameObject.SetActive(true);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, Mathf.Max(audiocliplength, animationLength));
        }

        private void SetUpAudio()
        {
            _audioSource = gameObject.EnsureComponent<AudioSource>();

            _audioSource.playOnAwake = false;
            _audioSource.enabled = true;
        }

        /// <summary>
        /// Play given audio clip.
        /// </summary>
        private float PlayAudioOnHit(AudioClip audioClip)
        {
            if (audioClip == null || _audioSource == null)
            {
                return 0f;
            }

            // Play the given audio clip
            _audioSource.clip = audioClip;
            _audioSource.PlayOneShot(_audioSource.clip);
            return _audioSource.clip.length;
        }

        /// <summary>
        /// Show given GameObject when target is selected. 
        /// </summary>
        private float PlayAnimationOnHit()
        {
            if (_visualFxOnHit == null)
            {
                return 0f;
            }

            _visualFxOnHit.SetActive(true);
            return _visualFxOnHit.GetComponent<ParticleSystem>().main.duration;
        }

        /// <summary>
        /// Rotates game object based on specified rotation speed and Euler angles.
        /// </summary>
        private IEnumerator RotateTarget()
        {
            while (true)
            {
                transform.eulerAngles += _speed * _rotateByEulerAngles;
                yield return null;
            }
        }
    }
}
