# Dictation

Dictation allows users to record audio clips and obtain a transcription. To use it make sure that a dictation system is registered in the *Input System Profile*. **Windows Dictation Input Provider** is the dictation system provided out of the box but alternative dictation systems can be created implementing [`IMixedRealityDictationSystem`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityDictationSystem).

## Requirements

The dictation system uses Unity's [DictationRecognizer](https://docs.unity3d.com/ScriptReference/Windows.Speech.DictationRecognizer.html)
which itself uses the underlying Windows speech APIs for handling dictation. Note that this
implies that this feature is only present on Windows-based platforms.

Usage of the Dictation system requires both the "Internet Client" and "Microphone" application
capabilities in the [PlayerSettings - Capabilities section](https://docs.unity3d.com/Manual/class-PlayerSettingsWSA.html#Capabilities).
See [Windows Mixed Reality Documentation](https://docs.microsoft.com/windows/mixed-reality/voice-input-in-unity#dictation)
for more details on voice input in Unity.

## Configuration

<img src="../../Documentation/Images/Input/DictationDataProvider.png" width="80%" class="center">

Once you have a dictation service set up, you can use the [`DictationHandler`](xref:Microsoft.MixedReality.Toolkit.Input.DictationHandler) script to start and stop recording sessions and obtain the transcription results via UnityEvents.

<img src="../../Documentation/Images/Input/DictationHandler.png" width="80%" class="center">

- **Dictation Hypothesis** is raised as the user speaks with early, rough transcriptions of the audio captured so far.
- **Dictation Result** is raised at the end of each sentence (i.e. when the user pauses) with the final transcription of the audio captured so far.
- **Dictation Complete** is raised at the end of the recording session with the full, final transcription of the audio.
- **Dictation Error** is raised to inform of errors in the dictation service. The transcription in this case contains a description of the error.

## Example scene

**Dictation** scene in `MRTK/Examples/Demos/Input/Scenes/Dictation` shows the `DictationHandler` script in use. If you need more control, you can either extend this script or create your own implementing [`IMixedRealityDictationHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityDictationHandler) to receive dictation events directly.

<img src="../../Documentation/Images/Input/DictationDemo.png" width="80%" class="center">
