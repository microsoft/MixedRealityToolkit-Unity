# MRTK におけるアイ トラッキング サンプル
このページでは、私たちが提供している [MRTK eye tracking examples](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking) を基に、MRTK で Eye Tracking (アイ トラッキング) を使用する方法を簡単に説明します。
サンプルでは、新しいマジカルな入力機能の1つである **アイ トラッキング** を体験できます！
このデモには、視線による暗黙的なアクティブ化から、見ているものに関する情報を **音声** および **ハンド** 入力とシームレスに組み合わせる方法まで、さまざまな使用例が含まれています。 
これにより、ターゲットを見て _「選択」_ と発話するか、ハンド ジェスチャを実行するだけで、ビュー内のホログラフィック コンテンツをすばやく簡単に選択して移動することができます。 
デモには、Slate (スレート) に表示されたテキストや画像に対して、視線によるスクロール、パン、ズームを行う例も含まれています。 
最後に、2D スレート上でユーザーの注視点を記録し可視化する例が提供されています。
次に、[MRTK eye tracking example package](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking) に含まれている各サンプルの詳細について説明します。

![List of eye tracking scenes](../../Documentation/Images/EyeTracking/mrtk_et_list_et_scenes.jpg)

まず、個々のアイ トラッキングのデモ シーンがどのようなものか、概要を簡単に説明します。
MRTK アイ トラッキングのデモ シーンは、[追加でロードされる](https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.Additive.html)ので、そのセット アップ方法をその次に説明します。

## アイ トラッキング デモ サンプルの概要

### [**目で支援するターゲット選択**](EyeTracking_TargetSelection.md)

このチュートリアルでは、視線データに簡単にアクセスしてターゲットを選択できることを示します。 
これには、ターゲットがフォーカスされていることをユーザーに確信させるが、圧倒しないための、繊細かつ強力なフィードバックの例が含まれています。
さらに、読んだ後に自動的に消えるスマートな通知の簡単な例もあります。

**概要**: 目、音声、ハンドの入力を組み合わせた、素早く簡単なターゲット選択。

<br>


### [**目で支援するナビゲーション**](EyeTracking_Navigation.md)

離れた場所にあるディスプレイや電子書籍リーダーで情報を読んでいるときに、表示されているテキストの末尾に達すると、テキストが自動的にスクロール アップしてより多くのコンテンツが表示されることを想像してください。 
あるいは、見ていた方向に直接、魔法のようにズームできたらどうでしょうか？
このチュートリアルでは、目で支援するナビゲーションの例をいくつか紹介します。
さらに、現在のフォーカスに基づいて 3D ホログラムを自動的に回転させ、ハンズフリーで回転させる例もあります。 

**概要**: 目、音声、ハンドの入力の組み合わせた、スクロール、パン、ズーム、3D 回転。

<br>


### [**目で支援する配置**](EyeTracking_Positioning.md)

