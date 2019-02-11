SpectatorView
=============

What is SpectatorView for?
------------
- Filming HD Holograms: Using SpectatorView we can film holograms at screen resolution, meaning full HD, apply antialiasing and even shadows.
- Streaming Live: Stream live HD holographic experiences to an AppleTV directly from your iPhone, completely lag-free!
- Inviting Guests: Let non-HoloLens users experience holograms directly from their phones or tablets.

Current Features
------------
- Network auto-discovery for adding phones to the session.
- Automatic session handling, so users are added to the correct session.
- Spatial synchronization of Holograms, so everyone sees holograms in the exact same place.
- iOS support (ARKit enabled devices).
- Multiple iOS guests
- Recording of Video + Holograms + Ambient sound + Hologram Sounds.
- Share sheet so you can Save the Video, email it, or share it with other supporting apps.


Licenses
--------
- OpenCV - (3-clause BSD License) https://opencv.org/license.html
- Unity ARKit - (MIT License) https://bitbucket.org/Unity-Technologies/unity-arkit-plugin/src/3691df77caca2095d632c5e72ff4ffa68ced111f/LICENSES/MIT_LICENSE?at=default&fileviewer=file-view-default


Requirements
------------
- SpectatorView plugin and required OpenCV binaries, which can be found here: https://github.com/Microsoft/MixedRealityToolkit/tree/master/SpectatorViewPlugin, listed here:
    - opencv_aruco343.dll
    - opencv_calib3d343.dll
    - opencv_core343.dll
    - opencv_features2d343.dll
    - opencv_flann343.dll
    - opencv_imgproc343.dll
    - zlib1.dll
    - SpectatorViewPlugin.dll
- UnityARKitPlugin
    - This can be downloaded from the asset store here: https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-arkit-plugin-92515
- SpectatorView uses UNET for its network discovery and spatial syncronizing.  This means all interactivity during the application needs to be synced between the devices.
- Unity 2017.2.1p2 or later
- Hardware
    - A HoloLens
    - Windows PC running Windows 10
    - ARKit compatible device (iPhone 6s onwards
    / iPad Pro 2016 onwards
    / iPad 2017 onwards) -
    running iOS 11 or above
    - Mac with xcode 9.2 onwards
