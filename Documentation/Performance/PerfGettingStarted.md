# Performance

## Getting started

FIX LINK below
The easiest way to rationalize performance is via framerate or how many times your application can render an image per second. It is important to meet the target framerate as outlined by the platform being targeted(i.e Windows Mixed Reality, Oculus, etc). For example, on HoloLens the target framerate is 60 FPS. Low framerate applications can result in deterioated user experiences such as deterioated (WC) [hologram stabilization](), world tracking, hand tracking, and more. To help developers track and achieve performant framerate, the Mixed Reality Toolkit provides a variety of tools and scripts. 

### Visual Profiler

To continously track performance over the lifetime of development, it is highly recommended to always show a framerate visual while running & debugging an application. The Mixed Reality Toolkit provides the [Visual Profiler](..\Diagnostics\UsingVisualProfiler.md) diagnostic tool which gives realtime information about the current FPS and memory usage in application view. The [Visual Profiler](..\Diagnostics\UsingVisualProfiler.md) can be configured via the [Diagnostics System Settings](..\Diagnostics\DiagnosticsSystemGettingStarted.md) under the [MRTK Profiles Inspector](..\MixedRealityConfigurationGuide.md). 

Furthermore, it is particullarly important to utilize the [Visual Profiler](..\Diagnostics\UsingVisualProfiler.md) to track framerate when running on device vs when running in Unity editor or an emulator. Running on the actual hardware will provide the most accurate performance information for your application. Finally, it is recommended to compile an application MASTER for /...........

(insert screenshot)

### Optimize Window

INSERT LINK*** <br/>
The [MRTK Optimize Window](..\Utilities\OptimizeWindow.md) offers information and automation tools to help mixed reality developers set up their environment for the most performant results and identify potential bottlenecks in their scene & assets. Certain key configurations in Unity can help deliver substantially more performant results for mixed reality projects. 

Generally these settings involve rendering configurations that are ideal for mixed reality. Mixed reality applications are unique compared to traditional 3d Graphics development in that there are two screens (i.e two eyes) to render for the entire scene.

The recommended settings referenced below can be auto-configured in a Unity project by leveraging the [MRTK Optimize Window](..\Utilities\OptimizeWindow.md).

(insert screenshot)

## Recommended settings for Unity

### Single-Pass Instanced Rendering

The default rendering configuration for XR in Unity is [Multi-pass](https://docs.unity3d.com/ScriptReference/StereoRenderingPath.MultiPass.html). This setting instructs Unity to execute the entire render pipeline twice, once for each eye. This can be optimized by selecting 
[Single Pass instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html) instead. This configuration leverages [render target arrays]() to be able to perform a single draw call that instances into the appropriate render target for a particular eye. Furthermore, [Single Pass instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html) allows all rendering to be done in a single execution of the rendering pipeline. Thus, selecting [Single Pass instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html) as the rendering path for a given mixed reality application can cave substantial time on both the CPU & GPU and is the recommended rendering configuration. 

However, in order to issue a single draw call for each mesh to each eye, [GPU instancing]() must be supported for all shaders which will allow the GPU to multiplex draws across both eyes. Unity standard shaders as well as the [MRTK Standard shader](../README_MRTKStandardShader.md) by default contain the necessary instancing instructions in shader code. If writing custom shaders though for Unity, these shader may need to be updated to support [Single Pass instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html).

TODO: insert code for fixing shaders
Give reference to Unity doc

INSERT Reference photo from blog of perf increase*
https://blogs.unity3d.com/2017/11/21/how-to-maximize-ar-and-vr-performance-with-advanced-stereo-rendering/

### Quality settings

Untiy provides presets to control quality of rendering for each platform endpoint. These presets control support grpahical features such as shadows, anit-aliasing, global illumination, and more. It is recommended to lower these settings and optimize the number of calculations performed during rendering. 

Update mixed reality Unity projects to use the "Low Quality" level setting
Project Settings > Quality > Select "Low Quality"

For every Unity scene file, disable realtime Global Illumination
BLAH > Lighting Settings > Uncheck "Realtime Global Illumination"

### Depth buffer sharing (HoloLens)

INSERT LINKS*
If developing for the Windows Mixed Reality platform and in particular HoloLens, enabling *Depth Buffer Sharing* under *XR Settings* can help with [hologram stabilization](). However, processing of the depth buffer can incur a performance cost, particullary if using 24-bit depth format. Thus, it is highly recommended to configure the depth buffer to 16-bit precision. If [z-fighting]() occurs due to the lower bit format, confirm the far clip plane of all cameras is set to the lowest possible value for the application(Unity by default sets a far clip plane of 1000m). On HoloLens, a far clip plane of 50m is generally more than enough for most application scenarios. 

## General Recommendations

Performance can be an ambigous and constantly changing challenge for mixed reality developers and the spectrum of knolwedge to rationlize performance is vast. There are some general recommendations for understanding how to approach performance for an application. 

It is useful to simplify the exeuction of an application into the pieces that run on the CPU or the GPU and thus identify whether an app is bounded by either component.  There can be bottlenecks that span both processing units and some unique scenarios that have to be carefully investigated. However, for getting started, it is good to grasp where an application is executing for the most amount of time. 

### GPU bounded 

Word spell check****
Since most platforms for mixed reality applications are utilizing [sterescopic rendering](), it is very common to be GPU bounded due to the nature of rendering a "double-wide" screen. Mobile mixed reality platform such as HoloLens or ARKit in particular will have mobile-class CPU & GPU processing power. 

When focusing on the GPU, there are generally two important stages that the application must complete every frame
1) Execute the [vertex shader]()
2) Execute the [pixel shader]() (also known as the fragment shader)

