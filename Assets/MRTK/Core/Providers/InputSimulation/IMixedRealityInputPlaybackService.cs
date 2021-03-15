// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Plays back input animation via the input simulation system.
    /// </summary>
    public interface IMixedRealityInputPlaybackService : IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// The animation currently being played.
        /// </summary>
        InputAnimation Animation { get; set; }

        /// <summary>
        /// True if the animation is currently playing.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// The local time in seconds relative to the start of the animation.
        /// </summary>
        float LocalTime { get; set; }

        /// <summary>
        /// Start playing the animation.
        /// </summary>
        void Play();

        /// <summary>
        /// Stop playing the animation and jump to the start.
        /// </summary>
        void Stop();

        /// <summary>
        /// Pause playback and keep the current local time.
        /// </summary>
        void Pause();

        /// <summary>
        /// Try to load input animation data from the given file.
        /// </summary>
        /// <returns>
        /// True if loading input animation from the file succeeded.
        /// </returns>
        bool LoadInputAnimation(string filepath);

        /// <summary>
        /// Try to load input animation data from the given file asynchronously.
        /// </summary>
        /// <returns>
        /// True if loading input animation from the file succeeded.
        /// </returns>
        Task<bool> LoadInputAnimationAsync(string filepath);
    }
}
