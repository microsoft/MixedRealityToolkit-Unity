# General Release Information

This release is targeted for the Unity 2017.x products 
- The recommended Unity version is [2017.4](https://unity3d.com/unity/qa/lts-releases?_ga=2.10765437.818138280.1527115303-289721018.1521153098)
    - The recommended **minimum** Unity version is 2017.1
        - We will investigate reported issues on Unity 5.6, please file them in [GitHub](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues) and tag them with "Unity 5.6
- Unity 2017.2 and newer is required for Immersive headset support
    - Unity 2017.1 can be used for HoloLens projects
- Windows SDK 10.0.16299 is required for Unity 2017.2 and newer
- Visual Studio 2017 (15.3 or newer) is required.
- Windows 10 Fall Creators Update (1709) or newer is required.
    - Windows 10 April 2018 Update (1803) is recommended 

```
Note: When upgrading the toolkit in your project, delete the following before importing the new package.
- Assets\HoloToolkit
- Assets\HoloToolkit-Examples (if present)
```

# Testing notes

The Mixed Reality Toolkit team tests on the recommended and the recommended minimum Unity versions.
- A small amount of testing is performed on other Unity releases
- HoloToolkit-Examples scenes are used for the majority of testing
- Compile testing is performed in the Master configuration for:
    - .NET Scripting
    - IL2CPP

Please see each the [release notes](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases/latest) for information regarding new features, fixes, breaking changes and known issues.