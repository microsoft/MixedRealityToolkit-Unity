# Spatial Processing + Solver Surface Magnetism example
This example shows how to make objects smoothly align with the physical surface. For more information about Solver system, please read 


## How to use 
You need to assign same Unity layer to the Spatial mapping’s plane and Solver Surface Magnetism’s Magnetic Surface.

1. Add a new layer for the planes created by Spatial Processing. (to snap to) This example shows a new layer named ‘SpatialProcessingPlane’
<img src="/External/ReadMeImages/MRTK_SurfaceMagnetism1.png" width="550">
<img src="/External/ReadMeImages/MRTK_SurfaceMagnetism2.png" width="550">

2. In SpatialMapping prefab, change Physics Layer to the new layer number. In this example, the new layer is 8
<img src="/External/ReadMeImages/MRTK_SurfaceMagnetism3.png" width="550">

3. Assign SolverSurfaceMagnetism script to an object. In Magnetic Surface option, select the new layer (SpatialProcessingPlane) that you created. 
<img src="/External/ReadMeImages/MRTK_SurfaceMagnetism4.png" width="550">

4. After initial room scanning, you will be able to see MRTK logo plane is smoothly snapping to the surfaces.

 

