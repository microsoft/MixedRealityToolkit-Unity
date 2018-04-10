# Two Hand Manipulation & normal mode Bounding Box
![Two Hand Manipulation](/External/ReadMeImages/MRTK_TwoHandManipulation.jpg)
This example shows how you can make any object interactable with [TwoHandManipulatable script](/Assets/HoloToolkit/Input/Scripts/Utilities/Interactions/TwoHandManipulatable.cs). This script allows for an object to be movable, scalable, and rotatable with one or two hands. You may also configure the script on only enable certain manipulations. The script works with both HoloLens' gesture input and immersive headset's motion controller input.

**Demo Video**: https://gfycat.com/ColossalEnlightenedAntipodesgreenparakeet

The example also includes Bounding Box for the normal mode. In normal mode, Bounding Box gives the visual boundary of the object when you interact with the object. You can find this behavior in the cliff house. (Grab and move, rotate, scale with two motion controllers)

![Bounding Box Normal Mode](/External/ReadMeImages/MRTK_BoundingBoxNormalMode.jpg)


### Known issues ###
- Adjust mode Bounding Box and App Bar will be added through different Pull Request.
- In the immersive headset, current input system only shows single pointer cursor. While holding an object with the first controller, pointing and seleting the object with the second controller will activate the manipulation behavior. Multi-pointer cursor visualization will be updated through different Pull Request. 


### Unity Scene ###
[TwoHandManipulationTest.unity](/Assets/HoloToolkit-Examples/Input/Scenes/TwoHandManipulationTest.unity)
![TwoHandManipulation Scene](/External/ReadMeImages/MRTK_TwoHandManipulationScene.jpg)

In the Hierarchy panel, you can find multiple objects that have TwoHandManipulatable script under SceneContent. Notice how each example object has a collider on it. The collider defines the 'hittable area' for the manipulatable--grabbing any collidable that is on the script's GameObject or any descendant will activate the script. 

### TwoHandManipulatable Script ###
![TwoHandManipulation Script](/External/ReadMeImages/MRTK_TwoHandManipulationScript.jpg)

Simply assign TwoHandManipulatable script to any GameObject to make it manipulatable.
The script has several configurable options:
- **HostTransform**: Use this to specify the transform that the scripts manipulates. By default it is the GameObject that this script is on.
- **Bounding Box Prefab**: To visualize the object bounding box, drop the [BoundingBoxBasic.prefab](/Assets/HoloToolkit/UX/Prefabs/BoundingBoxes/BoundingBoxBasic.prefab) here. This is optional.

- **Manipulation Mode**: Specify the enabled manipulations.

- **Constraint On Rotation**: If rotation is enabled, only rotate on these axes.

- **One Handed Movement**: Specify whether you can use just one hand to move the object.



### Bounding Box normal mode visualization ###
![BoundingBox Basic Prefab](/External/ReadMeImages/MRTK_BoundingBoxBasicPrefab.jpg)


**TwoHandManipulatable** script uses optional [**BoundingBoxBasic prefab**](/Assets/HoloToolkit/UX/Prefabs/BoundingBoxes/BoundingBoxBasic.prefab) to visualize the borders on manipulation interaction. It shows/hides BoundingBoxBasic in **OnManipulationStarted** and **OnManipulationEnded** event. **TwoHandManipulatable** script can work without BoundingBox visualization. Simply don't assign the BoundingBoxBasic prefab in the Inspector panel.