このチュートリアルでは、[Put-That-There](https://youtu.be/CbIn8p4_4CQ) と呼ばれる入力シナリオを示します。これは、目、ハンド、および音声による入力を使用した、1980年代初頭に MIT メディア ラボで行われた研究に遡るものです。
そのアイデアは単純で、目線を使ってターゲットの選択と配置を素早く行うことができます。 
単純に、ホログラムを見て _'put this'_、置きたい場所を見て _'there!'_ と言ってください。 
ホログラムをより正確に配置するために、ハンド、音声、またはコントローラからの追加入力を使用できます。 

**概要**: 目、音声、ハンド入力 (*ドラッグ＆ドロップ*) を使ったホログラムの配置。目 + ハンドを使用する目で支援するスライダー。 

<br>


### **注視点の可視化**

ユーザーがどこを見ているかに関する情報は、設計のユーザビリティを評価し、効率的な作業ストリームにおける問題を見極めるための非常に強力なツールです。 
このチュートリアルでは、さまざまなアイ トラッキングの可視化と、それらがさまざまなニーズにどのように適合するかについて説明します。 
アイ トラッキング データの記録とロードのための基本的な例とそれらを可視化する方法の例を提供しています。 

**概要**: スレート上の二次元注意マップ(ヒートマップ)。アイ トラッキング データの記録と再生。

<br>


## MRTK アイ トラッキング サンプルのセット アップ

### 前提条件

デバイス上でアイ トラッキング サンプルを使用するには、HoloLens 2 と、パッケージの AppXManifest で "Gaze Input (視線入力)" の機能を有効にしてビルドしたサンプル アプリ パッケージが必要なことに注意してください。

これらのアイ トラッキング サンプルをデバイスで使用するには、Visual Studio でアプリをビルドする前に、 [これらのステップ](EyeTracking_BasicSetup.md#testing-your-unity-app-on-a-hololens-2) に従ってください。

### 1. EyeTrackingDemo-00-RootScene.unity をロード
*EyeTrackingDemo-00-RootScene* は、すべてのコア MRTK コンポーネントを含むベース (_ルート_) シーンです。
これは最初にロードする必要があるシーンで、ここからアイ トラッキングのデモを実行します。 
グラフィカルなシーン メニューを使用すると、さまざまなアイ トラッキング サンプルを[追加でロード](https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.Additive.html)して簡単に切り替えることができます。

![Scene menu in eye tracking sample](../../Documentation/Images/EyeTracking/mrtk_et_scenemenu.jpg)

ルート シーンには、MRTK の設定プロファイルやシーン カメラなど、追加でロードするシーン全体で存続するいくつかのコア コンポーネントが含まれています。 
_MixedRealityBasicSceneSetup_ (以下のスクリーンショット参照) には、起動時に参照されたシーンを自動的にロードするスクリプトが含まれています。 
デフォルトでは、_EyeTrackingDemo-02-TargetSelection_ です。

![Example for the OnLoadStartScene script](../../Documentation/Images/EyeTracking/mrtk_et_onloadstartscene.jpg)


### 2. ビルド メニューへのシーン追加
追加のシーンを実行時にロードするには、最初にこれらのシーンを  _Build Settings -> Scenes in Build_ メニューに追加する必要があります。
ルート シーンがリストの最初のシーンとして表示されていることが重要です。

![Build Settings scene menu for eye tracking samples](../../Documentation/Images/EyeTracking/mrtk_et_build_settings.jpg)


### 3. Unity Editor でアイ トラッキング サンプルを再生
[Build Settings] にアイ トラッキング シーンを追加し、 _EyeTrackingDemo-00-RootScene_ をロードした後、最後に確認することがあります。_MixedRealityBasicSceneSetup_ ゲームオブジェクトにアタッチされている _'OnLoadStartScene'_ スクリプトは有効でしょうか？ このスクリプトは、最初にロードするデモ シーンをルート シーンに知らせます。

![Example for the OnLoad_StartScene script](../../Documentation/Images/EyeTracking/mrtk_et_onloadstartscene.jpg)

さあ、 _"Play"_ ボタンをクリックしましょう！
複数の宝石が表示され、上部にシーン メニューが表示されます。

![Sample screenshot from the ET target select scene](../../Documentation/Images/EyeTracking/mrtk_et_targetselect.png)

ゲーム ビューの中央に小さな半透明の円が表示されます。 
これは、_シミュレートされた視線_ のインジケーター (カーソル) として動作します。
_マウスの右ボタン_ を押し、マウスを移動してその位置を変更するだけです。 
カーソルを宝石の上に移動すると、現在見ている宝石の中心にスナップすることに気づくでしょう。 
これは、ターゲットを _"見ている"_ ときに、期待した通りにイベントがトリガーされるかどうかをテストするのにとても良い方法です。 
注意してほしいのは、マウス操作による _シミュレートされた視線_ は、我々の迅速で意図しない目の動きを補うにはかなり不十分であるということです。 
HoloLens 2 デバイスにデプロイしてデザインを繰り返す前に、基本的な機能をテストするのは素晴らしいことです。
アイ トラッキングのサンプル シーンに話を戻します。宝石は見ている限り回転し、「見ながら」以下を行うことで破壊されます。
- _Enter_ キーを押す ("select" という発話をシミュレートしています)
- マイクに向かって _"select"_ と言う
- シミュレートされたハンド入力を表示するため _Spece_ キーを押している間に、シミュレートのピンチを実行するために左マウス ボタンをクリックする

これらの操作を実現する方法については、[**目で支援するターゲット選択**](EyeTracking_TargetSelection.md) チュートリアルで詳しく説明します。

カーソルをシーンのトップ メニュー バーに移動すると、現在ホバーしている項目が微妙にハイライト表示されます。 
上記のいずれかのコミット方法 (例: _Enter_ キーを押す) を使用すると、現在ハイライト表示されている項目を選択できます。
このようにして、異なるアイ トラッキング サンプル シーンを切り替えることができます。

### 4. 特定のサブ シーンをテストする方法
特定のシナリオで作業しているときに、毎回シーン メニューを経由したくない場合があります。
代わりに、_Play_ ボタンを押したときに、現在作業しているシーンから直接開始できるようにしたい場合があります。 
問題ありません！ 次のようにしてください。
1. _root_ シーンをロードします。
2. _root_ シーンで、 _'OnLoadStartScene'_ スクリプトを無効にします。 
3. 以下に説明するアイ トラッキング テスト シーンのいずれか（またはその他のシーン） を下のスクリーンショットに示すように _Hierarchy_ ビューに _ドラッグ & ドロップ_ します。

![Example for additive scene](../../Documentation/Images/EyeTracking/mrtk_et_additivescene.jpg)

4.  _Play_ を押します。

このようなサブ シーンのロードは永続的ではないことに注意してください。
つまり、アプリを HoloLens 2 デバイスにデプロイすると、ルート シーンのみがロードされます (Build Settings の一番上にルート シーンが表示されているものとします)。 
また、プロジェクトを他のユーザと共有する場合、サブ シーンは自動的にはロードされません。 

<br>

MRTK によるアイ トラッキングのサンプル シーンを機能させる方法を理解したところで、次は目でホログラムを選択する方法について詳しく見ていきましょう: [Eye-supported target selection](EyeTracking_TargetSelection.md)。

---
["MixedRealityToolkit のアイ トラッキング" に戻る](EyeTracking_Main.md)
