# RiggedHandVisualizer
<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_Main.png" width="600px"  style="display:inline;">

The RiggedHandVisualizer can be used to control a SkinnedMesh to visualize the hands.  
This works on HoloLens 2 with detected hands, when using the Ultraleap sensor or with input simulation in the Unity editor. 

### Windows Mixed Reality headset with attached Ultraleap sensor
<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_Leapmotion.gif" width="600px"  style="display:inline;">

### Unity Editor InputSimulation
<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_InputSimulation.gif" width="600px"  style="display:inline;">

## Example scene
You can find and example in the **RiggedHandVisualizer** scene under:
[MixedRealityToolkit.Examples/Experimental/RiggedHandVisualizer/Scenes](../../../Examples/Experimental/RiggedHandVisualizer/Scenes)

## Using the RiggedHandVisualizer in your scene
To enable the use of rigged hand visualization navigate to

**MixedRealityToolkit > Controllers > Input > Controller Visualization Settings > Global Left Hand Visualizer**  

Set it to use a prefab with the skinned mesh and RiggedHandVisualizer component.  Do the same for the right hand.
RiggedHandLeft and RiggedHandRight prefabs are included in the MRTK as defaults, but you can also configure your own skinned mesh models.

<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_ControllerVisualizationSettings.png"  width="600px" style="display:inline;">

## Setting up a custom rigged mesh

The left and right hand use separate models with the following joint structure. Note that the wrist is often modelled at location of the palm.  
<img src="../../../../../Documentation/Images/InputSimulation/MRTK_Core_Input_Hands_JointNames.png" height="400px" style="display:inline;">

After importing the model in Unity create a prefab that uses it and add the RiggedHandVisualizer component. Assign the wrist and palm transforms. For each finger the metacarpal transform has to be assigned to the corresponding field. The rest of the finger transforms are assumed to be hierarchical children of the metacarpal transform.   

<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_PrefabSetup.png" width="600px"  style="display:inline;">

Since models can be imported with different orientations you may need to apply a correction.
Configure **Model Finger Pointing** and **Model Palm Facing** values with the cardinal axes that they are closest to.
The easiest way to find these axes is to look at the model in it's default orientation after importing it in Unity. Make sure palm and wrist nodes have an orientation of **0, 0, 0**
In the example below you can see that the palm is pointing in the Y-direction (green arrow) so the **Model Palm Facing** is set to **0, 1, 0**. The fingers are pointing in the negative X-direction (red arrow) so the **Model Finger Pointing** is set to **-1, 0, 0**.





