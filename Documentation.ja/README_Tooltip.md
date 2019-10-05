# Tooltip（ツールチップ） #

![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Main.png)

Tooltip は通常、オブジェクトを詳しく調べるときのヒントや追加情報を伝えるために使用されます。Tooltip を使用して、物理環境内のオブジェクトにアノテーションを付けることができます。

## Tooltip の使い方 ##
Tooltip は、[Hierarchy](ヒエラルキー) に直接追加し、オブジェクトをターゲットとすることができます。

この方法を使用するには、単にゲーム オブジェクトと[Tooltip プレハブ](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Tooltips) をシーンの [Hierarchy](ヒエラルキー) に追加するだけです。プレハブのインスペクタ パネルで、*Tooltip* (スクリプト)を展開します。Tip State を選択し、Tooltip の設定を行います。Tooltip のテキスト フィールドにそれぞれのテキストを入力します。*ToolTipConnector* (スクリプト) を展開し、Tooltip を持たせるオブジェクトを [Hierarchy](ヒエラルキー) から *Target* というフィールドにドラッグします。これにより、Tooltip がオブジェクトにアタッチされます。
![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Connector.png)


この使用方法は、Tooltip が常に表示される場合や、Tooltip コンポーネントの Tooltip State プロパティをスクリプトで変更して表示/非表示を切り替える場合を想定しています。
 
## 動的に Tooltip を作成する##
Tooltip は、事前に設定しておいてタップやフォーカスによって表示と非表示を行うだけでなく、実行時にオブジェクトに動的に追加することもできます。 [`ToolTipSpawner`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Tooltips/ToolTipSpawner.cs) スクリプトを任意のゲーム オブジェクトに追加するだけですスクリプトのインスペクターにて、設定された期間が過ぎると Tooltip が消えるように生存期間を設定したり、現れたり消えたりする際の遅延時間を設定したりできます。Tooltip は背景の見た目のようなスタイルのプロパティも備えており、ToolTipSpawner スクリプトで設定できます。デフォルトでは、Tooltip は ToolTipSpawner スクリプトを持つオブジェクトに固定されます。これは、アンカー フィールドに GameObject を割り当てることで変更できます。

## サンプルシーン ##
[サンプルシーン](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/UX/Tooltips/Scenes)では、Tooltip のさまざまな例を見つけることができます。

![Tooltip](../Documentation/Images/Tooltip/MRTK_Tooltip_Examples.png)
