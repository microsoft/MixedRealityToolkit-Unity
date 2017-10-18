# General Release Guidelines

When updating the Mixed Reality Toolkit for Unity in your project always be sure to completely remove all the folders and assets of the toolkit before importing the unity package.  Unity does not handle assets that have been deleted, removed, or renamed.

General releases will list [breaking changes](/BreakingChanges.md) in their descriptions.

## 2017.2 Upgrade Guide for Window Mixed Reality

### What you'll need

- [Unity Editor 2017.2.0f3 MRTP](http://beta.unity3d.com/download/edcd66fb22ae/download.html) or later
- Running the Windows Fall Creator's Update Build 1709 or later
- Visual Studio 2017 build 15.3 or later
- Window 10 SDK 10.0.16299.0 or later

### List of breaking changes
- None to date.  See [Unity's upgrade guide](https://docs.unity3d.com/Manual/UpgradeGuide20172.html) for Unity Specific changes.

## 2017.1 Upgrade Guide

### What you'll need

- [Unity Editor 2017.1.2f1](https://unity3d.com/unity/whats-new/unity-2017.1.2) or later
- Window Build 1603 or later
- Visual Studio 2015 or later
- Windows 10 SDKs 10.0.10240.0 though 10.0.15063.0

### List of breaking changes
- `TryGetPosition` is now `TryGetPointerPosition`
- `TryGetOrientation` is now `TryGetPointerRotation`
- `SpeechKeywordRecognizedEventData` is now `SpeechEventData`
- All other changes should throw `Obsolete` warnings with suggested updates.
- Renamed feature folders
- Merged Test and Example folders
- Removed `MicStream` scripts and libraries until they pass WACK
- `GameObjects` that utilize the `WorldAnchorStore` need to have unique names.
- `SetGlobalListeners` now registers/unregisters during OnEnable/OnDisable.
