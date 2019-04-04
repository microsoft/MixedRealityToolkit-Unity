# Using the Visual Profiler

The VisualProfiler provides an easy to use, in application view of a mixed reality application's performance. The profiler is supported on all Mixed Reality Toolkit platforms, including:

- Microsoft HoloLens
- Microsoft HoloLens 2
- Windows Mixed Reality Immersive headsets
- OpenVR

While developing an application, be sure to focus on multiple parts of the scene as the Visual Profiler displays data relative to the current view. 

> Focus attention on portions of the scene with complex objects, particle effects or activity. These and other factors often contribute to reduction in application performance and a less than ideal user experience.

## Visual Profiler Interface

![Visual Profiler Interface](../../External/ReadMeImages/VisualProfiler.png)

The Visual Profiler interface includes the following components:

- [Frame Rate](#frame-rate)
- [Frame Time](#frame-time)
- [Frame Graph](#frame-graph)
- [Memory Utilization](#memory-utilization)

### Frame Rate

In the upper left corner of the interface is the frame rate, measured in frames per second. For the best user experience and comfort, this value should be as high as possible. 

The specific platform and hardware configuration will play a significant role in the maximum achievable frame rate. Some common target values include:

- Microsoft HoloLens: 60
- Windows Mixed Reality Ultra: 90

### Frame Time

<< >>

### Frame Graph

The frame graph provides a graphical display of the application frame rate history.

![Visual Profiler Frame Graph](../../External/ReadMeImages/VisualProfilerMissedFrames.png)

> When using the application, look for missed frames. Missed frames indicate the application is not hitting its target frame rate and may need optimization work.

### Memory Utilization

The memory utilization display allows for easy understanding of how the current view is impacting an application's memory consumption. 

![Visual Profiler Frame Graph](../../External/ReadMeImages/VisualProfilerMemory.png)

> When using application, look for total memory usage. Key indicators include: nearing the memory limit and rapid changes in usage.

## Customizing the Visual Profiler

The Visual Profiler's appearance and behavior are customizable via the diagnostics system profile. Please see [Configuring the Diagnostics System](ConfiguringDiagnostics.md) for more information.

## See Also

- [Diagnostics System](DiagnosticsSystemGettingStarted.md)
- [Configuring the Diagnostics System](ConfiguringDiagnostics.md)
