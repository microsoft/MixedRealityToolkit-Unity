# Mixed Reality Toolkit プロファイル設定ガイド
<a name="mixed-reality-toolkit-profile-configuration-guide"></a>

![MRTK logo](../Documentation/Images/MRTK_Logo_Rev.png)

Mixed Reality Toolkit は、ツールキットを管理するために必要な設定を可能な限り集中化しています (ランタイムの「モノ」は除く)。

このガイドは、ツールキットで現在使用できる、設定プロファイル画面の簡単なチュートリアルです。

<a name="the-main-mixed-reality-toolkit-configuration-profile"></a>

## Mixed Reality Toolkit の主な設定プロファイル (Congifuration Profile)

シーンの _MixedRealityToolkit_ GameObject にアタッチされている主な設定プロファイル (Congifuration Profile) は、ツールキットの最初のエントリーポイントです。

> [!NOTE]
> Mixed Reality Toolkit は、プロジェクトの共通のスタートポイントを常に持たせるように、デフォルトの設定画面を「ロック」しています。そして、プロジェクトの進捗に合わせて独自の設定を定義することをお勧めします。MRTK の設定はプレイモード中は編集できません。

![MRTK configuration profile](../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_ActiveConfiguration.png)

Mixed Reality Toolkit のすべての「デフォルト」プロファイルは、Assets/MRTK/SDK/Profiles フォルダ内の SDK プロジェクトにあります。

> [!IMPORTANT]
> デフォルトの HoloLens 2 の設定プロファイルは、HoloLens 2 用に最適化されています。 [Profiles](Profiles/Profiles.md) をご確認ください。

メインの Mixed Reality Toolkit 設定プロファイルを開くと、Inspector (インスペクター)に次の画面が表示されます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_MixedRealityToolkitConfigurationScreen.png" width="650px" style="display:block;">

シーン内で MixedRealityToolkit を使用せずに MixedRealityToolkitConfigurationProfile アセットを選択すると、MRTK がシーンを自動的に設定するかどうかを確認するメッセージが表示されます。これはオプションですが、すべての設定画面にアクセスするには、シーン内にアクティブな MixedRealityToolkit オブジェクトが存在する必要があります。

これにより、プロジェクトの現在アクティブなランタイム設定が保存されます。

ここから、MRTK のすべての設定プロファイルに移動できます。

