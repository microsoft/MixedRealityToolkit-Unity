# Mixed Reality Toolkit packages

The Mixed Reality Toolkit (MRTK) is a collection of packages that enable cross platform Mixed Reality application development by providing support for Mixed Reality hardware and platforms.

The MRTK ships via the following Unity packages:

- [Foundation](#foundation-package)
- [Extensions](#extensions-package)
- [Examples](#examples-package)
- [Tools](#tools-package)

These packages are released and supported by Microsoft from source code in the [mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release) branch on GitHub.

## Foundation package

The Mixed Reality Toolkit Foundation is the set of code that enables your application to leverage common functionality across Mixed Reality Platforms.

<img src="../../Documentation/Images/Input/MRTK_Package_Foundation.png" width="350px" style="display:block;">  
<sup>MRTK Foundation Package</sup>

The MRTK Foundation is comprised of:

* **Core Package**

The Core Package contains the definitions for all of the common interfaces, classes and data types that are used by all other components. It is highly recommended that applications access MRTK components exclusively through the defined interfaces to enable the highest level of compatibility across platforms.

* **Platform Providers**

The MRTK Platform Provider packages are the components that enable the Mixed Reality Toolkit to target Mixed Reality hardware and platform functionality.

Supported platforms include:

- Windows Mixed Reality
- OpenVR
- Windows Voice

* **System Services**

Core services provide the default implementations for the system service interfaces, defined in the core package.

The MRTK foundation includes the following system services:

- [Boundary System](../Boundary/BoundarySystemGettingStarted.md)
- [Diagnostic System](../Diagnostics/DiagnosticsSystemGettingStarted.md)
- [Input System](../Input/Overview.md)
- [Spatial Awareness System](../SpatialAwareness/SpatialAwarenessGettingStarted.md)
- [Teleport System](../TeleportSystem/Overview.md)

* **Feature Assets**

Feature Assets are collections of related functionality delivered as Unity assets and scripts including user interface controls, Standard assets, and more.

## Extensions package

The extensions package contains additional services and components that extend the functionality of the foundation package.

- [Scene Transition Service](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md)

## Examples package

The examples package contains demos, sample scripts, and sample scenes that exercise functionality in the foundation package. This package contains the [HandInteractionExample scene](../README_HandInteractionExamples.md) (pictured below) which contains sample objects
that respond to various types of hand input (articulated and non-articulated).

![HandInteractionExample scene](../Images/MRTK_Examples.png)

This package also contains eye tracking demos, which are [documented here](../EyeTracking/EyeTracking_ExamplesOverview.md)

More generally, any new feature in the MRTK should contain a corresponding example in the examples package, roughly following
the same folder structure and location.

## Tools package

The tools package contains tools that are useful for creating mixed reality experiences whose code will ultimately not
ship as part of an application.

- [Dependency Window](../Tools/DependencyWindow.md)
- [Extension Service Creation Wizard](../Tools/ExtensionServiceCreationWizard.md)
- [Optimize Window](../Tools/OptimizeWindow.md)
- [Screenshot Utility](../Tools/ScreenshotUtility.md)

## See also

- [Architecture Overview](../Architecture/Overview.md)
- [Systems, Extension Services and Data Providers](../Architecture/SystemsExtensionsProviders.md)


