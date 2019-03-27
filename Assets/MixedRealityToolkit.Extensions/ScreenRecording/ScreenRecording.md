# Screen Recording
Microsoft.MixedReality.Toolkit.Extensions.ScreenRecording contains interface definitions and helper classes for capturing screen content.

## Interfaces
* [**IRecordingService**](IRecordingService.cs) - A recording service is a component capable of recording screen content to a video file. Different platforms (HoloLens, iOS, Android, etc) have different recording service implementations.

* [**IRecordingServiceVisual**](IRecordingServiceVisual.cs) - A recording service visual is aware of IRecordingService functionality and is responsible for the UI/user interactions that kick off any recording logic. 

## Classes
* [**AndroidRecordingService**](AndroidRecordingService.cs) - A screen recording service for the Android platform.

* [**iOSRecordingService**](iOSRecordingService.cs) - A screen recording service for the iOS platform.
