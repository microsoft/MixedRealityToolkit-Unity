# Dictation

Dictation allows users to record audio clips and obtain a transcription of the audio. To use it make sure that a dictation system is registered in the *Input System Profile*. **Windows Dictation Input Provider** is the dication system provided out of the box but alternative dictation systems can be created implementing [`IMixedRealityDictationSystem`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityDictationSystem).

<img src="../../External/ReadMeImages/Input/DictationDataProvider.png" style="max-width:100%;">

Once a dictation system has been configured it is necessary to start a recording session to obtain dictation events. These events are produced at different stages of the transcription process:
- Hypothesis: raised as the user talks it provides text of what the dictation system it's heard so far.