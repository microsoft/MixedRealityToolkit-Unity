# Mixed Reality Toolkit パッケージ

Mixed Reality Toolkit (MRTK) は、Mixed Reality ハードウェアとプラットフォームをサポートすることにより、クロスプラットフォーム Mixed Reality アプリケーション開発を可能にするパッケージのコレクションです。

MRTK は、次の Unity パッケージを介してリリースしています。

- [Foundation](#foundation-パッケージ)
- [Extensions](#extensions-パッケージ)
- [Examples](#examples-パッケージ)
- [Tools](#tools-パッケージ)

これらのパッケージは、GitHub の [mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release) ブランチのソース コードから、Microsoft によってリリース、サポートされています。

## Foundation パッケージ

Mixed Reality Toolkit Foundation は、アプリケーションが Mixed Reality プラットフォーム間で共通の機能を活用できるようにするコードのセットです。

<img src="../../Documentation/Images/Input/MRTK_Package_Foundation.png" width="350px" style="display:block;">  
<sup>MRTK Foundation パッケージ</sup>

MRTK Foundation は以下から構成されています。

* **Core Package**

Core Package には他のすべてのコンポーネントで使用される共通のインターフェイス、クラス、データ型のすべての定義が含まれています。プラットフォーム間で最高レベルの互換性を実現するために、定義されたインターフェイスのみを介して MRTK コンポーネントにアクセスすることを強くお勧めします。

* **Platform Providers**

MRTK Platform Provider パッケージは、Mixed Reality Toolkit が Mixed Reality ハードウェアおよびプラットフォーム機能を対象とすることを可能にするコンポーネントです。

サポートされているプラットフォームは次のとおりです。

- Windows Mixed Reality
- OpenVR
- Windows Voice

* **System Services**

Core サービスは、Core Package で定義されているシステム サービス インターフェイスのデフォルト実装を提供しています。

MRTK foundation には、以下のシステム サービスが含まれています。

- [Boundary System](../Boundary/BoundarySystemGettingStarted.md)
- [Diagnostic System](../Diagnostics/DiagnosticsSystemGettingStarted.md)
- [Input System](../Input/Overview.md)
- [Spatial Awareness System](../SpatialAwareness/SpatialAwarenessGettingStarted.md)
- [Teleport System](../TeleportSystem/Overview.md)

* **Feature Assets**

Feature Assets は、Unity アセットとスクリプトとして提供される関連機能のコレクションです。ユーザー インターフェイス コントロールやスタンダード アセットなどが含まれます。

## Extensions パッケージ

Extensions パッケージには、Foundation パッケージの機能を拡張する追加のサービスとコンポーネントが含まれています。

- [シーン遷移サービス](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md)

## Examples パッケージ

Examples パッケージには、Foundation パッケージの機能を使用するデモ、サンプル スクリプト、およびサンプル シーンが含まれています。このパッケージには、さまざまなタイプのハンド インプット (多関節および非多関節) に反応するサンプル オブジェクトが含まれる [HandInteractionExample シーン](../README_HandInteractionExamples.md) (下図) が含まれています。

![HandInteractionExample シーン](../../Documentation/Images/MRTK_Examples.png)

このパッケージには、アイ トラッキング デモも含まれています。詳細は[こちら](../EyeTracking/EyeTracking_ExamplesOverview.md)に記載されています。

より一般的には、MRTK のすべての新機能について、同じフォルダ構造と場所にほぼ従って、対応するサンプルが Examples パッケージに含まれているべきです。

## Tools パッケージ

Tools パッケージには、Mixed Reality エクスペリエンスを作成するのに役立つツールが含まれています。これらのコードは、最終的にアプリケーションの一部としてリリースされることはありません。

- [Dependency Window](../Tools/DependencyWindow.md)
- [Extension Service Creation Wizard](../Tools/ExtensionServiceCreationWizard.md)
- [Optimize Window](../Tools/OptimizeWindow.md)
- [Screenshot Utility](../Tools/ScreenshotUtility.md)

## 関連項目

- [アーキテクチャ 概要](../Architecture/Overview.md)
- [Systems, Extension Services and Data Providers](../Architecture/SystemsExtensionsProviders.md)
