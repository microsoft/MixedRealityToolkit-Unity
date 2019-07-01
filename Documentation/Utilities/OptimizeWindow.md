# Optimize Window

What is it? 

The MRTK Optimize Window is a utility to help automate and inform in the process of configuring a mixed reality for best performance in Unity. This tool generally focuses on rendering configurations that when set to the correct preset can save milliseconds of processing. 

[Performance](../Documentation/Performance/PerfGettingStarted.md)

(INSERT PICTURE HERE)

## Project settings

### Single Pass Instanced Rendering

[Single Pass instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html) is the most efficent rendering path for mixed reality applications. This configuration ensures the render pipeline is executed only once for both eyes and that draw calls are instanced acrossed both eyes. 

### Depth buffer sharing

To improve hologram stabilization, developers can share the application's depth buffer which gives the platform information of where and what holograms to stabilize in the rendered scene. 

## Scene settings

### Global Illumination

[Global illumination]() in Unity can provide fantastic aesthic results but at a very high cost. Realtime global illumination is very expensive in mixed reality and thus it is recommended to disable global illumination for every scene. 

### Scene diagnosis 

sdfsdf

## Shader settings

### Shader optimization
sdfsdf
