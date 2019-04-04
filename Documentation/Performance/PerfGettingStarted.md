# Performance

## Getting Started

If you are new to Unity performance analysis, there are some great resources that Unity provides that
walk you through the general thought process of how to conduct a performance analysis, along with common
pitfalls and recommendations. As other resources will say, there is no magic solution for performance issues
– every situation is unique – and digging into performance issues involves an iterative process where you
typically identify a problematic scenario, measure the problem (using some sort of profiling tool),
dig into the hotspots (using that profiling tool), test out a fix to that hotspot, and then repeat
until performance is within desired bounds.

**[Introduction to Unity performance optimization for beginners](https://www.youtube.com/watch?v=1e5WY2qf600)**

For people completely new to performance optimization, this is a great video to watch.

**[Unity performance optimization tutorials](https://unity3d.com/learn/tutorials/topics/performance-optimization)**

These tutorials introduce the Unity performance diagnostic tools and then share some
common issues and pitfalls that people new to (and experienced with!) Unity can encounter.

**[Unity optimization best practices](https://docs.unity3d.com/2019.1/Documentation/Manual/BestPracticeUnderstandingPerformanceInUnity.html)**

Additional in-depth best practices that can apply to Unity in general. 

**[Event Tracing for Windows Unity Guide](https://docs.unity3d.com/uploads/ExpertGuides/Analyzing_your_game_performance_using_Event_Tracing_for_Windows.pdf)**

This guide goes into using a more advanced method for diving into performance issues using [Windows
Performance Analyzer](https://docs.unity3d.com/uploads/ExpertGuides/Analyzing_your_game_performance_using_Event_Tracing_for_Windows.pdf)
tools. Note that the guide doesn’t include how to take traces from a HoloLens device, which can be done by using the
[HoloLens device portal](https://docs.microsoft.com/en-us/windows/mixed-reality/using-the-windows-device-portal). 
The rest of the guide, however, is helpful in setting up symbols when diving into the CPU view, along with a concrete set of things
that you can first try as you explore the WPA tools.

### Other Resources

As a platform built on Unity, all the other performance recommendations for building mixed reality experiences are relevant here:

* [Performance recommendations for Unity](https://docs.microsoft.com/en-us/windows/mixed-reality/performance-recommendations-for-unity)
* [Performance recommendations for immersive headset apps](https://docs.microsoft.com/en-us/windows/mixed-reality/performance-recommendations-for-immersive-headset-apps)
* [Performance recommendations for HoloLens apps](https://docs.microsoft.com/en-us/windows/mixed-reality/performance-recommendations-for-hololens-apps)

## Common Considerations

### Use Single Pass Instanced Rendering 

This recommendation is called out in 
[other](https://docs.microsoft.com/en-us/windows/mixed-reality/performance-recommendations-for-immersive-headset-apps)
resources, but is worth repeating here because enabling this is crucial for performant mixed reality applications.
For more information on what Single Pass Instanced Rendering is, check out the Unity docs on 
[Single Pass Stereo rendering](https://docs.unity3d.com/Manual/SinglePassStereoRendering.html)
and [Single Pass instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html).

This [Unity doc](https://docs.unity3d.com/Manual/SinglePassStereoRenderingHoloLens.html) describes how
to turn on Single Pass Instanced Rendering. To give some context, it’s been observed in some cases that
turning on this option has saved ~5ms per frame (and when there is a budget of 16.6ms per frame in order
to achieve 60 FPS, this is clearly a massive improvement). 

Note that in order to enable this rendering option, it's possible that you may need to update your shaders
to support single pass instanced rendering. See the “Adding Single Pass Stereo rendering support to
Shaders” section of this [resource](https://docs.unity3d.com/Manual/SinglePassStereoRendering.html)
for more details. All the built-in MRTK shaders already support single pass instanced rendering.
Other packages may not (for example, Text Mesh Pro shaders).

## Visual Profiler 

The MRTK provides a Visual Profiler, which provides information about the current FPS and memory usage.

It is useful as a tool to identify overall areas for further investigation (for example, if you notice that
looking at specific parts of your experience cause memory to spike or FPS to tank, it can be helpful to
identify that overall part of your app to dig further into). However, it is recommended to turn the
profiler off when doing deeper dives in performance (when you already know what areas aren’t performant),
as the profiler itself adds overhead to the scene. Ultimately when your app ships, it won’t have this
profiler turned on.

The recommended path is to have the visual profiler on during general development to get an overall
sense of performance, and then once particular areas of interest have been identified for further
deep dives (using the Unity profiler, for example), the visual profiler should then be turned off
during those deep dives. 
