# ジェスチャ

ジェスチャは人の手に基づいた入力イベントです。MRTK にはジェスチャ入力イベントを発生させる2種類のデバイスがあります。

- HoloLens のような Windows Mixed Reality デバイス。これは、ピンチ動作 (「エア タップ」) とタップ & ホールド ジェスチャを扱います。

  HoloLens のジェスチャに関するより多くの情報は、[Windows Mixed Reality Gestures ドキュメント](https://docs.microsoft.com/windows/mixed-reality/gestures) をご覧ください。

  [`WindowsMixedRealityDeviceManager`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input.WindowsMixedRealityDeviceManager) は、HoloLens デバイスからの Unity のジェスチャ イベントを利用するために、[Unity XR.WSA.Input.GestureRecognizer](https://docs.unity3d.com/ScriptReference/XR.WSA.Input.GestureRecognizer.html) をラップしています。

- タッチ スクリーン デバイス。

  [`UnityTouchController`](xref:Microsoft.MixedReality.Toolkit.Input.UnityInput) は、物理的なタッチ スクリーンをサポートする [Unity Touch class](https://docs.unity3d.com/ScriptReference/Touch.html) をラップしています。

これらの入力ソースの両方で、Unity の Touch と Gesture イベントを MRTK の [Input Actions (入力アクション)](InputActions.md) にそれぞれ変換するため、 _Gesture Settings_ プロファイルを使用します。このプロファイルは、_Input System Settings_ プロファイルの下にあります。

<img src="../../Documentation/Images/Input/GestureProfile.png" style="max-width:100%;">

## Gesture Events (ジェスチャ イベント)

ジェスチャ イベントは、以下のジェスチャ ハンドラー インターフェイスの1つを実装したものによって、受け取られます。[`IMixedRealityGestureHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler) または [`IMixedRealityGestureHandler<TYPE>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) ([イベント ハンドラー](InputEvents.md) の表をご覧ください).

ジェスチャ イベント ハンドラーの実装例については、[サンプル シーン](#サンプル-シーン) をご覧ください。

汎用のバージョンを実装する場合、*OnGestureCompleted* と *OnGestureUpdated* イベントは、以下の型のデータを受け取ることができます。

- `Vector2` - 2D 位置のジェスチャ。タッチ スクリーンによって、[`deltaPosition`](https://docs.unity3d.com/ScriptReference/Touch-deltaPosition.html) を通知するために生成される。
- `Vector3` - 3D 位置のジェスチャ。HoloLens によって、以下を通知するために生成される。
  - マニピュレーション イベントの [`cumulativeDelta`](https://docs.unity3d.com/ScriptReference/XR.WSA.Input.ManipulationUpdatedEventArgs-cumulativeDelta.html)
  - ナビゲーション イベントの [`normalizedOffset`](https://docs.unity3d.com/ScriptReference/XR.WSA.Input.NavigationUpdatedEventArgs-normalizedOffset.html)
- `Quaternion` - 3D 回転のジェスチャ。カスタムの入力ソースで利用可能だが、現在は存在するどの入力ソースからも生成されていない。
- `MixedRealityPose` - 3D 位置と回転が組み合わさったジェスチャ。カスタムの入力ソースで利用可能だが、現在は存在するどの入力ソースからも生成されていない。

## イベントの順序

ユーザーの入力に応じて、2つの主なイベントのチェーンがあります。

- "Hold (ホールド)":
    1. タップをホールド:
        * _Manipulation_ 開始
    1. [HoldStartDuration](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile.HoldStartDuration) を超えてタップをホールド:
        * _Hold_ 開始
    1. タップをリリース:
        * _Hold_ 完了
        * _Manipulation_ 完了

- "Move (動かす)":
    1. タップをホールド:
        * _Manipulation_ 開始
    1. [HoldStartDuration](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile.HoldStartDuration) を超えてタップをホールド:
        * _Hold_ 開始
    1. [NavigationStartThreshold](xref:Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile.NavigationStartThreshold) を超えて手を動かす:
        * _Hold_ キャンセル
        * _Navigation_ 開始
    1. タップをリリース:
        * _Manipulation_ 完了
        * _Navigation_ 完了

## サンプル シーン

`MixedRealityToolkit.Examples\Demos\HandTracking\Scenes` 内の **HandInteractionGestureEventsExample** シーンでは、ポインターの結果を使って、オブジェクトをヒットした位置に生成する方法を示しています。

[Gesture Tester script](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/HandTracking/Script/GestureTester.cs) は、ジェスチャ イベントを GameObject を介して可視化する実装例です。このハンドラー 関数はインジケーター オブジェクトの色を変え、最後に記録されたイベントをシーンのテキスト オブジェクトに表示します。
