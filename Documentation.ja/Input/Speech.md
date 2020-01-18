# Speech (音声)

*Windows Speech Input* などの音声入力プロバイダーは、コントローラーを作成しませんが、代わりに認識時に音声入力イベントを発生させるキーワードを定義することができます。*Input System Profile* の **Speech Commands Profile** は、認識するキーワードを設定する場所です。各コマンドでは次のこともできます：

- マップする **input action** を選択します。この方法では、たとえば、キーワード *Select* を使用して、マウスの左クリックと同じアクションにマップすることにより、左クリックと同じ効果を得ることができます。
- 押されたときに同じ音声イベントを生成する **key code** を指定します。
- UWP アプリで使用される **localization key** を追加して、アプリのリソースからローカライズされたキーワードを取得します。

<img src="../../Documentation/Images/Input/SpeechCommandsProfile.png" width="450px">

## 音声入力のハンドリング

[**`Speech Input Handler`**](xref:Microsoft.MixedReality.Toolkit.Input.SpeechInputHandler) スクリプトは、[**UnityEvents**](https://docs.unity3d.com/Manual/UnityEvents.html) を使用して音声コマンドを扱うために、GameObject に追加することができます。**Speech Commands Profile** で定義されたキーワードのリストが自動的に表示されます。

<img src="../../Documentation/Images/Input/SpeechCommands_SpeechInputHandler1.png" width="450px">

オプションの **SpeechConfirmationTooltip.prefab** を割り当てることによって、認識時にアニメーションする確認用ツールチップ ラベルを表示することができます。 

<img src="../../Documentation/Images/Input/SpeechCommands_SpeechInputHandler2.png">

別の方法として、開発者はカスタム スクリプト コンポーネントに [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) インターフェイスを実装することにより、[音声入力イベントを扱う](InputEvents.md#input-event-interface-example)ことができます。

## サンプルシーン

`MixedRealityToolkit.Examples\Demos\Input\Scenes\Speech` の **SpeechInputExample** シーンでは、音声を扱う方法を見ることができます。また、[`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) を実装することにより、自分のスクリプトで直接音声コマンドを聞き取ることができます([event handlers](InputEvents.md) の表を参照してください)。

<img src="../../Documentation/Images/Input/SpeechExampleScene.png" width="750px">
