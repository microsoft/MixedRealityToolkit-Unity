UAudioMiniManager                        {#uaudiominimanager}
============

## Description

The Mini Manager is a non-singleton version of the UAudioManager. It has all of the same audio functionality of the UAudioManager, but is meant to be for individual pieces of audio content rather than a global manager for all audio content. The most common implementation is to create a prefab for the MiniManager and all of the emitters needed, and play several audio events at once on the instantiated object.

For sound designers, the MiniManager works exactly the same as the main UAudioManager - see the UAudioManager documentation for information on how to set up events.

Note that the mini manager has a few API additions that are specific to it.