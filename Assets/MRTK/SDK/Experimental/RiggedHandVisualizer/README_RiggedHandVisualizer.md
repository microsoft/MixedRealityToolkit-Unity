# RiggedHandVisualizer
<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_Main.png" height="450px"  style="display:inline;">

The RiggedHandVisualizer can be used to control a SkinnedMesh to visualize the hands. This works on HoloLens 2 with detected hands and in Editor with inputsimulation or with UltraLeap sensor. 

## Windows Mixed Reality headset with attached Ultraleap sensor
<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_Leapmotion.gif" height="450px"  style="display:inline;">

## Unity Editor InputSimulation
<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_InputSimulation.gif" height="450px"  style="display:inline;">

## Example scene
You can find and example in the **RiggedHandVisualizer** scene under:
[MixedRealityToolkit.Examples/Experimental/RiggedHandVisualizer/Scenes](../../../Examples/Experimental/RiggedHandVisualizer/Scenes)

## Configuring the RiggedHandVisualizer
To enable the use of rigged hands visualization navigate in the MixedRealityToolkit configuration tree to

**MixedRealityToolkit > Controllers > Input > Controller Visualization Settings > Global Left Hand Visualizer**  

Set it to use a prefab with the skinned mesh and RiggedHandVisualizer component.  Do the same for the right hand.
RiggedHandLeft and RiggedHandRight prefabs are included in the MRTK as defaults, but you can also configure your own skinned mesh models.

<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_ControllerVisualizationSettings.png" height="300px" style="display:inline;">

## Setting up a custom rigged mesh

<img src="../../../../../Documentation/Images/InputSimulation/MRTK_Core_Input_Hands_JointNames.png" height="350px"  style="display:inline;">

-Create a rigged hand mesh with a bone hierarchy that consists of 5 joints per bone 
-Matching orientation 
<img src="../../../../../Documentation/Images/RiggedHandVisualizer/MRTK_RiggedHandVisualizer_PrefabSetup.png" height="450px"  style="display:inline;">



