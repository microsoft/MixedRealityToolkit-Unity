// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend.Cycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend.Automation
{
    /// <summary>
    /// A component for playing 1 to many audio clips. They can be played in a sequence or one at a time.
    /// Uses the CycleArray functionality, to Play the sequence from the beginning, call PlayAll();
    /// </summary>
    
    [System.Serializable]
    public struct PlayAudioClipsData
    {
        [Tooltip("Audio clip to play at this index")]
        public AudioClip Clip;

        [Tooltip("A reference transform for spacial sound, leave empty if a static location is desired")]
        public Transform Position;

        [Tooltip("On audio clip playback complete")]
        public UnityEvent OnComplete;
    }

    [RequireComponent(typeof(AudioSource))]
    public class PlayAudioClips : CycleArray<PlayAudioClipsData>
    {
        [Tooltip("Amount of time between audio clip playback when playing a sequence")]
        public float GapTime = 0.5f;

        [Tooltip("Should the sequence loop until manually stopped?")]
        public bool Loop = false;

        //audio source
        private AudioSource mSource;

        // playing a sequence?
        private bool mPlayingSequence = false;

        // playing a clip?
        private bool mPlayingClip = false;

        // coroutine to handle gap time.
        private Coroutine mPlayAfterGap;
        
        protected override void Awake()
        {
            base.Awake();
            mSource = GetComponent<AudioSource>();
            
        }

        /// <summary>
        /// Is there a clip playing?
        /// </summary>
        public bool IsPlaying { get { return mPlayingClip; } }

        /// <summary>
        /// Start playing a sequence of audio from the specified index
        /// </summary>
        /// <param name="startIndex"></param>
        public void PlayAll(int startIndex = 0)
        {
            mPlayingSequence = true;
            SetIndex(startIndex);
        }

        /// <summary>
        /// Stop audio playback
        /// </summary>
        public void Stop()
        {
            mPlayingSequence = false;
            mSource.Stop();
            mPlayingClip = false;

            if (mPlayAfterGap != null)
            {
                StopCoroutine(mPlayAfterGap);
                mPlayAfterGap = null;
            }
        }

        /// <summary>
        /// choose the audio clip to play by index
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);
            PlayAudioClipsData data = Array[index];
            mSource.clip = data.Clip;
            mPlayingClip = true;
            if(data.Position != null)
            {
                this.transform.position = data.Position.position;
            }
            mSource.Play();
        }

        /// <summary>
        /// An audio clip has completed playback
        /// </summary>
        private void AudioComplete()
        {
            mPlayingClip = false;

            PlayAudioClipsData data = Array[Index];
            data.OnComplete.Invoke();

            if (mPlayingSequence)
            {
                if (Loop || Index < Array.Length - 1)
                {
                    mPlayAfterGap = StartCoroutine(PlayAudioAfterGap());
                }
            }
        }

        /// <summary>
        /// coroutine to handle playback gap between clips
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayAudioAfterGap()
        {
            yield return new WaitForSeconds(GapTime);
            mPlayAfterGap = null;

            if (mPlayingSequence)
            {
                if (Loop || Index < Array.Length - 1)
                {
                    SetIndex(Index + 1);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (mPlayingClip)
            {
                if (!mSource.isPlaying)
                {
                    // ended
                    AudioComplete();
                }
            }
        }
    }
}
