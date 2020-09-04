# How to configure MRTK for iOS and Android [Experimental]

## Install required packages

1. Download and import the **Microsoft.MixedReality.Toolkit.Unity.Foundation** package, from [GitHub](https://github.com/microsoft/MixedRealityToolkit-Unity/releases/tag/v2.3.0) or the [Unity Package Manager](../usingupm.md)

1. In the Unity Package Manager (UPM), install the following packages:

    **Unity 2018.4.x**

    | **Android** | **iOS** | Comments |
    | --- | --- | --- |
    | AR Foundation  <br/> Version: 1.5.0 - preview 6 | AR Foundation  <br/> Version: 1.5.0 - preview 6 | For Unity 2018.4, this package is included as a preview. To view the package: `Window` > `Package Manager` > `Advanced` > `Show Preview Packages` |
    | ARCore XR Plugin <br/> Version: 2.1.2 | ARKit XR Plugin <br/> Version: 2.1.2 | |

    **Unity 2019.4.x**

    | **Android** | **iOS** |
    | --- | --- |
    | AR Foundation  <br/> Version: 2.1.8 |  AR Foundation  <br/> Version: 2.1.8 |
    | ARCore XR Plugin <br/> Version: 2.1.11 | ARKit XR Plugin <br/> Version: 2.1.9 |

    **Unity 2020.1.x (Not currently formally supported, included for informational purposes only)**

    | **Android** | **iOS** |
    | --- | --- |
    | AR Foundation  <br/> Version: 3.1.3 |  AR Foundation  <br/> Version: 3.1.3 |
    | ARCore XR Plugin <br/> Version: 3.1.4 | ARKit XR Plugin <br/> Version: 3.1.3 |

1. Update the MRTK UnityAR assembly definitions by invoking the menu item: **Mixed Reality Toolkit > Utilities > UnityAR > Update Assembly Definitions**

## Enabling the Unity AR camera settings provider

The following steps presume use of the MixedRealityToolkit object. Steps required for other service registrars may be different.

1. Select the MixedRealityToolkit object in the scene hierarchy.

    ![MRTK Configured Scene Hierarchy](../Images/MRTK_ConfiguredHierarchy.png)

1. Select **Copy and Customize** to Clone the MRTK Profile to enable custom configuration.

    ![Clone MRTK Profile](../Images/CameraSystem/CloneProfileARFoundation.png)

1. Select **Clone** next to the Camera Profile.

    ![Clone MRTK Camera Profile](../Images/CameraSystem/CloneCameraProfileARFoundation.png)

1. Navigate the Inspector panel to the camera system section and expand the **Camera Settings Providers** section.

    ![Expand settings providers](../Images/CameraSystem/ExpandProviders.png)

1. Click **Add Camera Settings Provider** and expand the newly added **New camera settings** entry.

    ![Expand new settings provider](../Images/CameraSystem/ExpandNewProvider.png)

1. Select the Unity AR Camera Settings provider

    ![Select Unity AR settings provider](../Images/CameraSystem/SelectUnityArSettings.png)

    For more information about configuring the Unity AR camera settings provider: [Unity AR camera settings provider](../CameraSystem/UnityArCameraSettings.md).

> [!NOTE]
> This installation checks (when the application starts) if the AR Foundation components are in the scene. If not, they are automatically added to make it work with ARCore and ARKit.
> If you need to set a specific behaviour, you should add the components you need by yourself. 
> For more information about AR Foundation components and installation, check this [documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@2.2/manual/index.html#samples).

## Building a scene for Android and iOS devices

1. Make sure you have added the UnityAR Camera Settings Provider to your scene.

1. Switch platform to either Android or iOS in the Unity Build Settings

    When you switch the platform you should see the MRTK Project Configurator Window with settings for your chosen platform.  Click Apply to enable platform specific settings.

    iOS Project Configurator Settings

    ![iOS Project Configurator](../Images/CameraSystem/MRTKProjectConfigurator.png)

1. There are no additional steps after switching the platform for Android.

1. If the platform is iOS, Edit > Project Settings > Player > Other Settings, under the Optimization header, **uncheck** Strip Engine Code

    ![iOS Settings](../Images/CameraSystem/UncheckStripEngineCodeiOS.png)

    > [!NOTE]
    > Unchecking Strip Engine Code is the short term solution to an error in Xcode [#6646](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6646).  We are working on a long term solution.

1. Build and run the scene

## See also

- [Unity AR Camera Settings](../CameraSystem/UnityArCameraSettings.md)
