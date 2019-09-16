# ボタン (Button)

![Button](../Documentation/Images/Button/MRTK_Button_Main.png)

ボタン (button) を使うと、ユーザは即座にアクションを引き起こすことができます。Mixed Reality の最も基本的なコンポーネントの１つです。MRTK は、様々なタイプのボタンプレハブを提供しています。

## MRTK のボタンプレハブ

``MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs`` フォルダ下のボタンプレハブのサンプル

|  ![PressableButtonHoloLens2](../Documentation/Images/Button/MRTK_Button_Prefabs_HoloLens2.png) PressableButtonHoloLens2 | ![PressableButtonHoloLens2Unplated](../Documentation/Images/Button/MRTK_Button_Prefabs_HoloLens2Unplated.png) PressableButtonHoloLens2Unplated | ![PressableButtonHoloLens2Circular](../Documentation/Images/Button/MRTK_Button_Prefabs_HoloLens2Circular.png) PressableButtonHoloLens2Circular |
|:--- | :--- | :--- |
| HoloLens 2 のバックプレート付きのシェルスタイルボタンは、ボーダーライト (border light)、近接ライト (proximity light)、扁平なフロントプレート (compressed front plate) などの様々な視覚フィードバックをサポートします。 | バックプレートのない HoloLens 2 のシェルスタイルボタン | HoloLens 2 の円形シェルスタイルボタン |
|  ![PressableButtonHoloLens2_32x96](../Documentation/Images/Button/MRTK_Button_Prefabs_HoloLens2_32x96.png) **PressableButtonHoloLens2_32x96** | ![PressableButtonHoloLens2Bar3H](../Documentation/Images/Button/MRTK_Button_Prefabs_HoloLens2BarH.png) **PressableButtonHoloLens2Bar3H** | ![PressableButtonHoloLens2Bar3V](../Documentation/Images/Button/MRTK_Button_Prefabs_HoloLens2BarV.png) **PressableButtonHoloLens2Bar3V** |
| ワイドな HoloLens 2 のシェルスタイルボタン 32x96mm | バックプレートを共有する水平な HoloLens 2 ボタンバー | バックプレートを共有する垂直な HoloLens 2 ボタンバー |
|  ![Radial](../Documentation/Images/Button/MRTK_Button_Radial.png) **Radial** | ![Checkbox](../Documentation/Images/Button/MRTK_Button_Checkbox.png) **Checkbox** | ![ToggleSwitch](../Documentation/Images/Button/MRTK_Button_ToggleSwitch.png) **ToggleSwitch** |
| Radial button | Checkbox  | Toggle switch |
|  ![ButtonHoloLens1](../Documentation/Images/Button/MRTK_Button_HoloLens1.png) **ButtonHoloLens1** | ![PressableRoundButton](../Documentation/Images/Button/MRTK_Button_Round.png) **PressableRoundButton** | ![Button](../Documentation/Images/Button/MRTK_Button_Base.png) **Button** |
| HoloLens 1 のシェルスタイルボタン | 丸型押しボタン | ベーシックなボタン |

[`Button.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/Button.prefab) は、 [Interactable](README_Interactable.md) のコンセプトに基づいており、ボタンまたはその他のタイプのインタラクション可能な面に簡単な UI コントロールを提供します。 ベースラインボタンは、近くのインタラクション要素に対して多関節ハンド (articulated hand) で入力する場合や、遠くのインタラクション要素に対してゲイズ＋エアタップなど、利用可能な全ての入力方法をサポートします。音声コマンドを使用してボタンをトリガーすることもできます。

[`PressableButtonHoloLens2.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2.prefab) は、HoloLens 2 のシェルスタイルボタンで、ダイレクトハンドトラッキングの入力用に精密な動きをサポートします。 `Interactable` のスクリプトと `PressableButton` のスクリプトを組合わせています。

## Pressable button の使い方

単に [`PressableButtonHoloLens2.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2.prefab) または[`PressableButtonHoloLens2Unplated.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2Unplated.prefab) をドラッグしてシーンに置くだけです。これらのボタンプレハブは、多関節ハンド (articulated hand) 入力やゲイズなど、様々なタイプの入力に対して視聴覚フィードバックするように既に設定されています。

プレハブ自体と [Interactable](README_Interactable.md) コンポーネントで公開されるイベントを使用して、追加のアクションをトリガーできます。 [HandInteractionExample のシーン](README_HandInteractionExamples.md)の pressable buttons は、Interactable の *OnClick* イベントを使って、キューブの色の変更をトリガーします。このイベントは、Gaze、AirTap、HandRay などの様々なタイプの入力メソッド、及び pressable button のスクリプトを介した物理的なボタンの押下に対してトリガーされます。