- Apple developer account, free or paid ( https://developer.apple.com/ )
- Microsoft Visual Studio 2017

Building the SpectatorView Native Plugin
----------------------------------------
The SpectatorViewPlugin will only be used in the HoloLens application. It needs to be built on a PC with visual studio.
- See: https://github.com/Microsoft/MixedRealityToolkit/blob/master/SpectatorViewPlugin/README.md

After building the SpectatorViewPlugin project, you will need to copy the generated dlls into your unity project's plugins directory.
- The compile output for SpectatorViewPlugin will likely be found at:
>MixedRealityToolkit\SpectatorViewPlugin\SpectatorViewPlugin\Release\SpectatorViewPlugin\

- The unity plugin directory will likely be found at:
>MixedRealityToolkit-Unity\Assets\Plugins\WSA\x86\

>NOTE: This unity plugin directory may not exist by default. You can create this directory in windows file explorer or in the unity editor. The Plugins directory is one of unity's special folders (https://docs.unity3d.com/Manual/SpecialFolders.html). The \WSA\x86\ subdirectories signal that these dlls are for a windows store application with x86 architecture (These are the specifications for HoloLens).

Project Setup
-------------
### Example
- It may prove helpful to look at an example scene before integrating SpectatorView components into your application. After having added the SpectatorViewPlugin dlls to your unity project, you should be able to build and run the provided example scene:
    - HoloToolkit-Examples\SpectatorView\Scenes\SpectatorViewExample.unity

### High Level Overview
The logic in spectator view can be broken down into three main steps:

#### 1) Device Discovery
SpectatorView supports having multiple iPhones view a single HoloLens's experience. For devices to discover one another, SpectatorView relies on UNET. The HoloLens configures itself as a client. iPhones joining the scene configure themselves as servers. iPhones broadcast that they exists to the HoloLens. And after hearing these broadcasts, the HoloLens starts looking for ArUco markers. Device discovery functionality can be found in the following script:
> MixedRealityToolkit-Unity/Assets/HoloToolkit-Preview/SpectatorView/Scripts/Networking/NewDeviceDiscovery.cs

#### 2) Marker Detection
After going through device discovery, a different networking component is used for telling the iPhone where its marker is located in the HoloLens scene. Using UNET, the HoloLens configures itself as a server while the iPhone acts as a client. At this point, the iPhone is showing a random ArUco marker on its screen. The HoloLens searches for any ArUco markers in 3D space using its Photo/Video camera and OpenCV. After the HoloLens locates a marker, it broadcasts to all iPhones the marker's location. The iPhone who is showing a marker assesses whether the located marker is the one they are displaying based on marker ids. If it is the correct marker, the iPhone moves onto spatial synchronization. If it's not, the iPhone continues listening (Note: there is a timeout for how long the HoloLens searches for a marker).
> Marker Detection: MixedRealityToolkit-Unity/Assets/HoloToolkit-Preview/SpectatorView/Scripts/SpatialSync/MarkerDetectionHololens.cs
> Marker Location Broadcast: MixedRealityToolkit-Unity/Assets/HoloToolkit-Preview/SpectatorView/Scripts/Networking/SpectatorViewNetworkDiscovery.cs

#### 3) Spatial Synchronization
After the iPhone registers its marker's location in the HoloLens's scene, the iPhone locates the marker relative to its own scene origin. The transform from the marker to the iPhone scene origin is calculated as the transform from the marker to the iPhone camera combined with the transform of the iPhone camera to the iPhone's scene origin. The transform between the marker and the camera has some hard coded assumptions around phone dimensions/screen-camera-offsets. The transform of the iPhone camera to the scene origin is facilitated by AR Kit's device tracking. After finding the marker location in both the HoloLens and iPhone scenes a transform between scene origins is created. This transform between origins is then applied to the root game object, which updates content on the iPhone to be repositioned correctly relative to the HoloLens user. The logic in this third step is primarily facilitated in the following scripts:
> MixedRealityToolkit-Unity/Assets/HoloToolkit-Preview/SpectatorView/Scripts/SpatialSync/AnchorLocated.cs
> MixedRealityToolkit-Unity/Assets/HoloToolkit-Preview/SpectatorView/Scripts/SpatialSync/WorldSync.cs

It seems worth noting that SpectatorView does the work to share a scene origin across devices. But SpectatorView does NOT currently synchronize all content across devices. Additional work is needed to synchronize animations or dynamically create/destroy game objects.

### Integrating into your Application
- Prepare your scene
    - Ensure all visable gameobjects, within your scene, are contained under a world root gameobject.
    ![World Root](Images/WorldRoot.PNG)

- Add the SpectatorView prefab (Assets/SpectatorView/Prefabs/SpectatorView.prefab) into your scene.
- Add the SpectatorViewNetworking prefab (Assets/SpectatorView/Prefabs/SpectatorViewNetworking.prefab) into your scene.
- Select the SpectatorViewNetworking gameobject and on the SpectatorViewNetworingManager component, there's a few things you can link up. If left untouched this component will search for necessary scripts at runtime.
    - Marker Detection Hololens -> SpectatorView/Hololens/MarkerDetection
    - Marker Generation 3D -> SpectatorView/IPhone/SyncMarker/3DMarker
    ![SpectatorView Network Discovery](Images/SpectatorViewNetworkDiscovery.PNG)

Building for the Different Platforms
------------------------------------
- The HoloLens application will have to be built on a PC using visual studio. The iOS application will have to be built on a mac using Xcode. However, generating solutions for visual studio and Xcode can all be completed using a PC.
- At the top level of the SpectatorView prefab there is a component called 'Platform Switcher'. 
![Platform Switcher](Images/PlatformSwitcher.PNG)
- Select the platform you want to build for.
- If selecting 'Hololens' you should see all gameobjects beneath the iPhone gameobject in the SpectatorView prefab become inactive and all the gameobjects under 'Hololens' become active.
- This can take a little while as depending on the platform you choose the HoloToolkit is being added or removed from the project.
>NOTE: When building for iOS on mac make sure to remove the GLTF component of MRTK as it is not yet compatibile with this platform. It's suggested to rename \MixedRealityToolkit-Unity\Assets\HoloToolkit\Utilities\Scripts\GLTF\ to \MixedRealityToolkit-Unity\Assets\HoloToolkit\Utilities\Scripts\\~GLTF\ when building for iOS. The '\~' at the beginning of the GLTF directory name will cause it to register as a hidden asset in unity (https://docs.unity3d.com/Manual/SpecialFolders.html). You will have to remove the '\~' when building in unity for HoloLens.
- After switching platforms, you can build your visual studio/xcode solution from Unity in the Build Settings dialogue (File -> Build Settings). Press build after opening this dialogue.
>NOTE: Ensure you build both the visual studio and xcode solution for your application using the same Unity editor instance (do not close Unity between builds), this is due to an unresolved issue with Unity.

Running your Application
------------------------
- Once you have a built and deployed a version of you application on the iPhone and on the HoloLens you should be able to connect them.
- Ensure that both devices are on the same WIFI network.
- Start the application on the both devices, in no specific order.
- The process of starting the application on the iPhone should trigger the Hololens camera to turn on and begin taking pictures.
- As soon as iPhone app starts, it will look for surfaces like floors or tables.
- When surfaces are found you should see a marker similar to this one:

    ![Marker](Images/Marker.PNG)
- Show this marker to the Hololens.
- Once the marker has been detected by the Hololens it should disappear and both devices should be connected and spatially syncronized. 

Video Capture
-------------
- To capture and save a video from the iPhone, tap and hold the screen for 1 second.  This should open the recording menu.
- Tap the red record button, this should start a countdown before begining to record the screen.
- To finish recording tap and hold the screen for another 1 second and tap the stop button.
- Click the preview button (blue button), to watch the recorded video.
- Open the sharesheet and click save to camera roll.

Networking your Application 
---------------------------
- SpectatorView uses UNET for its networking and manages all host-client connections for you.
- Any app specific data has to be synced and implemented by you, using e.g. SyncVars, NetworkTransform, NetworkBehaviour.
- For more information and tutorials on Unity Networking please visit https://unity3d.com/learn/tutorials/s/multiplayer-networking

Troubleshooting
---------------
- The Unity Editor won't connect to a session hosted by a HoloLens device
    - This could be the windows firewall.
    - Goto Windows Firwall options.
    - Allow an app or feature through Windows Firewall.
    - For all instances of Unity Editor in the list tick, Domain, Private and Public.
    - Then go back to Windows Firewall options.
    - Click Advanced settings.
    - Click Inbound Rules.
    - All instances of Unity Editor should have a green tick.
    - For any instances of Unity Editor that don't have a green tick:
        - Double click it.
        - In the action dialog select Allow the connection.
        - Click OK.
    - Restart the Unity Editor.

- At runtime the iPhone screen says "Locating Floor..." but does not display an AR marker.
    - The iPhone is looking for a horizontal surface so try pointing the iPhone camera towards the floor or a table. This will help ARKit find surfaces necessary to spatially sync with HoloLens.

- HoloLens camera does not turn on automatically when iPhone tries to join.
    - Make sure both HoloLens and iPhone are running on the same Wi-Fi network.

- When launching an SpectatorView application on a mobile device, other hololens running other SpectatorView apps turn on their camera
    - Goto the NewDeviceDiscovery component and change the both the Broadcast Key and Broadcast port to two unique values.  Goto SpectatorViewDiscovery and change the Broadcast Key and Broadcast port to another set of unique numbers.

- The HoloLens won't connect with the mac Unity Editor
    - This is a known issue which could be linked to the Unity version.  Building to the iPhone should still work and connect as normal.

- The HoloLens camera turns on but is not able to scan the marker.
    - Ensure that you build all versions of the application using the same Unity Editor instance (do not close Unity between builds).  This is due to an unknown issue with Unity.


