# Dictation

Dictation allows users to record audio clips and obtain a transcription of the audio. To use it make sure that a dication data provider is registered in the *Input System Profile*. **Windows Dictation Input Provider** is the dication service provided out of the box but alternative dictation systems can be created implementing [`IMixedRealityDictationSystem`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityDictationSystem).

<img src="../../External/ReadMeImages/Input/DictationDataProvider.png" width="80%" class="center">

Once you have a dictation service set up, you can use the [`DictationHandler`](xref:Microsoft.MixedReality.Toolkit.Input.DictationHandler) script to start and stop recording sessions and obtain the transription results via UnityEvents.



