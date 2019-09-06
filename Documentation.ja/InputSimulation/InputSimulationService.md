# 入力シミュレーションサービス (Input Simulation Service)

入力シミュレーションサービスは、Unity Editor 上で使用できない場合があるデバイスやプラットフォームをエミュレートします。例：

* HoloLens または VR デバイスのヘッドトラッキング
* HoloLens のハンドジェスチャー
* HoloLens 2 の多関節ハンドトラッキング
* HoloLens 2 のアイトラッキング

ユーザーは、従来のキーボードやマウスの組み合わせで実行時に操作をシミュレーションすることができます。それにより、デバイスにデプロイする前に Unity Editor 上でインタラクションをテストすることができます。

> [!WARNING]
> これは、Unity の XR Holographic Emulation > Emulation Mode = "Simulate in Editor" では動作しません。Unity Editor 内でのシミュレーションは、MRTK の入力シミュレーションの制御を奪い取ります。MRTK の入力シミュレーションサービスを使用するには、XR Holographic Emulation を、Emulation Mode = *"None"* にセットする必要があります。

## 入力シミュレーションサービスの有効化

MRTK では、入力シミュレーションはデフォルトで有効化されています。

入力シミュレーションサービスは、[Mixed Reality service](../MixedRealityServices.md) のオプションです。データプロバイダーとして、[Input System profile](../Input/InputProviders.md) に追加することができます。

* __Type__ は、_Microsoft.MixedReality.Toolkit.Input > InputSimulationService_ である必要があります
* __Platform(s)__ は、サービスがキーボードとマウスの入力に依存しているため、常に _Windows Editor_ である必要があります
* __Profile__ は、入力シミュレーションの全ての設定を保持しています。

> [!WARNING]
> 現時点では、どのタイプのプロファイルもアサインすることができます。別のプロファイルをサービスにアサインする場合は、必ず _Input Simulation_ タイプのプロファイルを使用してください。そうしないと機能しません。

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_InputSystemDataProviders.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_InputSystemDataProviders.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

リンクされたプロファイルを開くことで入力シミュレーションの設定にアクセスすることができます。

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_InputSimulationProfile.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_InputSimulationProfile.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

# カメラコントロール

頭の動きは入力シミュレーションサービスでエミュレートされます。

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_CameraControlSettings.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_CameraControlSettings.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

## カメラの回転

1. Editor ウインドウにカーソルを合わせます

   _ボタンを押しても動作しない場合、ウインドウのクリックが必要な場合があります_

2. __Mouse Look Button__ (デフォルトではマウスの右クリック)を押したままにします
3. マウスをウインドウ内で動かしてカメラを回転させます

## カメラの移動

移動キーを押し続けます（W/A/S/D が 前/左/後/右 に対応します）

<iframe width="560" height="315" src="https://www.youtube.com/embed/Z7L4I1ET7GU" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

# ハンドシミュレーション

入力シミュレーションサービスは、ハンドデバイスをサポートします。バーチャルハンドは、ボタンやグラブ可能オブジェクトなどの、通常のハンドデバイスをサポートしたオブジェクトとインタラクションできます。

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandSimulationMode.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandSimulationMode.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

__Hand Simulation Mode__ は、２つの異なる入力モデルを切り替えて使用できます。

* _Articulated Hands_: 関節の位置のデータを持った多関節ハンドをシミュレートします

   HoloLens 2 のインタラクションモデルをエミュレートします

   このモードでは、手の正確な位置やタッチに基づいたインタラクションをシミュレートできます

