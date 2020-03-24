# SurfaceMeshesToPlanes

We provide the `SurfaceMeshesToPlanes.cs` script as an utility for simple Planefinding applications in MRTK. This script processes mesh data from the Spatial Awareness system and builds planes for Floors, Ceilings, Walls and Platforms in a scene. This script is fully compatible with Hololens 1 and 2.

## Import Planefinding DLL
The `Planefinding.dll` is not included by default in MRTK. To generate this DLL go to [the native Mixed Reality Toolkit repo](https://github.com/microsoft/MixedRealityToolkit/tree/master/SpatialMapping/PlaneFinding) and clone the project. Build the `Planefinding.sln` project and copy the resulting binaries  into a Plugins folder in your Unity assets. The script should correctly resolve the DLL after this.

## Setup
To use the SurfaceMeshesToPlanes functionality, you can either choose to attach the script to a GameObject in your scene or add the new `SurfacePlaneObserver` to the Spatial Awareness system. Both approaches are outlined below:

1. Click on the *Spatial Awareness System* tab in the *Mixed Reality Toolkit* GameObject and ensure that the system is enabled
2. Add the `WindowsMixedRealitySpatialMeshObserver` (for device) or `SpatialMeshObserver` (to load a mesh in editor) as an observer to the Spatial Awareness system

### Option 1: Attach script to GameObject
Create a new Unity GameObject in your scene and attach the `SurfaceMeshesToPlanes.cs` script as a component.

### Option 2: Use the `SurfacePlane` Observer
Add the `SurfacePlaneObserver` to the Spatial Awareness system. To configure the observer, edit the values in the `DefaultSurfacePlaneObserverProfile`