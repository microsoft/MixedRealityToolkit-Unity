// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using System.Collections;
    using UnityEngine.Serialization;

    /// <summary>
    /// Handles events triggered from the attached <see cref="StatefulInteractable"/>
    /// </summary>
    public class EyeTrackingTarget : MonoBehaviour
    {
        [Tooltip("Visual effect (e.g., particle explosion or animation) that is played when a target is selected.")]
        [SerializeField]
        [FormerlySerializedAs("_visualFxOnHit")]
        private GameObject visualEffectsOnHit = null;

        [Tooltip("Audio clip that is played when a target is selected.")]
        [SerializeField]
        [FormerlySerializedAs("_audioFxCorrectTarget")]
        private AudioClip audioFxCorrectTarget = null;

        [Tooltip("Audio clip that is played when a wrong target is selected.")]
        [SerializeField]
        [FormerlySerializedAs("_audioFxIncorrectTarget")]
        private AudioClip audioFxIncorrectTarget = null;

        [Tooltip("Manually indicate whether this is an incorrect target.")]
        [SerializeField]
        [FormerlySerializedAs("_isValidTarget")]
        private bool isValidTarget = true;

        [Tooltip("Euler angles by which the object should be rotated by.")]
        [SerializeField]
        [FormerlySerializedAs("_rotateByEulerAngles")]
        private Vector3 rotateByEulerAngles = Vector3.zero;

        [Tooltip("Rotation speed factor.")]
        [SerializeField]
        [FormerlySerializedAs("_speed")]
        private float speed = 1f;

        /// <summary>
        /// Coroutine that plays when the game object is hovered over.
        /// </summary>
        private Coroutine rotationCoroutine;

        /// <summary>
        /// Internal audio source associated with the game object.
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// The StatefulInteractable associated with this game object.
        /// </summary>
        private StatefulInteractable interactable;

        private void Awake()
        {
            SetUpAudio();
            interactable = GetComponent<StatefulInteractable>();
        }

        public void OnGazeHoverEntered()
        {
            rotationCoroutine = StartCoroutine(RotateTarget());
        }

        public void OnGazeHoverExited()
        {
            StopCoroutine(rotationCoroutine);
        }

        public void OnTargetSelected()
        {
            if (!interactable.isHovered)
            {
                return;
            }

            if (!isValidTarget)
            {
                PlayAudioOnHit(audioFxIncorrectTarget);
                return;
            }

            // Play audio clip
            float audioClipLength = PlayAudioOnHit(audioFxCorrectTarget);

            // Play animation
            float animationLength = PlayAnimationOnHit();

            // Destroy target
            gameObject.SetActive(true);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, Mathf.Max(audioClipLength, animationLength));
        }

        private void SetUpAudio()
        {
            audioSource = gameObject.EnsureComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.enabled = true;
        }

        /// <summary>
        /// Play given audio clip.
        /// </summary>
        private float PlayAudioOnHit(AudioClip audioClip)
        {
            if (audioClip == null || audioSource == null)
            {
                return 0f;
            }

            // Play the given audio clip
            audioSource.clip = audioClip;
            audioSource.PlayOneShot(audioSource.clip);
            return audioSource.clip.length;
        }

        /// <summary>
        /// Show given GameObject when target is selected. 
        /// </summary>
        private float PlayAnimationOnHit()
        {
            if (visualEffectsOnHit == null)
            {
                return 0f;
            }

            visualEffectsOnHit.SetActive(true);
            return visualEffectsOnHit.GetComponent<ParticleSystem>().main.duration;
        }

        /// <summary>
        /// Rotates game object based on specified rotation speed and Euler angles.
        /// </summary>
        private IEnumerator RotateTarget()
        {
            while (true)
            {
                transform.eulerAngles += speed * rotateByEulerAngles;
                yield return null;
            }
        }
    }
}