* _Gestures_: Air Tap や基本的なジェスチャーを持ったシンプルな手をシミュレートします

  [HoloLens interaction model](https://docs.microsoft.com/en-us/windows/mixed-reality/gestures) をエミュレートします。

   フォーカスは視線ポインタを使ってコントロールします。_Air Tap_ ジェスチャーでボタンと対話します

## 手の動きのコントロール

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandControlSettings.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandControlSettings.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

_Left/Right Hand Manipulation Key_ （デフォルトでは 左Shift/Space がそれぞれ 左手/右手 に対応）を押し続けてそれぞれの手をコントロールします。キーを押し続けている間、手が表示されます。マウスの動きで手を移動できます。

Manipuration キーが押されなくなると、手は短い _Hand Hide Timeout_ の後に表示されなくなります。
手の表示を永続的に切り替えるには、 _Toggle Left/Right Hand Key_ （デフォルトでは T/Y が 左手/右手 に対応）を押してください。Toggle キーを再度押すと、再度手の表示を消すことができます。

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandPlacementSettings.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandPlacementSettings.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

手は、_mouse wheel_ を使って遠ざけたり近づけたりすることができます。
デフォルトでは、手はマウスのスクロールに応じてややゆっくり動きますが、*Hand Depth Multiplier* に大きな数字を入れることによって動きを速くすることができます。

初期状態のカメラと手が表示される距離は、*Default Hand Distance.* で制御できます。

デフォルトでは、シミュレートされた手の関節は完全に静的なものです。実機デバイスでは、ハンドトラッキングの原理的な特性として、いくらかのジッターやノイズが発生することに注意してください。
実機デバイスでは手のメッシュや関節が有効になっているのを見ることができます（そして、手を完全に静止させていても少しジッターが発生するのがわかります）。*Hand Jitter Amount* をプラスの値（例として上の画像にあるように 0.1 程度）にすることにより、ジッターのシミュレーションを行うことができます。

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandRotationSettings.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandRotationSettings.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

正確な方向が必要な場合、手を回転させることができます。

* ヨー : Y軸回転 (デフォルトでは E/Q キーが  時計回り/反時計回り回転に対応)
* ピッチ : X軸回転 (デフォルトでは F/R キーが  時計回り/反時計回り回転に対応)
* ロール : Z軸回転 (デフォルトでは X/Z キーが  時計回り/反時計回り回転に対応)

<iframe width="560" height="315" src="https://www.youtube.com/embed/uRYfwuqsjBQ" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## ハンドジェスチャー

ピンチ、グラブ、指差しなどのハンドジェスチャーをシミュレートできます

<a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandGestureSettings.png">
  <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_HandGestureSettings.png" title="Full Hand Mesh" width="80%" class="center" />
</a>

1. 最初に、Manipuration キー（左Shift/Space）を使って手を有効にしてください

   別の方法では、Toggle キー（T/Y）を使って手の on/off を切り替えることもできます

2. 操作の間、マウスボタンを押し続けることによってハンドジェスチャーを実行することができます

_Left/Middle/Right Mouse Hand Gesture_ 設定を使用して、各マウスボタンを、手の形が異なるジェスチャーにマップすることができます。 _Default Hand Gesture_ は、どのボタンも押されていないときの手の形です。

> [!NOTE]
> _Pinch_ ジェスチャーは、この時点では "Select" アクションを発生させる唯一のジェスチャーです

## 片手での操作

1. 手をコントロールするキー（Space/左Shift）を押し続けてください
2. オブジェクトをポイントしてください
3. マウスボタンを押し続けるとピンチ操作となります
4. マウスでオブジェクトを動かしてください
5. マウスボタンを離すと操作が終了します

<iframe width="560" height="315" src="https://www.youtube.com/embed/rM0xaHam6wM" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## 両手での操作

オブジェクトを両手で操作するには、手を永続化するモードが推奨されます

1. Toggle キー（T/Y）を押すことで両手の表示を切り替えることができます
2. １回に１つの手を操作します
   1. _Space_ を押し続けて右手を操作します
   2. グラブしたいオブジェクトに手を動かします
   3. マウスボタンを押して _Pinch_ ジェスチャーを有効にします。永続化モードではマウスボタンを離すまでジェスチャーが有効になります
3. 同じ手順をもう片方の手で繰り返します。同じオブジェクトの別のポイントをグラブします
4. これで両手で同じオブジェクトをグラブした状態になるので、どちらかの手を動かして両手での操作を行います

<iframe width="560" height="315" src="https://www.youtube.com/embed/Qol5OFNfN14" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## GGV インタラクション

1. [Input Simulation Profile](#enabling-the-input-simulation-service) で、__Hand Simulation Mode__ を _Gestures_ にすることにより、GGV のシミュレーションに切り替えることができます

    <a target="_blank" href="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_SwitchToGGV.png">
      <img src="../../Documentation/Images/InputSimulation/MRTK_InputSimulation_SwitchToGGV.png" title="Full Hand Mesh" width="80%" class="center" />
    </a>

2. カメラを回転させて、視線カーソルをインタラクション可能なオブジェクトにポイントします（右マウスボタンを使用）
3. _Space_ を押し続けて、右手を操作します
4. _left mouse button_ を押し続けてインタラクションします
5. 再度カメラを回転させてオブジェクトを操作します

<iframe width="560" height="315" src="https://www.youtube.com/embed/6841rRMdqWw" class="center" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen />

## アイトラッキング

[Eye tracking simulation](../EyeTracking/EyeTracking_BasicSetup.md#simulating-eye-tracking-in-the-unity-editor) は、[Input Simulation Profile](#enabling-the-input-simulation-service) の __Simulate Eye Position__ オプションをチェックすることで有効になります。これは GGV スタイルのインタラクションでは使用すべきではありません（ですので、 __Hand Simulation Mode__ が _Articulated_ にセットされていることを確認してください）

