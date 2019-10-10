# 入力シミュレーション サービス (Input Simulation Service)

入力シミュレーション サービスは、Unity Editor 上で使用できない場合があるデバイスやプラットフォームをエミュレートします。例：

* HoloLens または VR デバイスのヘッド トラッキング
* HoloLens のハンド ジェスチャ
* HoloLens 2 の多関節ハンド トラッキング
* HoloLens 2 のアイ トラッキング

ユーザーは、従来のキーボードやマウスの組み合わせで実行時に操作をシミュレーションすることができます。それにより、デバイスにデプロイする前に Unity Editor 上でインタラクションをテストすることができます。

> [!WARNING]
> これは、Unity の XR Holographic Emulation > Emulation Mode = "Simulate in Editor" では動作しません。Unity Editor 内でのシミュレーションは、MRTK の入力シミュレーションの制御を奪い取ります。MRTK の入力シミュレーション サービスを使用するには、XR Holographic Emulation を、Emulation Mode = *"None"* にセットする必要があります。

## 入力シミュレーション サービスの有効化

MRTK では、入力シミュレーションは既定で有効化されています。

入力シミュレーション サービスは、[Mixed Reality service](../MixedRealityServices.md) のオプションです。データ プロバイダーとして、[Input System profile](../Input/InputProviders.md) に追加することができます。

* __Type__ は、_Microsoft.MixedReality.Toolkit.Input > InputSimulationService_ である必要があります
* __Platform(s)__ は、デフォルトで全ての _Editor_ プラットフォームを含みます。これは、サービスがキーボードとマウスの入力を思料するためです。

> [!WARNING]
> 現時点では、どのタイプのプロファイルもアサインすることができます。別のプロファイルをサービスにアサインする場合は、必ず _Input Simulation_ タイプのプロファイルを使用してください。そうしないと機能しません。

## 入力シミュレーションツール ウィンドウ

_Mixed Reality Toolkit > Utilities > Input Simulation_ メニューから、入力シミュレーションツール ウィンドウを有効化します。このウィンドウは、プレイモードの間、入力シミュレーションサービスの状態を提供します。

## ビューポートボタン

