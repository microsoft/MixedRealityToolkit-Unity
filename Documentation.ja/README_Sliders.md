# Sliders (スライダー)

![Slider example](../Documentation/Images/Slider/MRTK_UX_Slider_Main.jpg)

スライダーは、トラック上でスライダーを動かして、値を連続的に変更できる UI コンポーネントです。現在のところ、Pinch Slider (ピッチ スライダー) は、直接または少し離れた位置でスライダーをつかんで移動できます。スライダーは、モーション コントローラー、ハンド、または、ジェスチャ+音声を使用して、AR および VR で動作します。

## サンプルシーン

`MRTK/Examples/Demos/UX/Slider/Scenes` 以下の **SliderExample** シーンにサンプルがあります。

## スライダの使用方法

**PinchSlider** プレハブをシーン ヒエラルキーにドラッグ＆ドロップします。独自のスライダを変更または作成する場合は、次の操作を行います:

- thumb オブジェクトにコライダーがあることを確認します。PinchSlider プレハブでは、コライダーは `SliderThumb/Button_AnimationContainer/Slider_Button` にあります。
- スライダーを近くでつかみたい場合は、コライダーを含むオブジェクトが Near Interaction Grabbable コンポーネントも持っていることを確認します。

また、次のヒエラルキーを使用することをお勧めします

- PinchSlider - sliderComponent を含みます
  - SliderThumb - 移動可能な thumb を含みます
  - TrackVisuals - トラックやその他のビジュアルを含みます
  - OtherVisuals - その他のビジュアルを含みます

## スライダー イベント

スライダーは次のイベントを公開します:

- OnValueUpdated - スライダーの値が変更されるたびに呼び出されます
- OnInteractionStarted - ユーザーがスライダーをつかんだときに呼び出されます
- OnInteractionEnded - ユーザーがスライダーを離したときに呼び出されます
- OnHoverEntered - ユーザーのハンドまたはコントローラーが、ニアまたはファー インタラクションを使用してスライダーの上に移動したときに呼び出されます
- OnHoverExited - ユーザーのハンドまたはコントローラーが、スライダーの近くから離れたときに呼び出されます。

## スライダの境界と軸の設定

シーン内でハンドルを移動することにより、スライダの開始点と終了点を直接移動できます:

![Sliders Config](../Documentation/Images/Sliders/MRTK_Sliders_Setup.png)

_Slider Axis_ フィールドを使用してスライダの軸 (ローカル空間における) を指定することもできます。

ハンドルを使用できない場合は、代わりに _Slider Start Distance_ フィールドと _Slider End Distance_ フィールドを使用して、スライダーの開始点と終了点を指定できます。スライダの開始 / 終了位置を、スライダの中心からの距離としてローカル座標で指定します。つまり、スライダーの開始距離と終了距離を必要に応じて設定すると、開始距離と終了距離を更新しなくても、スライダーを縮小または拡大できます。

## インスペクターのプロパティ

**Thumb Root** スライダーの Thumb (つまみ) を含んでいるゲームオブジェクト

**Slider Value** スライダーの値

**Track Visuals** スライダーに付随する、視覚的な跡を含むゲームオブジェクト

**Tick Marks** スライダーに付随する、目盛りを含むゲームオブジェクト

**Thumb Visuals** スライダーに付随する、つまみの見た目を含むゲームオブジェクト

**Slider Axis** スライダーが動く軸

**Slider Start Distance** スライダーが移動し始める、ローカル スペースでのスライダーの軸中心からの距離

**Slider End Distance** スライダーの移動が終わる、ローカル スペースでのスライダーの軸中心からの距離

ユーザーがスライダー軸の値をエディターで更新した際、Track Visuals か Tick Visuals が指定されていればそれらのトランスフォームは更新されます。
特に、それらのローカル ポジションはリセットされ、ローカル ローテーションは Slider Axis の姿勢に合うようにセットされます。
それらのスケールは変更されません。
もし、Tick Marks が Grid Object Collection コンポーネントを持っていれば、Layout と CellWidth または CellHeight が Slider Axis に合うように更新されます。