Without going into the complex field of computer graphics & rendering pipeline, each shader stage is a program that runs on the GPU to produce the following
1) Vertex shaders transform world-space vertices to coordinates in screen-space (i.e code executed per vertex)
2) Pixel shaders calculate the color to draw for a given pixel and mesh fragment (i.e code execute per pixel)

In regards to performance tuning, it is ussually more fruitful to focus on opitmizing the operations in the pixel shader. An application may only need draw a cube which will just be 8 vertices. However, the screen space that cube occupies is likely on the order of millions of pixels. Thus, reducing a shader stage by say 10 operations can save significantly more work if reduced on the pixel shader than the vertex shader. 

This is one of the primary reasons for leveraging the [MRTK Standard shader](../README_MRTKStandardShader.md) in mixed reality applications as this shader generally executes many less instructions per pixel & vertex than the Unity standard shader while achieving comprable aesthic results.

|          CPU Tasks        |                  GPU Tasks                 |
|---------------------------|--------------------------------------------|
| App simulation logic      | Rendering operations |
| Simplify Physics          | Reduce lighting calculations |
| Simplify Animations       | Reduce polycount & # of drawable objects |
| Manage Garbage Collection | Reduce # of transparent objects |
|                           | Avoid post-processing/full-screen effects  |
|---------------------------|--------------------------------------------|

### Draw call instancing

INSERT LINKS***
One of the most common mistakes in Unity that reduces performance is instancing materials at runtime. If gameobjects share the same material and/or are the same mesh, they can be optimized into single draw calls via techniques such as *[static batching]()*, *[dynamic batching]()*, and *[GPU Instancing]()*. However, Unity will create a new instance of a material, a copy, if developers modify properties on a gameobject's material at runtime. 

For example, if there are 100 cubes in a scene, a developer may want to assign a unique color to each at runtime. The access of *renderer.material.color* in C# will make Unity create a new material in memory for this particular renderer/gameobject. Now each of the 100 cubes has it's own material and thus they cannot be merged together into one draw call, but instead will become 100 draw call requests from the CPU to the GPU. 

SHOW CODE EXAMPLE. 

To overcome this obstacle and yet still assign a unique color per cube, developers should leverage [material property blocks]()

SHOW CODE EXAMPLE

## Unity performance tools
- Unity Profiler
- Unity Frame Debugger

Code to calculate operations stats shaders*

## See also
### Unity
* [Introduction to Unity performance optimization for beginners](https://www.youtube.com/watch?v=1e5WY2qf600)
* [Unity performance optimization tutorials](https://unity3d.com/learn/tutorials/topics/performance-optimization)
* [Unity optimization best practices](https://docs.unity3d.com/2019.1/Documentation/Manual/BestPracticeUnderstandingPerformanceInUnity.html)

### Windows Mixed Reality
* [Recommended Settings for Unity](https://docs.microsoft.com/en-us/windows/mixed-reality/recommended-settings-for-unity)
* [Understanding Performance for Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/understanding-performance-for-mixed-reality)
* [Performance recommendations for Unity](https://docs.microsoft.com/en-us/windows/mixed-reality/performance-recommendations-for-unity)
* [Event Tracing for Windows Unity Guide](https://docs.unity3d.com/uploads/ExpertGuides/Analyzing_your_game_performance_using_Event_Tracing_for_Windows.pdf)






If you are new to Unity performance analysis, there are some great resources that Unity provides that
walk you through the general thought process of how to conduct a performance analysis, along with common
pitfalls and recommendations. As other resources will say, there is no magic solution for performance issues
– every situation is unique – and digging into performance issues involves an iterative process where you
typically identify a problematic scenario, measure the problem (using some sort of profiling tool),
dig into the hotspots (using that profiling tool), test out a fix to that hotspot, and then repeat
until performance is within desired bounds.

**[Recommended Settings for Unity](https://docs.microsoft.com/en-us/windows/mixed-reality/recommended-settings-for-unity)**

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

### Other resources

As a platform built on Unity, all the other performance recommendations for building mixed reality experiences are relevant here:

* [Understanding Performance for Mixed Reality](https://docs.microsoft.com/en-us/windows/mixed-reality/understanding-performance-for-mixed-reality)
* [Performance recommendations for Unity](https://docs.microsoft.com/en-us/windows/mixed-reality/performance-recommendations-for-unity)

## Common Considerations

### Use Single Pass Instanced Rendering 

This recommendation is called out in 
[other](https://docs.microsoft.com/en-us/windows/mixed-reality/recommended-settings-for-unity)
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