<img src="../Documentation/Images/Button/MRTK_Button_HowToUse_Interactable.png" width="450">

ボタンの `PhysicalPressEventRouter` を介して、Pressable button が *OnClick* イベントを発生させるタイミングを設定できます。例えば、*Interactable On Click* を *Event On Press* に設定することにより、*OnClick* をボタンを押して離した時ではなく、最初に押した時にトリガーするよう設定できます。

<img src="../Documentation/Images/Button/MRTK_Button_HowTo_Events.png" width="450">

Articulated hand の入力状態情報を活用するには、pressable button イベントの - *Touch Begin*, *Touch End*, *Button Pressed*, *Button Released* を使用できます。ただし、これらのイベントは、AirTap、HandRay、Gaze 入力には応答して発生はしません。

<img src="../Documentation/Images/Button/MRTK_Button_HowTo_PressableButton.png" width="450">

## インタラクションの状態

アイドル状態では、ボタンの前面プレートは見えません。指が近づいたり、視線入力のカーソルが表面をターゲットすると、前面のプレートの光る境界線が現れます。前面プレートの表面には、指先の位置がさらにハイライトされます。指で押すと、前面プレートが指先で動きます。指先が前面プレートの表面に触れると、わずかにパルスのエフェクトが現れ、タッチポイントの視覚的なフィードバックが得られます。

<img src="../Documentation/Images/Button/MRTK_Button_InteractionStates.png" width="600">

このわずかなパルスエフェクトは、現在インタラクションしているポインター上に存在する  *ProximityLight(s)* を探す、pressable button によってトリガーされます。近接ライトが見つかった場合、  `ProximityLight.Pulse` メソッドが呼び出され、シェーダーパラメーターを自動的にアニメーション化してパルスを表示します。

## [Inspector] (インスペクタ―) プロパティ

![Button](../Documentation/Images/Button/MRTK_Button_Structure.png)

**ボックスコライダー**
ボタンの前面プレートのための `Box Collider`。

**Pressable Button**
ハンドプレス インタラクション を使ったボタン移動のロジック。

**Physical Press Event Router**
ハンドプレス インタラクションから [Interactable](README_Interactable.md) へイベントを送るスクリプト。

**Interactable**
[Interactable](README_Interactable.md) は様々なタイプのインタラクションの状態とイベントを処理します。HoloLens のゲイズ，ジェスチャ，及び音声入力と，没入型ヘッドセットのモーションコントローラの入力は，このスクリプトによって直接処理されます。

**オーディオソース**
音声フィードバッククリップ用の Unity のオーディオソース。

*NearInteractionTouchable.cs*
多関節ハンドでオブジェクトをタッチ可能にするために必要です。

## Prefab のレイアウト

*ButtonContent* オブジェクトには、フロントプレート、テキストラベル、およびアイコンが含まれています。 *FrontPlate*は、*Button_Box* シェーダーを使用して、人差し指の近接に応答します。 光る境界線、近接ライト、およびタッチのパルスエフェクトを示します。 テキストラベルは TextMesh Pro で作成されます。 *SeeItSayItLabel* の可視性は、[Interactable]（README_Interactable.md）のテーマによって制御されます。

![Button](../Documentation/Images/Button/MRTK_Button_Layout.png)

## 音声コマンド ('See-it, Say-it')

**Speech Input Handler**
Pressable Buttonの[Interactable]（README_Interactable.md）スクリプトは、すでに `IMixedRealitySpeechHandler`を実装しています。 ここで音声コマンドのキーワードを設定できます。

<img src="../Documentation/Images/Button/MRTK_Button_Speech1.png" width="450">

**音声入力プロファイル (Speech Input Profile)**
さらに、グローバルな *Speech Commands Profile* に音声コマンドキーワードを登録する必要があります。

<img src="../Documentation/Images/Button/MRTK_Button_Speech2.png" width="450">

**See-it, Say-it ラベル**
Pressable Button プレハブには、*SeeItSayItLabel* オブジェクトの下にプレースホルダー TextMesh Pro ラベルがあります。このラベルを使用して、ボタンの音声コマンドキーワードをユーザーに伝えることができます。

<img src="../Documentation/Images/Button/MRTK_Button_Speech3.png" width="450">

