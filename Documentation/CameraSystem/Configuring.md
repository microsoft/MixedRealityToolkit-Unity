# Configuring the Camera System

The *Camera System Profile* provides options for configuring the behavior of the camera in mixed reality applications. This behavior includes platform specific settings as well as display options, such as the background color to use when clearing the screen.

## Camera Settings Providers

![Camera Settings Providers](../Images/CameraSystem/CameraSettingsProviders.png)

Camera setting providers enable platform specific configuration of the camera. These settings may include custom configuration steps and/or components.

> [!Note]
> Not all platforms will require a camera settings provider. If there are no providers that are compatible with the platform on which the application is running, the Microsoft Mixed Reality Toolkit will apply basic defaults.

Providers can be added by clicking the **Add Camera Settings Provider** button. They can be removed by clicking the **-** button to the right of the provider's name.

## Display Settings

![Camera Display Settings](../Images/CameraSystem/CameraDisplaySettings.png)

Display settings are specified for both opaque (ex: Virtual Reality) and transparent (ex: Microsoft HoloLens) displays. The camera is configured, at run time, using these settings.

**Near Clip**

The near clip plane is the closest, in meters, that a virtual object can be to the camera and still be rendered. For greatest user comfort, it is recommended to make this value greater than zero. The previous image contains values that have been found to be comfortable on a variety of devices.

**Far Clip**

The far clip plane is the furthest, in meters, that a virtual object can be to the camera and still be rendered. For transparent devices, it is recommended that this value be relatively close as not to overly exceed the real world space and break the application's immersive qualities.

**Clear Flags**

The clear flags value indicates how the display is cleared as it is drawn. For virtual reality experiences, this value is most often set to Skybox. For transparent displays, it is recommended to set this to Color.

**Background Color**

If the clear flags are not set to Skybox, the background color property will be displayed.

> [!Note]

**Quality Settings**

The quality settings value indicates the graphics quality that Unity should use when it renders the scene. The quality level is a project level setting and is not specific to any one camera. For more information, please see the [Quality](https://docs.unity3d.com/Manual/class-QualitySettings.html) article in Unity's documentation.

## See Also

- [Camera System Overview](CameraSystemOverview.md)
