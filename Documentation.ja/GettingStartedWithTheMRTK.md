# MRTK を始める

![MRTK Logo](../Documentation/Images/MRTK_Logo_Rev.png)

Mixed Reality Toolkit (MRTK) は、Virtual Reality (VR) 及び Augmented Reality (AR) の Mixed Reality エクスペリエンス を構築するためのクロスプラットフォームツールキットです。

## 前提条件

Mixed Reality Toolkit を始めるには、次のものが必要です。

* [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
* [Unity 2018.4.x, 2019.1.x or 2019.2.x](https://unity3d.com/get-unity/download/archive)

  MRTKは、 Unity 2018 で IL2CPP と .NET scripting backends の両方をサポートします。

* [最新の MRTK release](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases)
* [Windows SDK 18362+](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk).

  これは WMR、HoloLens 1、または HoloLens 2 向けの UWPアプリを構築する場合に必要です。OpenVR 向けに構築する場合は不要です。

## チュートリアルを始める

MRTK、またはMR開発が初めての場合は、MRTKv2を使った [チュートリアルを始める](https://docs.microsoft.com/en-us/windows/mixed-reality/mrlearning-base)をチェックすることをお勧めします。

## MRTK を Unity Projectに追加する

### 最新の MRTK Unity パッケージを取得する

1. [MRTK のリリースページ](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases) を開きます。
2. Assets の下から以下をダウンロードします。
    * `Microsoft.MixedRealityToolkit.Unity.Foundation.unitypackage`
    * `Microsoft.MixedRealityToolkit.Unity.Extensions.unitypackage`
    * `Microsoft.MixedRealityToolkit.Unity.Tools.unitypackage`
    * `Microsoft.MixedRealityToolkit.Unity.Examples.unitypackage`

より詳細な配布の仕組みは、[MRTK をダウンロードする](DownloadingTheMRTK.md)を参照して下さい。

### Unity プロジェクトをターゲットプラットフォームに切り替える

次のステップ **MRTK のパッケージ を Unity プロジェクトへインポート** するでは、インポートした時点でプロジェクトで選択されているプラットフォームに応じた変更がプロジェクトに適応されます。

次のステップへ進む前に、正しいプラットフォームを選択していることを確認してください。

例えば、HoloLens アプリケーションを作成する場合は、Universal Windows Platform に切り替えます。

* File > Build Settings からメニューを開きます。
* **Platform** の一覧から、**Universal Windows Platform** を選択します。
* **Switch Platform** ボタンを押します。

### MRTK のパッケージを Unity プロジェクトにインポートする

1. 新しい Unity プロジェクトを作成するか、既存のプロジェクトを開きます。新しいプロジェクトを作成する場合は、テンプレートタイプに "3D" が選択されていることを確認してください。
1. ダウンロードした `Microsoft.MixedRealityToolkit.Unity.Foundation.unitypackage` をインポートします。「Asset -> Import Package -> Custom Package」から, .unitypackage ファイルを選択し, インポートする全ての項目がチェックされていることを確認してから、「Import」を選択します.
1. `Microsoft.MixedRealityToolkit.Unity.Examples.unitypackage` も上記と同様の手順でインポートします。examples のパッケージは、オプションであり、現在の MRTK の機能の有用なデモシーンが含まれています。
1. `Microsoft.MixedRealityToolkit.Unity.Tools.unitypackage` も Foundaiton パッケージと同様にインポートします。tools のパッケージは、オプションであり、MRTK 開発者のエクスペリエンスを向上させる、ExtensionServiceCreator などの便利なツールが含まれています。
1. Import the `Microsoft.MixedRealityToolkit.Unity.Extensions.unitypackage` も Foundaiton パッケージと同様にインポートします。 extensions パッケージは、オプションで、 MRTK の便利なオプションコンポーネントのセットを提供します。

Foundation パッケージをインポートすると、次のようなセットアッププロンプトが表示される場合があります。

![UnitySetupPrompt](../Documentation/Images/MRTK_UnitySetupPrompt.png)

MRTK は、以下を実行することで Mixed Reality ソリューションを構築するためのプロジェクトをセットアップします。

* 現在のプラットフォームで XR Settings を有効にします (XR チェックボックスを有効にします)。
* テキストのシリアライズを強制 / メタファイルの可視化 （ソース管理をする　Unity プロジェクトに推奨）。

これらのオプションを適応するかは選択的ですが、推奨されています。

一部のプレハブ、及びアセットには、TextMesh Pro が必要です。つまり、TextMesh Pro のパッケージをインストールし、アセットがプロジェクト内にあることが必要です。（Window -> TextMeshPro -> Import TMP Essential Resources）
 **TMP Essentials Resources をインポートした後、変更を確認するには Unity を再起動する必要があります**。

### HandInteractionExamples のシーンを Editor で開いて実行する

[![HandInteractionExample scene](../Documentation/Images/MRTK_Examples.png)](README_HandInteractionExamples.md)

[Hand Interaction のサンプルシーン](README_HandInteractionExamples.md) は、MRTK の様々なUX コントロールとインタラクションを公開しているため、MRTK を始めるには最適な場所です。はじめに、MRTK をインポートし、サンプルシーンを開いて、Editor でシーンを探索します。

1. 新しい Unity プロジェクトを作成し、[上記の手順](#import-mrtk-packages-into-your-unity-project) に従って、**Foundation** と **Examples** の Unity パッケージの両方をインポートします。
1. `Assets\MixedRealityToolkit.Examples\Demos\HandTracking\Scenes\HandInteractionExamples` の下の HandInteractionExamples　のシーンを開きます。

1. 「TMP Essentials」をインポートするかを尋ねるプロンプトが表示されます。
  
    ![TMP Essentials](../Documentation/Images/getting_started/MRTK_GettingStarted_TMPro.png)
  
    このようなプロンプトが表示された場合は、「Import TMP essentials」 ボタンを選択します。「TMP Essentials」とは、Text Mesh プライグインを指し、MRTK のサンプルの一部はテキストレンダリングを改善するために使用しています。(詳細については、[Unity のテキスト](https://docs.microsoft.com/en-us/windows/mixed-reality/text-in-unity)を参照してください。)

1. TMP ダイアログを閉じます。この後、シーンをリロードする必要があります。これを行うには、プロジェクトタブでシーンをダブルクリックします。

1. Play ボタンを押します。

## In-Editor Hand Input Simulation を使ってシーンをテストする

Editor 内の入力シミュレーション を使って、[手](InputSimulation/InputSimulationService.md#hand-simulation)や[目](EyeTracking/EyeTracking_BasicSetup.md#simulating-eye-tracking-in-the-unity-editor)などの特定のタイプの入力に対しバーチャルなオブジェクトの動作をテストすることができます。

シーン内を移動する：

* W/A/S/D キーを使用して、カメラを前後/左右に移動します。
* 右マウスを押したままにして、カメラを回転させます。

手の入力をシミュレートする：

* スペースキーを押し続けて、右手を有効にします。
* スペースキーを押しながら、マウスを動かして手を動かします。
* 中央のマウススクロールを使用して、手の奥行を調整します。
* 左マウスをクリックして、ジェスチャーを切り替えます。

シーンの探索を楽しんでください！ UI コントロールの詳細については、[ハンドインタラクションのサンプルガイド](README_HandInteractionExamples.md) で学ぶことができます。また、[入力シミュレーションのドキュメント](InputSimulation/InputSimulationService.md)を読んで、MRTK のEditor ないの手の入力シミュレーションの詳細を確認してください。

おめでとうございます、最初の MRTK のシーンを使うことができました。これであなた自身のエクスペリエンスを創りはじめることができます。

### MRTK を新しいシーン、または新しいプロジェクトに追加する

1. 新規のプロジェクトを作成する、あるいは新しいシーンを現在のプロジェクトに作成します。

1. MRTK のパッケージが[上記の手順](#MRTK-のパッケージを-Unity-プロジェクトにインポートする) に従ってインポートされていることを確認します。(Examples は必須ではありませんが、 Foundation と Examles の両方をインポートすることを推奨します。 )

1. メニューバーから、Mixed Reality Toolkit -> Add to Scene and Configure　を選択します。

      ![Configure to scene](../Documentation/Images/MRTK_ConfigureScene.png)

1. 次のようなプロンプトが表示されます。

      ![MRTK Configure Dialog](../Documentation/Images/MRTK_ConfigureDialog.png)

    「OK」を押します。  

1. その後、MRTK Configuration profile を選択するよう求められます。「DefaultMixedRealityToolkitConfigurationProfile」をダブルクリックします。

    ![MRTK Select Configure Dialog](../Documentation/Images/MRTK_SelectConfigurationDialog.png)

    > **注意**： HoloLens 2 で始める場合は、「DefaultHoloLens2ConfigurationProfile」を選択することを推奨します。
    > DefaultMixedRealityToolkitConfigurationProfile と DefaultHoloLens2ConfigurationProfile の違いは、
    > [プロファイル](Profiles/Profiles.md#hololens-2-profile) を参照してください。

    シーンの階層が以下のようになります。

    ![MRTK Scene Setup](../Documentation/Images/MRTK_SceneSetup.png)

    階層には以下のものが含まれます。

    * Mixed Reality Toolkit - フレームワーク全体へ中心の設定のエントリポイントを提供します。
    * MixedRealityPlayspace - ヘッドセットの親オブジェクト。ヘッドセット / コントローラ及びその他の必要なシステムがシーンで正しく管理されるようにします。
    * Playspace の下に移動した Main Camera - プレイスペースがSDKと連動してカメラを管理できるようにします。

    > **注意**: シーンで作業している間、シーンの原点 (0,0,0) から **Main Camera を動かさないでください** (または playspace)。これは MRTK と アクティブな SDK によって制御されます。player を初期位置から動かしたい場合は、**カメラではなくシーンのコンテンツを移動してください**！

1. Play を押して再生し、スペースキーを押して、ハンドシミュレーションでテストします。

これで、デバイスにビルドしてデプロイする準備ができました！[MRTK のビルドとデプロイ](BuildAndDeploy.md) の手順に従ってください。

## 次のステップ

お勧めの次のステップを紹介します。

* [PressableButton](../Documentation/README_Button.md) をシーンに追加する。(最初は、 `PressableButtonPlated` プレハブを使うことを推奨します。)
* キューブをシーンに追加して、それを [ManipulationHandler](../DocumentationREADME_ManipulationHandler.md) コンポーネントを使って動かせるようにする。
* [building blocks for UI and interactions](#UIとインタラクションのビルディングブロック) で UX コントロール について学ぶ。
* [入力シミュレーションのガイド](../Documentaition/InputSimulation/InputSimulationService.md) を読んで、Editor 内で手の入力をシミュレートする方法を学ぶ。)
* [Mixed Reality 設定ガイド](../Documentation/MixedRealityConfigurationGuide.md) で MRTK Configuration profile の使い方を学ぶ。

## UIとインタラクションのビルディングブロック

|  [![Button](../Documentation/Images/Button/MRTK_Button_Main.png)](../Documentation/README_Button.md) [Button](../Documentation/README_Button.md) | [![Bounding Box](../Documentation/Images/BoundingBox/MRTK_BoundingBox_Main.png)](../Documentation/README_BoundingBox.md) [Bounding Box](../Documentation/README_BoundingBox.md) | [![Manipulation Handler](../Documentation/Images/ManipulationHandler/MRTK_Manipulation_Main.png)](../Documentation/README_ManipulationHandler.md) [Manipulation Handler](../Documentation/README_ManipulationHandler.md) |
|:--- | :--- | :--- |
| HoloLen 2 の関節式の手を含む様々な入力方法をサポートするボタンコントロール | 3D空間でオブジェクトを操作するための標準UI | 片手、または両手でオブジェクトを操作するためのスクリプト |
|  [![Slate](../Documentation/Images/Slate/MRTK_Slate_Main.png)](../Documentation/README_Slate.md) [Slate](../Documentation/README_Slate.md) | [![System Keyboard](../Documentation/Images/SystemKeyboard/MRTK_SystemKeyboard_Main.png)](../Documentation/README_SystemKeyboard.md) [System Keyboard](../Documentation/README_SystemKeyboard.md) | [![Interactable](../Documentation/Images/Interactable/InteractableExamples.png)](../Documentation/README_Interactable.md) [Interactable](../Documentation/README_Interactable.md) |
| 関節式の手の入力によるスクロールをサポートする2D スタイル平面 | Unity でシステムキーボードを使用するスクリプトのサンプル  | 視覚的にオブジェクトと対話可能にするためのスクリプトとテーマサポート |
|  [![Solver](../Documentation/Images/Solver/MRTK_Solver_Main.png)](../Documentation/README_Solver.md) [Solver](../Documentation/README_Solver.md) | [![Object Collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Main.png)](../Documentation/README_ObjectCollection.md) [Object Collection](../Documentation/README_ObjectCollection.md) | [![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Main.png)](../Documentation/README_Tooltip.md) [Tooltip](../Documentation/README_Tooltip.md) |
| tag-along、body-lock、constant view size、 surface magnetism のような様々なオブジェクト配置動作 |  オブジェクトの配列を3次元にレイアウトするためのスクリプト |モーションコントローラーとオブジェクトのラベル付けに使用できる柔軟なアンカー/ピボットシステムを備えた注釈UI。 |
|  [![App Bar](../Documentation/Images/AppBar/MRTK_AppBar_Main.png)](../Documentation/README_AppBar.md) [App Bar](../Documentation/README_AppBar.md) | [![Pointers](../Documentation/Images/Pointers/MRTK_Pointer_Main.png)](../Documentation/Input/Pointers.md) [Pointers](../Documentation/Input/Pointers.md) | [![Fingertip Visualization](../Documentation/Images/Fingertip/MRTK_FingertipVisualization_Main.png)](../Documentation/README_FingertipVisualization.md) [Fingertip Visualization](../Documentation/README_FingertipVisualization.md) |
| Bounding Box の手動で有効化するUI | 様々なタイプのポインターについて学ぶ | 直接的なインタラクションの信頼性を向上させる、指先の視覚的アフォーダンス |
|  [![Slider](../Documentation/Images/Slider/MRTK_UX_Slider_Main.jpg)](../Documentation/README_Sliders.md) [Slider](../Documentation/README_Sliders.md) | [![MRTK Standard Shader](../Documentation/Images/MRTKStandardShader/MRTK_StandardShader.jpg)](../Documentation/README_MRTKStandardShader.md) [MRTK Standard Shader](../Documentation/README_MRTKStandardShader.md) | [![Hand Joint Chaser](../Documentation/Images/HandJointChaser/MRTK_HandJointChaser_Main.jpg)](../Documentation/README_HandJointChaser.md) [Hand Joint Chaser](../Documentation/README_HandJointChaser.md) |
| 直接的な手を使ったインタラクションをサポートする値を調整するためのスライダー | フルーエントデザインの要素をサポートし、パフォーマンスの良い MRTK の標準シェーダー | ソルバーを使用してオブジェクトを手の関節にアタッチする方法のデモ |
|  [![Eye Tracking: Target Selection](../Documentation/Images/EyeTracking/mrtk_et_targetselect.png)](../Documentation/EyeTracking/EyeTracking_TargetSelection.md) [Eye Tracking: Target Selection](../Documentation/EyeTracking/EyeTracking_TargetSelection.md) | [![Eye Tracking: Navigation](../Documentation/Images/EyeTracking/mrtk_et_navigation.png)](../Documentation/EyeTracking/EyeTracking_Navigation.md) [Eye Tracking: Navigation](../Documentation/EyeTracking/EyeTracking_Navigation.md) | [![Eye Tracking: Heat Map](../Documentation/Images/EyeTracking/mrtk_et_heatmaps.png)](EyeTracking/EyeTracking_ExamplesOverview.md#visualization-of-visual-attention) [Eye Tracking: Heat Map](../Documentation/EyeTracking/EyeTracking_ExamplesOverview.md#visualization-of-visual-attention) |
| 目、音声、手の入力を組み合わせて、シーン内のhologramsを簡単に選択する | 見ている場所に基づいてテキストを自動スクロールする方法やフォーカスされたコンテンツをズームする方法を学ぶ | アプリでユーザーが見ているものを記録、読み込み、視覚化するサンプル |

## ツール

|  [![Optimize Window](../Documentation/Images/MRTK_Icon_OptimizeWindow.png)](../Documentation/Tools/OptimizeWindow.md) [Optimize Window](../Documentation/Tools/OptimizeWindow.md) | [![Dependency Window](../Documentation/Images/MRTK_Icon_DependencyWindow.png)](../Documentation/Tools/DependencyWindow.md) [Dependency Window](../Documentation/Tools/DependencyWindow.md) | ![Build Window](../Documentation/Images/MRTK_Icon_BuildWindow.png) Build Window | [![Input recording](../Documentation/Images/MRTK_Icon_InputRecording.png)](../Documentation/InputSimulation/InputAnimationRecording.md) [Input recording](../Documentation/InputSimulation/InputAnimationRecording.md) |
| :--- | :--- | :--- | :--- |
| パフォーマンスの最適化のための Mixed Reality プロジェクトの構成の自動化 | アセット間の依存関係を分析し、未使用のアセットを特定する | Mixed Reality アプリケーションのエンドツーエンドのビルドプロセスを構成および実行する | エディターでのヘッドの動きとハンドトラッキングデータの記録と再生 |

## HoloToolkit (HTK/MRTK v1) からアップグレードする

フレームワークが再構築されたため、Holotoolkit から Mixed Reality Toolkit v2 への直接的なアップグレードパスはありません。ただし、MRTK を HoloToolkit プロジェクトにインポートし、実装を移行することは可能です。詳細については、 [HoloToolkit to Mixed Reality Toolkit Porting Guide](../Documentation/HTKToMRTKPortingGuide.md) を参照してください。
