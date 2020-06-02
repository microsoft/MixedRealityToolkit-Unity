# MRTK のビルドとデプロイ

アプリケーションを HoloLens, Android, iOS などのデバイス上でスタンドアロン アプリとして実行するには、Unity プロジェクトでビルドとデプロイのステップが必要です。MRTK を使ったアプリケーションのビルドとデプロイ方法は、他の Unity アプリケーションのビルドとデプロイ方法と同様です。MRTK 特有の方法はありません。HoloLens 向けに Unity アプリケーションをビルド、デプロイする方法の詳細なステップは、以下をお読みください。他のプラットフォーム向けのビルドについては、 [Publishing Builds](https://docs.unity3d.com/Manual/PublishingBuilds.html) をご確認ください。

## HoloLens 1 または HoloLens 2 (UWP) への、MRTK のビルドとデプロイ
Hololens 1 または Hololens 2 (UWP) へビルドとデプロイする方法の説明は、[building your application to device](https://docs.microsoft.com/windows/mixed-reality/mrlearning-base-ch1#build-your-application-to-your-device) をご覧ください。

**ヒント:** WMR (Windows Mixed Reality), HoloLens 1, HoloLens 2 向けにビルドする際は、ビルド設定の 「Target SDK Version」 
と 「Minimum Platform Version」 を以下の画像のように設定することをおすすめします。

![ビルド ウィンドウ](../Documentation/Images/getting_started/BuildWindow.png)

その他の設定は違っていることもあります。（例えば、Build Configuration, Architecture, Build Type やその他いくつかの設定は、
Visual Studio のソリューションでいつでも変更可能です。）

「Target SDK Version」 のドロップダウンに 「10.0.18362.0」 が含まれていることを確認してください。もし存在しない場合は、
[最新の Windows SDK](https://developer.microsoft.com/windows/downloads/windows-10-sdk) のインストールが必要です。

### Unity 2019.3 and HoloLens

If a HoloLens app appears as a 2D panel on device, make sure the following settings have been configured in Unity 2019.3.x before deploying your UWP app:

If using the legacy XR:

1. Navigate to Edit > Project Settings, Player
1. Under **XR Settings** in the UWP tab, make sure **Virtual Reality Supported** is enabled and the **Windows Mixed Reality** SDK has been added to SDKs.
1. Build and deploy in Visual Studio

If using the XR-Plugin:

1. Follow the steps found in [Getting Started with XRSDK](GettingStartedWithMRTKAndXRSDK.md)
1. Make sure the configuration profile is the **DefaultXRSDKConfigurationProfile**
1. Navigate to **Edit > Project Settings, XR-Plugin Management** and make sure **Windows Mixed Reality** is enabled.
1. Build and deploy in Visual Studio

>[!IMPORTANT]
> If using Unity 2019.3.x, select **ARM64** and not **ARM** as the build architecture in Visual Studio. With the default Unity settings in Unity 2019.3.x, a Unity app will not deploy to a HoloLens if ARM is selected due to a Unity bug. This can be tracked on [Unity's issue tracker](https://issuetracker.unity3d.com/issues/enabling-graphics-jobs-in-2019-dot-3-x-results-in-a-crash-or-nothing-rendering-on-hololens-2).
>
> If the ARM architecture is required, navigate to **Edit > Project Settings, Player**, and under the **Other Settings** menu disable **Graphics Jobs**. Disabling **Graphics Jobs** will allow the app to deploy using the ARM build architecture for Unity 2019.3.x, but ARM64 is recommended.

## Building and deploying MRTK to a Windows Mixed Reality Headset

The Windows Mixed Reality (WMR) headset can be used for Universal Windows Platform (UWP) and Standalone builds.  A Standalone build for a WMR headset requires the following extra steps:

> [!NOTE]
> Unity's XR SDK also supports native WMR in Standalone builds, but does not require SteamVR or WMR plugin. These steps are required for Unity's legacy XR.

1. Install [Steam](https://store.steampowered.com/about/)
1. Install [SteamVR](https://store.steampowered.com/app/250820/SteamVR/)
1. Install the [WMR Plugin](https://store.steampowered.com/app/719950/Windows_Mixed_Reality_for_SteamVR/)

### How to use WMR plugin

1. Open Steam and search for the Windows Mixed Reality Plugin
    - Make sure SteamVR is closed before launching the WMR Plugin. Launching the WMR plugin also launches SteamVR.
    - Make sure the WMR headset is plugged in.

    ![WMR Plugin Search](../Documentation/Images/BuildDeploy/WMR/SteamSearchWMRPlugin.png)

1. Select **Launch** for the Windows Mixed Reality for SteamVR Plugin.

    ![WMR Plugin](../Documentation/Images/BuildDeploy/WMR/WMRPlugin.png)

    - SteamVR and the WMR plugin will launch and a new tracking status window for the WMR headset will appear.
    - For more information visit the [Windows Mixed Reality Steam Documentation](https://support.microsoft.com/help/4053622/windows-10-play-steamvr-games-in-windows-mixed-reality)

        ![WMR Launch Appearance](../Documentation/Images/BuildDeploy/WMR/WMRPluginActive.png)

1. In Unity, with your MRTK scene open, navigate to **File > Build Settings**

1. Build the scene
    - Select **Add Open Scene**
    - Make sure the Platform is **Standalone**
    - Select **Build**
    - Choose the location for the new build in File Explorer

    ![Build Settings for Standalone](../Documentation/Images/BuildDeploy/WMR/BuildSettingsStandaloneUnity.png)

1. A new Unity executable will be created, to launch your app select the Unity executable in File Explorer.

    ![File Explorer Unity](../Documentation/Images/BuildDeploy/WMR/FileExplorerUnityExe.png)

## See also

- [Android and iOS Support](CrossPlatform/UsingARFoundation.md)
- [Leap Motion Support](CrossPlatform/LeapMotionMRTK.md)
- [Detecting Platform Capabilities](DetectingPlatformCapabilities.md)
