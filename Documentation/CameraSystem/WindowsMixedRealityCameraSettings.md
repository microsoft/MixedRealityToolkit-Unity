# Windows Mixed Reality camera settings provider

The Windows Mixed Reality camera settings provider determines the type of device, upon which the application is running and applies the appropriate configuration settings based on the display (transparent or opaque).

## Enabling the Windows Mixed Reality camera settings provider

The following steps presume use of the MixedRealityToolkit object. Steps required for other service registrars may be different.

1. Select the MixedRealityToolkit object in the scene hierarchy.

    ![MRTK Configured Scene Hierarchy](../Images/MRTK_ConfiguredHierarchy.png)

2. Navigate the Inspector panel to the camera system section and expand the **Camera Settings Providers** section.

    ![Expand settings providers](../Images/CameraSystem/ExpandProviders.png)

3. Click **Add Camera Settings Provider** and expand the newly added **New camera settings** entry.

    ![Expand new settings provider](../Images/CameraSystem/ExpandNewProvider.png)

4. Select the Windows Mixed Reality Camera Settings provider

    ![Select Windows Mixed Reality settings provider](../Images/CameraSystem/SelectWindowsMixedRealitySettings.png)

> [!Note]
> When using the Microsoft Mixed Reality Toolkit default profiles, the Windows Mixed Reality camera settings provider will already be enabled and configured.

## See Also

- [Camera System Overview](CameraSystemOverview.md)
- [Creating a Camera Settings Provider](CreateSettingsProvider.md)
