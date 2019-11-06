# Pulse Shader

Use the pulse shader to animate a visual pulse effect over surface reconstruction, articulated hand mesh, or any other meshes.

## Getting started with Pulse Shader
Open PulseShaderExamples.unity scene, and observe the pulsing effect on the spheres, surface reconstruction, and the articulated hand mesh.

Use the SurfacePulse.cs script to animate the pulse effect on the assigned material, or turn on "Auto Pulse" in the material itself.

### Prerequisites

For surface reconstruction, ensure that MRTK_SurfaceReconstruction.mat is assigned under MRTK Settings -> Spatial Awareness -> Display Settings -> Visible Material.
For articulated hand, ensure that MRTK_ArticulatedHandMesh.mat is assigned in ArticulatedHandMesh.prefab, which itself should be assigned in MRTK Settings -> Input -> Hand Tracking -> Hand Mesh Prefab.

## How it works

## Notes
The hand mesh shader uses UVs to map the pulse along the hand mesh, and to fade out the wrist..  The surface reconstruction shader uses the vertex positions to map the pulse.