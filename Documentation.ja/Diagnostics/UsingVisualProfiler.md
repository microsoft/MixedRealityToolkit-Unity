# ビジュアルプロファイラ (Visual Profiler) を使用する

ビジュアルプロファイラ (Visual Profiler) は、Mixed Reality アプリケーションのパフォーマンスを観察できる、便利な機能を提供します。プロファイラーは、以下の Mixed Reality Toolkit プラットフォームでサポートされています：

- Microsoft HoloLens
- Microsoft HoloLens 2
- Windows Mixed Reality 没入型ヘッドセット
- OpenVR

アプリケーションの開発中は、ビジュアルプロファイラが現在シーンと関連のあるデータを表示するので、シーンの複数の部分に注目してください。

> [!IMPORTANT]
> シーン内にある複雑なオブジェクト、パーティクルエフェクト　(particle effects) 、またはアクティビティなどにご注意ください。これらの要因か、その他の要因は、多くの場合、アプリケーションのパフォーマンスとユーザーエクスペリエンスを低下させてしまいます。

## Visual Profiler Interface (ビジュアルプロファイラインターフェイス)

![ビジュアルプロファイラ インターフェイス](../../Documentation/Images/Diagnostics/VisualProfiler.png)

ビジュアルプロファイラインターフェイスには、次のコンポーネントが含まれます。

- [フレームレート](#frame-rate-フレームレート)
- [フレーム時間](#frame-time-フレーム時間)
- [フレームグラフ](#frame-graph-フレームグラフ)
- [メモリ使用率](#メモリ使用率)

### Frame Rate (フレームレート)

インターフェイスの左上隅には、1 秒あたりのフレームレートで測定されるフレームレートがあります。最高のユーザーエクスペリエンスと快適性を実現するために、この値は可能な限り高くする必要があります。

特定のプラットフォームとハードウェア構成は、達成可能なフレームレートの最大値で重要な役割を果たします。一般的にターゲット値の例は次の通りです：

- Microsoft HoloLens: 60
- Windows Mixed Reality Ultra: 90

### Frame Time (フレーム時間)

フレームレートの右側は、CPU に費やされるフレーム時間 (ミリ秒単位)です。前述の目標フレームレートを実現するために、アプリケーションがフレームごとに費やすことのできる時間は以下の通りです。

- 60 fps: 16.6 ms
- 90 fps: 11.1 ms

GPU 時間は、これからのリリースで追加される予定です。

### Frame Graph (フレームグラフ)

フレームグラフは、アプリケーションフレームレート履歴のグラフを提供します。

![ビジュアルプロファイラフレーム グラフ](../../Documentation/Images/Diagnostics/VisualProfilerMissedFrames.png)

アプリケーションを使用する場合は、欠落したフレームを探します。フレームの欠落は、アプリケーションがターゲットフレームレートに達していないと、まだ最適化作業が必要な場合があることを示します。

### メモリ使用率

メモリ使用率表示を使用すると、現在のビューがアプリケーションのメモリ消費に与える影響を容易に理解できます。

![ビジュアルプロファイラフレーム グラフ](../../Documentation/Images/Diagnostics/VisualProfilerMemory.png)

アプリケーションを使用する場合は、メモリの合計使用量を探します。主な指標に、メモリ使用制限の近さと使用状況の急激な変化を含んでいます。

## Visual Profiler (ビジュアルプロファイラ) のカスタマイズ

ビジュアルプロファイラの見た目と動きは、診断システムプロファイルを通じてカスタマイズできます。詳細については、「診断システム (Diagnostic System) の構成」(ConfiguringDiagnostics.md) を参照してください。

## 参考

- [診断システム (Diagnostic System)](DiagnosticsSystemGettingStarted.md)
- [診断システム (Diagnostic System) の構成](ConfiguringDiagnostics.md)
