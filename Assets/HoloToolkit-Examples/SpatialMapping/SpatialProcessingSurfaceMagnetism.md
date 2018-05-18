# Spatial Processing + Solver Surface Magnetism example
This example shows how to make objects smoothly align with the physical surface. For more information about Spatial Processing and Solver system, please read these README files.
- [Spatial Processing](/Assets/HoloToolkit/SpatialMapping/README.md)
- [Solver System](/Assets/HoloToolkit-Examples/Utilities/Readme/README_SolverSystem.md)

## Demo Video
https://gfycat.com/PitifulRareKodiakbear

## Scene
[SpatialProcessingSurfaceMagnetism.unity](/Assets/HoloToolkit-Examples/SpatialMapping/Scenes/SpatialProcessingSurfaceMagnetism.unity)

## How to use 
You need to assign same Unity layer to the Spatial mapping’s plane and Solver Surface Magnetism’s Magnetic Surface.

1. Add a new layer for the planes created by Spatial Processing. Mixed Reality Toolkit's **Apply Mixed Reality Projcet Settings** automatically addes a new layer **31:Spatial Mapping**. <img src="/External/ReadMeImages/MRTK_SurfaceMagnetism1.png" width="550"> <img src="/External/ReadMeImages/MRTK_SurfaceMagnetism2.png" width="300"> <img src="/External/ReadMeImages/MRTK_SurfaceMagnetism3.png" width="550"> <img src="/External/ReadMeImages/MRTK_SurfaceMagnetism4.png" width="300">

2. In **SpatialMapping** prefab, make sure the **Physics Layer** is set to **31**. <br/><img src="/External/ReadMeImages/MRTK_SurfaceMagnetism5.png" width="550">

3. Assign **SolverSurfaceMagnetism** script to an object. In **Magnetic Surface** option, select the **SpatialMapping** layer.<br/><img src="/External/ReadMeImages/MRTK_SurfaceMagnetism6.png" width="550">

4. After initial room scanning, you will be able to see MRTK logo plane smoothly snaps to the surfaces.

 

