# 入力シミュレーション サービス (Input Simulation Service)

入力シミュレーション サービスは、Unity Editor 上で使用できない場合があるデバイスやプラットフォームをエミュレートします。例:

* HoloLens または VR デバイスのヘッド トラッキング
* HoloLens のハンド ジェスチャ
* HoloLens 2 の多関節ハンド トラッキング
* HoloLens 2 のアイ トラッキング

ユーザーは、従来のキーボードやマウスの組み合わせで実行時に操作をシミュレーションすることができます。それにより、デバイスにデプロイする前に Unity Editor 上でインタラクションをテストすることができます。

> [!WARNING]
> これは、Unity の XR Holographic Emulation > Emulation Mode = "Simulate in Editor" では動作しません。Unity Editor 内でのシミュレーションは、MRTK の入力シミュレーションの制御を奪い取ります。MRTK の入力シミュレーション サービスを使用するには、XR Holographic Emulation を、Emulation Mode = *"None"* にセットする必要があります。

## 入力シミュレーション サービスの有効化

MRTK では、入力シミュレーションはデフォルトで有効化されています。

入力シミュレーション サービスは、[Mixed Reality service](../MixedRealityServices.md) のオプションです。データ プロバイダーとして、[Input System profile](../Input/InputProviders.md) に追加することができます。

* __Type__ は、_Microsoft.MixedReality.Toolkit.Input > InputSimulationService_ である必要があります
* __Platform(s)__ は、デフォルトで全ての _Editor_ プラットフォームを含みます。これは、サービスがキーボードとマウスの入力を使用するためです。

## 入力シミュレーション ツール ウィンドウ

_Mixed Reality Toolkit > Utilities > Input Simulation_ メニューから、入力シミュレーション ツール ウィンドウを有効化します。このウィンドウは、プレイモードの間、入力シミュレーションの状態へのアクセスを提供します。

## ビューポートボタン

