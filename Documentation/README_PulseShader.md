# Pulse shader

![MRTK_SpatialMesh_Pulse](https://user-images.githubusercontent.com/13754172/68261851-3489e200-fff6-11e9-9f6c-5574a7dd8db7.gif)

Use the pulse shader to animate a visual pulse effect over surface reconstruction, articulated hand mesh, or any other meshes.

## Shader and material

Following materials use **SR_Triangles** shader. You can configure various options such as fill color, line color, and pulse color.

- **MRTK_Pulse_SpatialMeshBlue.mat** 
- **MRTK_Pulse_SpatialMeshPurple.mat** 
- **MRTK_Pulse_ArticulatedHandMeshBlue.mat** 
- **MRTK_Pulse_ArticulatedHandMeshPurple.mat** 

## Prerequisites

For the spatial mesh example, ensure that MRTK_Pulse_ArticulatedHandMeshBlue.mat or MRTK_Pulse_ArticulatedHandMeshPurple.mat is assigned under MRTK Settings -> Spatial Awareness -> Display Settings -> Visible Material.

For the hand mesh example, ensure that MRTK_Pulse_SpatialMeshBlue.mat or MRTK_Pulse_SpatialMeshPurple.mat is assigned in ArticulatedHandMesh.prefab, which itself should be assigned in MRTK Settings -> Input -> Hand Tracking -> Hand Mesh Prefab.

## How it works

The hand mesh shader uses UVs to map the pulse along the hand mesh, and to fade out the wrist. The surface reconstruction shader uses the vertex positions to map the pulse.

## Spatial Mesh Example - PulseShaderSpatialMeshExample.unity

Similar to HoloLens 2's shell experience, you can point and air-tap with the hand ray to generate a pulsing effect on the spatial mesh. The example scene contains ExampleSpatialMesh object which is a test spatial mesh data for Unity's game mode. This object will be disabled and hidden on the device.

**PulseShaderSpatialMeshHandler.cs** script generates the pulse effect on the spatial mesh at the hit point position if `PulseOnSelect` is true. The  `Auto Pulse` property can also be set to true in the material itself for a repeating animation.  In the example scene, this script is attached to the PulseShaderSpatialMeshParent prefab.  This prefab is referenced under the Spatial Awareness Profile through Runtime Spatial Mesh Prefab property. During runtime, the PulseShaderSpatialMeshParent prefab and is instantiated and added to the spatial mesh hierarchy (only on device, this behavior cannot be observed in the editor).

## Hand Mesh Example - PulseShaderHandMeshExample.unity

This example scene demonstrates the hand mesh visualization using pulse shader. When a hand is detected by the HoloLens device, pulse animation will be triggered once. This visual feedback can increase the user's interaction confidence. 

**PulseShaderHandMeshHandler.cs** script generates pulse effect on the assigned material. By default, 'Pulse On Hand Detected' is checked.