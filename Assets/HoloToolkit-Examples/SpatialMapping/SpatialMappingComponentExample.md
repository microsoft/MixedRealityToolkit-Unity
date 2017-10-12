# SpatialMappingComponent Example
This example shows how to use the built-in Unity components for spatial mapping: SpatialMappingRenderer and SpatialMappingCollider. It shows an example of a static play space which preserves physics around it (marked by a cube) while maintaining physics and wire frame rendering of Spatial Mapping around the camera. 

Additionally, you can tap to drop a cube in front of the camera with a Rigidbody component to interact with physics. As you drop cubes, you will notice that only the cubes that are dropped within the static play space will maintain a connection with the floor. If you wander too far from the static play space, cubes dropped outside of the space will eventually fall through the floor and disappear as you walk away from them.

#### SpatialMappingCollider:
Use this component for performing physics collisions with the Spatial Mapping mesh.

The example scene contains two 'SpatialMappingCollider' components. One is attached to the MainCamera object, and the second is attached to the 'StaticPlaySpaceLocation' object. As the user moves around, they can perform an 'air tap' gesture to drop cubes on the floor.

#### SpatialMappingRenderer:
Use this component for rendering the Spatial Mapping mesh.

The example scene has a 'SpatialMappingRenderer' attached to the MainCamera object. As the user moves around, a wireframe mesh will appear wherever they go. The color of the mesh will change based on distance from user.

#### Design Considerations

1. If you want spatial mapping to work wherever the user travels, attach the components to the camera and the specified bounds will move with the user. 
2. If you want collisions to continue working even after walking away, you may want to leave a second SpatialMappingCollider around the play space where collisions should continue. 
3. If you want physics collisions and to render the Spatial Mapping mesh for an area, you should add both a SpatialMappingCollider and a SpatialMappingRenderer

Though all of the defaults are usable out of the box with no customization, you can also customize the component to your scenario. Through script, additions such as PlaneFinding.cs can also be used with these components.

These components by default implement caching of removed Spatial Mapping meshes such that removed meshes will still be present for at least 10 updates after they are actually removed. This number and the feature can be configured via script. This feature enables mesh a great distance from the user to not be removed as well as quick re-hydration of mesh only removed as a user moved away and back to a single location.