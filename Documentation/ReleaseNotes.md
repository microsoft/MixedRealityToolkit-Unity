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

**WindowsApiChecker: IsMethodAvailable(), IsPropertyAvailable() and IsTypeAvailable()**

This version of MRTK adds three new methods to the `WindowsApiChecker` class: `IsMethodAvailable`, `IsPropertyAvailable` and `IsTypeAvailable`. These methods allow for checking for feature support on Windows 10 and are prefered over using the `UniversalApiContractV#_IsAvailable` properties.

### Known issues in 2.4.0

*Coming soon*
