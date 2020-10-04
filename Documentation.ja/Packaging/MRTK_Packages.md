# Mixed Reality Toolkit パッケージ

Mixed Reality Toolkit (MRTK) は、Mixed Reality ハードウェアとプラットフォームをサポートすることにより、クロスプラットフォーム Mixed Reality アプリケーション開発を可能にするパッケージのコレクションです。

MRTK は [アセット](#asset-package) (.unitypackage) パッケージとして、そして [Unity Package Manager](#unity-package-manager) を通して利用可能です。

<a name="asset-package"></a>

## アセット パッケージ

MRTK のアセット (.unitypackage) は、[GitHub](https://github.com/microsoft/MixedRealityToolkit-Unity/releases) からダウンロードできます。

アセット パッケージを使う利点としては以下があります。

- Unity 2018.4 以降で利用可能
- MRTK への変更が簡単
  - MRTK は Assets フォルダ内にある

課題としては以下があります。
- MRTK がプロジェクトのアセット フォルダの一部になるため、
  - プロジェクトが大きくなる
  - コンパイル時間が遅くなる
- 依存管理がない
  - パッケージの依存関係を手動で解決する必要がある
- 手動でのアップデート プロセス
  - 複数のステップ
  - 巨大な (3000 以上のファイルの) ソース コントロールの更新
  - MRTK への変更を失ってしまうリスク
- Examples パッケージをインポートするということは全ての Examples を含むことを意味する

利用可能なパッケージは以下のものです。

- [Foundation](#foundation-パッケージ)
- [Extensions](#extensions-パッケージ)
- [Tools](#tools-パッケージ)
- [Test utilities](#test-utilities-パッケージ)
- [Examples](#examples-パッケージ)

これらのパッケージは、GitHub の [mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release) ブランチのソース コードから、Microsoft によってリリース、サポートされています。

### Foundation パッケージ

Mixed Reality Toolkit Foundation は、アプリケーションが Mixed Reality プラットフォーム間で共通の機能を活用できるようにするコードのセットです。

<img src="../../Documentation/Images/Input/MRTK_Package_Foundation.png" width="350px" style="display:block;">  
<sup>MRTK Foundation パッケージ</sup>

MRTK Foundation パッケージは以下を含んでいます。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Core | | Interface and type definitions, base classes, standard shader. |
| MRTK/Core/Providers | | Platform agnostic data providers |
| | Hands | Base class support and services for hand tracking. |
| | [InputAnimation](../InputSimulation/InputAnimationRecording.md) | Support for recording head movement and hand tracking data. |
| | [InputSimulation](../InputSimulation/InputSimulationService.md) | Support for in-editor simulation of hand and eye input. |
| | [ObjectMeshObserver](../SpatialAwareness/SpatialObjectMeshObserver.md) | Spatial awareness observer using a 3D model as the data. |
| | UnityInput | Common input devices (joystick, mouse, etc.) implemented via Unity's input API. |
| MRTK/Providers | | Platform specific data providers |
| | LeapMotion | Support for the UltraLeap Leap Motion controller. |
| | OpenVR | Support for OpenVR devices. |
| | Oculus | Support for Oculus devices, such as the Quest. |
| | [UnityAR](../CameraSystem/UnityArCameraSettings.md) | (Experimental) Camera settings provider enabling MRTK use with mobile AR devices. |
| | WindowsMixedReality | Support for Windows Mixed Reality devices, including Microsoft HoloLens and immersive headsets. |
| | Windows | Support for Microsoft Windows specific APIs, for example speech and dictation. |
| | XR SDK | (Experimental) Support for [Unity's new XR framework](https://blogs.unity3d.com/2020/01/24/unity-xr-platform-updates/) in Unity 2019.3 and newer. |
| MRTK/SDK | | |
| | Experimental | Experimental features, including shaders, user interface controls and individual system managers. |
| | Features | Functionality that builds upon the Foundation package. |
| | Profiles | Default profiles for the Microsoft Mixed Reality Toolkit systems and services. |
| | StandardAssets | Common assets; models, textures, materials, etc. |
| MRTK/Services | | |
| | [BoundarySystem](../Boundary/BoundarySystemGettingStarted.md) | System implementing VR boundary support. |
| | [CameraSystem](../CameraSystem/CameraSystemOverview.md) | System implementing camera configuration and management. |
| | [DiagnosticsSystem](../Diagnostics/DiagnosticsSystemGettingStarted.md) | System implementing in application diagnostics, for example a visual profiler. |
| | [InputSystem](../Input/Overview.md) | System providing support for accessing and handling user input. |
| | [SceneSystem](../SceneSystem/SceneSystemGettingStarted.md) | System providing multi-scene application support. |
| | [SpatialAwarenessSystem](../SpatialAwareness/SpatialAwarenessGettingStarted.md) | System providing support for awareness of the user's environment. |
| | [TeleportSystem](../TeleportSystem/Overview.md) | System providing support for teleporting (moving about the experience in jumps). |
| MRTK/StandardAssets | | MRTK Standard shader, basic materials and other standard assets for mixed reality experiences |

### Extensions パッケージ

オプションの Microsoft.MixedRealityToolkit.Unity.Extensions パッケージには、MRTK の機能を拡張する追加サービスが含まれています。

> [!NOTE]
> Extensions パッケージは Microsoft.MixedRealityToolkit.Unity.Foundation が必要です。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Extensions | |
| | [HandPhysicsService](../Extensions/HandPhysicsService/HandPhysicsServiceOverview.md) | Service that adds physics support to articulated hands. |
| | LostTrackingService | Service that simplifies handling of tracking loss on Microsoft HoloLens devices. |
| | [SceneTransitionService](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md) | Service that simplifies adding smooth scene transitions. |

### Tools パッケージ

オプションの  Microsoft.MixedRealityToolkit.Unity.Tools パッケージには、MRTK を使った Mixed Reality 開発エクスペリエンスを強化する役に立つツールが含まれています。
これらのツールは Unity Editor の **Mixed Reality Toolkit > Utilities** メニューにあります。

> [!NOTE]
> Tools パッケージは Microsoft.MixedRealityToolkit.Unity.Foundation が必要です。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tools | |
| | BuildWindow | Tool that helps simplify the process of building and deploying UWP applications. |
| | [DependencyWindow](../Tools/DependencyWindow.md) | Tool that creates a dependency graph of assets in a project. |
| | [ExtensionServiceCreator](../Tools/ExtensionServiceCreationWizard.md) | Wizard to assist in creating extension services. |
| | [MigrationWindow](../Tools/MigrationWindow.md) | Tool that assists in updating code that uses deprecated MRTK components.  |
| | [OptimizeWindow](../Tools/OptimizeWindow.md) | Utility to help automate configuring a mixed reality project for the best performance in Unity. |
| | ReserializeAssetsUtility | Provides support for reserializing specific Unity files. |
| | [RuntimeTools/Tools/ControllerMappingTool](../Tools/ControllerMappingTool.md) | Utility enabling developers to quickly determine Unity mappings for hardware controllers. |
| | ScreenshotUtility | Enables capturing application images in the Unity editor. |
| | TextureCombinerWindow | Utility to combine graphics textures. |
| | [Toolbox](../README_Toolbox.md) | UI that makes it easy to discover and use MRTK UX components. |

### Test utilities パッケージ

オプションの Microsoft.MixedRealityToolkit.TestUtilities パッケージは開発者が簡単に[PlayMode テストを作る](../Contributing/UnitTests.md#play-mode-tests)ためのヘルパー スクリプトのコレクションです。これらのユーティリティは MRTK のコンポーネントを作る開発者に特に有用です。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tests | |
| | TestUtilities | Methods to simplify creation of play mode tests, including hand simulation utilities. |

### Examples パッケージ

Examples パッケージには、Foundation パッケージの機能を使用するデモ、サンプル スクリプト、およびサンプル シーンが含まれています。このパッケージには、さまざまなタイプのハンド インプット (多関節および非多関節) に反応するサンプル オブジェクトが含まれる [HandInteractionExample シーン](../README_HandInteractionExamples.md) (下図) が含まれています。

![HandInteractionExample scene](../../Documentation/Images/MRTK_Examples.png)

このパッケージには、アイ トラッキング デモも含まれています。詳細は[こちら](../EyeTracking/EyeTracking_ExamplesOverview.md)に記載されています。

より一般的には、MRTK のすべての新機能について、同じフォルダ構造と場所にほぼ従って、対応するサンプルが Examples パッケージに含まれているべきです。

> [!NOTE]
> Examples パッケージは Microsoft.MixedRealityToolkit.Unity.Foundation が必要です。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Examples | | |
| | Demos | Simple scenes illustrating one or two related features. |
| | Experimental | Demo scenes illustrating experimental features. |
| | StandardAssets | Common assets shared by multiple demo scenes. |

## Unity Package Manager

Unity 2019.4 以降では、MRTK は [Unity Package Manager](https://docs.unity3d.com/Manual/Packages.html) を通じて利用可能です。

アセット パッケージを使う利点としては以下があります。

- プロジェクトが小さくなる
  - Visual Studio のソリューションがきれいになる
  - 管理ファイルが少なくなる（MRTK は `Packages/manifest.json` ファイルのシンプルな参照)
- コンパイルの高速化
  - Unity がビルド時に MRTK を再コンパイルする必要がない
- 依存関係の解決
  - 依存先としてパッケージを指定した時に必要な MRTK のパッケージが自動でインストールされる
- 新しい MRTK のバージョンへの簡単なアップデート
  - `Packages/manifest.json` ファイルのバージョンを変更する

課題としては以下があります。

- MRTK は変更不可能になる
  - パッケージの解決から取り除かないと変更することができない
- Unity 2018.4 では MRTK は UPM パッケージをサポートしない

### Foundation パッケージ

Foundation パッケージ (`com.microsoft.mixedreality.toolkit.foundation`)は、Mixed Reality Toolkit の基礎を形作ります。 

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Core | | Interface and type definitions, base classes, standard shader. |
| MRTK/Core/Providers | | Platform agnostic data providers |
| | Hands | Base class support and services for hand tracking. |
| | [InputAnimation](../InputSimulation/InputAnimationRecording.md) | Support for recording head movement and hand tracking data. |
| | [InputSimulation](../InputSimulation/InputSimulationService.md) | Support for in-editor simulation of hand and eye input. |
| | [ObjectMeshObserver](../SpatialAwareness/SpatialObjectMeshObserver.md) | Spatial awareness observer using a 3D model as the data. |
| | UnityInput | Common input devices (joystick, mouse, etc.) implemented via Unity's input API. |
| MRTK/Providers | | Platform specific data providers |
| | LeapMotion | Support for the UltraLeap Leap Motion controller. |
| | OpenVR | Support for OpenVR devices. |
| | Oculus | Support for Oculus devices, such as the Quest. |
| | [UnityAR](../CameraSystem/UnityArCameraSettings.md) | (Experimental) Camera settings provider enabling MRTK use with mobile AR devices. |
| | WindowsMixedReality | Support for Windows Mixed Reality devices, including Microsoft HoloLens and immersive headsets. |
| | Windows | Support for Microsoft Windows specific APIs, for example speech and dictation. |
| | XR SDK | (Experimental) Support for [Unity's new XR framework](https://blogs.unity3d.com/2020/01/24/unity-xr-platform-updates/) in Unity 2019.3 and newer. |
| MRTK/SDK | | |
| | Experimental | Experimental features, including shaders, user interface controls and individual system managers. |
| | Features | Functionality that builds upon the Foundation package. |
| | Profiles | Default profiles for the Microsoft Mixed Reality Toolkit systems and services. |
| | StandardAssets | Common assets; models, textures, materials, etc. |
| MRTK/Services | | |
| | [BoundarySystem](../Boundary/BoundarySystemGettingStarted.md) | System implementing VR boundary support. |
| | [CameraSystem](../CameraSystem/CameraSystemOverview.md) | System implementing camera configuration and management. |
| | [DiagnosticsSystem](../Diagnostics/DiagnosticsSystemGettingStarted.md) | System implementing in application diagnostics, for example a visual profiler. |
| | [InputSystem](../Input/Overview.md) | System providing support for accessing and handling user input. |
| | [SceneSystem](../SceneSystem/SceneSystemGettingStarted.md) | System providing multi-scene application support. |
| | [SpatialAwarenessSystem](../SpatialAwareness/SpatialAwarenessGettingStarted.md) | System providing support for awareness of the user's environment. |
| | [TeleportSystem](../TeleportSystem/Overview.md) | System providing support for teleporting (moving about the experience in jumps). |

依存パッケージ:

- Standard Assets (`com.microsoft.mixedreality.toolkit.standardassets`)

### Standard Assets

Standard Assets パッケージ (`com.microsoft.mixedreality.toolkit.standardassets)` は全ての Mixed Reality エクスペリエンスに推奨されるコンポーネントのコレクションで、以下が含まれています。

- MRTK Standard シェーダー
- MRTK Standard シェーダーを使った基本的なマテリアル
- Audio ファイル
- Fonts
- Textures
- Icons

> [!Note]
> Assembly Definitions の破壊的変更を防ぐため、MRTK Standard シェーダーのいくつかの機能をコントロールするのに使われているスクリプトは Standard Assets パッケージに含まれていません。これらのスクリプトは Foundation パッケージの `MRTK/Core/Utilities/StandardShader` フォルダにあります。

依存パッケージ: なし

### Extension パッケージ

オプションの Extensions パッケージ (`com.microsoft.mixedreality.toolkit.extensions)` には MRTK の機能を拡張する追加のコンポーネントが含まれています。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Extensions | |
| | [HandPhysicsService](../Extensions/HandPhysicsService/HandPhysicsServiceOverview.md) | Service that adds physics support to articulated hands. |
| | LostTrackingService | Service that simplifies handing of tracking loss on Microsoft HoloLens devices. |
| | [SceneTransitionService](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md) | Service that simplifies adding smooth scene transitions. |
| | Samples~ | A hidden (in the Unity Editor) folder that contains the sample scenes and assets. |

サンプル プロジェクトを含むパッケージの利用手順の詳細は、[Mixed Reality Toolkit と Unity Package Manager](../usingupm.md#using-mixed-reality-toolkit-examples) をご覧ください。

依存パッケージ:

- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)

### Tools パッケージ

オプションの Tools パッケージ (`com.microsoft.mixedreality.toolkit.tools)` には Mixed Reality 体験を作るのに便利なツールが含まれています。おおむねこれらのツールは Editor コンポーネントで、コードはアプリケーションの一部としてはリリースされません。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tools | |
| | BuildWindow | Tool that helps simplify the process of building and deploying UWP applications. |
| | [DependencyWindow](../Tools/DependencyWindow.md) | Tool that creates a dependency graph of assets in a project. |
| | [ExtensionServiceCreator](../Tools/ExtensionServiceCreationWizard.md) | Wizard to assist in creating extension services. |
| | [MigrationWindow](../Tools/MigrationWindow.md) | Tool that assists in updating code that uses deprecated MRTK components.  |
| | [OptimizeWindow](../Tools/OptimizeWindow.md) | Utility to help automate configuring a mixed reality project for the best performance in Unity. |
| | ReserializeAssetsUtility | Provides support for reserializing specific Unity files. |
| | [RuntimeTools/Tools/ControllerMappingTool](../Tools/ControllerMappingTool.md) | Utility enabling developers to quickly determine Unity mappings for hardware controllers. |
| | ScreenshotUtility | Enables capturing application images in the Unity editor. |
| | TextureCombinerWindow | Utility to combine graphics textures. |
| | [Toolbox](../README_Toolbox.md) | UI that makes it easy to discover and use MRTK UX components. |

依存パッケージ:

- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)

### Test utilities パッケージ

オプションの Test utilities パッケージ (`com.microsoft.mixedreality.toolkit.testutilities`) には、開発者が簡単に PlayMode テストを作れるようにするヘルパー スクリプトのコレクションが含まれています。これらのユーティリティは MRTK のコンポーネントを作る開発者に特に有用です。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Tests | |
| | TestUtilities | Methods to simplify creation of play mode tests, including hand simulation utilities. |

依存パッケージ:

- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)

### Examples パッケージ

Examples パッケージ (`com.microsoft.mixedreality.toolkit.examples`) は開発者が興味のあるサンプルのみをインポートできるように構成されています。

サンプルプロジェクトを含むパッケージを使う手順の詳細は、[Mixed Reality Toolkit と Unity Package Manager](../usingupm.md#using-mixed-reality-toolkit-examples) のページをご覧ください。

| Folder | Component | Description |
| --- | --- | --- |
| MRTK/Examples | | |
| | Samples~ | A hidden (in the Unity Editor) folder that contains the sample scenes and assets. |
| | StandardAssets | Common assets shared by multiple demo scenes. |

依存パッケージ:

- Foundation (`com.microsoft.mixedreality.toolkit.foundation`)
- Extensions (`com.microsoft.mixedreality.toolkit.extensions`)

## 関連項目

- [アーキテクチャ概要](../Architecture/Overview.md)
- [Systems, Extension Services と Data Providers](../Architecture/SystemsExtensionsProviders.md)
- [Mixed Reality Toolkit と Unity Package Manager](../usingupm.md)
