# Spatial Processing + Solver Surface Magnetism example
This example shows how to make objects smoothly align with the physical surface. For more information about Solver system, please read 


## How to use 
You need to assign same Unity layer to the Spatial mapping’s plane and Solver Surface Magnetism’s Magnetic Surface.

1. Add a new layer for the planes created by Spatial Processing. (to snap to) This example shows a new layer named ‘SpatialProcessingPlane’
 

2. In SpatialMapping prefab, change Physics Layer to the new layer number. In this example, the new layer is 8
 

3. Assign SolverSurfaceMagnetism script to an object. In Magnetic Surface option, select the new layer (SpatialProcessingPlane) that you created. 


4. After initial room scanning, you will be able to see MRTK logo plane is smoothly snapping to the surfaces.


 

