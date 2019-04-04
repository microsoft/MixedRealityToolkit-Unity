#Basic MRTK Setup to use Eye Tracking
This page covers how to set up your Unity MRTK scene to use Eye Tracking in your app. 
The following assumes you’re starting out with a fresh new scene. 


### Setting up the Scene
Set up the MixedRealityToolkit by simply clicking *'Mixed Reality Toolkit -> Configure…'* in the menu bar.

![MRTK](/External/ReadMeImages/EyeTracking/mrtk_setup_configure.png)



### Setting up the MRTK Profiles
After setting up your MRTK scene, you will be asked to choose a profile for MRTK. 
You can simply select the default one ( *DefaultMixedRealityToolkitConfigurationProfile* ) and then select the *'Copy & Customize'* option.

![MRTK](/External/ReadMeImages/EyeTracking/mrtk_setup_configprofile.png)



### Create an "Eye Gaze Data Provider"
- Navigate to the *'Extension Services'*

- To edit the default one ( *DefaultMixedRealityRegisteredServiceProvidersProfile* ), click the *'Clone'* button next to it. 

- Create an *Eye Tracking Service Provider* (the precise name is not important) by clicking *'+ Register a new Service Provider'*

    - **Component Name:** 
Name your service provider to remember it better in the future (e.g., Eye Tracking Service Provider)

    - **Component Type:** Select *'Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input'* -> *'WindowsMixedRealityEyeGazeDataProvider'*

![MRTK](/External/ReadMeImages/EyeTracking/mrtk_setup_eyes_serviceprovider.png)




### Enabling Eye Tracking in the GazeProvider
In HoloLens 1, head gaze was used as primary pointing technique. 
While head gaze is still available via the *GazeProvider* in MRTK, you can check to use eye gaze instead by ticking the *'Prefer Eye Tracking'* checkbox. 

![MRTK](/External/ReadMeImages/EyeTracking/mrtk_setup_eyes_gazeprovider.png)




### Simulating Eye Tracking in the Unity Editor
You can simulate Eye Tracking input in the Unity Editor to ensure that events are correctly triggered before deploying the app to your HoloLens 2.
Follow the below steps to use your scene camera's location as eye gaze origin and the camera's forward vector as eye gaze direction.

1. **Enable simulated Eye Tracking**: 
    - Navigate to *MRTK Configuration Profile* -> *Additional Service Providers* -> *In-Editor Input Simulation*.
    - Check the *'Simulate Eye Position'* checkbox.

    
2. **Disable head gaze cursor**: In general, we recommend not to show an eye gaze cursor or if you do to make it *very* subtle. 
    - Navigate to *MRTK Configuration Profile* -> *Input System Profile* -> *PointerSettings.PointerProfile*
    - At the bottom of the *PointerProfile*, you should assign an invisible cursor prefab to the *GazeCursor*.



### Accessing eye gaze data
Now that your scene is set up to use Eye Tracking, let's take a look at how to access it in your scripts: 
[Accessing Eye Tracking Data in your Unity Script](/Documentation/EyeTracking/EyeTracking_EyeGazeProvider.md).

 
### Building the Unity app for your device
The *'Gaze Input'* capability is unfortunately not yet supported under 'Player Settings -> Publishing Settings -> Capabilities' in Unity. 
To use Eye Tracking on your HoloLens 2 device, you need to manually edit the package manifest.
First, build your Unity project as you would normally do. 
Open your compiled Visual Studio project and then open the `Package.appxmanifest` in your solution (see screenshot below).
Make sure to tick the *'Gaze Input'* checkbox under *Capabilities*.

![Enabling Gaze Input in Visual Studio](/External/ReadMeImages/EyeTracking/mrtk_et_gazeinput.jpg)

---
[Back to "Eye Tracking in the MixedRealityToolkit"](/Documentation/EyeTracking/EyeTracking_Main.md)