# Screen Recording
Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording contains interface definitions and helper classes for capturing screen content.

## Interfaces
* [**IRecordingService**](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingService) - A recording service is a component capable of recording screen content to a video file. Different platforms (HoloLens, iOS, Android, etc) have different recording service implementations.

* [**IRecordingServiceVisual**](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.IRecordingServiceVisual) - A recording service visual is aware of IRecordingService functionality and is responsible for the UI/user interactions that kick off any recording logic. 

## Classes
* [**AndroidRecordingService**](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.AndroidRecordingService) - A screen recording service for the Android platform.

* [**iOSRecordingService**](xref:Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording.iOSRecordingService) - A screen recording service for the iOS platform.
