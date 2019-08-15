# Microsoft Mixed Reality Toolkit Release Notes

- [Version 2.0.0](#version-200)

## Version 2.0.0

- [Upgrading projects](#upgrading-projects-to-200)
- [What's new](#whats-new-in-200)
- [Known issues](#known-issues-in-200)

This release of the Microsoft Mixed Reality Toolkit 2.0.0 supports the following devices and platforms.

- Microsoft HoloLens 2
- Microsoft HoloLens (1st gen)
- Windows Mixed Reality Immersive headsets
- OpenVR

The following software is required.

- Microsoft Visual Studio (2017 or 2019) Community Edition or higher
- Windows 10 SDK 18362 or later (installed by the Visual Studio Installer)
- Unity 2018.4, 2019.1 or 2019.2

### Upgrading projects to 2.0.0

Since the RC2 release, there have been several changes that may impact your application projects,
including some files moving to new folder locations. For the smoothest upgrade path in your
projects, please use the following steps.

1. Close Unity
1. Delete all **MixedRealityToolkit** (you may not have all listed folders)
    - MixedRealityToolkit
    - MixedRealityToolkit.Examples
    - MixedRealityToolkit.Extensions
    - MixedRealityToolkit.Providers
    - MixedRealityToolkit.SDK
    - MixedRealityToolkit.Services
    - MixedRealityToolkit.Tools
1. Delete your **Library** folder
1. Re-open your project in Unity
1. Import the new unity packages
    - Foundation - _Import this package first_
    - (Optional) Tools
    - (Optional) Extensions,
    - (Optional) Examples are optional)
1. For each scene in your project
    - Delete **MixedRealityToolkit** and **MixedRealityPlayspace**, if present, from the Hierarchy
    - Select **MixedRealityToolkit -> Add to Scene and Configure**

> [!Important]
> Some profiles have been changed (properties have been added) in 2.0.0. If you have created custom
profiles, please open them to verify that all of the updated properties are correctly configured.

### What's new in 2.0.0

- [Default HoloLens (1st gen) profile](#default-hololens-1st-gen-profile)
- [CoreServices](#coreservices)
- [IMixedRealityRaycastProvider](#imixedrealityraycastprovider)
- [SpatialObjectMeshObserver](#spatialobjectmeshobserver)
- [SceneSystem](#scenesystem)
- [HoloLens 2 Shall Parity](#hololens-2-shell-parity)
- [Input Animation Recording](#input-animation-recording)
- [HandConstraint Solvers](#handconstraint-solvers)
- [UX Controls](#ux-controls)
- [MRTK Standard Shader](#mrtk-standard-shader)
- [Tools](#tools)
- [Service Managers (experimental)](#service-managers-experimental)

#### Default HoloLens (1st gen) profile

We have added a new profile for HoloLens (1st gen) development that includes some of the
recommended MRTK configurations for best performance.

To configure your application for HoloLens (1st gen) optimized settings, set the
Mixed Reality Toolkit's **Active Profile** to **DefaultHoloLens1ConfigurationProfile**.

![Default HoloLens (1st gen) Configuration Profile](../../Documentation/Images/ReleaseNotes/DefaultHoloLens1ConfigurationProfile.png)

#### CoreServices

The [CoreServices](xref:Microsoft.MixedReality.Toolkit.CoreServices) static class works in conjunction with the
MixedRealityServiceRegistry to provide applications with a fast and convenient mechanism to acquire instances of core
services (ex: Input System).

To access extension service instances, use `MixedRealityServiceRegistry.TryGetService<T>`.

#### IMixedRealityRaycastProvider

The Input System was modified to add a reference to an IMixedRealityRaycastProvider.

Specify your desired raycast provider in the Input System's configuration profile. 

![Selecting the Raycast provider](../../Documentation/Images/ReleaseNotes/SelectingRaycastProvider.png)

#### SpatialObjectMeshObserver

We have added the SpatialObjectMeshObserver to improve developer productivity when working
with the Spatial Awareness system. This observer reads mesh data from imported 3D models
and uses them to simulate environmental data from devices such as Microsoft HoloLens 2.

SpatialObjectMeshObserver is not enabled in the default profiles, please see the
[Spatial Awareness Gettniving Started](SpatialAwareness/SpatialAwarenessGettingStarted.md) article
for more information on configuring your application. 

#### HoloLens 2 Shell Parity

This release updates the MRTK to better mirror the featires. behaviors and visuals of the HoloLens 2 shell experience. This [GitHub](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4200) issue describes the changes.

#### System Keyboard

The system keyboard can now be used on all platforms. See the HandInteractionExamples scene in the Examples
package (Demos\HandInteraction\Scenes\HandInteractionDemos.unity) for a demonstration of using the
SystemKeyboardExample script.


#### Launch applications from within your Unity app

On HoloLens 2 and Windows Mixed Reality Immersive applications, you can now launch applications
from within your Uniity application. See the HandInteractionExamples scene in the Examples package
(Demos\HandInteraction\Scenes\HandInteractionDemos.unity) for a demonstration of using the LaunchUri script
to start an external application.

#### SceneSystem

MRTK 2.0.0 has added the [Scene System](SceneSystem/SceneSystemGettingStarted.md) to help with
applications that contain more than once scene.

#### Input Animation Recording
 
MRTK 2.0.0 features a [recording system](InputSimulation/InputAnimationRecording.md) by which head movement and hand tracking
data can be stored in animation files. The recorded data can then be played back using the [input simulation system](InputSimulation/InputSimulationService.md).

#### Hand Menu Graduated from Experimental

The HandConstraint and HandConstraintPalmUp solvers are now an official feature (no longer experimental) and have improved documentation.

The HandConstraintPalmUp solver now has a toggle to enforce the hand’s fingers are coplanar before activating.

#### UX Controls

The following UX controls have been added and/or improved in version 2.0.0. In addition, those in the
following list can now be instantiated and configured from code.

- BoundingBox
- ManipulationHandler
- HandInteractionPanZOom
- Interactable (basic features)

**HoloLens 2 Button**

- Improved many visual details to match the HoloLens 2 shell including
    - Compressing visuals
    - Far interaction support
    - Focus highlight
    - Shader effects
- HoloLens 2 style Round Button has been added

**Fingertip Cursor**

The fingertip cursor has been updated to better match the HoloLens 2 shell.

**BoundingBox**

- Improvements
    - Normalized the handle asset size and scaling logic
    - The handle asset is now sized to 1-meter
    - Default values and examples are updated
- New features
    - Animated Handle by Proximity
    - Match the HoloLens 2 shell behavior
    - You can now make the handles appear only when your hand is close to them
- New example scene
    - The BoundingBoxExample scene shows various types of configurations

Please refer to the [Bounding Box](README_BoundingBox.md) documentation for more details.

**Radial Solver**

There have been improvements on vertical positioning. Check ‘Use Fixed Vertical Position’ to lock the vertical movement to achieve shell-style tag-along behavior. You can see the example of lazy-following tag-along behavior in the ‘ToggleFeaturesPanel’ prefab .

**Clipping Example**

The ClippingExamples scene, demonstates using the MRTK Standard Shader’s new clipping feature.

**Slate**

- Improved
    - Usability of slates by adding the shadow based on the finger proximity
    - ‘Follow Me’ behavior to match the HoloLens 2 shell behavior, using Radial Solver.
- Fixed
    - Border thickness issue fixed on flattend Bounding Box

#### MRTK Standard Shader

The [MRTK Standard Shader](README_MRTKStandardShader) now supports Unity's Lightweight Scriptable render pipeline.

#### Tools

Version 2.0.0 adds a number of new tools to help you build your mixed reality experiences.

**Dependency Window**

A [Dependency Window](Tools/DependencyWindow.md) has been added which displays how assets reference and depend on each other.
This tool can be used to easily determine which assets within a project are not being used.

**Extension Service Creation Wizard**

Making the transition from singletons to services can be difficult. The [Extension Service Creation Wizard](Tools/ExtensionServiceCreationWizard.md)
supplements documentation and sample code by enabling devs to create new services easily

**Optimize Window**

The MRTK [Optimize Window](Tools/OptimizeWindow.md) is a utility to help automate and inform in the process of
configuring a mixed reality project for best performance in Unity.

**Take Screenshot**

A Take Screenshot utility menu item (Mixed Reality Toolkit > Utilities > Take Screenshot) has been added
to capture high resolution screenshots within the editor. Screenshots can be captured with a transparent
clear color for use in easy post compositing of images for documentation or media.

#### Service managers (experimental)

This release adds service managers to enable the light-weight addition of specific Microsoft 
Mixed Reality Toolkit features, such as the Spatial Awareness system, individually.

These service managers are imported as part of the Foundation package and are located in the
MixedRealityToolkit.SDK\Experimental\Features folder and are a work in progress.

Service manager prefabs are provided for the following services.

- BoundarySystem
- CameraSystem
- DiagnosticsSystem
- InputSystem
- SpatialAwarenessSystem
- TeleportSystem (requires the Input System)

To use, drag and drop the desired prefab into your heirarchy and select the configuration
profile. 

> [!Note]
> These service managers are currently experimental, may have issues and
are subject to change. Please file any and all issues you encounter on GitHub

**Updated Architecture Documentation**

The [archtecture documentation](Architecture/Overview.md) is all new for version 2.0.0.

### Known issues in 2.0.0

The sections below highlight some of the known issues in the Microsoft Mixed Reality Toolkit
2.0.0.

**VR/Immersive devices: Content in some demo scenes is placed below the user**

Some demo scenes contained in the Examples package are optimized for HoloLens devices. These scenes
may place objects below the user when run on VR/Immersive devices. To work around this issue, 
select the **Scene Content** object, in the Hierarchy, and set the Transform's Position Y value to **1.5**.

![Adjusting Scene Content Height](../../Documentation/Images/ReleaseNotes/AdjustContentHeight.png)

**Unity 2019: Could not copy the file HolographicAppRemoting.dll**

There is a known issue with version 3.0.0 of the Windows Mixed Reality package for Unity 2019. If
your project has this version installed, you will see the following error when compiling in Microsoft
Visual Studio.

To work around the issues, please check for a newer version or roll back to version 3.0.2 using Window > Package Manager in the Unity editor.

**Runtime profile swapping**

MRTK 2.0.0 does not fully support profile swapping at runtime. This feature is being
investigated for a future release.

## Extension Service Wizard

When using the Extension Service Wizard,  *Generate Inspector* and/or *Generate Profile* are not actually optional. Trying to create an extension service with either of these deselected will result in an error on the following page. Furthermore, the extension service created for the user will create a property for the ScriptableObject profile that was not actually created. This results in a compiler error until the property line is removed. 

Current workaround steps:

1. Ignore error message in Extension Service Wizard
1. Open up the *ExtensionService.cs file created and remove reference to the non-existent profile.

Issue [#5654](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/5654) is tracking this problem.
