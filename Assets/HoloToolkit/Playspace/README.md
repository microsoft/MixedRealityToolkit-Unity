## [Playspace]()

##Scripts that leverage Windows Mixed Reality StageRoot and play space concepts.
These are useful for finding the floor for occluded devices and also finding the root 0,0,0 position so game objects can be placed accurately.
These also help with drawing playspace bounds that you might have setup during the Mixed Reality Portal first run experience.

### [Prefabs](Prefabs)
Prefabs related to the playspace features.

#### FloorQuad.prefab
A simple quad scaled up to 10x that will be rendered as the floor for an occluded device.

### [Scripts](Scripts)
Scripts related to the playspace features.

#### PlayspaceManager.cs
Uses the StageRoot component to ensure we the coordinate system grounded at 0,0,0 for occluded devices.
Places a floor quad as a child of the stage root at 0,0,0.
Will also draw the bounds of your placespace if you set it during the Mixed Reality Portal first run experience.

### [Tests](Tests)
Tests related to the playspace features. To use the scene:

1. Navigate to the Tests folder.
2. Double click on the test scene you wish to explore.
3. Either click "Play" in the unity editor or File -> Build Settings.
4. Add Open Scenes, Platform -> Windows Store, SDK -> Universal 10, Build Type -> D3D, Check 'Unity C# Projects'.
5. Click 'Build' and create an App folder. When compile is done, open the solution and deploy to device.

#### PlayspaceTest.unity 
Shows how to use the StageRoot component and render a floor quad.

Observe the Managers object, where we attach a StageRoot to define our grounding coordinates.
Then we render the floor quad a child of this object at 0,0,0.
This scene will also draw the bounds using a line renderer if you have set them up.
All applications don't need to draw bounds as they are setup during first run but drawing the floor is a good idea to ground your users.
Users can choose to change the StageRoot if they wish to.

####

---
##### [Go back up to the table of contents.](../../../README.md)
---
