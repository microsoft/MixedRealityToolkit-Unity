// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// An abstract <see cref="IPlayableEffect"> that controls a <see cref="Playable"/> based on the
    /// supplied parameter.
    /// </summary>
    internal abstract class PlayableEffect : IPlayableEffect
    {
        internal enum PlaybackType
        {
            OneShot,
            PlaybackTimeMatchesValue
        }

        /// <inheritdoc />
        public Playable Playable { get; protected set; }

        /// <summary>
        /// Playback speed of the playable in OneShot mode.
        /// </summary>
        /// <remarks>
        /// In Playback Time Matches Value mode, this value has no effect.
        /// </remarks>
        protected abstract float Speed { get; }

        /// <summary>
        /// Should the playable be played back as a one-shot triggered effect,
        /// or should the playback time be directly driven by the state's value?
        /// </summary>
        protected abstract PlayableEffect.PlaybackType PlaybackMode { get; }

        /// <inheritdoc />
        public abstract void Setup(PlayableGraph graph, GameObject owner);

        /// <inheritdoc />
        public virtual bool Evaluate(float parameter)
        {
            if (Playable.IsValid())
            {
                // If we're parameter-based, we set speed to zero
                // and directly drive the time value from the parameter.
                if (PlaybackMode == PlaybackType.PlaybackTimeMatchesValue)
                {
                    Playable.SetSpeed(0);
                    Playable.SetTime(parameter * Playable.GetDuration());
                    return Mathf.Approximately(parameter, 1.0f) || Mathf.Approximately(parameter, 0.0f);
                }
                // Otherwise, we just make sure the playable plays forward if the parameter
                // is active, and backwards if not.
                else if (PlaybackMode == PlaybackType.OneShot)
                {
                    Playable.SetSpeed(!Mathf.Approximately(parameter, 0.0f) ? Speed : -Speed);

                    // Clamp playable time to > 0.
                    // Not sure why the playables system doesn't do this to begin with.
                    if (Playable.GetTime() < 0)
                    {
                        Playable.SetTime(0);
                    }

                    return (Playable.GetTime() == 0.0f && Mathf.Approximately(parameter, 0.0f)) ||
                            (Playable.GetTime() == Playable.GetDuration() && Mathf.Approximately(parameter, 1.0f));
                }
            }

            return true;
        }
    }
}