# Microsoft Mixed Reality Toolkit 2.5.0 draft release notes

- [What's new](#whats-new)
- [Breaking changes](#breaking-changes)
- [Updating guidance](Updating.md#upgrading-to-a-new-version-of-mrtk)
- [Known issues](#known-issues)

### What's new

**Oculus Quest XRSDK support**

MRTK now supports running Oculus Quest Headsets and Controllers using the native XR SDK pipeline.

For instructions on how to deploy your device on the Oculus Quest using the new pipeline, see the [Oculus XRSDK Guide](CrossPlatform/OculusQuestMRTK.md)

**Input Simulation Cheat Sheet**
The HandInteractionExamples scene now has a configurable shortcut to show a help page for input simulation

![Input Simulation Cheat Sheet](https://user-images.githubusercontent.com/39840334/86066480-13637f00-ba27-11ea-8814-d222d548f684.gif)

**Input Simulation Eye Gaze with mouse**
Users can now use the Mouse for simulating eye tracking. See the `Eye Simulation Mode` field in the input simulation profile and set it to Mouse. This replaces the previous `Simulate Eye Position` field 

![Eye Gaze Mouse](https://user-images.githubusercontent.com/39840334/87720928-892b5280-c76a-11ea-9411-73ab69fc756c.gif)

**Conical Grab Pointer**

Grab pointers can now be configured to query for nearby objects using a cone from the grab point rather than a sphere. This more closely resembles the behavior from the default Hololens 2
interface, which queries for nearby objects using a cone. The DefaultHoloLens2InputSystemProfile has also been adjusted to use the new `ConicalGrabPointer`.

**TestUtilities package**

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

**Link.xml created in the MixedRealityToolkit.Generated folder**

With the introduction of Unity Package Manger MRTK, MRTK now writes a `link.xml` file to the `Assets/MixedRealityToolkit.Generated` folder, if none is present. It is recommended to add this file (and `link.xml.meta`) be added to source control. Link.xml is used to influence the [managed code stripping](https://docs.unity3d.com/Manual/ManagedCodeStripping.html#LinkXML) functionality of the Unity linker.

More information on the MRTK link.xml file can be found in the [MRTK and managed code stripping](MRTK_and_managed_code_stripping.md) article.

**Enable MSBuild for Unity removed from the configuration dialog**

To prevent the MRTK configuration dialog from repeatedly displaying when `Enable MSBuild for Unity` is unchecked, it has been moved to the `Mixed Reality Toolkit' menu as shown in the following image.

![MSBuild for Unity menu items](Images/ConfigurationDialog/MSB4UMenuItems.png)

This change also adds the ability to remove MSBulid for Unity.

There is a confirmation dialog that will be displayed when selecting `Use MSBuild for Unity dependency resolution`.

![MSBuild for Unity confirmation](Images/ConfigurationDialog/EnableMSB4UPrompt.png)

### Breaking changes

**Rest / Device Portal API**

The `UseSSL` static property has been moved from `Rest` to `DevicePortal`.

If you did this previously...

```csharp
Rest.UseSSL = true
```

Do this now...

```csharp
DevicePortal.UseSSL = true
```

**Link.xml**

If an application was previously using the NuGet distribution of the MRTK, the `link.xml` file has been removed from the Foundation package. To restore code preservation rules, opening the project in Unity once will create a default `link.xml` file in `Assets/MixedRealityToolkit.Generated`. It is recommended that this file (and `link.xml.meta`) be added to source control.

**Transform Constraint Changes**
TargetTransform property has been marked as obsolete as it wasn't used by constraint system. Constraint logic is based on the transform passed into Initialize and Apply methods. Derived user constraints that rely on this property can cache the TargetTransform in their implementation by storing the transform of the constraint component to achieve the same behavior.

The stored initial world pose `worldPoseOnManipulationStart` data type has been changed from MixedRealityPose to MixedRealityTransform, which includes the local scale value of the manipulated object. With this change it's not necessary to separately cache any initial scale values anymore.


### Known issues
