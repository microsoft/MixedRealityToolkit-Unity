# ビジュアルプロファイラ (Visual Profiler) を使用する

ビジュアルプロファイラ (Visual Profiler) は、Mixed Reality アプリケーションのパフォーマンスを観察できる、便利な機能を提供します。プロファイラーは、以下の Mixed Reality Toolkit プラットフォームでサポートされています：

- Microsoft HoloLens
- Microsoft HoloLens 2
- Windows Mixed Reality 没入型ヘッドセット
- OpenVR

アプリケーションの開発中は、ビジュアルプロファイラが現在シーンと関係性のあるデータを表示するので、シーンの複数の部分に注目してください。

> [!IMPORTANT]
> シーン内にある複雑なオブジェクト、パーティクルエフェクト　(particle effects) 、またはアクティビティなどにご注意ください。これらの要因か、その他の要因は、多くの場合、アプリケーションのパフォーマンスとユーザーエクスペリエンスを低下させてしまいます。

## Visual Profiler Interface (ビジュアルプロファイラインターフェイス)

![ビジュアル プロファイラ インターフェイス](../../Documentation/Images/Diagnostics/VisualProfiler.png)

ビジュアルプロファイラインターフェイスには、次のコンポーネントが含まれます。

- [フレームレート](#Frame Rate (フレームレート))
- [フレーム時間](#Frame Time (フレーム時間))
- [フレームグラフ](#Frame Graph (フレームグラフ))
- [メモリ使用率](#メモリ使用率)

### Frame Rate (フレームレート)

インターフェイスの左上隅には、1 秒あたりのフレームレートで測定されるフレームレートがあります。最高のユーザーエクスペリエンスと快適性を実現するために、この値は可能な限り高くする必要があります。

特定のプラットフォームとハードウェア構成は、達成可能なフレームレートの最大値で重要な役割を果たします。一般的にターゲット値の例は以下です：

- Microsoft HoloLens: 60
- Windows Mixed Reality Ultra: 90

### Frame Time (フレーム時間)

フレームレートの右側には、CPU に費やされるフレーム時間 (ミリ秒単位)です。前述の目標フレームレートを実現するために、アプリケーションはフレームごとに次の時間を費やすことができます。

- 60 fps: 16.6 ms
- 90 fps: 11.1 ms

GPU 時間は、これからのリリースで追加される予定です。

### Frame Graph (フレームグラフ)

フレームグラフは、アプリケーションフレームレート履歴のグラフを提供します。

![ビジュアルプロファイラフレームグラフ](../../Documentation/Images/Diagnostics/VisualProfilerMissedFrames.png)

アプリケーションを使用する場合は、欠落したフレームを探します。フレームの欠落は、アプリケーションがターゲットフレームレートに達していないと、まだ最適化作業が必要な場合があることを示します。

### メモリ使用率

メモリ使用率表示を使用すると、現在のビューがアプリケーションのメモリ消費に与える影響を簡易に理解できます。

![ビジュアルプロファイラフレームグラフ](../../Documentation/Images/Diagnostics/VisualProfilerMemory.png)

アプリケーションを使用する場合は、メモリの合計使用量を探します。主な指標は：メモリ制限の近い、使用状況の急激な変化です。

## ビジュアルプロファイラ (Visual Profiler) のカスタマイズ

ビジュアルプロファイラの見た目と動きは、診断システムプロファイルを通じてカスタマイズできます。詳細については、「診断システム (Diagnostic System) の構成」(ConfiguringDiagnostics.md) を参照してください。

## 参考に

- [診断システム (Diagnostic System)](DiagnosticsSystemGettingStarted.md)
- [診断システム (Diagnostic System) の構成](ConfiguringDiagnostics.md)
