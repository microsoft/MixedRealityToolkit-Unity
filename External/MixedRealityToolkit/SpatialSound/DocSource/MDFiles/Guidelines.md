UAudio Guidelines                        {#guidelines}
============

Make sure Spatial Sound is enabled for your app! Go to Edit à Project Settings à Audio, and make sure the "Spatializer Plugin" is set to "MS HRTF Spatializer". A checkbox for "Spatialize" should now be present on your AudioSource components.

You can set the DSP (digital signal processing) buffer to one of four different options in Edit à Project Settings à Audio à DSP Buffer Size. The options are Default, Best latency, Good latency, Best performance. If you’re experiencing issues with streaming audio, you can adjust this option and observe how it affects performance. Other helpful settings are described here: AudioManager
 
Unity's profiler can show you:

- The number of voices playing at any given time
- Amount of streaming sound memory being used

UAudioManager’s profiler (Addons à UAudioTools à Profiler) can show you

- Events playing
- Objects playing those events
- The bus each event is on

The more variety you have in playing audio, the harder it will be to pin down where the choke points are. SYI had 3 different ways to play audio: through the UAudioManager, a VO manager, and loose AudioSources that were set to play on awake, and this made it difficult to track down and fix audio related issues as there were so many different places to look. If additional audio functionality is needed, like a VO manager, it is recommended that it still uses UAudioManager events, so that all audio leads to one place.

Setting up budgeting guidelines for CPU and memory usage up front can help you determine how much audio to play at any given time.

Audio compression settings can be set in bulk by going to Addons à UAudioTools à Set Audio Import Settings

It is recommended to have at least one Audio Mixer asset, and all sounds should be set to a bus within that mixer - no sound should be routed directly to the master audio bus.

Make use of Audio Mixer Snapshots. They can be accessed from UAudioManager events, and provide a quick and easy way to smoothly transition between mixes.
