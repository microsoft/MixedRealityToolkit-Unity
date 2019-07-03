# Optimize Window

The MRTK Optimize Window is a utility to help automate and inform in the process of configuring a mixed reality for best [performance](../Documentation/Performance/PerfGettingStarted.md) in Unity. This tool generally focuses on rendering configurations that when set to the correct preset can save milliseconds of processing.

## Setting optimizations

The settings optimization tab covers some of the important rendering configurations for a Unity project. This section can help automate and inform what settings should be changed for the best performing results.

(INSERT PICTURE HERE)

### Single Pass Instanced Rendering

[Single Pass instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html) is the most efficient rendering path for mixed reality applications. This configuration ensures the render pipeline is executed only once for both eyes and that draw calls are instanced across both eyes.

### Depth buffer sharing

To improve [hologram stabilization](..\Hologram-Stabilization.md), developers can share the application's depth buffer which gives the platform information of where and what holograms to stabilize in the rendered scene.

### Depth buffer format

Furthermore, it is recommended to utilize a 16-bit depth format when enabling depth buffer sharing compared to 24-bit. This means lower precision but saves on performance. If [z-fighting](https://en.wikipedia.org/wiki/Z-fighting) occurs because there is less precision in calculating depth for pixels, then it is recommended to move the [far clip plane](https://docs.unity3d.com/Manual/class-Camera.html) closer to the camera (ex: 50m instead of 1000m).

### Real-time Global Illumination

[Real-time Global illumination](https://docs.unity3d.com/Manual/GIIntro.html) in Unity can provide fantastic aesthetic results but at a very high cost. Global illumination lighting is very expensive in mixed reality and thus it is recommended to disable this feature in development.

>Note: This value is set per-scene in Unity and not once across the entire project.

## Scene analysis

The scene analysis tab is designed to inform developers what elements currently in the scene will likely have the biggest impact on performance.

(INSERT PICTURE HERE)

### Lighting analysis

This tool will examine the number of lights currently in the scene as well as any lights that should disable shadows. Shadow casting is a very expensive operation.

### Polygon count analysis

 The tool also provides polygon count statistics. It can be very helpful to quickly identify which GameObjects have the highest polygon complexity in a given scene.

## Shader analysis

sdfsdf
(INSERT PICTURE HERE)