基本的な手の配置をコントロールするエディタ内ボタンのプレファブは、入力シミュレーションサービスの __Indicators Prefab__ 配下のプロファイルで指定することができます。これは、オプションのユーティリティで、同じ機能には [input simulation tools window](#input-simulation-tools-window) からアクセスすることができます。

Hand icons show the state of the simulated hands:

* ![Untracked hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Untracked.png "Untracked hand icon") 手は追跡されていません。クリックで手を有効化します。
* ![Tracked hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Tracked.png "Tracked hand icon") 手は追跡されています。しかし、ユーザーにコントロールされていません。クリックで手を非表示にします。
* ![Controlled hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Controlled.png "Controlled hand icon") 手は追跡されており、ユーザーにコントロールされています。クリックで手を非表示にします。
* ![Reset hand icon](../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandIndicator_Reset.png "Reset hand icon") クリックして手を既定の位置にリセットします。

# カメラ コントロール

頭の動きは入力シミュレーションサービスでエミュレートされます。

## カメラを回転する

1. Editor ウインドウにカーソルを合わせます。_ボタンを押しても動作しない場合は、フォーカスを得るためにウィンドウをクリックする必要があります。_
1. __Mouse Look Button__ (既定: 右マウスボタン)を押したままにします
1. マウスをウインドウ内で動かしてカメラを回転させます
1. スクロールホイールを使ってカメラを視線方向を軸として回転させます

## カメラを移動する

__Move Horizontal__/__Move Vertical__ 軸を使用してカメラを移動させます（規定：WASDキーまたはゲームコントローラーの左スティック）

カメラの位置と回転角度は、ツールウインドウで明示的にセットすることもできます。__Reset__　ボタンでカメラを既定の状態にリセットすることができます。

<iframe width="560" height="315" src="https://www.youtube.com/embed/Z7L4I1ET7GU" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

# ハンド シミュレーション

入力シミュレーション サービスは、ハンド デバイスをサポートします。バーチャル ハンドは、ボタンやグラブ可能オブジェクトなどの、通常のハンド デバイスをサポートしたオブジェクトとインタラクションできます。

## ハンドシミュレーションモード

[input simulation tools window](#input-simulation-tools-window) 内で、 __Hand Simulation Mode__ は、２つの異なる入力モデルの切り替えを行います。
既定のモードは入力シミュレーションプロファイルでもセットできます。

* _Articulated Hands_: 関節の位置のデータを持った多関節ハンドをシミュレートします

   HoloLens 2 のインタラクション モデルをエミュレートします

   このモードでは、手の正確な位置やタッチに基づいたインタラクションをシミュレートできます

* _Gestures_: Air Tap や基本的なジェスチャを持ったシンプルな手をシミュレートします

  [HoloLens interaction model](https://docs.microsoft.com/en-us/windows/mixed-reality/gestures) をエミュレートします。

   フォーカスは視線ポインターを使ってコントロールします。_Air Tap_ ジェスチャでボタンと対話します

## 手の動きのコントロール

__Left/Right Hand Manipulation Key__ （既定：左Shift/Space がそれぞれ 左手/右手 に対応）を押し続けてそれぞれの手をコントロールします。キーを押し続けている間、手が表示されます。マウスの動きで手を移動できます。Manipuration キーが押されなくなると、手は短い __Hand Hide Timeout__ の後に表示されなくなります。

[input simulation tools window](#input-simulation-tools-window) で手の表示を永続的に切り替えるには、 __Toggle Left/Right Hand Key__ （既定：T/Y が 左手/右手 に対応）を押してください。Toggle キーを再度押すと、再度手の表示を消すことができます。

マウスの動きは、ビュー内で手の動きとなります。手は、__mouse wheel__ を使って遠ざけたり近づけたりすることができます。

To rotate hands using the mouse, hold both the __Left/Right Hand Control Key__ (shift/space) _and_ the __Hand Rotate Button__ (default: right mouse button). Hand rotation speed can be configured by changing the __Mouse Hand Rotation Speed__ setting in the input simulation profile.

マウスを使用して手を回転させるには、__Left / Right Hand Control Key__（シフト/スペース）_と_ __Hand Rotate Button__（既定：マウスの右ボタン）の両方を押します。入力シミュレーションプロファイルの __Mouse Hand Rotation Speed__ 設定を変更することにより、手の回転速度を設定できます。

全ての手の配置（手を既定の状態にリセットすることも含む）は[input simulation tools window](#input-simulation-tools-window)でも変更することができます。

## 追加のプロファイル設定

* __Hand Depth Multiplier__ は、マウススクロールホイールの深さの動きの感度を制御します。数値を大きくすると、手のズームが速くなります。
* __Default Hand Distance__ は、カメラからの手の初期距離です。 __Reset__ ボタンの手をクリックすると、手もこの距離に配置されます。
* __Hand Jitter Amount__ は、ランダムな動きを手に追加します。これを使用して、デバイス上の不正確な手の追跡をシミュレートし、ノイズの多い入力でインタラクションが適切に機能することを確認できます。

<iframe width="560" height="315" src="https://www.youtube.com/embed/uRYfwuqsjBQ" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## ハンド ジェスチャ

ピンチ、グラブ、指差しなどのハンド ジェスチャをシミュレートできます

1. 最初に、Manipuration キー（左Shift/Space）を使って手を有効にしてください

   別の方法では、Toggle キー（T/Y）を使って手の on/off を切り替えることもできます

2. 操作の間、マウスのボタンを押し続けることによってハンド ジェスチャを実行することができます

_Left/Middle/Right Mouse Hand Gesture_ 設定を使用して、それぞれのマウスのボタンを、手の形が異なるジェスチャにマップすることができます。 _Default Hand Gesture_ は、どのボタンも押されていないときの手の形です。

> [!NOTE]
> _Pinch_ ジェスチャは、この時点では "Select" アクションを発生させる唯一のジェスチャです

## 片手での操作

1. 手をコントロールするキー（Space/左Shift）を押し続けてください
2. オブジェクトをポイントしてください
3. マウスのボタンを押し続けるとピンチ操作となります
4. マウスでオブジェクトを動かしてください
5. マウスのボタンを離すと操作が終了します

<iframe width="560" height="315" src="https://www.youtube.com/embed/rM0xaHam6wM" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## 両手での操作

オブジェクトを両手で操作するには、手を永続化するモードが推奨されます

1. Toggle キー（T/Y）を押すことで両手の表示を切り替えることができます
1. １回に１つの手を操作します
1. _Space_ を押し続けて右手を操作します
1. グラブしたいオブジェクトに手を動かします
1. マウスのボタンを押して _Pinch_ ジェスチャを有効にします。永続化モードではマウスのボタンを離すまでジェスチャが有効になります
1. 同じ手順をもう片方の手で繰り返します。同じオブジェクトの別のポイントをグラブします
1. これで両手で同じオブジェクトをグラブした状態になるので、どちらかの手を動かして両手での操作を行います

<iframe width="560" height="315" src="https://www.youtube.com/embed/Qol5OFNfN14" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## GGV インタラクション

1. [Input Simulation Profile](#enabling-the-input-simulation-service) で、__Hand Simulation Mode__ を _Gestures_ にすることにより、GGV のシミュレーションに切り替えることができます
1. カメラを回転させて、視線カーソルをインタラクション可能なオブジェクトにポイントします（マウスの右クリックを使用）
1. _Space_ を押し続けて、右手を操作します
1. _left mouse button_ を押し続けてインタラクションします
1. 再度カメラを回転させてオブジェクトを操作します

<iframe width="560" height="315" src="https://www.youtube.com/embed/6841rRMdqWw" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## アイ トラッキング

[Eye tracking simulation](../EyeTracking/EyeTracking_BasicSetup.md#simulating-eye-tracking-in-the-unity-editor) は、[Input Simulation Profile](#enabling-the-input-simulation-service) の __Simulate Eye Position__ オプションをチェックすることで有効になります。これは GGV スタイルのインタラクションでは使用すべきではありません（ですので、 __Hand Simulation Mode__ が _Articulated_ にセットされていることを確認してください）