## How to make a button from scratch ##
これらのボタンの例は、**PressableButtonExample** のシーンにあります。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube0.png">

### 1. キューブで Pressable Button を作成する (Near interaction のみ)

1. Unity のキューブを作成します（GameObject> 3D Object> Cube）
2. `PressableButton.cs` のスクリプトを追加します
3. `NearInteractionTouchable.cs` のスクリプトを追加します

`PressableButton` の [Inspector] (インスペクター) パネルで、キューブオブジェクトを **Moving Button Visuals** に割り当てます。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube3.png" width="450">

キューブを選択すると、オブジェクト上に複数の色付きのレイヤーが表示されます。 これにより、**Press Settings** の下の距離の値が視覚化されます。 ハンドルを使用して、プレスを開始するタイミング（オブジェクトを動かす）とイベントをトリガーするタイミングを設定できます。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube1.jpg" width="450">

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube2.png" width="450">

ボタンを押すと、ボタンが移動し，TouchBegin()、TouchEnd()、ButtonPressed()、ButtonReleased() などの `PressableButton.cs` のスクリプトで公開される適切なイベントが生成されます。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun1.jpg">

### 2. 視覚的なフィードバックをベーシックなキューブボタンに加える

MRTK Standard Shader は、視覚的なフィードバックを簡単に追加できるさまざまな機能を提供しています。 マテリアルを作成し、シェーダー `Mixed Reality Toolkit/Standard` を選択します。 または、MRTK 標準シェーダーを使用している `/SDK/StandardAssets/Materials/` にある既存のマテリアルの１つを使用または複製できます。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube4.png" width="450">

**Fluent Options** の下の `Hover Light` と `Proximity Light` をチェックします。 これにより、近くの手 (近接ライト) と遠くのポインター (ホバーライト) の両方のインタラクションの視覚的なフィードバックが可能になります。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube5.png" width="450">

<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun2.jpg">

### 3. オーディオフィードバックをベーシックなキューブボタンに加える

`PressableButton.cs` のスクリプトは TouchBegin()、TouchEnd()、ButtonPressed()、ButtonReleased() などのイベントを公開するため、音声フィードバックを簡単に割り当てることができます。Unity の `Audio Source` をキューブオブジェクトに追加し、AudioSource.PlayOneShot() を選択してオーディオクリップを割り当てます。`/SDK/StandardAssets/Audio/` フォルダーの下のMRTK_Select_Main および MRTK_Select_Secondary オーディオクリップを使用できます。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube7.png" width="450">

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube6.png" width="450">

### 4. 視覚的な状態と遠方のインタラクションイベントの処理を加える

[Interactable]（README_Interactable.md）は、さまざまなタイプの入力インタラクションの視覚的な状態を簡単に作成できるスクリプトです。 また、遠方のインタラクションイベントも処理します。`Interactable.cs` を追加し、キューブオブジェクトを **Profiles** の下の **Target** フィールドにドラッグアンドドロップします。 次に、**ScaleOffsetColorTheme** タイプの新しいテーマを作成します。このテーマでは、**Focus** や **Pressed** などの特定のインタラクションの状態におけるオブジェクトの色を指定できます。スケールとオフセットも制御できます。**Easing** をチェックし、継続時間を設定して視覚的な移行をスムーズにします。

 <img src="../Documentation/Images/Button/MRTK_PressableButtonCube8.png" width="450">
 <img src="../Documentation/Images/Button/MRTK_PressableButtonCube9.png" width="450">

オブジェクトが遠く（ハンドレイ，またはゲイズカーソル）と近く（ハンド）の両方のインタラクションに応答するのがわかります。

<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun3.jpg">
<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun4.jpg">

## カスタムボタンのサンプル ##

[HandInteractionExample のシーン](README_HandInteractionExamples.md) で、ピアノと丸ボタンの例を見てみましょう。どちらも `PressableButton` を使用しています。

<img src="../Documentation/Images/Button/MRTK_Button_Custom1.png" width="450">

<img src="../Documentation/Images/Button/MRTK_Button_Custom2.png" width="450">

各ピアノのキーには、`PressableButton` と `NearInteractionTouchable` のスクリプトが割り当てられています。`NearInteractionTouchable` の *Local Forward* の方向が正しいことを確認することが重要です。エディターでは白い矢印で表されます。矢印がボタンの前面から離れていることを確認してください。

<img src="../Documentation/Images/Button/MRTK_Button_Custom3.png" width="450">
