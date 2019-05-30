# Requirements
### HoloLens
1. Windows PC
2. HoloLens
3. [Visual Studio 2017](https://visualstudio.microsoft.com/vs/) installed on the PC
4. [Unity](https://unity3d.com/get-unity/download) installed on the PC
5. [AzureSpatialAnchors v1.1.1](https://github.com/Azure/azure-spatial-anchors-samples/releases/tag/v1.1.1)

### Android
1. Windows PC
2. Android Device that supports [AR Core](https://developers.google.com/ar/discover/supported-devices)
3. [Android Studio](https://developer.android.com/studio)
4. [ARCore v1.7.0](https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.7.0) (Note: only v1.7.0 has been tested, use other versions at your own risk)
5. [AzureSpatialAnchors v1.1.1](https://github.com/Azure/azure-spatial-anchors-samples/releases/tag/v1.1.1)

## Before building
1. Obtain your HoloLens's ip address from the settings menu via Settings -> Network & Internet -> Wi-Fi -> Hardware Properties.
2. Setup an [Azure Spatial Anchors account](https://docs.microsoft.com/en-us/azure/spatial-anchors/quickstarts/get-started-unity-hololens) and obtain the Account Domain, Account ID and the Primary Key.
3. Import [ARCore v1.7.0](https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.7.0) and [AzureSpatialAnchors v1.1.1](https://github.com/Azure/azure-spatial-anchors-samples/releases/tag/v1.1.1) to your Unity project.
4. In your Unity project, call Spectator View -> Update All Asset Caches to prepare content for state synchronization.

### HoloLens scene setup
5. Add the SpectatorView.ASA.HoloLens prefab to the scene you intend to run on the HoloLens device.
6. Add a GameObjectHierarchyBroadcaster to the root game object of the content you want synchronized. 
7. In the unity inspector, set 'Broadcasted Content' in the Spectator View script to be the root game object that now contains the GameObjectHierarchyBroadcaster.
8. Add a parent game object to your unity camera.
9. In the unity inspector, set 'Parent Of Main Camera' in the Spectator View script to be the parent game object you just created.
10. In the unity inspector, set the Account Domain, Account Id and Account Key for the Spatial Anchors Localizer using values you obtained creating an azure spatial anchors account above.
11. Build and deploy the application to your HoloLens device.

### Android scene setup
12. Open the SpectatorView.ASA.Android unity scene in your unity project.
13. Again call Spectator View -> Update All Asset Caches to prepare content for state synchronization.
14. Set the 'User Ip Address' in the Spectator View script to the ip address of your HoloLens device.
15. In the unity inspector, set the Account Domain, Account Id and Account Key for the Spatial Anchors Localizer using values you obtained creating an azure spatial anchors account above.
16. Build and deploy the application to your android device.
