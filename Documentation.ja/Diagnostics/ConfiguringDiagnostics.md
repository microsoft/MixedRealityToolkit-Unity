# 診断システムの構成

診断システムプロファイルには、次の構成オプションが提供されます。
- [一般的な設定](#一般的な設定)
- [プロファイラーの設定](#プロファイラーの設定)

## 一般的な設定

![診断の一般的な設定](../../Documentation/Images/Diagnostics/DiagnosticsGeneralSettings.png)

### 診断を表示

診断システムに設定された診断オプションを表示するかどうかを示します。

無効にすると、設定されたすべての診断オプションが非表示になります。

#### プログラムによる診断システムの制御

また、実行時に診断システムとプロファイラーの可視性を切り替えることもできます。
たとえば、次のコードでは、診断システムとプロファイラを非表示にします。

```C#
if (MixedRealityServiceRegistry.TryGetService<IMixedRealityDiagnosticsSystem>(out var service))
{
    service.ShowDiagnostics = false;
    service.ShowProfiler = false;
}
```

## プロファイラーの設定

![診断プロファイラの設定](../../Documentation/Images/Diagnostics/DiagnosticsProfilerSettings.png)

### プロファイラーの表示

ビジュアルプロファイラ (Visual Profiler) を表示するかどうかを示します。

### フレームサンプルレート (Frame Sample Rate)

フレームレート計算用にフレームを収集する時間 (秒単位) 。範囲は 0 ~ 5 秒です。

### ウィンドウアンカー (Window Anchor)

プロファイラウィンドウを固定するビューポート (view port) のどの部分に固定するかを設定します。デフォルト値は[下中央]です。

### ウィンドウオフセット (Window Offset)

ビューポート (view port) の中心からビジュアルプロファイラを配置するまでのオフセット。オフセットは[ウィンドウアンカー (Window Anchor)](#ウィンドウアンカー (Window Anchor))の方向に向けます。

### ウィンドウスケール (Window Scale)

プロファイラーウィンドウ (profiler window) に適用されるサイズ乗数。たとえば、値を 2 に設定すると、ウィンドウのサイズが 2 倍になります。

### ウィンドウ追従スピード

ビューポート (view port) 内の可視性を維持するためにプロファイラウィンドウを移動する速度。

## 参照に

- [診断システム (Diagnostic System)](DiagnosticsSystemGettingStarted.md)
- [ビジュアルプロファイラー (Visual Profiler) を使用する](UsingVisualProfiler.md)