- [Mixed Reality Toolkit プロファイル設定ガイド](#mixed-reality-toolkit-profile-configuration-guide)
  - [Mixed Reality Toolkit の主な設定プロファイル](#the-main-mixed-reality-toolkit-configuration-profile)
  - [Experience settings（エクスペリエンスの設定）](#experience-settings)
  - [Camera settings（カメラの設定）](#camera-settings)
  - [Input system settings（入力システムの設定）](#input-system-settings)
  - [Boundary visualization settings（境界可視化の設定）](#boundary-visualization-settings)
  - [Teleportation system selection（テレポーテーション システムの選択）](#teleportation-system-selection)
  - [Spatial awareness settings（空間認識の設定）](#spatial-awareness-settings)
  - [Diagnostics settings（診断の設定）](#diagnostics-settings)
  - [Scene system settings（シーン システムの設定）](#scene-system-settings)
  - [追加サービスの設定](#additional-services-settings)
  - [Input actions settings（入力アクションの設定）](#input-actions-settings)
  - [Input action rules （入力アクション ルール）](#input-actions-rules)
  - [Pointer configuration（ポインターの設定）](#pointer-configuration)
  - [Gestures configuration（ジェスチャの設定）](#gestures-configuration)
  - [Speech commands（音声コマンド）](#speech-commands)
  - [Controller mapping configuration（コントローラー マッピングの設定）](#controller-mapping-configuration)
  - [Controller visualization settings（コントローラーの可視化の設定）](#controller-visualization-settings)
  - [Editor Utilities](#editor-utilities)
    - [Service inspectors（サービス インスペクター）](#service-inspectors)
    - [Depth buffer renderer（デプス バッファー レンダラー）](#depth-buffer-renderer)
  - [実行時のプロファイル変更](#changing-profiles-at-runtime)
  - [MRTK 初期化前にプロファイルを差し替える](#swapping-profiles-prior-to-mrtk-initialization)
  - [関連項目](#see-also)

これらの設定プロファイルについては、以下の関連セクションで詳しく説明します。

---
<a name="experience-settings"></a>

## Experience settings（エクスペリエンスの設定）

Mixed Reality Toolkit のメイン設定ページにあるこの項目では、プロジェクトの [Mixed Reality 環境のスケール](https://docs.microsoft.com/en-us/windows/mixed-reality/coordinate-systems-in-unity) のデフォルトの操作が定義されます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_ExperienceSettings.png" width="650px" style="display:block;">

---
<a name="camera-settings"></a>

## Camera settings（カメラの設定）

カメラ設定では、Mixed Reality プロジェクトのカメラの設定方法を定義し、一般的なクリッピング (clipping) 、クオリティ、透明度 (transparency) の設定を定義します。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_CameraProfile.png" width="650px" style="display:block;">

---
<a name="input-system-settings"></a>

## Input system settings（入力システムの設定）

Mixed Reality プロジェクトは、すべての入力イベントを取り扱うための堅牢で十分に熟練した入力システムを提供しており、これはデフォルトで選択されています。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputSystemSelection.png" width="650px" style="display:block;">

MRTK が提供する入力システムの背後にはその他のいくつかのシステムがあり、マルチプラットフォーム / Mixed Reality フレームワークの複雑さを抽象化するために、複雑な絡み合わせを駆動させ、管理するのを補助します。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputSystemProfile.png" width="650px" style="display:block;">

個々のプロファイルの詳細は以下のとおりです。

* Focus Settings（フォーカスの設定）
* [Input actions settings（入力アクションの設定）](#input-actions-settings)
* [Input action rules （入力アクション ルール）](#input-actions-rules)
* [Pointer configuration（ポインターの設定）](#pointer-configuration)
* [Gestures configuration（ジェスチャの設定）](#gestures-configuration)
* [Speech commands（音声コマンド）](#speech-commands)
* [Controller mapping configuration（コントローラー マッピングの設定）](#controller-mapping-configuration)
* [Controller visualization settings（コントローラーの可視化の設定）](#controller-visualization-settings)

---
<a name="boundary-visualization-settings"></a>

## Boundary visualization settings（境界可視化の設定）

境界 (Boundary) システムは、元となるプラットフォームの境界/ガーディアン システムから取得した境界を解釈します。境界ビジュアライザー (Boundary visualizer) 設定を使用すると、シーン内で記録された境界をユーザーの位置に対して自動的に表示できます。境界線は、ユーザーがシーン内でテレポートする場所に基づいて反応/更新も行われます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_BoundaryVisualizationProfile.png" width="650px" style="display:block;">

---
<a name="teleportation-system-selection"></a>

## Teleportation system selection（テレポーテーション システムの選択）

Mixed Reality プロジェクトは、プロジェクトでのテレポーテーション イベントを管理するための完全な機能を備えたテレポーテーション システムを提供しており、これはデフォルトで選択されています。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_TeleportationSystemSelection.png" width="650px" style="display:block;">

---
<a name="spatial-awareness-settings"></a>

## Spatial awareness settings（空間認識の設定）

Mixed Reality プロジェクトは、空間スキャン システムと連携する再構成された空間認識システムを提供しており、これはデフォルトで選択されています。MRTK 空間認識システムの背後にあるアーキテクチャは[こちら](Architecture/SpatialAwareness.md)で見ることができます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpatialAwarenessSystemSelection.png" width="650px" style="display:block;">

Mixed Reality Toolkit の空間認識設定を使用すると、どのようにシステムが開始されるかを調整できます。それは、アプリケーションの起動時に自動的に行うことも、視野の範囲を設定するのと同様に後でプログラムから行うこともできます。

また、メッシュとサーフェスの設定を行い、プロジェクトが周囲の環境をどのように認識するかをさらにカスタマイズすることもできます。

これは、スキャンされた環境を提供できるデバイスにのみ適用されます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpatialAwarenessProfile.png" width="650px" style="display:block;">

---
<a name="diagnostics-settings"></a>

## Diagnostics settings（診断の設定）

オプションですが、MRTK の非常に便利な機能は、プラグイン診断機能です。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_DiagnosticsSystemSelection.png" width="650px" style="display:block;">

診断プロファイルは、シーン内の表示パネルを有効/無効にする便利なスイッチオン/オフなど、プロジェクト実行中での監視のためのいくつかの簡単なシステムを提供します。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_DiagnosticsProfile.png" width="650px" style="display:block;">

---
<a name="scene-system-settings"></a>

## Scene system settings（シーン システムの設定）

MRTK は、複雑なシーンのロード/アンロードの管理に役立つオプション サービスを提供します。シーン システムがプロジェクトに適しているかどうかを判断するには、[シーン システムの開始ガイド](SceneSystem/SceneSystemGettingStarted.md)を参照してください。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_SceneSystemProfile.png" width="650px" style="display:block;">

---
<a name="additional-services-settings"></a>

## 追加サービスの設定

Mixed Reality Toolkit のより高度な領域の1つは、フレームワークに任意の「サービス」の登録を可能にする[サービス ロケーター パターン](https://en.wikipedia.org/wiki/Service_locator_pattern)の実装です。これにより、フレームワークに新しい機能/システムを簡単に拡張できるだけでなく、プロジェクトはこれらの機能を利用して独自のランタイムコンポーネントを登録することもできます。


登録済みのサービスは、MonoBehaviour または不格好なシングルトン パターンを実装するオーバーヘッドとコストなしで、すべての Unity イベントを最大限に活用できます。これにより、フォアグラウンド（foreground）プロセスとバックグラウンド (background) プロセスの両方で、シーン オーバーヘッドのない純粋な C# コンポーネントを使用できます。例えば、スポーン システム、ランタイム ゲーム ロジックなど、実質的には全てのもので利用できます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_RegisteredServiceProvidersProfile.png" width="650px" style="display:block;">

---
<a name="input-actions-settings"></a>

## Input actions settings（入力アクションの設定）

入力アクションを使用すると、ランタイムプロジェクトからのすべての物理的なインタラクションや入力を抽象化することができます。すべての物理入力 (コントローラー/手/マウス等) は、ランタイムプロジェクトで使用するための論理入力アクションに変換されます。これにより、入力元に関係なく、プロジェクトはシーン内でこれらのアクションを「実行する操作」または「対象とのインタラクション」としてシンプルに実装できます。

新しい入力アクションを作成するには、[Add a new Action] ボタンをクリックし、それが何を表すのかがわかりやすい名前を入力します。次に、アクションが伝える軸 (データ型) を選択するか、物理コントローラーの場合は、アクションにひもづく物理入力タイプを選択するだけです。例えば、以下のような軸やデータ型があります。

| 軸拘束 | データ型 | 説明 | 使用例 |
| :--- | :--- | :--- | :--- |
| None（なし） | データなし | 空のアクションまたはイベントに使用 | イベント トリガー |
| Raw（生データ） （予約済み） | object | 将来の使用のために予約済み| N/A |
| Digital（デジタル） | bool | boolean 値のオン オフ データ型  | コントローラー ボタン |
| Single Axis（1軸）| float | 単精度のデータ値  | トリガーなど、範囲のある入力  |
| Dual Axis（2軸）| Vector2 | 複数軸のための、2つの float データ型 | 十字キーやジョイスティック |
| Three Dof Position（3自由度位置） | Vector3 | 3つの float データからなる位置のタイプデータ | 3D 位置スタイルのみコントローラー  |
| Three Dof Rotation（3自由度回転） | Quaternion | 4つの float 軸をもつ回転のみの入力 | 3軸のコントローラー、例えば Oculus Go コントローラー |
| Six Dof（6自由度） | Mixed Reality Pose (Vector3, Quaternion) | Vector3 コンポーネントと Quaternion コンポーネントの両方を使用した位置と回転の入力 | モーション コントローラーまたは Pointer |

入力アクションを利用するイベントは物理コントローラーに限定されず、プロジェクト内で実行時に新しいアクションを生成できます。

> [!NOTE]
> 入力アクションは、実行時に編集できない数少ないコンポーネントの 1 つであり、設計時のみ変更できます。このプロファイルは、各アクションに対して生成された ID に対しフレームワーク (およびプロジェクト) が依存しているため、プロジェクトの実行中に入れ替えをしないでください。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputActionsProfile.png" width="650px" style="display:block;">

---
<a name="input-actions-rules"></a>

## Input action rules （入力アクション ルール）

Input action rules を使用すると、1 つの入力アクションに対して発生したイベントを、そのデータ値に基づいて別のアクションに自動的に変換できます。これらはフレームワーク内でシームレスに管理され、パフォーマンスコストは発生しません。

たとえば、1 つの十字キーの 2 軸入力イベントを、4 つの対応する "DPad Up" / "DPad Down" / "Dpad Left" / "Dpad Right" アクションに変換します (下の図に示すように)。

これは、独自のコードで行うこともできます。しかし、これは非常に一般的なパターンであったので、フレームワークはこれをすぐに使えるようにするメカニズムを提供しています。

Input Action Rules は、使用可能な入力軸のすべてに対して設定できます。ただし、1つの軸タイプからの入力アクションは、同じ軸タイプの別の入力アクションに変換できます。2 軸アクションを別の 2 軸アクションにマップできますが、デジタルアクションまたは None アクションにはマップできません。

![Input action rules profile](../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputActionRulesProfile.png)

---
<a name="pointer-configuration"></a>

## Pointer configuration（ポインターの設定）

ポインターは、シーン内ですべての入力デバイスからの相互作用を働かせるために使われ、入力の向きとヒット テストの両方を、シーン内の任意のオブジェクト (コライダーがアタッチされているもの、または UI コンポーネント) に対し与えます。ポインターは、デフォルトでは、コントローラー、ヘッドセット(ゲイズ / フォーカス)、マウス / タッチ入力用に自動的に設定されます。

ポインターは、Mixed Reality Toolkit によって提供される多くのラインコンポーネントの 1 つを使用してアクティブなシーン内で可視化することもできますし、MRTK IMixedRealityPointer インターフェイスを実装している場合は独自のコンポーネントを使用して可視化することもできます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_InputPointerProfile.png" width="650px" style="display:block;">

* Pointing Extent: ゲイズを含むすべてのポインターのグローバルなポインティング範囲を決定します。
* Pointing Raycast Layer Masks: ポインターがどのレイヤーに対してレイキャストを行うかを決定します。
* Debug Draw Pointing Rays: レイキャストに使用されるレイを可視化するためのデバッグ ヘルパー
* Debug Draw Pointing Rays Colors: 可視化に使用する色のセット
* Gaze cursor prefab: 任意のシーンにグローバルなゲイズ カーソルを簡単に指定できるようにします。

必要に応じて Gaze のなにか特定の値をオーバーライドするために、Gaze Provider にすばやくジャンプするための追加のヘルパーボタンがあります。

---
<a name="gestures-configuration"></a>

## Gestures configuration（ジェスチャの設定）

ジェスチャは、入力アクションをさまざまな SDK (HoloLens など) によって提供されるさまざまな「ジェスチャ」入力メソッドに割り当てることができるシステム固有の実装です。

> [!NOTE]
> 現在のジェスチャの実装は HoloLens 専用で、将来ツールキットに他のシステムが追加される際に拡張される予定です (日付は未定です)。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_GesturesProfile.png" width="650px" style="display:block;">

---
<a name="speech-commands"></a>

## Speech Commands（音声コマンド）

ジェスチャと同様に、一部のランタイム プラットフォームでは、Unity プロジェクトで受信できるコマンドを生成する機能を備えたインテリジェントな "Speech to Text" 機能も提供します。この設定プロファイルを使用すると、以下の設定ができます。

1. General Settings - "Start Behavior" を Auto Start に設定するか Manual Start に設定するかによって、KeywordRecognizer を入力システムの起動時に初期化するか、KeywordRecognizer をいつ初期化するかをプロジェクトに決めさせるかが決定されます。"Recognition Confidence Level" は Unity の [KeywordRecognizer API](https://docs.unity3d.com/ScriptReference/Windows.Speech.KeywordRecognizer-ctor.html) を初期化するのに使われます。
2. Speech Commands - 「単語」を登録し、プロジェクトで受信できる入力アクションに変換します。必要に応じて、キーボード アクションにアタッチすることもできます。

> [!IMPORTANT]
> このシステムは現在、HoloLens や Windows 10 デスクトップなどの Windows 10 プラットフォームで実行されている場合にのみ音声認識をサポートしており、将来 MRTK に他のシステムが追加される際に拡張される予定です (日付は未定です)。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_SpeechCommandsProfile.png" width="650px" style="display:block;">


---
<a name="controller-mapping-configuration"></a>

## Controller mapping configuration（コントローラー マッピングの設定）

Mixed Reality Toolkit の主要な設定画面の1つは、プロジェクトで使用できるさまざまな種類のコントローラーを設定およびマッピングする機能です。

以下の設定画面では、ツールキットに現在認識されているコントローラーのいずれかを設定できます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_ControllerMappingProfile.png" width="650px" style="display:block;">

MRTK は、以下のコントローラー / システムのデフォルト設定を提供しています。

* マウス (3D 空間マウスのサポートを含む)
* タッチ スクリーン
* Xbox コントローラー
* Windows Mixed Reality コントローラー
* HoloLens ジェスチャー
* HTC Vive ワンド コントローラー
* Oculus Touch コントローラー
* Oculus Remote コントローラー
* 汎用 OpenVR デバイス (上級ユーザーのみ)

あらかじめ構築されたコントローラー システムのいずれかの画像をクリックすると、対応するすべての入力に対して 1 つの入力アクションを設定できます (例えば、以下の Oculus Touch コントローラーの設定画面をご覧ください)。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_WindowsMixedRealityControllerConfigScreen.png" width="650px" style="display:block;">

上記で識別されていない他の OpenVR または Unity 入力コントローラーを設定するための高度な画面もあります。

---
<a name="controller-visualization-settings"></a>

## Controller visualization settings（コントローラーの可視化の設定）

コントローラーのマッピングに加えて、シーン内でのコントローラーの表示方法をカスタマイズするための個別の設定プロファイルが用意されています。

これは、「Global」(特定のハンド用コントローラーのすべてのインスタンス) または個々のコントローラー タイプ/ハンドに対して個別に設定が可能です。

MRTK はまた、 Windows Mixed Reality と OpenVR に関してネイティブ SDK のコントローラー モデルをサポートしています。これらはシーン内で GameObjects としてロードされ、プラットフォームのコントローラー トラッキングを使って配置されます。

シーン内のコントローラーの表示を物理コントローラーの位置からオフセットする必要がある場合は、コントローラー モデルのプレハブに対してオフセットを設定するだけです (例えば、コントローラー プレハブの transform position をオフセット位置に設定します)。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_ControllerVisualizationProfile.png" width="650px" style="display:block;">

<a name="editor-utilities"></a>

## Editor Utilities

以下のユーティリティはエディターでのみ動作し、開発の生産性を向上させるのに役に立ちます。

![MRTK Editor Configuration Utilities](../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_EditorConfiguration.png)

<a name="service-inspectors"></a>

### Service Inspectors（サービス インスペクター）

サービス インスペクターは、アクティブなサービスを表すシーン内オブジェクトを生成する、エディター上のみの機能です。これらのオブジェクトを選択すると、\[Inspector]（インスペクター）にドキュメントへのリンク、エディターでの可視化のコントロール、サービスの状態に関する情報が表示されます。

<img src="../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_ServiceInspectors.PNG" width="350px" style="display:block;">

Configuration Profile の *Editor Settings* の *Use Service Inspectors* をチェックすると、サービス インスペクターを有効にできます。

<a name="depth-buffer-renderer"></a>

### Depth Buffer Renderer (デプス バッファー レンダラー)

いくつかの mixed reality プラットフォームではデプス バッファーを共有することで[ホログラムの安定性]を向上させることができます。例えば、Windows Mixed Reality プラットフォームでは、フレームを描画するのにかかる時間でのわずかな頭の動きを考慮して、ピクセルごとに描画したシーンを修正できます。しかしながらこれらの技術は、オブジェクトの形状がユーザーからどこにどれくらい離れているのかを知るために、正確なデータをもったデプス バッファーが必要です。 

シーンが必要なデータすべてをデプス バッファーに描画していることを確かめるため、開発者は Configuration Profile の *Editor Settings* 以下の *Render Depth Buffer* 機能をトグルしてください。これは、現在のデプス バッファーを使い、[`DepthBufferRenderer`](xref:Microsoft.MixedReality.Toolkit.Rendering.DepthBufferRenderer) というポストプロセッシング エフェクトを適用することでメイン カメラのシーン ビューに色をつけて描画します。

![Render Depth Buffer Utility](../Documentation/Images/MixedRealityToolkitConfigurationProfileScreens/MRTK_DepthBufferExample.gif)
<sup>シーン内の青い円柱は ZWrite off のマテリアルを持っているため、デプス データは書き込まれません</sup>

<a name="changing-profiles-at-runtime"></a>

## 実行時のプロファイル変更

プロファイルを実行時に更新することも可能で、一般に2つのシナリオとタイミングでこれは有用です。

1. 起動時、MRTK が初期化される前にプロファイルを差し替え、デバイスの機能に基づいてさまざまな機能を有効/無効にします。
   例えば、もし空間マッピングのハードウェアを持っていない VR で実行されている場合は、おそらく空間マッピング コンポーネントが有効になっている意味はないでしょう。
1. 起動後、MRTK が初期化された後にプロファイルを差し替え、特定の機能のふるまい変更します。
   例えば、アプリケーション内にファー ハンド ポインターを完全に取り除きたいような特定のサブコンテンツがあるかもしれません。
   このタイプの変更は、現在は次の issue のために機能しないことに**ご注意ください**。
   https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4289

<a name="swapping-profiles-prior-to-mrtk-initialization"></a>

## MRTK 初期化前にプロファイルを差し替える

これは MRTK 初期化前に動作する以下のような MonoBehaviour をアタッチすることで実現できます。

```csharp
using Microsoft.MixedReality.Toolkit;
using UnityEditor;
using UnityEngine;

/// <summary>
/// MixedRealityToolkit オブジェクトの前に動作し、プロファイルを変更するサンプルの MonoBehaviour です。
/// MRTK はシーンに保存されたプロファイルではなく、以下で指定したプロファイルを使って初期化します。
/// </summary>
/// <remarks>
/// このスクリプトは、MixedRealityToolkit.cs よりも実行順で高い優先度でなければなりません。スクリプトの実行順序についての
// より多くの情報は https://docs.unity3d.com/Manual/class-MonoManager.html をご覧ください。
/// </remarks>
public class ProfileSwapper : MonoBehaviour
{
    void Start()
    {
        // ここで任意の MixedRealityToolkitConfigurationProfile を選択できます。
        // 例えば、どのプロファイルをロードするかを決定するためにプラットフォームをチェックするコードを追加することもできます。
        var profile = AssetDatabase.LoadAssetAtPath<MixedRealityToolkitConfigurationProfile>("Assets/MixedRealityToolkit.Generated/CustomProfiles/RuntimeSwapparoo.asset");
        MixedRealityToolkit.Instance.ActiveProfile = profile;
    }
}
```

"RuntimeSwapparoo.asset" の代わりに、特定のプラットフォームに適用する任意のプロファイル セット（例えば、HoloLens 用プロファイル、VR 用, HoloLens2 用など）を持つことも可能です。
さまざまな他の指標 (すなわち、https://docs.unity3d.com/ScriptReference/SystemInfo.html やカメラが不透明/透明かどうかなど) を使ってどのプロファイルをロードするか決めることも可能です。

<a name="see-also"></a>

## 関連項目

- [ホログラムの安定化](hologram-stabilization.md)
