// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend.Automation
{
    public class SequenceManager : MonoBehaviour
    {
        [System.Serializable]
        public struct SequenceItem
        {
            public float Delay;
            public bool Pause;
            public UnityEvent OnComplete;
        }

        [Tooltip("The sequnce instructions")]
        public SequenceItem[] Sequence;

        [Tooltip("Should the sequnce loop when complete?")]
        public bool Loop = false;

        [Tooltip("Current index, or starting index")]
        public int Index;

        [Tooltip("Status, is playing or auto start")]
        public bool IsPlaying;

        private float timer;
        private float currentTime;
        private float percentage;

        private void Start()
        {
            if (IsPlaying)
            {
                StartSequence(Index, true);
            }
        }

        /// <summary>
        /// Get the current time
        /// </summary>
        /// <returns></returns>
        public float GetTime()
        {
            return percentage * timer;
        }

        /// <summary>
        /// start the sequence
        /// </summary>
        /// <param name="index"></param>
        public void Play(int index = 0)
        {
            Index = index;
            StartSequence(Index);
        }

        /// <summary>
        /// Stop and reset the sequence
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
            Index = 0;
        }

        /// <summary>
        /// resume after a pause
        /// </summary>
        /// <param name="skipDelay"></param>
        public void Continue(bool skipDelay = false)
        {
            IsPlaying = true;

            if (skipDelay)
            {
                currentTime = timer;
            }
        }

        /// <summary>
        /// Internal config for starting a sequence
        /// </summary>
        /// <param name="index"></param>
        /// <param name="start"></param>
        private void StartSequence(int index, bool start = true)
        {
            if (index < Sequence.Length)
            {
                IsPlaying = !Sequence[index].Pause || start;
                timer = Sequence[index].Delay;
                currentTime = 0;
            }
        }

        /// <summary>
        /// process the sequence
        /// </summary>
        private void Update()
        {
            if (currentTime <= timer && IsPlaying)
            {
                currentTime += Time.deltaTime;

                if (currentTime >= timer)
                {
                    currentTime = timer;
                }

                if (timer > 0)
                {
                    percentage = currentTime / timer;
                }
                else if (timer == 0 && currentTime == 0)
                {
                    percentage = 1;
                }


                if (percentage >= 1)
                {
                    if (Index < Sequence.Length)
                    {
                        Sequence[Index].OnComplete.Invoke();
                    }

                    if (Index < Sequence.Length - 1)
                    {
                        Index += 1;
                        StartSequence(Index, false);
                    }
                    else
                    {
                        if (Loop)
                        {
                            Index = 0;
                            StartSequence(Index);
                        }
                        else
                        {
                            IsPlaying = false;
                        }

                    }
                }
            }
        }
    }
}
