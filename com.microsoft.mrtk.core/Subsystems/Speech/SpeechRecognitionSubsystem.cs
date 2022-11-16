// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A subsystem that enables speech recognition.
    /// </summary>
    [Preserve]
    public class SpeechRecognitionSubsystem :
        MRTKSubsystem<SpeechRecognitionSubsystem, SpeechRecognitionSubsystemDescriptor, SpeechRecognitionSubsystem.Provider>,
        ISpeechRecognitionSubsystem
    {
        /// <summary>
        /// Construct the <c>SpeechRecognitionSubsystem</c>.
        /// </summary>
        public SpeechRecognitionSubsystem()
        { }

        /// <summary>
        /// Interface for providing recognition functionality for the implementation.
        /// </summary>
        public abstract class Provider : MRTKSubsystemProvider<SpeechRecognitionSubsystem>, ISpeechRecognitionSubsystem
        {
            #region ISpeechRecognitionSubsystem implementation

            /// <inheritdoc/>
            public abstract void StartRecognition();

            /// <inheritdoc/>
            public abstract void StopRecognition();

            /// <inheritdoc/>
            public event Action<SpeechRecognitionResultEventArgs> Recognizing;

            /// <inheritdoc/>
            public event Action<SpeechRecognitionResultEventArgs> Recognized;

            /// <inheritdoc/>
            public event Action<SpeechRecognitionSessionEventArgs> RecognitionFinished;

            /// <inheritdoc/>
            public event Action<SpeechRecognitionSessionEventArgs> RecognitionFaulted;

            #endregion ISpeechRecognitionSubsystem implementation

            /// <summary>
            /// Trigger for the <see cref="Recognizing"/> Action.
            /// </summary>
            protected void OnRecognizing(SpeechRecognitionResultEventArgs eventArgs)
            {
                Recognizing?.Invoke(eventArgs);
            }

            /// <summary>
            /// Trigger for the <see cref="Recognized"/> Action.
            /// </summary>
            protected void OnRecognized(SpeechRecognitionResultEventArgs eventArgs)
            {
                Recognized?.Invoke(eventArgs);
            }

            /// <summary>
            /// Trigger for the <see cref="RecognitionFinished"/> Action.
            /// </summary>
            protected void OnRecognitionFinished(SpeechRecognitionSessionEventArgs eventArgs)
            {
                RecognitionFinished?.Invoke(eventArgs);
            }

            /// <summary>
            /// Trigger for the <see cref="RecognitionFaulted"/> Action.
            /// </summary>
            protected void OnRecognitionFaulted(SpeechRecognitionSessionEventArgs eventArgs)
            {
                RecognitionFaulted?.Invoke(eventArgs);
            }

        }

        #region ISpeechRecognitionSubsystem implementation

        /// <inheritdoc/>
        public void StartRecognition() => provider.StartRecognition();

        /// <inheritdoc/>
        public void StopRecognition() => provider.StopRecognition();

        /// <inheritdoc/>
        public event Action<SpeechRecognitionResultEventArgs> Recognizing
        {
            add => provider.Recognizing += value;
            remove => provider.Recognizing -= value;
        }

        /// <inheritdoc/>
        public event Action<SpeechRecognitionResultEventArgs> Recognized
        {
            add => provider.Recognized += value;
            remove => provider.Recognized -= value;
        }

        /// <inheritdoc/>
        public event Action<SpeechRecognitionSessionEventArgs> RecognitionFinished
        {
            add => provider.RecognitionFinished += value;
            remove => provider.RecognitionFinished -= value;
        }

        /// <inheritdoc/>
        public event Action<SpeechRecognitionSessionEventArgs> RecognitionFaulted
        {
            add => provider.RecognitionFaulted += value;
            remove => provider.RecognitionFaulted -= value;
        }

        #endregion ISpeechRecognitionSubsystem implementation

        /// <summary>
        /// Registers a SpeechRecognition subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="SpeechRecognitionSubsystemParams">The parameters defining the SpeechRecognition subsystem functionality implemented
        /// by the subsystem provider.</param>
        /// <returns>
        /// <c>true</c> if the subsystem implementation is registered. Otherwise, <c>false</c>.
        /// </returns>
        public static bool Register(SpeechRecognitionSubsystemCinfo SpeechRecognitionSubsystemParams)
        {
            var descriptor = SpeechRecognitionSubsystemDescriptor.Create(SpeechRecognitionSubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            return true;
        }
    }

}
