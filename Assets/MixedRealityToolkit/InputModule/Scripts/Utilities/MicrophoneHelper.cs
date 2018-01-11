// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

//*********************************************************
//
// This sample code is a modified version of an official UWP sample which can be found on:
// https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/SpeechRecognitionAndSynthesis/cs/AudioCapturePermissions.cs
//
//*********************************************************

#if ENABLE_WINMD_SUPPORT
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
#endif

namespace MixedRealityToolkit.InputModule.Utilities
{
    public enum MicrophoneStatus
    {
        MicrophoneReady,
        MicrophoneUseNotAuthorized,
        MicrophoneNotPresent,
        MicrophoneMuted,
        MicrophoneUseFailed,
        MediaComponentsMissing,
    }

    /// <summary>
    /// Provides a working status information of the default microphone on the system.
    /// Class is using UWP APIs. Everything UWP related is wrapped around ENABLE_WINMD_SUPPORT
    /// preprocessor definition. The definition is enabled in Unity in the following 2 cases:
    ///     - IL2CPP backend and .NET 4.6
    ///     - .NET backend and Net Core compilation override
    /// For full details see unity documentation page
    /// https://docs.unity3d.com/Manual/IL2CPP-WindowsRuntimeSupport.html
    /// </summary>
    public class MicrophoneHelper
    {
#if ENABLE_WINMD_SUPPORT

        /// <summary>
        /// Error code returned if there is no audio device in the system
        /// </summary>
        /// <remarks>
        /// If no recording device is attached, attempting to get access to audio capture devices will throw 
        /// a System.Exception object, with HResult = ‭0xC00DABE0‬ set.
        /// </remarks>
        private static int NoCaptureDevicesHResult = -1072845856;

        /// <summary>
        /// On desktop/tablet systems, users are prompted to give permission to use capture devices on a 
        /// per-app basis. Along with declaring the microphone DeviceCapability in the package manifest,
        /// this method tests the privacy setting for microphone access for this application.
        /// Note that this only checks the Settings->Privacy->Microphone setting, it does not handle
        /// the Cortana/Dictation privacy check, however (Under Settings->Privacy->Speech, Inking and Typing).
        /// 
        /// Developers should ideally perform a check like this every time their app gains focus, in order to 
        /// check if the user has changed the setting while the app was suspended or not in focus.
        /// </summary>
        /// <returns>
        /// Returns a MicrophoneStatus value indicating the specific issue if microphone cannot be used.
        /// If everything went well return value is MicrophoneStatus.MicrophoneReady
        /// </returns>
        public async static Task<MicrophoneStatus> GetMicrophoneStatus()
        {
            // Request access to the microphone only, to limit the number of capabilities we need
            // to request in the package manifest.
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
            settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
            settings.MediaCategory = MediaCategory.Speech;
            MediaCapture capture = new MediaCapture();
            try
            {
                // If this is the first time we run the app, this will show the
                // microphone permissions dialog, where the user is asked if they
                // allow the app to use the microphone. If the dialog is dismissed, or not
                // accepted, the app will not ask for permissions again. The user will need
                // to go to Settings->Privacy->Microphone Settings to allow the app to
                // use the microphone
                await capture.InitializeAsync(settings);
            }
            catch (TypeLoadException)
            {
                // On SKUs without media player (eg, the N SKUs), we may not have access to the Windows.Media.Capture
                // namespace unless the media player pack is installed.
                return MicrophoneStatus.MediaComponentsMissing;
            }
            catch (UnauthorizedAccessException)
            {
                // The user has turned off access to the microphone. If this occurs, we should show an error, or disable
                // functionality within the app to ensure that further exceptions aren't generated when 
                // recognition is attempted.
                return MicrophoneStatus.MicrophoneUseNotAuthorized;
            }
            catch (Exception exception)
            {
                // In mixed reality this would be the case when your HMD uses headset jack, your host machine does not have connected microphone
                //       (i.e. desktop with no integrated mic, nothing plugged into mic/headset jack or you disabled all recording endpoints)
                // If the HMD has built in microphones, they would be disabled when HMD is not worn and will be enabled when you put the device on.
                if (exception.HResult == NoCaptureDevicesHResult)
                {
                    return MicrophoneStatus.MicrophoneNotPresent;
                }
                else
                {
                    return MicrophoneStatus.MicrophoneUseFailed;
                }
            }

            // if microphone is muted, speech recognition will not hear anything.
            // return appropriate value to indicate that.
            if(capture.AudioDeviceController.Muted)
            {
                return MicrophoneStatus.MicrophoneMuted;
            }

            return MicrophoneStatus.MicrophoneReady;
        }

        /// <summary>
        /// This function provides an easier query when one is only interested to know if the microphone
        /// can be used or not, without dealing with the specifics of why it may not be working
        /// </summary>
        /// <returns>
        /// Returns true if the microphone is accessible and ready for use.
        /// </returns>
        public static async Task<bool> IsMicrophoneReady()
        {
            var status = await GetMicrophoneStatus();
            return status == MicrophoneStatus.MicrophoneReady;
        }
#else
        /// <summary>
        /// Stub function imitating the original GetMicrophoneStatus
        /// </summary>
        /// <returns>
        /// Always returns microphone ready.
        /// </returns>
        public static MicrophoneStatus GetMicrophoneStatus()
        {
            return MicrophoneStatus.MicrophoneReady;
        }

        /// <summary>
        /// Stub function imitating the original IsMicrophoneReady
        /// </summary>
        public static bool IsMicrophoneReady()
        {
            var status = GetMicrophoneStatus();
            return status == MicrophoneStatus.MicrophoneReady;
        }
#endif
    }
}
