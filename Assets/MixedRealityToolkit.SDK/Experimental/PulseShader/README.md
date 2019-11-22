# Pulse Shader
![MRTK_SpatialMesh_Pulse](https://user-images.githubusercontent.com/13754172/68261851-3489e200-fff6-11e9-9f6c-5574a7dd8db7.gif)

![MRTK_HandMesh_Pulse2](https://user-images.githubusercontent.com/13754172/68262035-e4f7e600-fff6-11e9-9858-796afd1cabc5.gif)
Use the pulse shader to animate a visual pulse effect over surface reconstruction, articulated hand mesh, or any other meshes.

## Shader and material
**MRTK_SurfaceReconstruction.mat** and **MRTK_ArticulatedHandMeshPulse.mat** uses **SR_Triangles** shader. You can configure various options such as fill color, line color, and pulse color.

## Example scene
Open **PulseShaderExamples.unity** scene, and observe the pulsing effect on the spheres, surface reconstruction, and the articulated hand mesh.

Use the SurfacePulse.cs script to animate the pulse effect on the assigned material, or turn on "Auto Pulse" in the material itself.

## Prerequisites
For surface reconstruction, ensure that MRTK_SurfaceReconstruction.mat is assigned under MRTK Settings -> Spatial Awareness -> Display Settings -> Visible Material.

For articulated hand, ensure that MRTK_ArticulatedHandMeshPulse.mat is assigned in ArticulatedHandMesh.prefab, which itself should be assigned in MRTK Settings -> Input -> Hand Tracking -> Hand Mesh Prefab.

## How it works
The hand mesh shader uses UVs to map the pulse along the hand mesh, and to fade out the wrist. The surface reconstruction shader uses the vertex positions to map the pulse.
