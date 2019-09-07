# ボタン-Button

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
| HoloLens 第一世代のシェルスタイルボタン | 丸型押しボタン | ベーシックなボタン |

[`Button.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/Button.prefab) は、 [Interactable](README_Interactable.md) のコンセプトに基づいており、ボタンまたはその他のタイプのインタラクション可能な面に簡単な UI コントロールを提供します。 ベースラインボタンは、近くのインタラクション要素に対して多関節ハンド (articulated hand) で入力する場合や、遠くのインタラクション要素に対して注視＋エアタップなど、利用可能な全ての入力方法をサポートします。音声コマンドを使用してボタンをトリガーすることもできます。
[`PressableButtonHoloLens2.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2.prefab) は、HoloLens 2 のシェルスタイルボタンで、ダイレクトハンドトラッキングの入力用に精密な動きをサポートします。 `Interactable` のスクリプトと `PressableButton` のスクリプトを組み合わせています。

## Pressable button の使い方

単に [`PressableButtonHoloLens2.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2.prefab) または[`PressableButtonHoloLens2Unplated.prefab`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2Unplated.prefab) をドラッグしてシーンに置くだけです。これらのボタンプレハブは、多関節ハンド (articulated hand) 入力や注視など、様々なタイプの入力に対して視聴覚フィードバックするように既に設定されています。

プレハブ自体と [Interactable](README_Interactable.md) コンポーネントで公開されるイベントを使用して、追加のアクションをトリガーできます。 [HandInteractionExample のシーン](README_HandInteractionExamples.md)の pressable buttons は、Interactable の *OnClick* イベントを使って、キューブの色の変更をトリガーします。このイベントは、Gaze、AirTap、HandRay などの様々なタイプの入力メソッド、及び pressable button のスクリプトを介した物理的なボタンの押下に対してトリガーされます。

<img src="../Documentation/Images/Button/MRTK_Button_HowToUse_Interactable.png" width="450">

ボタンの `PhysicalPressEventRouter` を介して、pressable button が *OnClick* イベントを発生させるタイミングを設定できます。例えば、*OnClick* は、*Interactable On Click* を *Event On Press* に設定することにより、ボタンが最初に押された時と離された時に起動するように設定できます。

<img src="../Documentation/Images/Button/MRTK_Button_HowTo_Events.png" width="450">

Articulated hand の入力状態情報を活用するには、pressable button イベントの - *Touch Begin*, *Touch End*, *Button Pressed*, *Button Released* を使用できます。ただし、これらのイベントは、AirTap、HandRay、Gaze 入力には応答して発生はしません。

<img src="../Documentation/Images/Button/MRTK_Button_HowTo_PressableButton.png" width="450">

## インタラクション状態

アイドル状態では、ボタンの全面プレートは見えません。指が近づいたり、視線入力のカーソルが表面をターゲットすると、全面のプレートの光る境界線が可視化されます。全面プレートの表面には、指先の位置がさらにハイライトされます。指で押すと、全面プレートが指先で動きます。指先が全面プレートの表面に触れると、わずかにパルスのエフェクトが現れ、タッチポイントの視覚的なフィードバックが得られます。

<img src="../Documentation/Images/Button/MRTK_Button_InteractionStates.png" width="600">

このわずかなパルスエフェクトは、現在インタラクションしているポインター上に存在する  *ProximityLight(s)* を探す、pressable button によってトリガーされます。近接ライトが見つかった場合、  `ProximityLight.Pulse` メソッドが呼び出され、シェーダーパラメーターを自動的にアニメーション化してパルスを表示します。

## Inspector properties ##

![Button](../Documentation/Images/Button/MRTK_Button_Structure.png)

**Box Collider**
`Box Collider` for the button's front plate.

**Pressable Button**
The logic for the button movement with hand press interaction.

**Physical Press Event Router**
This script sends events from hand press interaction to [Interactable](README_Interactable.md).

**Interactable**
[Interactable](README_Interactable.md) handles various types of interaction states and events. HoloLens gaze, gesture, and voice input and immersive headset motion controller input are directly handled by this script.

**Audio Source**
Unity audio source for the audio feedback clips.

*NearInteractionTouchable.cs*
Required to make any object touchable with articulated hand input.

## Prefab Layout
The *ButtonContent* object contains front plate, text label and icon. The *FrontPlate* responds to the proximity of the index fingertip using the *Button_Box* shader. It shows glowing borders, proximity light, and a pulse effect on touch. The text label is made with TextMesh Pro. *SeeItSayItLabel*'s visibility is controlled by [Interactable](README_Interactable.md)'s theme.

![Button](../Documentation/Images/Button/MRTK_Button_Layout.png)

## Voice command ('See-it, Say-it') ##

**Speech Input Handler**
The [Interactable](README_Interactable.md) script in Pressable Button already implements `IMixedRealitySpeechHandler`. A voice command keyword can be set here. 

<img src="../Documentation/Images/Button/MRTK_Button_Speech1.png" width="450">

**Speech Input Profile**
Additionally, you need to register the voice command keyword in the global *Speech Commands Profile*. 

<img src="../Documentation/Images/Button/MRTK_Button_Speech2.png" width="450">

**See-it, Say-it label**
The pressable button prefab has a placeholder TextMesh Pro label under the *SeeItSayItLabel* object. You can use this label to communicate the voice command keyword for the button to the user.

<img src="../Documentation/Images/Button/MRTK_Button_Speech3.png" width="450">

## How to make a button from scratch ##
You can find the examples of these buttons in the **PressableButtonExample** scene.

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube0.png">

### 1. Creating a Pressable Button with Cube (Near interaction only)
1. Create a Unity Cube (GameObject > 3D Object > Cube)
2. Add `PressableButton.cs` script
3. Add `NearInteractionTouchable.cs` script

In the `PressableButton`'s Inspector panel, assign the cube object to the **Moving Button Visuals**. 

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube3.png" width="450">

When you select the cube, you will see multiple colored layers on the object. This visualizes the distance values under **Press Settings**. Using the handles, you can configure when to start press (move the object) and when to trigger event.

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube1.jpg" width="450">

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube2.png" width="450">

When you press the button, it will move and generate proper events exposed in the `PressableButton.cs` script such as TouchBegin(), TouchEnd(), ButtonPressed(), ButtonReleased().

<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun1.jpg">

### 2. Adding visual feedback to the basic cube button
MRTK Standard Shader provides various features that makes it easy to add visual feedback. Create an material and select shader `Mixed Reality Toolkit/Standard`. Or you can use or duplicate one of the existing materials under `/SDK/StandardAssets/Materials/` that uses MRTK Standard Shader.

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube4.png" width="450">

Check `Hover Light` and `Proximity Light` under **Fluent Options**. This enables visual feedback for both near hand(Proximity Light) and far pointer(Hover Light) interactions.

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube5.png" width="450">

<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun2.jpg">

### 3. Adding audio feedback to the basic cube button
Since `PressableButton.cs` script exposes events such as TouchBegin(), TouchEnd(), ButtonPressed(), ButtonReleased(), we can easily assign audio feedback. Simply add Unity's `Audio Source` to the cube object then assign audio clips by selecting AudioSource.PlayOneShot(). You can use MRTK_Select_Main and MRTK_Select_Secondary audio clips under `/SDK/StandardAssets/Audio/` folder.

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube7.png" width="450">

<img src="../Documentation/Images/Button/MRTK_PressableButtonCube6.png" width="450">

### 4. Adding visual states and handle far interaction events
[Interactable](README_Interactable.md) is a script that makes it easy to create a visual states for the various types of input interactions. It also handles far interaction events. Add `Interactable.cs` and drag and drop the cube object onto the **Target** field under **Profiles**. Then, create a new Theme with a type **ScaleOffsetColorTheme**. Under this theme, you can specify the color of the object for the specific interaction states such as **Focus** and **Pressed**. You can also control Scale and Offset as well. Check **Easing** and set duration to make the visual transition smooth.

 <img src="../Documentation/Images/Button/MRTK_PressableButtonCube8.png" width="450">
  <img src="../Documentation/Images/Button/MRTK_PressableButtonCube9.png" width="450">

You will see the object responds to both far(hand ray or gaze cursor) and near(hand) interactions.

<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun3.jpg">
<img src="../Documentation/Images/Button/MRTK_PressableButtonCubeRun4.jpg">

## Custom Button Examples ##

In the [HandInteractionExample scene](README_HandInteractionExamples.md), you can take a look at the piano and round button examples which are both using `PressableButton`. 

<img src="../Documentation/Images/Button/MRTK_Button_Custom1.png" width="450">

<img src="../Documentation/Images/Button/MRTK_Button_Custom2.png" width="450">

Each piano key has a `PressableButton` and a `NearInteractionTouchable` script assigned. It is important to verify that the *Local Forward* direction of `NearInteractionTouchable` is correct. It is represented by a white arrow in the editor. Make sure the arrow points away from the button's front face:

<img src="../Documentation/Images/Button/MRTK_Button_Custom3.png" width="450">


