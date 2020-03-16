# Microsoft Mixed Reality Toolkit release notes

- [What's new](#whats-new-in-240)
- [Known issues](#known-issues-in-240)
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

**WindowsApiChecker: IsMethodAvailable(), IsPropertyAvailable() and IsTypeAvailable()**

This version of MRTK adds three new methods to the `WindowsApiChecker` class: `IsMethodAvailable`, `IsPropertyAvailable` and `IsTypeAvailable`. These methods allow for checking for feature support on Windows 10 and are prefered over using the `UniversalApiContractV#_IsAvailable` properties.

### Known issues in 2.4.0

*Coming soon*
