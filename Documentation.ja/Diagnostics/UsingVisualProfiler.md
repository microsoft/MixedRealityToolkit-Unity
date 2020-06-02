# ビジュアル プロファイラー (Visual Profiler) を使用する

ビジュアル プロファイラー (Visual Profiler) は、簡単に使える、Mixed Reality アプリケーションのパフォーマンスをアプリケーション内で表示する機能を提供します。プロファイラーは、以下の Mixed Reality Toolkit プラットフォームでサポートされています：

- Microsoft HoloLens (第一世代)
- Microsoft HoloLens 2
- Windows Mixed Reality 没入型ヘッドセット
- OpenVR

アプリケーションの開発中は、ビジュアル プロファイラーが現在シーンと関連のあるデータを表示するため、シーンの複数の部分に注目してください。

> [!IMPORTANT]
> シーン内にある複雑なオブジェクト、パーティクル エフェクト　(particle effects) 、またはアクティビティなどにご注意ください。これらの要因か、その他の要因は、多くの場合、アプリケーションのパフォーマンスとユーザー エクスペリエンスを低下させてしまいます。

## Visual Profiler Interface (ビジュアル プロファイラー インターフェイス)

![ビジュアル プロファイラー インターフェイス](../../Documentation/Images/Diagnostics/VisualProfiler.png)

ビジュアル プロファイラー インターフェイスには、次のコンポーネントが含まれます。

- [フレーム レート](#frame-rate-フレーム-レート)
- [フレーム時間](#frame-time-フレーム時間)
- [フレーム グラフ](#frame-graph-フレーム-グラフ)
- [メモリ使用率](#メモリ使用率)

### Frame Rate (フレーム レート)

インターフェイスの左上隅には、1 秒あたりのフレーム レートで測定されるフレーム レートがあります。最高のユーザー エクスペリエンスと快適性を実現するために、この値は可能な限り高くする必要があります。

特定のプラットフォームとハードウェア構成は、達成可能なフレーム レートの最大値で重要な役割を果たします。一般的にターゲット値の例は次の通りです：

- Microsoft HoloLens: 60
- Windows Mixed Reality Ultra: 90

> [!NOTE]
> [デフォルトの MRC が有効な場合フレーム レートが制限される](https://docs.microsoft.com/windows/mixed-reality/mixed-reality-capture-for-developers#what-to-expect-when-mrc-is-enabled-on-hololens)ため、ビデオや写真が撮影されている間ビジュアル プロファイラーは非表示になります。この設定は診断システムのプロファイルで上書きすることができます。

### Frame Time (フレーム時間)

フレーム レートの右側は、CPU に費やされるフレーム時間 (ミリ秒単位)です。前述の目標フレーム レートを実現するために、アプリケーションがフレームごとに費やすことのできる時間は以下の通りです。

- 60 fps: 16.6 ms
- 90 fps: 11.1 ms

GPU 時間は、これからのリリースで追加される予定です。

### Frame Graph (フレーム グラフ)

フレーム グラフは、アプリケーション フレーム レート履歴のグラフを提供します。

![ビジュアル プロファイラー フレーム グラフ](../../Documentation/Images/Diagnostics/VisualProfilerMissedFrames.png)

アプリケーションを使用する場合は、欠落したフレームを探します。フレームの欠落は、アプリケーションがターゲット フレーム レートに達しておらず、最適化の作業が必要かもしれないことを示しています。

### メモリ使用率

メモリ使用率表示を使用すると、現在のビューがアプリケーションのメモリ消費に与える影響を容易に理解できます。

![ビジュアル プロファイラーフレーム グラフ](../../Documentation/Images/Diagnostics/VisualProfilerMemory.png)

アプリケーションを使用する場合は、メモリの合計使用量を探します。主な指標に、メモリ使用制限の近さと使用状況の急激な変化を含んでいます。

## Visual Profiler (ビジュアル プロファイラー) のカスタマイズ

ビジュアル プロファイラーの見た目と動きは、診断システム プロファイルを通じてカスタマイズできます。詳細については、[診断システム (Diagnostic System) の構成](ConfiguringDiagnostics.md) を参照してください。

## 関連項目

- [診断システム (Diagnostic System)](DiagnosticsSystemGettingStarted.md)
- [診断システム (Diagnostic System) の構成](ConfiguringDiagnostics.md)
