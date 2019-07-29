# MRTK Examples Hub #

![MRTK Examples Hub](../Documentation/Images/ExamplesHub/MRTK_ExamplesHub.png)

MRTK Examples Hub is a Unity scene that makes it easy to experience multiple scenes. It uses MRTK's Scene System to load & unload the scenes. 

## Scene System Setup ##
1. In the MRTK Profile, enable **Scene System** and select **MRTKExamplesHubSceneSystemProfile** which is located at ``MixedRealityToolkit.Examples/Demos/ExamplesHub/Profiles/`` folder. This Scene System Profile contains the list of the scenes that are defined in the Hub's cube buttons. <br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem1.png" width="550">

2. Check **Use Service Inspectors** in the **Editor** menu. This allows us to select the Scene System service in Unity's Hierarchy.
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem2.png" width="550">

3. Open **DefaultManagerScene** and select **MixedRealitySceneSystem** in the Hierarchy. It will display the Scene System's information in the Inspector panel. On the bottom of the Inspector, it displays the list of the scenes defined in the Scene System Profile. You can click the scene names to load/unload them. 
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem3.png">Example of loading _MRTKExamplesHub_ scene by clicking the scene name in the list.
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem4.png">Example of loading _HandInteractionExamples_ scene.
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem5.png">You can load multiple scenes.

4. To build and deploy, simply build **DefaultManagerScene** with other scenes that are included in the Scene System's list. The inspector also makes it easy to add scenes to the Build Settings.

## How MRTKExamplesHub loads a scene ##
In the _MRTKExamplesHub_ scene, you can find the ``ExamplesHubButton`` prefab. 
There is a **FrontPlate** object in the prefab which contains ``Interactable``. 
Using the Interactable's ``OnClick()`` and ``OnTouch()`` event, it triggers the **LoadContentScene** script's **LoadContent()** function. 
In the **LoadContentScene** script's Inspector, you can define the scene name to load.
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem6.png">
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem8.png" width="550">
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem7.png" width="550">

The script uses the Scene System's LoadContent() function to load the scene. 
Please refer to the [Scene System](SceneSystem/SceneSystemGetingStarted.md) page for more details.
```csharp
MixedRealityToolkit.SceneSystem.LoadContent(contentName, loadSceneMode);
```
 
## Adding additional buttons ##
In the **CubeCollection** object, duplicate (or add) _ExampleHubButton_ prefabs and click **Update Collection** in the ``GridObjectCollection``. 
This will update the cylinder layout based on the new total number of buttons. 
Please refer to the [Object Collection](README_ObjectCollection.md) page for more details.
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem9.png">
<br/><br/><img src="../Documentation/Images/ExamplesHub/MRTK_ExamplesHub_SceneSystem10.png">

After adding the buttons, update the scene name in the **LoadContentScene** script(explained above). 
Add additional scenes to the Scene System's profile.

