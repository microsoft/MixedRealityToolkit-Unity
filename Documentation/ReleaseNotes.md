# Microsoft Mixed Reality Toolkit 2.5.0 release notes

- [What's new](#whats-new)
- [Breaking changes](#breaking-changes)
- [Updating guidance](Updating.md#upgrading-to-a-new-version-of-mrtk)
- [Known issues](#known-issues)

## What's new

### Unity Package Manager (UPM) support

The Mixed Reality Toolkit can now be managed using the Unity Package Manager.

![MRTK Foundation UPM Package](Images/Packaging/MRTK_FoundationUPM.png)

> [!Note]
> There are some manual steps required to import the MRTK UPM packages. Please review [Mixed Reality Toolkit and Unity Package Manager](usingupm.md) for more information. 

### Oculus Quest XRSDK support

MRTK now supports running Oculus Quest Headsets and Controllers using the native XR SDK pipeline. Hand tracking is also supported with the [Oculus Integration Unity package](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) thanks to [Eric Provencher's](https://twitter.com/prvncher) work on MRTK-Quest!

For instructions on how to deploy your device on the Oculus Quest using the new pipeline, see the [Oculus Quest Setup Guide](CrossPlatform/OculusQuestMRTK.md)

### Scrolling Object Collection

The MRTK UX component has been upgraded from an experimental feature and offers more freedom for layouting 3D content of different sizes with added support for objects that have no colliders attached. A new option for disabling content masking was also added, making prototyping easier. 

See [Scrolling Object Collection](README_ScrollingObjectCollection.md) for more information.

![Scrolling Object Collection](https://user-images.githubusercontent.com/16922045/94465118-51537900-01b7-11eb-8f8b-bf864a8fee03.gif)

### Teleport pointer animation, handling, and sound improvements

The teleport pointer now has improved animations and audio feedback. We also improved the handling of the teleport pointer so it handles smoother when transitioning from pointing at nearby surfaces to farther away surfaces.

https://streamable.com/3f222q

### Input Simulation Cheat Sheet
The HandInteractionExamples scene now has a configurable shortcut to show a help page for input simulation

![Input Simulation Cheat Sheet](https://user-images.githubusercontent.com/13754172/93232433-dea8cd80-f7b4-11ea-8500-eaee202f606f.png)

### Input Simulation Eye Gaze with mouse
Users can now use the Mouse for simulating eye tracking. See the `Eye Simulation Mode` field in the input simulation profile and set it to Mouse. This replaces the previous `Simulate Eye Position` field 

![Eye Gaze Mouse](https://user-images.githubusercontent.com/39840334/87720928-892b5280-c76a-11ea-9411-73ab69fc756c.gif)

### Input Simulation Motion Controller in Editor Play Mode

Users can now simulate motion controller just like hands in editor play mode. The trigger, grab and menu buttons are currently supported.

### Conical Grab Pointer

Grab pointers can now be configured to query for nearby objects using a cone from the grab point rather than a sphere. This more closely resembles the behavior from the default Hololens 2
interface, which queries for nearby objects using a cone. The DefaultHoloLens2InputSystemProfile has also been adjusted to use the new `ConicalGrabPointer`.

![Conical Grab Pointer](https://user-images.githubusercontent.com/39840334/82500569-72d58300-9aa8-11ea-8102-ec9a62832d4e.png)

### TestUtilities package

There is now a package (Microsoft.MixedReality.Toolkit.Unity.TestUtilities.2.5.0.unitypackage) that contains the
PlayMode and TestMode test infrastructure that the MRTK uses to create end-to-end tests. This infrastructure has
been extremely handy for the MRTK team itself, and we're excited to have consumers use this to add test coverage
to their own projects.

The following code shows how to create a test hand, show it at a certain location, move it around, and then
pinch and open.

```csharp
TestHand leftHand = new TestHand(Handedness.Left);
yield return leftHand.Show(new Vector3(-0.1f, -0.1f, 0.5f));
yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
yield return leftHand.Move(new Vector3(0.2f, 0.2f, 0));
yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
```

For instructions on how to write a test using these TestUtilities, see this section on
[writing tests](Contributing/UnitTests.md#writing-tests)

For examples of existing tests that use this infrastructure, see MRTK's [PlayModeTests](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/mrtk_development/Assets/MRTK/Tests/PlayModeTests)


### Support for the Leap Motion 4.5.1 Unity Modules

Support for the Leap Motion Unity Modules version 4.5.1 has been added and support for the 4.4.0 assets has been removed. The current supported versions of the Leap Motion Unity Modules are 4.5.0 and 4.5.1.

There is also an additional step for initial Leap Motion integration, see [How to Configure the Leap Motion Hand Tracking in MRTK](CrossPlatform/LeapMotionMRTK.md) for more information.

### Spatial Awareness Mesh Observer better handles customization of materials

With this release, the `Windows Mixed Reality Spatial Mesh Observer` and the `Generic XR SDK Spatial Mesh Observer` components have improved visual material handling. Materials are now preserved when a mesh has been updated by the observer where, previously, they were reset to the default VisibleMaterial as configured in the profile.

This enables developers to alter the mesh material and not have the changes overwritten unexpectedly.

### Link.xml created in the MixedRealityToolkit.Generated folder

With the introduction of Unity Package Manger MRTK, MRTK now writes a `link.xml` file to the `Assets/MixedRealityToolkit.Generated` folder, if none is present. It is recommended to add this file (and `link.xml.meta`) be added to source control. Link.xml is used to influence the [managed code stripping](https://docs.unity3d.com/Manual/ManagedCodeStripping.html#LinkXML) functionality of the Unity linker.

More information on the MRTK link.xml file can be found in the [MRTK and managed code stripping](MRTK_and_managed_code_stripping.md) article.

### Unity 2019.3+: MRTK configuration dialog no longer attempts to enable legacy XR support

To avoid potential conflicts when using Unity's XR Platform, the option to enable legacy XR support has been removed
from the MRTK configuration dialog. If desired, legacy XR support can be enabled, in Unity 2019, using **Edit** > **Project Settings** >
**Player** > **XR Settings** > **Virtual Reality Supported**.

### Reduction in InitializeOnLoad overhead
We've been doing work to reduce the amount of work that runs in InitializeOnLoad handlers, which should lead to
improvements in inner loop development speed. InitializeOnLoad handlers run every time a script is compiled, prior
to entering play mode, and also at editor launch. These handlers now run in far fewer cases, resulting in general
Unity responsiveness improvements.

In some cases there was a tradeoff that had to be made:

- See [Leap Motion Hand Tracking Configuration](CrossPlatform/LeapMotionMRTK.md) for the extra integration step.
- For those who are using ARFoundation, there's now an additional manual step in its getting started steps.
See [ARFoundation](CrossPlatform/UsingARFoundation.md#install-required-packages) for the new steps.
- For those who will be using [Holographic Remoting with legacy XR pipeline](Tools/HolographicRemoting.md#legacy-xr-setup-instructions) on HoloLens 2, there is now a [manual step](Tools/HolographicRemoting.md#dotnetwinrt_present-define-written-into-player-settings) to perform.

###  Bounds control graduated
![Bounds control](../Documentation/Images/BoundsControl/MRTK_BoundsControl_Main.png)
[Bounds control](README_BoundsControl.md) graduated out of experimental and comes with a bunch of new features and tons of bug fixes.
Here a list of the highlights of this update:
- properties are split into configurations which makes it easier to set up bounds control
- configurations can be shared through scriptable objects
- every property / scriptable property is runtime configurable
- bounds control rig isn't recreated on property changes anymore
- translation handles support
- full constraint support through constraint manager
- elastics system integration (experimental)

The old bounding box is now deprecated and existing game objects using bounding box can be [upgraded using the migration tool](Tools/MigrationWindow.md) or the [bounding box inspector](README_BoundingBox.md#migrating-to-bounds-control).

###  Constraint manager component
Constraints can now be used by both, bounds control and object manipulator via the new [constraint manager component](README_ConstraintManager.md). Both components will create a constraint manager per default and process any attached constraints automatically.

Additionally to the automatic behavior constraint manager also comes with a manual mode that lets users decide which constraint should be processed.
For this reason the way we display constraints in the property inspector changed a bit. 

<img src="../Documentation/Images/ConstraintManager/ManualSelection.png" width="600">

The constraints that are applied to the component are now shown as a list in the constraint manager component whereas the component using the constraint manager (either [bounds control](README_BoundsControl.md#constraint-system) or [object manipulator](README_ObjectManipulator.md#constraint-manager)) will now show the selected constraint manager and mode (auto or manual). 
For more information read the [constraint manager](README_ConstraintManager.md) section in our docs.

###  HoloLens 2 Button material update
Updated HoloLens 2 Button's front cage material to remove black color in MRC.

![HoloLens 2 button material update](https://user-images.githubusercontent.com/13754172/94341269-dcf7c900-0042-11eb-9028-e55abd2ead67.png)

### Description panel update, movable example scene
Updated description panel. (SceneDescriptionPanelRev.prefab) New design provides a grabbable top bar which allows the user to adjust/move the entire scene.

![Description panel update](https://user-images.githubusercontent.com/13754172/91176366-28a21480-e71d-11ea-9e80-7e219595de9c.png)

### Spatial mesh visualization - Pulse on air-tap
Updated pulse shader example for the spatial mesh to match HoloLens 2's shell behavior.

![Pulse on air-tap](https://user-images.githubusercontent.com/13754172/90310153-d0536180-df29-11ea-939a-e9572d4f5670.gif)

### Elastic system - Experimental
![Elastic System2](Images/Elastics/Elastics_Main.gif)

MRTK now comes with an [elastic simulation system](Elastics/ElasticSystem.md) that includes a wide variety of extensible and flexible subclasses, offering bindings for 4-dimensional quaternion springs, 3-dimensional volume springs and simple linear spring systems.

Currently the following MRTK components supporting the [elastics manager](xref:Microsoft.MixedReality.Toolkit.Experimental.Physics.ElasticsManager) can leverage elastics functionality:

- [Bounds control](README_BoundsControl.md#elastics-experimental)
- [Object manipulator](README_ObjectManipulator.md#elastics-experimental)  

<img src="https://user-images.githubusercontent.com/5544935/88151572-568cba00-cbaf-11ea-91c2-d6b51829b638.gif" width="38%">
<img src="https://user-images.githubusercontent.com/5544935/88151578-58567d80-cbaf-11ea-8f96-d24f2cf0d6e9.gif" width="45.7%">

### Joystick (Experimental)
An example of joystick interface that can control a large target object.

![Joystick](https://user-images.githubusercontent.com/43013191/86156887-769ef100-babb-11ea-85be-ed6a6aed89d2.png)

### Color picker (Experimental)
An experimental control that makes it easy to change material colors on any object at runtime. 
![Color picker](https://user-images.githubusercontent.com/43013191/85468370-3b536e00-b561-11ea-812c-b3f7d43dd999.png)

![Color picker](https://user-images.githubusercontent.com/43013191/85468994-fa0f8e00-b561-11ea-89f2-0810d1998518.png)

<br/><br/>
## Breaking changes

### Assembly Definition Files Changes

Some asmdef files are changed and are now only supporting Unity 2018.4.13f1 or later. Compilation errors will show up when upating to MRTK 2.5 in earlier versions of Unity. This can be fixed by going to `Assets\MRTK\Providers\XRSDK\Microsoft.MixedReality.Toolkit.Providers.XRSDK.asmdef` in the project window and removing the missing reference in the inspector. Repeat those steps with `Assets\MRTK\Providers\Oculus\XRSDK\Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.asmdef` and `Assets\MRTK\Providers\WindowsMixedReality\XRSDK\Microsoft.MixedReality.Toolkit.Providers.XRSDK.WMR.asmdef`. Note you must revert the changes by replacing those three asmdef files with original (i.e. unmodified) ones when upgrading to Unity 2019.

### IMixedRealityPointerMediator

This interface has been updated to have a new function:

```csharp
void SetPointerPreferences(IPointerPreferences pointerPreferences);
```

If you have a custom pointer mediator that doesn't subclass DefaultPointerMediator, you will need to implement this
new function. See [this issue](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8243) for more background
on why this was added. This was added to ensure that pointer preferences would be explicitly passed to the mediator,
rather than having it be implicitly done based on the presence of a constructor that took a IPointerPreferences.

### Rest / Device Portal API

The `UseSSL` static property has been moved from `Rest` to `DevicePortal`.

If you did this previously...

```csharp
Rest.UseSSL = true
```

Do this now...

```csharp
DevicePortal.UseSSL = true
```

### Link.xml

If an application was previously using the NuGet distribution of the MRTK, the `link.xml` file has been removed from the Foundation package. To restore code preservation rules, opening the project in Unity once will create a default `link.xml` file in `Assets/MixedRealityToolkit.Generated`. It is recommended that this file (and `link.xml.meta`) be added to source control.

### Transform Constraint Changes

TargetTransform property has been marked as obsolete as it wasn't used by constraint system. Constraint logic is based on the transform passed into Initialize and Apply methods. Derived user constraints that rely on this property can cache the TargetTransform in their implementation by storing the transform of the constraint component to achieve the same behavior.

The stored initial world pose `worldPoseOnManipulationStart` data type has been changed from MixedRealityPose to MixedRealityTransform, which includes the local scale value of the manipulated object. With this change it's not necessary to separately cache any initial scale values anymore.

### New Property in IMixedRealityDictationSystem

A new property `AudioClip` has been added to the IMixedRealityDictationSystem interface. The `AudioClip` property enables access to the audio clip associated with the current dictation session. Users must implement the property in their scripts implementing the interface.

### Service Facades turn down

[Services facades](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/06a06778e38da622b37cc299a93f16e143b7bdeb/Assets/MRTK/Core/Inspectors/MixedRealityToolkitFacadeHandler.cs)
are being turned down in 2.5. This feature was originally added to make configuration
of the MRTK profiles easier (by creating fake in-scene GameObjects that represented each of MRTK's
services). In the long run, we want to avoid creating fake in-game objects and trying to keep them
in sync (as data sync and "source of truth" issues are notoriously difficult to scale and get right).

In 2.5, the service facade handlers are kept around to ensure that project upgrade goes smoothly -
any facades that exist in the project will be deleted by the service facade handler to ensure that
scenes opened up in 2.5 get automatically fixed.

The remaining code associated with the service facade feature will be removed in a future release.

### Addition of Motion Controller to Input Simulation Service

Motion Controller simulation is now offered in editor play mode along side the existing hand simulation. To enable this change, many current functions/fields/properties are now marked obsolete, with `InputSimulationService.cs` and `MixedRealityInputSimulationProfile.cs` getting the most significant changes. The logic and behavior of relevant code largely remain the same, and the majority of obsoleted functions etc. are related to replacing reference to "hand" to the more generic term "controller" (e.g. from `DefaultHandSimulationMode` to `DefaultControllerSimulationMode`). Besides getting new names, the return type of certain new functions are updated to match the name/behavior change (e.g. `GetControllerDevice` based on the original `GetHandDevice` now returns `BaseController` instead of `SimulatedHand`).

`IInputSimulationService` now has new properties `MotionControllerDataLeft` and `MotionControllerDataRight`. `MixedRealityInputSimulationProfile` now includes new fields for the keyboard mapping of certain motion controller buttons.

## Known issues

### CameraCache may create a new camera on shutdown

In some situations (e.g. when using the LeapMotion provider in the Unity Editor), it is possible for the CameraCache to re-create the MainCamera on shutdown. Please see [this issue](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8459) for more information.

### FileNotFoundException when examples are imported via Unity Package Manager

Depending on the length of the project path, importing examples via Unity Package Manager may generate FileNotFoundException messages in the Unity Console. The
cause of this is the path to the "missing" file being longer than MAX_PATH (256 characters). To resolve, please shorten the length of the project path.

### No spatializer was specified. The application will not support Spatial Sound.

A "No spatializer was specified" warning will appear if an audio spatializer is not configured. This can occur if no XR package is installed, as Unity includes spatializers in these pacakges.

To resolve, please ensure that:

- **Window** > **Package Manager** has one or more XR packages installed
- **Mixed Reality Toolkit** > **Utilities** > **Configure Unity Project** and make a selection for **Audio Spatializer**

  ![Select Audio Apatializer](Images/ReleaseNotes/SpatializerSelection.png)


### NullReferenceException: Object reference not set to an instance of an object (SceneTransitionService.Initialize)


In some situations, opening `EyeTrackingDemo-00-RootScene` may cause a NullReferenceException in the Initialize method of the SceneTransitionService class.
This error is due to the Scene Transition Service's configuration profile being unset. To resolve, please use the following steps:

- Navigate to the `MixedRealityToolkit` object in the Hierarchy
- In the Inspector window, select `Extensions`
- If not expanded, expand `Scene Transition Service`
- Set the value of `Configuration Profile` to **MRTKExamplesHubSceneTransitionServiceProfile**

<img src="Images/ReleaseNotes/FixSceneTransitionProfile.png" width="500px">


### Oculus Quest

There is currently a known issue for using the [Oculus XR plugin with when targeting Standalone platforms](https://forum.unity.com/threads/unable-to-start-oculus-xr-plugin.913883/).  Check the Oculus bug tracker/forums/release notes for updates.

The bug is signified with this set of 3 errors:

![Oculus XR Plugin Error](https://forum.unity.com/attachments/erori-unity-png.644204/)

### UnityUI and TextMeshPro

There's a known issue for newer versions of TextMeshPro (1.5.0+ or 2.1.1+), where the default font size for dropdowns and bold font character spacing has been altered.

![TMP image](https://user-images.githubusercontent.com/68253937/93158069-4d582f00-f6c0-11ea-87ad-94d0ba3ba6e5.png)

This can be worked around by downgrading to an earlier version of TextMeshPro. See [issue #8556](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8556)
for more details.
