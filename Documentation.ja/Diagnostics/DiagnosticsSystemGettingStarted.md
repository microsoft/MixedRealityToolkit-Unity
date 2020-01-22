# 診断システム (Diagnostic System)

Mixed Reality Toolkit 診断システムは、アプリケーションの問題の分析を可能にするツールです。

診断システムの最初のリリースには、アプリケーションの使用中にパフォーマンスの問題を分析するための[ビジュアル プロファイラー (Visual Profiler)](UsingVisualProfiler.md) が含まれています。

## はじめに

> [!IMPORTANT]
> 診断システムは、製品開発サイクル全体を通じて有効にし、最終バージョンをビルドしてリリースする前の最後の変更として無効にすることを **_強く_** お勧めします。

診断システムを使い始めるには、2つのキー ステップがあります。

1. [診断を有効にする](#診断を有効にする)
2. [診断のオプションを設定する](#診断のオプションを設定する)

### 診断を有効にする

診断システムは、MixedRealityToolkit オブジェクト (または別の[サービス登録 (service registar)](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar)) コンポーネントによって管理されます。

次の手順では、MixedRealityToolkit オブジェクトの使用を想定しています。他のサービス レジストラーに必要な手順は異なる場合があります。

1. シーンの \[Hierarchy](ヒエラルキー) ウィンドウで MixedRealityToolkit オブジェクトを選択します。

    ![シーン ヒエラルキーでMRTKの設定](../../Documentation/Images/MRTK_ConfiguredHierarchy.png)

1. \[Inspector](インスペクター) ウィンドウで「Diagnostics System」セクションに移動し、「Enable」にチェックを入れます。

    ![診断を有効にする](../../Documentation/Images/Diagnostics/MRTKConfig_Diagnostics.png)

1. 診断システムの実行を選択する

    ![診断システムの実装を選択する](../../Documentation/Images/Diagnostics/DiagnosticsSelectSystemType.png)

> [!NOTE]
> デフォルトのプロファイル [DefaultMixedRealityToolkitConfigurationProfile](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/DefaultMixedRealityToolkitConfigurationProfile.asset) を使うユーザーは、[`MixedRealityDiagnosticsSystem`](xref:Microsoft.MixedReality.Toolkit.Diagnostics.MixedRealityDiagnosticsSystem) オブジェクトを使う事前に設定された診断システムを持ちます。

### 診断のオプションを設定する

診断システムは、設定プロファイルを使用して、表示するコンポーネントを指定し、それらの設定を行います。使用可能なコンポーネント設定の詳細については、[診断システムの設定](../../Documentation/Diagnostics/ConfiguringDiagnostics.md)を参照してください。

![診断の設定オプション](../../Documentation/Images/Diagnostics/DiagnosticsProfile.png)

> [!IMPORTANT]
> アプリケーションを開発する際、ビルドとデプロイの手順が不要な Unity の Play モードを利用することも可能ですが、ターゲット ハードウェアとプラットフォーム上で実行されているコンパイル済みアプリケーションを使用して診断システムの結果を評価することが重要です。
> [ビジュアル プロファイラー (Visual Profiler)](UsingVisualProfiler.md) などのパフォーマンス診断では、エディター内から実行されたときに実際のアプリケーション のパフォーマンスが正確に反映されない場合があります。

## 関連項目

- [診断 API ドキュメンテーション](xref:Microsoft.MixedReality.Toolkit.Diagnostics)
- [診断システムの設定](ConfiguringDiagnostics.md)
- [ビジュアル プロファイラー (Visual Profiler) を使用する](UsingVisualProfiler.md)