基本的なハンドの配置をコントロールするエディタ内ボタンのプレファブは、入力シミュレーションサービスの __Indicators Prefab__ 配下の入力シミュレーション プロファイルで指定することができます。これは、オプションのユーティリティで、同じ機能には [入力シミュレーション ツール ウィンドウ](#入力シミュレーション-ツール-ウィンドウ) からアクセスすることができます。

ハンドアイコンは、シミュレーションされたハンドの状態を表します。

* ![Untracked hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Untracked.png "Untracked hand icon") ハンドは追跡されていません。クリックでハンドを有効化します。
* ![Tracked hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Tracked.png "Tracked hand icon") ハンドは追跡されています。しかし、ユーザーにコントロールされていません。クリックでハンドを非表示にします。
* ![Controlled hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Controlled.png "Controlled hand icon") ハンドは追跡されており、ユーザーにコントロールされています。クリックでハンドを非表示にします。
* ![Reset hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Reset.png "Reset hand icon") クリックしてハンドをデフォルトの位置にリセットします。

## カメラ コントロール

頭の動きは入力シミュレーション サービスでエミュレートされます。

### カメラの回転

1. Editor ウィンドウにカーソルを合わせます。_ボタンを押しても動作しない場合は、フォーカスを得るためにウィンドウをクリックする必要があります。_
1. __Mouse Look Button__ (デフォルト: 右マウスボタン)を押したままにします
1. マウスをウィンドウ内で動かしてカメラを回転させます
1. スクロールホイールを使ってカメラを視線方向を軸として回転させます

カメラの回転速度は、入力シミュレーション プロファイルの __Mouse Look Speed__ 設定を変更して構成できます。

または、カメラを回転させるために __Look Horizo​​ntal__/__Look Vertical__ 軸を使用します（デフォルト:ゲームコントローラの右スティック）

### カメラの移動

__Move Horizontal__/__Move Vertical__ 軸を使用してカメラを移動させます（規定:WASDキーまたはゲームコントローラーの左スティック）

カメラの位置と回転角度は、ツール ウィンドウで明示的にセットすることもできます。__Reset__　ボタンでカメラをデフォルトの状態にリセットすることができます。

<iframe width="560" height="315" src="https://www.youtube.com/embed/Z7L4I1ET7GU" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## ハンド シミュレーション

入力シミュレーション サービスは、ハンド デバイスをサポートします。バーチャル ハンドは、ボタンやグラブ可能オブジェクトなどの、通常のハンド デバイスをサポートしたオブジェクトとインタラクションできます。

### ハンド シミュレーション モード

[入力シミュレーション ツール ウィンドウ](#入力シミュレーション-ツール-ウィンドウ) 内で、 __Hand Simulation Mode__ は、２つの異なる入力モデルの切り替えを行います。
デフォルトのモードは入力シミュレーション プロファイルでもセットできます。

* _Articulated Hands_: 関節の位置のデータを持った多関節ハンドをシミュレートします

   HoloLens 2 のインタラクション モデルをエミュレートします

   このモードでは、ハンドの正確な位置やタッチに基づいたインタラクションをシミュレートできます

* _Gestures_: Air Tap や基本的なジェスチャを持ったシンプルなハンドをシミュレートします

  [HoloLens interaction model](https://docs.microsoft.com/en-us/windows/mixed-reality/gestures) をエミュレートします。

   フォーカスは視線ポインターを使ってコントロールします。_Air Tap_ ジェスチャでボタンと対話します

### ハンドの動きのコントロール

__Left/Right Hand Control Key__ （デフォルト: *左 Shift* が左ハンド、*Space* が右ハンド）を押し続けてそれぞれのハンドをコントロールします。操作キーを押し続けている間、ハンドが表示されます。操作キーが押されなくなると、短い __Hand Hide Timeout__ の後にハンドは表示されなくなります。


ハンドは[入力シミュレーション ツール ウィンドウ](#入力シミュレーション-ツール-ウィンドウ)内、または __Toggle Left/Right Hand Key__ （デフォルト: *T* が左ハンド、*Y* が右ハンド）を押すことで、永続的に表示を切り替えられます。Toggle キーを再度押すと、再度ハンドの表示を消すことができます。

マウスの動きは、ビュー内でハンドの動きとなります。ハンドは、__mouse wheel__ を使ってカメラから遠ざけたり近づけたりすることができます。

マウスを使用してハンドを回転させるには、__Left / Right Hand Control Key__（*左 Shift* か *Space*）_と_ __Hand Rotate Button__（デフォルト: *ctrl* ボタン）の両方を押し、マウスを動かしてハンドを回転させます。入力シミュレーション プロファイルの __Mouse Hand Rotation Speed__ 設定を変更することにより、ハンドの回転速度を設定できます。

全てのハンドの配置（ハンドをデフォルトの状態にリセットすることも含む）は[入力シミュレーション ツール ウィンドウ](#入力シミュレーション-ツール-ウィンドウ)でも変更することができます。

### 追加のプロファイル設定

* __Hand Depth Multiplier__ は、マウス スクロール ホイールの深さの動きの感度を制御します。数値を大きくすると、ハンドのズームが速くなります。
* __Default Hand Distance__ は、カメラからのハンドの初期距離です。 __Reset__ ボタンのハンドをクリックすると、ハンドもこの距離に配置されます。
* __Hand Jitter Amount__ は、ランダムな動きをハンドに追加します。これを使用して、デバイス上の不正確なハンドの追跡をシミュレートし、ノイズの多い入力でインタラクションが適切に機能することを確認できます。

<iframe width="560" height="315" src="https://www.youtube.com/embed/uRYfwuqsjBQ" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### ハンド ジェスチャ

ピンチ、グラブ、指差しなどのハンド ジェスチャをシミュレートできます。

1. 最初に、 __Left/Right Hand Control Key__ (*左 Shift* か *Space*) を使ってハンドを有効にしてください。

   別の方法としては、Toggle キー (*T* か *Y*) を使ってハンドの on/off を切り替えることもできます。

2. 操作の間、マウスのボタンを押し続けることによってハンド ジェスチャを実行することができます。

_Left/Middle/Right Mouse Hand Gesture_ 設定を使用して、それぞれのマウスのボタンを、ハンドの形が異なるジェスチャにマップすることができます。 _Default Hand Gesture_ は、どのボタンも押されていないときのハンドの形です。

> [!NOTE]
> _Pinch_ ジェスチャは、この時点では "Select" アクションを発生させる唯一のジェスチャです

### 片手での操作

1. __Left/Right Hand Control Key__ (*左 Shift* か *Space*) を押し続けてください
2. オブジェクトをポイントしてください
3. マウスのボタンを押し続けるとピンチ操作となります
4. マウスでオブジェクトを動かしてください
5. マウスのボタンを離すと操作が終了します

<iframe width="560" height="315" src="https://www.youtube.com/embed/rM0xaHam6wM" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### 両手での操作

オブジェクトを両手で操作するには、ハンドを永続化するモードが推奨されます

1. Toggle キー（T/Y）を押すことで両手の表示を切り替えることができます
1. １回に１つのハンドを操作します
    1. _Space_ を押し続けて右ハンドを操作します
    1. グラブしたいオブジェクトにハンドを動かします
    1. マウスのボタンを押して _Pinch_ ジェスチャを有効にします。永続化モードではマウスのボタンを離すまでジェスチャが有効になります
1. 同じ手順をもう片方のハンドで繰り返します。同じオブジェクトの別のポイントをグラブします
1. これで両手で同じオブジェクトをグラブした状態になるので、どちらかのハンドを動かして両手での操作を行います

<iframe width="560" height="315" src="https://www.youtube.com/embed/Qol5OFNfN14" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### GGV インタラクション

1. [Input Simulation Profile](#入力シミュレーション-サービスの有効化) で、__Hand Simulation Mode__ を _Gestures_ にすることにより、GGV のシミュレーションに切り替えることができます
1. カメラを回転させて、視線カーソルをインタラクション可能なオブジェクトにポイントします（マウスの右クリックを使用）
1. _Space_ を押し続けて、右ハンドを操作します
1. _left mouse button_ を押し続けてインタラクションします
1. 再度カメラを回転させてオブジェクトを操作します

<iframe width="560" height="315" src="https://www.youtube.com/embed/6841rRMdqWw" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

### アイ トラッキング

[Eye tracking simulation](../EyeTracking/EyeTracking_BasicSetup.md#simulating-eye-tracking-in-the-unity-editor) は、[Input Simulation Profile](#enabling-the-input-simulation-service) の __Simulate Eye Position__ オプションをチェックすることで有効になります。これは GGV スタイルのインタラクションでは使用すべきではありません（ですので、 __Hand Simulation Mode__ が _Articulated_ にセットされていることを確認してください）
