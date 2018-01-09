## [Boundary]()

Scripts that leverage boundary APIs in Unity.
These are useful for rendering the floor for immersive devices.
You can also check if a particular game object is within the established boundary or not.

### [Prefabs](Prefabs)
Prefabs related to the boundary features.

#### FloorQuad.prefab
A simple quad scaled up to 10x that will be rendered as the floor for an immersive device.

### [Scripts](Scripts)

#### BoundaryManager.cs
Places a floor quad to ground the scene.
Allows you to check if your GameObject is within setup boundary on the immersive headset.
Boundary can be configured via the Mixed Reality Portal.

### [Tests](Tests)
To use the scene:

1. Navigate to the Tests folder.
2. Double click on the test scene you wish to explore.
3. Either click "Play" in the unity editor or File -> Build Settings.
4. Add Open Scenes, Platform -> Windows Store, SDK -> Universal 10, Build Type -> D3D, Check 'Unity C# Projects'.
5. Click 'Build' and create an App folder. When compile is done, open the solution and deploy to device.

#### BoundaryTest.unity 
Shows how to check if an object is within boundary and render a floor quad.

We render the floor quad at (0,0,-3) in editor.
There are 4 different cubes in the test scene which try to demonstrate if an object is within or outside the setup boundary.

####

---
##### [Go back up to the table of contents.](../../../README.md)
---
