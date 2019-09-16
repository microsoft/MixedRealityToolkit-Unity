# 診断システム (Diagnostics System) の構成

診断システム プロファイルには、次の構成オプションが提供されます。
- [General Settings （一般的な設定）](#general-settings-一般的な設定)
- [Profiler Settings （プロファイラーの設定）](#profiler-settings-プロファイラーの設定)

## General Settings （一般的な設定）

![診断の一般的な設定](../../Documentation/Images/Diagnostics/DiagnosticsGeneralSettings.png)

### Show Diagnostics （診断の表示）

診断システムが設定された診断オプションを表示するかどうかを示します。

無効にすると、設定されたすべての診断オプションが非表示になります。

#### プログラムによる診断システムの制御

また、実行時に診断システムとプロファイラーの可視性を切り替えることもできます。
たとえば、次のコードでは、診断システムとプロファイラーを非表示にします。

```C#
if (MixedRealityServiceRegistry.TryGetService<IMixedRealityDiagnosticsSystem>(out var service))
{
    service.ShowDiagnostics = false;
    service.ShowProfiler = false;
}
```

## Profiler Settings （プロファイラーの設定）

![診断プロファイラーの設定](../../Documentation/Images/Diagnostics/DiagnosticsProfilerSettings.png)

### Show Profiler （プロファイラーの表示）

ビジュアル プロファイラー (Visual Profiler) を表示するかどうかを示します。

### Frame Sample Rate （フレーム サンプル レート）

フレームレート計算用にフレームを収集する時間 (秒単位) 。範囲は 0 ~ 5 秒です。

### Window Anchor （ウィンドウ アンカー）

プロファイラー ウィンドウをビュー ポート (view port) のどの部分に固定するかを設定します。デフォルト値は下中央です。

### Window Offset （ウィンドウ オフセット）

ビュー ポート (view port) の中心からビジュアル プロファイラーを配置するまでのオフセット。オフセットは [Window Anchor （ウィンドウ アンカー）](#window-anchor-ウィンドウ-アンカー)の方向です。

### Window Scale （ウィンドウ スケール）

プロファイラー ウィンドウ (profiler window) に適用されるサイズ乗数。たとえば、値を 2 に設定すると、ウィンドウのサイズが 2 倍になります。

### Window Follow Speed （ウィンドウ追従スピード）

ビュー ポート (view port) 内の可視性を維持するためにプロファイラー ウィンドウを移動する速度。

## 参考に

- [診断システム (Diagnostic System)](DiagnosticsSystemGettingStarted.md)
- [ビジュアル プロファイラー (Visual Profiler) を使用する](UsingVisualProfiler.md)
