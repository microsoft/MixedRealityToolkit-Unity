# Profiles

MRTK を設定する主な方法の一つは、foundation パッケージに含まれる多数の Profile を使用することです。
シーン内のメインとなる [`MixedRealityToolkit`](xref:Microsoft.MixedReality.Toolkit.MixedRealityToolkit) オブジェクトはアクティブな Profile を持っており、これは実装としては ScriptableObject です。トップ レベルの MRTK Configuration Profile は主要なコア システムのそれぞれに対しサブプロファイル データを含んでおり、それらは対応するサブ システムの振る舞いを設定するようデザインされています。さらに、これらのサブプロファイルも ScriptableObject であり、一階層下の他のプロファイルへの参照を含むことができます。本質的に、MRTK のサブシステムと機能をどう初期化するかの設定情報を作り上げる、結合されたプロファイルのツリーがあります。

例えば入力機能の振る舞いは [input system プロファイル オブジェクト](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/DefaultMixedRealityInputSystemProfile.asset)で管理されています。Profile の ScriptableObject を編集するにはエディターの \[Inspector](インスペクター) ウィンドウを常に使用することを強くお勧めします。

<img src="../../Documentation/Images/Profiles/input_profile.png" width="650px" style="display:block;"><br/>
<sup>Profile Inspector</sup>

> [!NOTE]
> 今後、実行時に Profile が変更可能になる予定ですが、[現状は対応していません](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/4289)。

## Default Profile

MRTK は様々なプラットフォームとシナリオをサポートするデフォルトの Profile を提供します。例えば、 [DefaultMixedRealityToolkitConfigurationProfile](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/DefaultMixedRealityToolkitConfigurationProfile.asset)
を選択すれば VR (OpenVR, WMR) と HoloLens(1, 2) 両方に対応したシナリオを試すことができます。これは幅広い用途のための Profile なので、特定のユースケースに最適化されてはいないことに注意してください。もし他のプラットフォーム向けに、より高パフォーマンスで特化した設定にしたければ、以下の他のプラットフォーム向けに調整された Profile をご覧ください。

## HoloLens 2 Profile

MRTK は HoloLens2 上での開発・テストに最適化された Profile を提供します。 [DefaultHoloLens2ConfigurationProfile](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.SDK/Profiles/HoloLens2/DefaultHoloLens2ConfigurationProfile.asset).
MixedRealityToolkit オブジェクトに Profile を設定するよう表示されたら、デフォルトではなくこちらを選択してください。

HoloLens2 Profile とデフォルト Profile の主な違いは以下の通りです。

**無効化されている** 機能:

- [Boundary System](../Boundary/BoundarySystemGettingStarted.md)
- [Teleport System](../TeleportSystem/Overview.md)
- [Spatial Awareness System](../SpatialAwareness/SpatialAwarenessGettingStarted.md)
- [Hand mesh visualization](../Input/HandTracking.md) (パフォーマンスのオーバーヘッドのため)

**有効化されている** システム:

- [Eye Tracking provider](../EyeTracking/EyeTracking_Main.md)
- Eye input (視線入力) シミュレーション

Camera の Profile はエディター上のクオリティとデバイス上のクオリティが同じになるように設定されています。これは Opaque ディスプレイが高いクオリティになるように設定された、デフォルトの Camera の Profile とは異なります。この変更は、エディター上でのクオリティは下がるが、実際のデバイスに描画される結果により近いものになるということを意味します。


> [!NOTE]
> Spatial Awareness システムは、クライアントからのフィードバックによりデフォルトではオフになっています。最初は視覚的に興味深いのですが、見た目の紛らわしさやパフォーマンス上の理由から通常はオフにされています。
  Spatial Awareness システムは、[こちらの手順](../SpatialAwareness/SpatialAwarenessGettingStarted.md)に従うことで、有効にすることができます。