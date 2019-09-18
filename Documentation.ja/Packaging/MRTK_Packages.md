# Mixed Reality Toolkit パッケージ

Mixed Reality Toolkit (MRTK) は、Mixed Reality ハードウェアとプラットフォームをサポートすることにより、クロスプラットフォーム Mixed Reality アプリケーション開発を可能にするパッケージのコレクションです。

MRTK は、次のパッケージを介してリリースしています。

- [Foundation](#foundation-パッケージ)
- [Extensions](#extensions-パッケージ)
- [Examples](#examples-パッケージ)
- [Tools](#tools-パッケージ)

## Foundation パッケージ

Mixed Reality Toolkit Foundation は、アプリケーションが Mixed Reality プラットフォーム間で共通の機能を活用できるようにするパッケージのセットです。これらのパッケージは、GitHub の [mrtk_release](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release) ブランチのソース コードからマイクロソフトによってリリースされ、サポートされています。

<img src="../../Documentation/Images/Input/MRTK_Package_Foundation.png" width="350px" style="display:block;"><br/>
<sup>MRTK Foundation パッケージ</sup>

MRTK Foundation は以下の通りに構成されています。

- [Core パッケージ](#core-パッケージ)
- [プラットフォーム プロバイダー](#プラットフォーム-プロバイダー)
- [システム サービス](#システム-サービス)
- [機能アセット](#機能アセット)

次のセクションでは、各カテゴリのパッケージの種類について説明します。

### Core パッケージ

Core パッケージは _必須_ コンポーネントであり、すべての MRTK Foundation パッケージが依存しています。

MRTK Core パッケージには以下が含まれます。

- [共通のインターフェイス、クラス、データ型](#共通型)
- [MixedRealityToolkit シーン コンポーネント](#mixedrealitytoolkit-シーン-コンポーネント)
- [MRTK スタンダード シェーダー](#mrtk-スタンダード-シェーダー)
- [Unity インプット プロバイダー (Unity Input Provider)](#unity-インプット-プロバイダー-unity-input-provider)
- [パッケージ管理](#パッケージ管理)

#### 共通型

Mixed Reality Toolkit Core パッケージには、他のすべてのコンポーネントで使用される共通のインターフェイス、クラス、データ型のすべての定義が含まれています。プラットフォーム間で最高レベルの互換性を実現するために、定義されたインターフェイスのみを介して MRTK コンポーネントにアクセスすることを強くお勧めします。

#### MixedRealityToolkit シーン コンポーネント

MixedRealityToolkit シーン コンポーネントは、Mixed Reality Toolkit の単一の集中化されたリソース マネージャーです。このコンポーネントは、プラットフォーム モジュールとサービス モジュールのライフスパンをロードおよび管理し、システムが構成設定にアクセスするためのリソースを提供します。

#### MRTK スタンダード シェーダー

MRTK のスタンダード シェーダーは MRTK によって提供される事実上すべてのマテリアルの基礎を提供します。このシェーダは非常に柔軟性が高く、MRTK がサポートされているさまざまなプラットフォームに最適化されています。最適なパフォーマンスを実現するには、アプリケーションのマテリアルで MRTK スタンダード シェーダーを使用することを _強く_ お勧めします。

#### Unity インプット プロバイダー (Unity Input Provider)

Unity インプット プロバイダーは、ゲーム コントローラ、タッチ スクリーン、3D 空間マウスなどの一般的な入力デバイスへのアクセスを提供します。

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

Windows Mixed Reality パッケージは、Microsoft HoloLens、HoloLens 2 および Windows Mixed Reality 没入型デバイスをサポートします。パッケージには、次を含む完全なプラットフォーム サポートが含まれています。

- 多関節ハンド
- アイトラッキング
- ゲイズ ターゲティング
- ジェスチャー
- 空間マッピング
- Windows Mixed Reality モーション コントローラー

#### OpenVR

OpenVR パッケージは、OpenVR プラットフォームを使用するデバイスのハードウェアとプラットフォームのサポートを提供します。

#### Windows Voice

Windows Voice パッケージは、Microsoft Windows 10 デバイスでのキーワード認識と聞き取り (dictation) 機能のサポートを提供します。

### システム サービス

Core プラットフォーム サービスは、システム サービス パッケージで提供されます。これらのパッケージには、[Core](#core-パッケージ) パッケージで定義されたシステム サービス インターフェイスの Mixed Reality Toolkit のデフォルトの実装が含まれています。

MRTK foundation には、以下のシステムサービスが含まれています。

- [境界システム](#境界システム-boundary-system)
- [診断システム](#診断システム-diagnostic-system)
- [入力システム](#入力システム-input-system)
- [空間認識システム](#空間認識システム-spatial-awareness-system)
- [テレポート システム](#テレポート-システム-teleport-system)

#### 境界システム (Boundary System)

MRTK 境界システムは、仮想現実 (VR) プレイスペースに関するデータを提供します。ユーザーが境界を設定しているシステムでは、床の平面、長方形のプレイスペース、トラッキング領域などを提供できます。

#### 診断システム (Diagnostic System)

MRTK 診断システムは、アプリケーション エクスペリエンス内のリアルタイムのパフォーマンス データを提供します。アプリケーションを使用する際、フレーム レート、プロセッサー時間、およびその他の主要なパフォーマンス指標をひと目で見ることができます。

#### 入力システム (Input System)

MRTK 入力システムを使用すると、クロスプラットフォームでの入力にアクセスできます。ユーザー アクションを指定し、ターゲット コントローラー上の最も適切なボタンと軸にそれらのアクションを割り当てることにより設定することができます。

#### 空間認識システム (Spatial Awareness System)

MRTK 空間認識システムを使用することで、Microsoft HoloLens などのデバイスから実際の環境データにアクセスできます。

#### テレポート システム (Teleport System)

MRTK テレポートシステムは、仮想現実 (VR) 移動サポートを提供します。

### 機能アセット

機能アセットは、Unity アセットとスクリプトとして提供される関連機能のコレクションです。これらの機能の一部：

- ユーザー インターフェイスの制御
- スタンダード アセット
- 更に

## Extensions パッケージ

Extensions パッケージには、Foundation パッケージの機能を拡張する追加のサービスとコンポーネントが含まれています。

- [シーン遷移サービス](../Extensions/SceneTransitionService/SceneTransitionServiceOverview.md)

## Examples パッケージ

Examples パッケージには、Foundation パッケージの機能を使用するデモ、サンプル スクリプト、およびサンプル シーンが含まれています

たとえば、このパッケージには、さまざまなタイプのハンド インプット (多関節および非多関節) に対応するサンプル オブジェクトを含む HandInteractionExample シーン (下図) が含まれています。

![HandInteractionExample シーン](../Images/MRTK_Examples.png)

このパッケージには、アイトラッキングデモも含まれています。詳細は[こちら](../EyeTracking/EyeTracking_ExamplesOverview.md)に記載されています：

より一般的には、MRTK のすべての新機能について、同じフォルダ構造と場所にほぼ従って、対応するサンプルが Examples パッケージに含まれているべきです。

## Tools パッケージ

Tools パッケージには、Mixed Reality エクスペリエンスを作成するのに役立つツールが含まれています。これらのコードは、最終的にはアプリケーションの一部としてリリースされることはありません。

- [Dependency Window](../Tools/DependencyWindow.md)
- [Extension Service Creation Wizard](../Tools/ExtensionServiceCreationWizard.md)
- [Optimize Window](../Tools/OptimizeWindow.md)
- [Screenshot Utility](../Tools/ScreenshotUtility.md)
