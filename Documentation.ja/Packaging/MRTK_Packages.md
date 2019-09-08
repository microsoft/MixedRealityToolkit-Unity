# Mixed Reality Toolkit パッケージ

Mixed Reality Toolkit (MRTK) は、Mixed Reality ハードウェアとプラットフォームをサポートすることにより、クロスプラットフォーム Mixed Reality アプリケーション開発を可能にするパッケージのコレクションです。

MRTK は、次のパッケージを介してリリースしています。

- [Foundation](#foundation-パッケージ)
- [Extensions](#extensions-パッケージ)
- [Examples](#examples-パッケージ)
- [Tools](#tools-パッケージ)

## Foundation パッケージ

Mixed Reality Toolkit Foundation は、アプリケーションが Mixed Reality プラットフォーム間で共通の機能を活用できるようにするパッケージのセットです。これらのパッケージは、GitHub の [mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release) ブランチのソースコードからマイクロソフトによってリリースされ、サポートされています。

<img src="../../Documentation/Images/Input/MRTK_Package_Foundation.png" width="350px" style="display:block;"><br/>
<sup>MRTK Foundation パッケージ</sup>

MRTK Foundation は以下の通りに構成されています。

- [Core パッケージ](#core-パッケージ)
- [プラットフォーム プロバイダー](#プラットフォーム-プロバイダー)
- [システムサービス](#システムサービス)
- [機能アセット](#機能アセット)

次のセクションでは、各カテゴリのパッケージの種類について説明します。

### Core パッケージ

Core パッケージは _必須_ コンポーネントであり、すべての MRTK Foundation パッケージが依存しています。。

MRTK Core パッケージには以下が含まれます。

- [共通のインターフェイス、クラス、データ型](#共通型)
- [MixedRealityToolkit シーン コンポーネント](#mixedrealitytoolkit-シーン-コンポーネント)
- [MRTK スタンダード シェーダー](#mrtk-スタンダード-シェーダー)
- [Unity インプットプロバイダ (Unity Input Provider)](#unity-インプットプロバイダ-unity-input-provider)
- [パッケージ管理](#パッケージ管理)

#### 共通型

Mixed Reality Toolkit Core パッケージには、他のすべてのコンポーネントで使用される共通のインターフェイス、クラス、データ型のすべての定義が含まれています。プラットフォーム間で最高レベルの互換性を実現するために、定義されたインターフェイスのみを介して MRTK コンポーネントにアクセスすることを _強く_ お勧めします。

#### MixedRealityToolkit シーン コンポーネント

MixedRealityToolkit シーン コンポーネントは、Mixed Reality Toolkit の単一の集中化されたリソースマネージャです。このコンポーネントは、プラットフォームモジュールとサービスモジュールのライフスパンをロードおよび管理し、システムが構成設定にアクセスするためのリソースを提供します。

#### MRTK スタンダード シェーダー

MRTK のスタンダードシェーダーは MRTK によって提供される事実上すべてのマテリアルの基礎を提供します。このシェーダは非常に柔軟性が高く、MRTK がサポートされているさまざまなプラットフォームに最適化されています。最適なパフォーマンスを実現するには、アプリケーションのマテリアルで MRTK スタンダードシェーダーを使用することを強くお勧めします。

#### Unity インプットプロバイダ (Unity Input Provider)

Unity 入力プロバイダは、ゲームコントローラ、タッチスクリーン、3D 空間マウスなどの一般的な入力デバイスにへのアクセスを提供します。

#### パッケージ管理

_Coming soon_

Mixed Reality Toolkit Core パッケージは、オプションの Foundation、Extensions、Experimental（実験的な）MRTK パッケージを発見し、管理するためのサポートを提供します。

### プラットフォーム プロバイダー

MRTK プラットフォーム プロバイダー パッケージは、Mixed Reality Toolkit が Mixed Reality ハードウェアおよびプラットフォーム機能を対象とすることを可能にするコンポーネントです。

サポートされているプラットフォームは次のとおりです。

- [Windows Mixed Reality](#windows-mixed-reality)
- [OpenVR](#openvr)
- [Windows Voice](#windows-voice)

#### Windows Mixed Reality

The Windows Mixed Reality package provides support for Microsoft HoloLens, HoloLens 2 and Windows Mixed Reality Immersive devices. The package contains full platform support, including:

- Articulated Hands
- Eye Tracking
- Gaze targeting
- Gestures
- Spatial Mapping
- Windows Mixed Reality Motion controllers

#### OpenVR

The OpenVR package provides hardware and platform support for devices using the OpenVR platform.

#### Windows Voice

The Windows Voice package provides support for keyword recognition and dictation functionality on Microsoft Windows 10 devices.

### システムサービス

Core platform services are provided in system service packages. These packages contain the Mixed Reality Toolkit's default implementations of the system service interfaces, defined in the [core](#core-package) package.

The MRTK foundation includes the following system services:

- [Boundary System](#boundary-system)
- [Diagnostic System](#diagnostic-system)
- [Input System](#input-system)
- [Spatial Awareness System](#spatial-awareness-system)
- [Teleport System](#teleport-system)

#### 境界システム (Boundary System)

The MRTK Boundary System provides data about the to virtual reality playspace. On systems for which the user has configured the boundary, the system can provide a floor plane, rectangular playspace, tracked area, and more.

#### 診断システム (Diagnostic System)

The MRTK Diagnostic System provides real-time performance data within your application experience. At a glace, you will be able to view frame rate, processor time and other key performance metrics as you use your application.

#### 入力システム (Input System)

The MRTK Input Systems enables applications to access input in a cross platform manner by specifying user actions and assigning those actions to the most appropriate buttons and axes on target controllers.

#### 空間認識システム (Spatial Awareness System)

The MRTK Spatial Awareness System enables access to real-world environmental data from devices such as the Microsoft HoloLens.

#### テレポートシステム (Teleport System)

The MRTK Teleport System provides virtual reality locomotion support.

### 機能アセット

Feature Assets are collections of related functionality delivered as Unity assets and scripts. Some of these features include:

- User Interface Controls
- Standard Assets
- more

## Extensions パッケージ

The extensions package contains additional services and components that extend the functionality of the foundation package.

- [Scene Transition Service](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md)

## Examples パッケージ

The examples package contains demos, sample scripts, and sample scenes that exercise functionality in the foundation package.

For example, this package contains the HandInteractionExample scene (pictured below) which contains sample objects
that respond to various types of hand input (articulated and non-articulated).

![HandInteractionExample scene](../Images/MRTK_Examples.png)

This package also contains eye tracking demos, which are [documented here](../EyeTracking/EyeTracking_ExamplesOverview.md)

More generally, any new feature in the MRTK should contain a corresponding example in the examples package, roughly following
the same folder structure and location.

## Tools パッケージ

The tools package contains tools that are useful for creating mixed reality experiences whose code will ultimately not
ship as part of an application.

- [Dependency Window](../Tools/DependencyWindow.md)
- [Extension Service Creation Wizard](../Tools/ExtensionServiceCreationWizard.md)
- [Optimize Window](../Tools/OptimizeWindow.md)
- [Screenshot Utility](../Tools/ScreenshotUtility.md)
