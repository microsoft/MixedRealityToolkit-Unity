# SaveSpatialMapping Example
This example shows how to save the meshes generated using the Spatial Mapping and Spatial Understanding components directly on the HoloLens.

#### SpatialMappingObserver Modifications
Underneath the hood SpatialMappingObserver contains a new method, SaveSpatialMeshes(string fileName). Calling this function with a string name will save a enumeration of meshes to a .room file on the HoloLens.

#### FileSurfaceObserver Modifications
Adds a button for quickly accessing the AppData folder of the Unity project. This is the directory where the FileSurfaceObserver looks for .room files to load.

#### Demo Scenes
SaveSpatialMapping and SaveSpatialUnderstanding scenes are setup exactly the same, except they are using the different spatial meshes.

Tip for testing: Add both scenes to be built into the application. The LevelManager script will make it easy to navigate between scenes if you say, "Load Next Scene". Therefore you can just install the app once and capture the meshes from the different scenes. The Room (.room) files are saved out different for each scene so you will not overwrite the other mesh.

Process: Scan, save, download, and load.

1. Install the app on a device.
2. Scan room.
3. When you're ready to save the mesh say, "Capture Mesh". Text will display to acknowledge the completion of the action.
4. Pull the saved mesh off of the device by going to the Device Portal. Navigate to System > File Explorer > User Files > LocalAppData > [Your App Name] > RoamingState. This is where .room files are saved.
5. Download the .room file to your computer in the location opened by FileSurfaceObserver's "Open File Location" button.
6. Press the Play button.
7. Click in the Game window to make sure it has focus.
8. Press the L key to load the mesh into the scene.
9. Navigate back to the Scene window so you can inspect the mesh loaded.

#### Convert Selection To Wavefront (.obj)
After following a demo to download a .room file you will be able to convert that .room file to a Wavefront (.obj). This flow is based off the highlighted selection within Unity's Hierarchy panel.

Process: Select GameObject(s) with MeshFilters then export.

1. Highlight the GameObject(s) containing MeshFilters you want to export to the Wavefront (.obj) file.
2. In the Unity menu context, click HoloToolkit > Export > Export Selection To Wavefront (.obj).
3. File / Folder menu will open and there will be a file named "Selection.obj" containing all MeshFilters saved in a single file.

Note that this could be done while in play mode. Therefore you could enter play mode, load any saved spatial meshes, select said meshes in the hierarchy panel, then export selection to file.

#### Convert Room (.room) To Wavefront (.obj)
After following a demo to download a .room file you will be able to convert that .room file to a Wavefront (.obj). In this particular process it will directly reference the Room (.room) files saved on the HoloLens.

Process: Download then export.

1. In the Unity menu context, click HoloToolkit > Export > Export Room (.room) To Wavefront (.obj)...
2. A open file dialog will pop up in the location where Room (.room) files are saved / located for your project.
3. Select the Room (.room) file you wish to export.
4. Click Open.
5. File / Folder menu will open and you should see a Wavefront (.obj) file with the same file name as the Room (.room) file selected in step 3.
