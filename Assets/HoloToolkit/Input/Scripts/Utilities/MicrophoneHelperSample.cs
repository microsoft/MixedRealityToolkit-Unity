
//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
// TODO: insert specific license information (MRTK is MIT)
//
// This sample code is a modified version of an official UWP sample which can be found on:
// https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/SpeechRecognitionAndSynthesis/cs/AudioCapturePermissions.cs
//
//*********************************************************

using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
#endif

// TODO: namespace might need to be updated for consistency with other MRTK samples
namespace MixedRealityToolkit
{    enum MicrophoneStatus
    {
        MicrophoneReady,
        MicrophoneUseNotAuthorized,
        MicrophoneNotPresent,
        MicrophoneMuted,
        MicrophoneUseFailed,
        MediaComponentsMissing,
    }

    class MicrophoneHelper
    {
#if ENABLE_WINMD_SUPPORT
        // If no recording device is attached, attempting to get access to audio capture devices will throw 
        // a System.Exception object, with HResult = ‭0xC00DABE0‬ set.
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
                // This can be replicated by using remote desktop to a system, but not redirecting the microphone input.
                // Can also occur if using the virtual machine console tool to access a VM instead of using remote desktop.
                // You can also get here if you disable all audio recording endpoints
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
        // if we run in the editor return that microphone is ready by default
        // It could be useful to test in the editor the behavior where the
        // microphone is reported ready.
        public static MicrophoneStatus GetMicrophoneStatus()
        {
            return MicrophoneStatus.MicrophoneReady;
        }

        public static bool IsMicrophoneReady()
        {
            var status = GetMicrophoneStatus();
            return status == MicrophoneStatus.MicrophoneReady;
        }
#endif
    }
}

public class MicrophoneHelperSample : MonoBehaviour
{
    [Tooltip("Assign key to quickly test the microphone helper code")]
    public KeyCode checkMicrophoneKey = KeyCode.M;
#if ENABLE_WINMD_SUPPORT
    async void Update ()
#else
    void Update()
#endif
    {
        if(Input.GetKeyDown(checkMicrophoneKey))
        {
#if ENABLE_WINMD_SUPPORT
            var status = await MixedRealityToolkit.MicrophoneHelper.GetMicrophoneStatus();
#else
            var status = MixedRealityToolkit.MicrophoneHelper.GetMicrophoneStatus();
#endif
            Debug.LogFormat("Microphone status: {0}", status);
        }
	}
}
