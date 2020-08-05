# How to configure Oculus Quest in MRTK using the XRSDK pipeline

A [Oculus Quest](https://www.oculus.com/quest/?locale=en_US) is required.

MRTK's support for the Oculus Quest comes in the form of two data providers, the **Oculus XRSDK Data Provider** and the **MRTK-Quest** data provider. 

The **Oculus XRSDK Data Provider** enables the use of the Oculus Quest with MRTK via the [Unity's XR Pipeline](https://docs.unity3d.com/Manual/XR.html).
This pipeline is the standard for developing XR applications in Unity 2019.3 and beyond. To use this pipeline, make sure that you using **Unity 2019.3 or newer**
Specifically, this pipeline currently allows for the use of Oculus Touch controllers and head tracking with the Oculus Quest.

The **MRTK-Quest Data Provider** enables the use of the Oculus Quest via the **Oculus Integration** Unity package. Specifically, this data provider has the additional benefit of 
supporting **hand tracking** with the Oculus Quest. This data provider does **NOT** use the **XRSDK pipeline**, so it is recommended to set up this data provider after the 
**Setting up the project for the Oculus Quest** to avoid potential issues with your project targeting the to-be-deprecated **Legacy XR Pipeline**.

## Setting up project for the Oculus Quest

1. Follow [these steps](https://developer.oculus.com/documentation/unity/book-unity-gsg/) to ensure that your project is ready to deploy on Oculus Quest.
    - Make sure that the Oculus Plug-in Provider is included in your project by going to **Edit --> Project Settings --> XR Plug-in Management --> Plug-in Providers**

    ![OculusPluginProvider](../Images/CrossPlatform/OculusQuest/OculusPluginProvider.png)

    - If it does not show up ensure that the **Oculus XR Plugin** is installed under **Window --> Package Manager**

    ![OculusXRPluginPackage](../Images/CrossPlatform/OculusQuest/OculusXRPluginPackage.png)

1. Ensure that [developer mode](https://developer.oculus.com/documentation/native/android/mobile-device-setup/) is enabled on your device. Installing the Oculus ADB Drivers is optional.

## Setting up the scene
    - Create a new Unity scene or open a pre-existing scene like HandInteractionExamples
    
    - Add MRTK to the scene by navigating to **Mixed Reality Toolkit** > **Add to Scene and Configure**

## Using the Oculus XRSDK Data Provider

1. Configure your profile to use the **Oculus XRSDK Data Provider**
    - If not intending to modify the configuration profiles
        - Change your profile to DefaultXRSDKInputSystemProfile and go to **Build and deploy your project to Oculus Quest**

    - Otherwise follow the following:
        - Select the MixedRealityToolkit game object in the hierarchy and select **Copy and Customize** to clone the default mixed reality profile.

        ![CloneProfile](../Images/CrossPlatform/CloneProfile.png)

        - Select the **Input** Configuration Profile

        ![InputConfigurationProfile](../Images/CrossPlatform/InputConfigurationProfile.png)

        - Select **Clone** in the input system profile to enable modification.

        ![CloneInputSystemProfile](../Images/CrossPlatform/CloneInputSystemProfile.png)

        - Open the **Input Data Providers** section, select **Add Data Provider** at the top, and new data provider will be added at the end of the list.  Open the new data provider and set the **Type** to **Microsoft.MixedReality.Toolkit.XRSDK.Oculus > OculusXRSDKDeviceManager**

        ![OculusAddXRSDKDataProvider](../Images/CrossPlatform/OculusQuest/OculusAddXRSDKDataProvider.png)


## Using the MRTK-Quest Data Provider for handtracking
1. Prepare MRTK project for MRTK-Quest

    - This step only applies for the Oculus Integration Assets version 1.7.0 and if the source of MRTK is cloned from the git repo, NOT from the Unity packages. 
    If the MRTK source is going to be from the Unity packages, start at the next step

    - Navigate to Mixed Reality Toolkit > Utilities > Oculus > Configure CSC File for Oculus. Updating the csc file filters out the obsolete warnings produced by the Oculus Integration Assets. 
    The MRTK repo contains a csc file that converts warnings to errors, this conversion halts the MRTK-quest configuration process. The obsolete warnings issue is tracked here.

    ![OculusIntegrationCsc](../Images/CrossPlatform/OculusQuest/OculusIntegrationCsc.png)

1. Importing MRTK and the Core Assets from Oculus Integration Assets version 1.7.0

Import the Microsoft.MixedReality.Toolkit.Foundation package into the Unity project.
Download and import [Oculus Integration version 1.7.0](https://developer.oculus.com/downloads/package/unity-integration-archive/)
Download and import the MRTK-Quest\_v101\_Core.unitypackage from the [MRTK-Quest release page](https://github.com/provencher/MRTK-Quest/releases)

1. Configure your project to use handtracking

In the imported Oculus (assets/Oculus) folder, there is a scriptable object called OculusProjectConfig. In that config file, you need to set HandTrackingSupport to "Controllers and Hands".

1. Configure your profile to use the **MRTK-Quest Data Provider**

    - If not intending to modify the configuration profiles
        - Change your profile to MRTK-Quest_Profile and go to **Build and deploy your project to Oculus Quest**

    - Otherwise follow the following:
        - Select the MixedRealityToolkit game object in the hierarchy and select **Copy and Customize** to clone the DefaultXRSDKInputSystemProfile profile.
            - This ensures that the controller and headtracking input data uses the **XRSDK Pipeline** rather than the deprecated **Legacy XR Pipeline**

        ![CloneProfile](../Images/CrossPlatform/CloneProfile.png)

        - Select the **Input** Configuration Profile

        ![InputConfigurationProfile](../Images/CrossPlatform/InputConfigurationProfile.png)

        - Select **Clone** in the input system profile to enable modification.

        ![CloneInputSystemProfile](../Images/CrossPlatform/CloneInputSystemProfile.png)

        - Open the **Input Data Providers** section, select **Add Data Provider** at the top, and new data provider will be added at the end of the list.  Open the new data provider and set the **Type** to **prvncher.MixedReality.Toolkit.OculusQuestInput > OculusQuestInputManager**

        ![OculusAddMrtkQuestDataProvider](../Images/CrossPlatform/OculusQuest/OculusAddMrtkQuestDataProvider.png)

## Build and deploy your project to Oculus Quest
    - Plug in your Oculus Quest via a USB 3.0 -> USB C cable
    - Navigate to **File > Build Settings**
    - Change the deployment to **Android**
    - Ensure that the Oculus Quest is selected as the applicable run device
    
    ![OculusRunDevice](../Images/CrossPlatform/OculusQuest/OculusRunDevice.png)

    - Select Build and Run 
    - Accept the _Allow USB Debugging_ prompt from inside the quest
    - See your scene inside the Oculus Quest


## Common errors

### Quest not recognized by Unity

Make sure your Android paths are properly configured. If you continue to encounter problems, follow this [guide](https://developer.oculus.com/documentation/unity/book-unity-gsg/#install-android-tools)

**Edit > Preferences > External Tools > Android**

![AndroidToolsConfig](../Images/CrossPlatform/OculusQuest/AndroidToolsConfig.png)