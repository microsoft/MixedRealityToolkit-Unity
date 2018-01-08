MixedRealityToolkit Examples - Spatial Understanding - Feature Overview
============

## Description
The MixedRealityToolkit.SpatialUnderstanding library encapsulates this technology, allowing you to quickly find empty spaces on the walls, place objects on the ceiling, identify placed for character to sit, and a myriad of other spatial understanding queries.

There are three primary interfaces exposed by the module: topology for simple surface and spatial queries, shape for object detection, and the object placement solver for constraint based placement of object sets. 

In addition to the three primary module interfaces, a ray casting interface can be used to retrieve tagged surface types and a custom watertight playspace mesh can be copied out.

This sample demonstrates many of the features of the Spatial Understanding addon. 

## SETUP
1. Import the MixedRealityToolkit into the project
2. Open "SpatialUnderstanding-FeatureOverview\Scenes\SpatialUnderstandingExample.unity"
3. Run the scene on HoloLens, in the editor using [holographic remoting](https://developer.microsoft.com/en-us/windows/holographic/unity_play_mode), or in the editor with the ObjectSurfaceObserver's room mesh.
