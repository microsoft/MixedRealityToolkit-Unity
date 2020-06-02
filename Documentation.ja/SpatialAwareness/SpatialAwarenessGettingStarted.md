# Spatial Awareness (空間認識)

![Spatial Awareness](../../Documentation/Images/SpatialAwareness/MRTK_SpatialAwareness_Main.png)

空間認識システムは、複合現実アプリケーションで現実世界の環境認識を提供します。 Microsoft HoloLens で導入されたとき、空間認識は環境のジオメトリを表すメッシュのコレクションを提供し、ホログラムと現実世界の間の魅力的なインタラクションを可能にしました。

> [!NOTE]
> 現時点では、Mixed Reality Toolkit には、HoloToolkit に元々パッケージされていた Spatial Understanding (空間理解) アルゴリズムは付属していません。 通常、Spatial Understanding では、空間メッシュデータを変換して、平面、壁、床、天井などの単純化またはグループ化されたメッシュデータを作成します。

## はじめに

空間認識のサポートを追加するには、Mixed Reality Toolkit の 2 つの主要なコンポーネント (空間認識システムとサポートされているプラットフォーム プロバイダー) が必要です。

1. 空間認識システムを[有効](#空間認識システムを有効にする)にする
2. メッシュデータを提供するために 1 つ以上の空間オブザーバーを[登録](#オブザーバーの登録)して[設定](ConfiguringSpatialAwarenessMeshObserver.md)する
3. 空間認識をサポートするプラットフォームに[ビルドしてデプロイ](#ビルドとデプロイ)する

### 空間認識システムを有効にする

空間認識システムは、MixedRealityToolkit オブジェクト (または別の [サービス レジストラ](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar) コンポーネント) によって管理されます。以下の手順に従って、*MixedRealityToolkit* プロファイルで *Spatial Awareness システム* を有効または無効にします。

Mixed Reality Toolkit には、いくつかのデフォルトの事前設定プロファイルが付属しています。これらの一部では、空間認識システムがデフォルトで有効または無効になっています。この事前設定の目的は、特に無効になっている場合、メッシュの計算とレンダリングの視覚的なオーバーヘッドを回避することです。

| プロファイル | デフォルトの有効/無効設定 |
| --- | --- |
| `DefaultHoloLens1ConfigurationProfile` (Assets/MRTK/SDK/Profiles/HoloLens1) | False |
| `DefaultHoloLens2ConfigurationProfile` (Assets/MRTK/SDK/Profiles/HoloLens2) | False |
| `DefaultMixedRealityToolkitConfigurationProfile` (Assets/MRTK/SDK/Profiles) | True |

1. [Inspector] (インスペクター) パネルを開くために [Hierarchy] (ヒエラルキー) から MixedRealityToolkit オブジェクトを選択する

    ![MRTK Configured Scene Hierarchy](../../Documentation/Images/MRTK_ConfiguredHierarchy.png)

1. *Spatial Awareness System* セクションに移動し、*Enable Spatial Awareness System* にチェックを入れる

    ![Enable Spatial Awareness](../../Documentation/Images/SpatialAwareness/MRTKConfig_SpatialAwareness.png)

1. 目的の空間認識システムの実装タイプを選択します。 [`MixedRealitySpatialAwarenessSystem`](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness.MixedRealitySpatialAwarenessSystem) がデフォルトで提供されます。

    ![Select the Spatial Awareness System Implementation](../../Documentation/Images/SpatialAwareness/SpatialAwarenessSelectSystemType.png)

### オブザーバーの登録

Mixed Reality Toolkit のサービスには、プラットフォーム固有のデータと実装制御でメイン サービスを補完する[データ プロバイダー サービス](../Architecture/SystemsExtensionsProviders.md)を含めることができます。 例として、さまざまなプラットフォーム固有の API からコントローラーおよびその他の関連入力情報を取得する[複数のデータ プロバイダー](../Input/InputProviders.md)を持つ Mixed Reality Input System があります。

空間認識システムは、データ プロバイダーが現実世界に関するメッシュデータをシステムに提供するという点で類似しています。Spatial Awareness プロファイルには、少なくとも 1 つの Spatial Observer (空間オブザーバー) が登録されている必要があります。空間オブザーバーは通常、プラットフォーム固有のエンドポイント（つまり、HoloLens）からさまざまな種類のメッシュデータを提供するためのプロバイダーとして機能するプラットフォーム固有のコンポーネントです。

1. *Spatial Awareness System* プロファイルを開く

    ![Spatial Awareness System Profile](../../Documentation/Images/SpatialAwareness/SpatialAwarenessProfile.png)

1. *Add Spatial Observer* ボタンをクリックする
1. *Spatial Observer の実装タイプ* を選択する

    ![Select the Spatial Observer Implementation](../../Documentation/Images/SpatialAwareness/SpatialAwarenessSelectObserver.png)

1. 必要に応じて[オブザーバーの構成プロパティを変更](ConfiguringSpatialAwarenessMeshObserver.md)する

> [!NOTE]
> `DefaultMixedRealityToolkitConfigurationProfile` (Assets/MRTK/SDK/Profiles) のユーザーは、[`WindowsMixedRealitySpatialMeshObserver`](xref:Microsoft.MixedReality.Toolkit.WindowsMixedReality.SpatialAwareness.WindowsMixedRealitySpatialMeshObserver) クラスを使用する Windows Mixed Reality プラットフォーム用に事前設定された Spatial Awareness システムを使用します

### ビルドとデプロイ

空間認識システムを目的のオブザーバーで構成すると、プロジェクトをビルドしてターゲット プラットフォームにデプロイできます。

> [!IMPORTANT]
> Windows Mixed Reality プラットフォーム（例：HoloLens）を対象とする場合、デバイスで Spatial Awareness システムを使用するには、[Spatial Perception capability](https://docs.microsoft.com/windows/mixed-reality/spatial-mapping-in-unity) が有効になっていることを確認することが重要です。

> [!WARNING]
> Microsoft HoloLens を含む一部のプラットフォームでは、Unity 内からリモート実行をサポートしています。この機能により、ビルドとデプロイの手順を必要とせずに、迅速な開発とテストが可能になります。ターゲット ハードウェアとプラットフォームで実行される、ビルドおよびデプロイされたバージョンのアプリケーションを使用して、最終的な受け入れテストを行うようにしてください。

## 次のステップ

上記手順に従い Spatial Awareness システムを有効にしたら、より詳細に設定やコントロールをすることができます。

インスペクターでのオブザーバーの設定に関する情報:

- [Configuring Observers for on device usage](ConfiguringSpatialAwarenessMeshObserver.md)
- [Configuring Observers for in-editor usage](SpatialObjectMeshObserver.md)

コードでのオブザーバーのコントロールと拡張に関する情報:

- [Configuring Observers via Code](UsageGuide.md)
- [Creating a custom Observer](CreateDataProvider.md)

## 関連項目

- [Spatial Awareness API documentation](xref:Microsoft.MixedReality.Toolkit.SpatialAwareness)
- [Spatial Mapping Overview WMR](https://docs.microsoft.com/windows/mixed-reality/spatial-mapping)
- [Spatial Mapping in Unity WMR](https://docs.microsoft.com/windows/mixed-reality/spatial-mapping-in-unity)
