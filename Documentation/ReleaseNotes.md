# Microsoft Mixed Reality Toolkit release notes

- [What's new](#whats-new-in-240)
- [Known issues](#known-issues-in-240)
- [Breaking changes](#breaking-changes-in-240)
- [Updating guidance](Updating.md#updating-230-to-240)

This release of the Microsoft Mixed Reality Toolkit supports the following devices and platforms.

- Microsoft HoloLens 2
- Microsoft HoloLens (1st gen)
- Windows Mixed Reality Immersive headsets
- OpenVR
- (Experimental) Unity 2019.3 XR platform
- Mobile AR via Unity AR Foundation
  - Android
  - iOS

The following software is required.

- [Microsoft Visual Studio](https://visualstudio.microsoft.com) (2017 or 2019) Community Edition or higher
- [Windows 10 SDK](https://developer.microsoft.com/windows/downloads/windows-10-sdk) 18362 or later (installed by the Visual Studio Installer)
- [Unity](https://unity3d.com/get-unity/download) 2018.4 LTS or 2019 (2019.3 recommended)

**NuGet requirements**

If importing the [Mixed Reality Toolkit NuGet packages](MRTKNuGetPackage.md), the following software is recommended.

- [NuGet for Unity 2.0.0 or newer](https://github.com/GlitchEnzo/NuGetForUnity/releases/latest)

### What's new in 2.4.0

**MRTK folder layout changes**

This version of MRTK modifies the layout of the MRTK folder structure. This change encapsulates all MRTK code into a single folder hierarchy and reduces the total path length of all MRTK files.

| Previous Folder | New Folder |
| --- | --- |
| MixedRealityToolkit | MRTK\Core |
| MixedRealityToolkit.Examples | MRTK\Examples |
| MixedRealityToolkit.Extensions | MRTK\Extensions |
| MixedRealityToolkit.Providers | MRTK\Providers |
| MixedRealityToolkit.SDK | MRTK\SDK |
| MixedRealityToolkit.Services | MRTK\Services |
| MixedRealityToolkit.Tests | MRTK\Tests |
| MixedRealityToolkit.Tools | MRTK\Tools |

> [!IMPORTANT]
> The `MixedRealityToolkit.Generated` contains customer generated files and remains unchanged.

**Unity Profiler markers**

This version of MRTK has added Unity Profiler markers to the input system and data providers. These markers provide detailed information on where time is spent in
the MRTK input system that can be used to help optimize applications.

Markers take the format of "[MRTK] ClassWithoutNamespace.Method".

_example image coming soon_

**WindowsApiChecker: IsMethodAvailable(), IsPropertyAvailable() and IsTypeAvailable()**

This version of MRTK adds three new methods to the [`WindowsApiChecker`](xref:Microsoft.MixedReality.Toolkit.Windows.Utilities.WindowsApiChecker) class: `IsMethodAvailable`, `IsPropertyAvailable` and `IsTypeAvailable`. These methods allow for checking for feature support on Windows 10 and are prefered over using the `UniversalApiContractV#_IsAvailable` properties.

**Helpers to get text input fields working with MixedRealityKeyboard for UnityUI, TextMeshPro (Experimental)**

<img src="https://user-images.githubusercontent.com/168492/77582981-86e07800-6e9d-11ea-86e5-bf2c0840296c.png" width="300" />

We have introduced two helper components, [`UI_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.UI_KeyboardInputField) and [`TMP_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.TMP_KeyboardInputField) that can be added to text input fields in Unity UI to enable the HoloLens 2 and Windows Mixed Reality Keyboard to show up when the fields are clicked.

For more information, see - [Mixed Reality Keyboard Helpers](../Assets/MRTK/SDK/Experimental/MixedRealityKeyboard/README_MixedRealityKeyboard.md).

**Grid Object Collection Alignment Options**

<img src="https://user-images.githubusercontent.com/39840334/79289136-5c228780-7e7d-11ea-82b4-07959e42c3ed.gif" width="300" />

We have added the ability to choose how the elements in the grid are aligned, whether they are aligned in the center or along the left/right axis (top/bottom axis when doing row then column layout)

### Breaking changes in 2.4.0

**Eye gaze setup change**

This version of MRTK modifies the steps required for eye gaze setup. The _'IsEyeTrackingEnabled'_ checkbox can be found in the gaze settings of the input pointer profile. Checking this box will enable eye based gaze, rather then the default head based gaze.

For more information on these changes and complete instructions for eye tracking setup, please see the [eye tracking](EyeTracking/EyeTracking_BasicSetup.md) article.

### Known issues in 2.4.0

*Coming soon*
