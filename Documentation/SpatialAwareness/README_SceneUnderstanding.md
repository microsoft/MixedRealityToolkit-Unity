# Scene Understanding

[Scene Understanding](https://docs.microsoft.com/en-us/windows/mixed-reality/scene-understanding) returns a semantic representation of scene entities as well as their geometric forms on __HoloLens 2__ (HoloLens 1st Gen is not supported).

Some expected use cases of this technology are:
* Place objects on nearest surface of a certain kind (e.g. wall and floor)
* Construct nav-mesh for platform style games
* Provide physics engine friendly geometry as quads
* Accelerate development by avoiding the need to write similar algorithms

Currently Scene Understanding is available as an __experimental__ feature. It is integrated into MRTK as a [spatial observer](SpatialAwarenessGettingStarted.md#register-observers) called [`WindowsSceneUnderstandingObserver`](xref:Microsoft.MixedReality.Toolkit.WindowsSceneUnderstanding.Experimental.WindowsSceneUnderstandingObserver).

## Observer overview

When asked, the [`WindowsSceneUnderstandingObserver`](xref:Microsoft.MixedReality.Toolkit.WindowsSceneUnderstanding.Experimental.WindowsSceneUnderstandingObserver) will return [SpatialAwarenessSceneObject](xref:Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness.SpatialAwarenessSceneObject) with attributes useful for the application to understand its surroundings. The observation frequency, returned object type (e.g. wall, floor) and other observer behaviors are dependent on the configuration of the observer via profile. For instance, if the occlusion mask is desired the observer must be configured to generate quads. The observed scene can be saved as serialized file that can be later loaded to recreate the scene in editor play mode.

## Setup

1. Ensure the platform is set to UWP in build settings.
1. Install Dot Net WinRT package through [NuGet for Unity](https://github.com/GlitchEnzo/NuGetForUnity).
<br/><img src="../Images/SceneUnderstanding/SU_NuGet_DotNetWinRT.png" width="600"><br/>
1. Acquire the SceneUnderstanding package through NuGet for Unity that contains the binaries required for the feature
<br/><img src="../Images/SceneUnderstanding/SU_NuGet_SU.png" width="600"><br/>
1. In the menu bar click Mixed Reality Toolkit -> Utilities -> Windows Mixed Reality -> Check Configuration (this operation may take a while)
1. In the menu bar click Mixed Reality Toolkit -> Utilities -> Scene Understanding -> Check Configuration (this operation may take a while)



## Using Scene Understanding

The quickest way to get started with Scene Understanding is to check out the sample scene.

### Scene Understanding sample scene

Open the scene file in `Examples/Experimental/SceneUnderstanding/Scenes/SceneUnderstandingExample.unity` and press play!

The scene demonstrates the following:

* Visualization of observed Scene Objects with in application UI for configuring the observer
* Example `DemoSceneUnderstandingController` script that shows how to change observer settings and listen to relevant events
* Saving scene data to device for offline development
* Loading previously saved scene data (.bytes files) to support in-editor development workflow

#### Configuring the observer service

Select the 'MixedRealityTookit' game object and check the inspector.

<img src="../Images/SceneUnderstanding/MRTKHierarchy.png" width="300"><br/>
<img src="../Images/SceneUnderstanding/MRTKLocation.png" width="600"><br/>

These options will allow one to configure the `WindowsSceneUnderstandingObserver`.

### Example script

The example script _DemoSceneUnderstandingController.cs_ demonstrates the major concepts in working with the Scene Understanding service.

* Subscribing to Scene Understanding events
* Handling Scene Understanding events
* Configuring the `WindowsSceneUnderstandingObserver` at runtime

The toggles on the panel in the scene change the behavior of scene understanding observer by calling public functions of this sample script.

Turning on *Instantiate Prefabs*, will demonstrate creating objects that size to fit themselves to all [SpatialAwarenessSceneObject](xref:Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness.SpatialAwarenessSceneObject), gathered neatly under a parent object.

<img src="../Images/SceneUnderstanding/Controller.png" width="600"><br/>

### Built app notes

Build and deploy to HoloLens in the standard way. Once running, a number of buttons should appear to play with the features.

Note, their are some pit falls in making queries to the observer. Misconfiguration of a fetch request result in your event payload not containing the expected data. For example, if one dosen't request quads, then no occlusion mask textures will be present. Like wise, no world mesh will appear if the observer is not configured to request meshes. The `DemoSceneUnderstandingController` script takes care of some of these dependencies, but not all.

Saved scene files can be accessed through the [device portal](https://docs.microsoft.com/en-us/windows/mixed-reality/using-the-windows-device-portal) at `User Folders/LocalAppData/[APP_NAME]/LocalState/PREFIX_yyyyMMdd_hhmmss.bytes`. These scene files can be used in editor by specifying them in the observer profile found in the inspector.

<img src="../Images/SceneUnderstanding/BytesInDevicePortal.png" width="800" alt="Device Portal location of bytes file"><br/>

<img src="../Images/SceneUnderstanding/BytesLocationInObserver.png" width="600" alt="Serialized scene bytes in observer"><br/>

## See Also

* https://docs.microsoft.com/en-us/windows/mixed-reality/scene-understanding
* https://docs.microsoft.com/en-us/windows/mixed-reality/scene-understanding-sdk
