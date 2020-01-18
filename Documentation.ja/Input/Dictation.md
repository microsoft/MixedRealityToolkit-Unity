# Dictation (ディクテーション)

ディクテーションにより、ユーザーが音声クリップを録音し、トランスクリプション(書き起こし)を得ることができます。利用するには、*Input System Profile* にディクテーション システムが登録されていることを確認してください。**Windows Dictation Input Provider** はすぐに利用できるディクテーション システムですが、代わりのディクテーション システムは [`IMixedRealityDictationSystem`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityDictationSystem) を実装することで作成できます。

## 必要条件

ディクテーション システムは、Unity の [DictationRecognizer](https://docs.unity3d.com/ScriptReference/Windows.Speech.DictationRecognizer.html) を使用します。
これは、ディクテーションを扱うために、基礎として Windows speech APIs を利用します。
このことは、この機能が Windows ベースのプラットフォームでのみ存在するということを意味することに注意してください。

ディクテーション システムの利用には、[PlayerSettings の Capabilities セクション](https://docs.unity3d.com/Manual/class-PlayerSettingsWSA.html#Capabilities)にて、
「Internet Client」と「Microphone」の機能が必要です。
Unity での音声入力の詳細は、[Windows Mixed Reality Documentation](https://docs.microsoft.com/en-us/windows/mixed-reality/voice-input-in-unity#dictation) 
をご覧ください。

## 設定

<img src="../../Documentation/Images/Input/DictationDataProvider.png" width="80%" class="center">

ディクテーション サービスのセットアップができたら、[`DictationHandler`](xref:Microsoft.MixedReality.Toolkit.Input.DictationHandler) スクリプトを使って録音セッションの開始と停止や、UnityEvents を通じた文字起こし結果の取得が可能です。

<img src="../../Documentation/Images/Input/DictationHandler.png" width="80%" class="center">

- **Dictation Hypothesis** は、ユーザーが話すにつれて、これまでキャプチャされた音声の初期の粗いトランスクリプションとともに発生されます。
- **Dictation Result** は、各センテンスの終わり (すなわち、ユーザーが間を置いたとき) に、これまでキャプチャされた音声の最終トランスクリプションとともに発生されます。
- **Dictation Complete** は、録音セッションの最後に、音声のすべての最終トランスクリプションとともに発生されます。
- **Dictation Error** はディクテーション サービスでのエラーを通知するために発生されます。この場合のトランスクリプションには、エラーの説明が含まれています。

## サンプルシーン

`MixedRealityToolkit.Examples\Demos\Input\Scenes\Dictation` 内の **Dictation** シーンは、`DictationHandler` スクリプトの使い方を示しています。より多くの制御が必要であれば、このスクリプトを拡張するか、または [`IMixedRealityDictationHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityDictationHandler) の独自実装を作ってディクテーション イベントを直接受け取ってください。

<img src="../../Documentation/Images/Input/DictationDemo.png" width="80%" class="center">
