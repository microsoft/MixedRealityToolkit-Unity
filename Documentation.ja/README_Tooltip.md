# Tooltip #

![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Main.png)

Tooltip は、通常、オブジェクトを詳しく調べるときにヒントや追加情報を伝えるために使用されます。Tooltip を使用して、物理環境内のオブジェクトにアノテーションを付けることができます。

## Tooltip の使い方 ##

Tooltip は、[Hierarchy](ヒエラルキー) ウィンドウ に直接追加し、オブジェクトをターゲットとすることができます。

この方法を使用するには、単にゲームオブジェクトと[Tooltip プレハブ](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Tooltips) をシーンの[Hierarchy](ヒエラルキー)に追加するだけです。プレハブのインスペクタ パネルで、*Tooltip* (スクリプト)を展開します。チップの状態を選択し、Tooltip の設定を行います。 それぞれの Tooltip のテキスト フィールドに入力します。*ToolTipConnector* (スクリプト) を展開し、Tooltip を持つオブジェクトを[Hierarchy](ヒエラルキー) ウィンドウから *Target* というラベルの付いたフィールドにドラッグします。これにより、Tooltip がオブジェクトにアタッチされます。
![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Connector.png)


この使用では、Tooltip ンポーネントのツールチップ状態プロパティを変更して、常に表示されるツールチップ、またはスクリプトを介して表示/非表示のツールチップを想定しています。
 
## 動的に tooltips を作成##
ツールチップは、実行時にオブジェクトに動的に追加できるだけでなく、タップまたはフォーカスに表示および非表示にする事前設定もできます。 [`ToolTipSpawner`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Tooltips/ToolTipSpawner.cs) スクリプトを任意のゲームオブジェクトに追加するだけです。表示と消失の遅延は、スクリプトインスペクタと有効期間で設定できるため、Tooltip は設定された期間が過ぎに消えます。Tooltip には、スポーン スクリプトの背景ビジュアルなどのスタイル プロパティも備えています。デフォルトでは、Tooltip は spawner スクリプトを使用してオブジェクトに固定されます。これは、アンカー フィールドに GameObject を割り当てることで変更できます。

## サンプルシーン ##
[サンプルシーン](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/UX/Tooltips/Scenes)では、ツールヒントのさまざまな例を見つけることができます。

![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Examples.png)
