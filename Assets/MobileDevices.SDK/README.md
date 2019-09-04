# Movbile Devices SDK (Android / iPhone)
This sdk is for mobile devices.Using ARFoundation,we can use 6dof head tracking!

## Required ARFoundation Version

This sdk is required ARFoundation.Required version is below:
* For Unity 2018.4.X
  *  ARFoundation 1.5.0-Preview6
  *  ARkit XR Plugin 2.1.1
  *  ARCore XR Plugin 2.1.1
* For Unity 2019.1.X
  *  ARFoundation 2.1.3
  *  ARkit XR Plugin 2.1.1
  *  ARCore XR Plugin 2.1.1

## How to build

1. Open Assets\MobileDevices.SDK\Scenes\HandInteractionExamplesUsingARFoundation.unity
2. Selecting "MixedRealityToolkit" in Hierarchy tab, set to Profile "MobileDevicesConfigurationProfile"
3. open Build Settings, and Switch Platform Android or iOS.(player settings is below.)
4. Build and deploy.

## For Android

Player Settings check below:
* unchecked "Auto Graphics API".
* Graphics API set to  only "OpenGLES3".
* Minimum API Level set to "Android 7.0 'nougat' (API Level 24)".
* XR Settings is all unchecked.
  
## For iOS

Player Settings check below:
* Camera Usage Description set to  something texts.
* Target minimum iOS version set to 11.0.
* Archtecture set to ARM64.
* XR Settings is all unchecked.