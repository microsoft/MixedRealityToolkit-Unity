# Hologram stabilization (ホログラムの安定化)

## パフォーマンス

基礎となる Mixed Reality プラットフォームとデバイスが最良の結果を生み出すためには、フレーム レートを達成することが重要です。ターゲットのフレーム レート (例： 60FPS または 90FPS) は、プラットフォームやデバイスによって異なります。しかし、フレーム レートを満たす Mixed Reality アプリケーションは、安定したホログラムだけではなく、効率的なヘッド トラッキング、ハンド トラッキングなどを持つことになります。

## 環境トラッキング

安定したホログラフィック レンダリングは、プラットフォームとデバイスによるヘッドポーズ トラッキングに大きく依存します。Unity は、基礎となるプラットフォームによって推定および提供されるカメラ ポーズから、シーンをフレームごとにレンダリングします。このトラッキングが実際のヘッドの動きに正確に追従しない場合、ホログラムは視覚的に不正確に見えます。HoloLens のような AR デバイスでは、ユーザーがバーチャル ホログラムを現実世界に関連付けることができるため、これは特に明白で重要です。パフォーマンスは信頼性の高いヘッド トラッキングに重要ですが、[そのほかにも重要な特徴](https://docs.microsoft.com/ja-jp/windows/mixed-reality/environment-considerations-for-hololens) があります。ユーザ エクスペリエンスに影響を与える環境要素のタイプは、対象となるプラットフォームの仕様によって異なります。

## Windows Mixed Reality

Windows Mixed Reality プラットフォームは、プラットフォーム上でホログラムを安定させるための[参考資料](https://docs.microsoft.com/ja-jp/windows/mixed-reality/hologram-stability)を提供しています。開発者がホログラムの視覚体験を改善するために利用できる主要なツールはいくつかあります。

### Depth Buffer Sharing (デプス バッファーの共有)

Unity の開発者はアプリケーションのデプス バッファーをプラットフォームと共有することができます。これは、現在のフレームに対してホログラムが存在する場合に、プラットフォームが Late-Stage Reprojection として知られるハードウェアで支援されたプロセスを介してホログラムを安定化するために利用できる情報を提供します。

#### Late-Stage Reprojection

フレームのレンダリングが終了すると、Windows Mixed Reality プラットフォームは、アプリケーションによって生成されたカラーとデプスのレンダー ターゲットを使用し、最後のヘッド ポーズ予測以降のわずかなヘッドの動きを考慮するために最終的な画面出力を変換します。アプリケーションのゲーム ループの実行には時間がかかります。たとえば、60 FPS の場合、これはアプリケーションがフレームをレンダリングするのに 16.667 ms 以下の時間がかかることを意味します。これは非常に短い時間のように思えるかもしれませんが、ユーザーの頭の位置と向きが変わり、結果的にレンダリング時のカメラのプロジェクション マトリックスが新しくなります。Late-Stage Reprojection では、この新しいパースペクティブを考慮して最終的な画像のピクセルを変換します。

#### ピクセル単位 vs 安定化平面 LSR

Windows Mixed Reality デバイス上で動作するデバイス エンドポイントと OS のバージョンに応じて、Late-Stage Reprojection アルゴリズムはピクセル単位または [安定化平面]　(https://docs.microsoft.com/ja-jp/windows/mixed-reality/hologram-stability#stabilization-plane) を介して実行されます。

##### ピクセルごとのデプス ベース

ピクセルごとのデプスに基づいた Reprojection (再投影) は、デプス バッファーを利用してピクセルごとの画像出力を修正し、従って様々な距離でホログラムを安定化することに関与します。例えば、1 m 離れた球が 10 m 離れた柱の前にあるとします。ユーザーが頭を少し傾けた場合、球を表すピクセルは、柱を表す遠くのピクセルとは異なる変換を持ちます。ピクセルごとの再投影では、より正確な再投影のために、各ピクセルでのこの距離差が考慮されます。

##### 安定化平面

プラットフォームと共有する正確なデプス バッファーを作成できない場合、別の形式の LSR は安定化平面を使用します。シーン内のすべてのホログラムにはある程度の安定化が適用されますが、目的の平面にあるホログラムには最大のハードウェア安定化が適用されます。平面の点と法線は、[Unity で提供される API の](https://docs.microsoft.com/ja-jp/windows/mixed-reality/focus-point-in-unity)  *HolographicSettings.SetFocusPointForFrame* を介してプラットフォームに提供されます。

#### デプス バッファー フォーマット

HoloLens を開発のターゲットとする場合は、24 ビットよりも 16 ビット デプス バッファー フォーマットを使用することを強くお勧めします。これにより、デプス値の精度は低くなりますが、パフォーマンスが大幅に向上します。精度の低さを補い [z-fighting](https://en.wikipedia.org/wiki/Z-fighting) を回避するには、[far clip plane](https://docs.unity3d.com/Manual/class-Camera.html) の値を Unity で設定されているデフォルト値の 1000m から減らすことをお勧めします。 

> [!NOTE]
> もし *16-bit depth format* を使う場合、[Unity はステンシル バッファーを作成しない](https://docs.unity3d.com/ScriptReference/RenderTexture-depth.html)ため、ステンシル バッファーが必要なエフェクトは動作しません。逆に、*24-bit depth format* を選択した場合、グラフィックのプラットフォームで適用できる場合は、一般的に [8 ビットのステンシル バッファー](https://docs.unity3d.com/Manual/SL-Stencil.html)が作成されます。

#### Unity での Depth Buffer Sharing (デプス バッファー シェアリング)

デプス ベースの LSR を利用するために、開発者が取る必要のある2つの重要なステップがあります。

1. **Edit** > **Project Settings** > **Player** > **XR Settings** > **Virtual Reality SDKs** 配下の **Depth Buffer Sharing** を有効にします。
    1. HoloLens をターゲットとする場合は、**16-bit depth format** も選択することをお勧めします。
1. 画面上でカラーをレンダリングする場合は、デプスも同様にレンダリングします。

一般に、Unity の [Opaque GameObjects](https://docs.unity3d.com/Manual/StandardShaderMaterialParameterRenderingMode.html) は、デプスに自動的に書き込みます。ただし、透明およびテキスト オブジェクトは、通常、デフォルトではデプスに書き込みません。MRTK Standard Shader、または、Text Mesh Pro を使用している場合は、簡単に修正できます。

> [!NOTE]
> シーン内のどのオブジェクトがデプス バッファーに書き込まないのかを視覚的に素早く決定するには、MRTK Configuration プロファイルの *Editor Settings* 以下にある [*Render Depth Buffer* ユーティリティ](MixedRealityConfigurationGuide.md#editor-utilities)を利用できます。

##### Transparent MRTK Standard Shader

[MRTK Standard shader](README_MRTKStandardShader.md) を使用した透明なマテリアルの場合は、マテリアルを選択して *Inspector* ウィンドウに表示します。次に、*Fix Now* ボタンをクリックして、マテリアルをデプスに書き込むように (すなわち、Z-Write On) に変更します。

変更前

![Depth Buffer Before Fix MRTK Standard Shader](../Documentation/Images/Performance/DepthBufferFixNow_Before.png)

変更後

![Depth Buffer Fixed MRTK Standard Shader](../Documentation/Images/Performance/DepthBufferFixNow_After.png)

##### Text Mesh Pro

Text Mesh Pro オブジェクトの場合は、TMP GameObject を選択してインスペクターに表示します。マテリアル コンポーネントの下で、割り当てられたマテリアルのシェーダーを MRTK TextMeshPro シェーダーを使用するように切り替えます。

![Text Mesh Pro Depth Buffer Fix](../Documentation/Images/Performance/TextMeshPro-DepthBuffer-Fix.PNG)

##### カスタム シェーダー

カスタムシェーダーを記述する場合は、[ZWrite flag](https://docs.unity3d.com/Manual/SL-CullAndDepth.html) を *Pass* ブロック定義の一番上に追加し、シェーダーがデプス バッファーに書き込むように設定します。

```
Shader "Custom/MyShader"
{
    SubShader
    {
        Pass
        {
            ...
            ZWrite On
            ...
        }
    }
}
```

##### Opaque backings (不透明な背面)

上記のメソッドが特定のシナリオ（すなわち、Unity UI 使用時）で機能しない場合、別のオブジェクトにデプス バッファーに書き込ませることができます。一般的な例として、シーン内のフローティング パネルで Unity UI テキストを使用する場合があります。パネルを不透明にするか、少なくともデプスに書き込むことによって、テキストとパネルの両方の Z 値が互いに非常に近くなるため、プラットフォームによってそれらが安定化されます。

### ワールドアンカー (HoloLens)

視覚的な安定性を確保するための正しい設定がなされていることを確実にするとともに、ホログラムが正しい物理的位置で安定することを確実にすることが重要です。物理空間での重要な場所についてプラットフォームに通知するために、開発者はある場所に留まる必要がある GameObjects で  [WorldAnchors](https://docs.unity3d.com/ScriptReference/XR.WSA.WorldAnchor.html) を活用することができます。[WorldAnchors](https://docs.unity3d.com/ScriptReference/XR.WSA.WorldAnchor.html) は、GameObject に追加されるコンポーネントで、オブジェクトの Transform を完全に制御します。

HoloLens のようなデバイスは常に環境をスキャンし、学習しています。そのため、HoloLens が空間での動きと位置を追跡するにつれて、その予測値は更新され、[Unity 座標系は調整されます](https://docs.microsoft.com/en-us/windows/mixed-reality/coordinate-systems-in-unity)。例えば、最初にカメラから 1 m 離れたところに GameObject を配置した場合、HoloLens が環境を追跡するにつれて、GameObject が配置されている物理的な位置が実際には 1.1 m 離れていることに気づくかもしれません。これにより、ホログラムがドリフトします。GameObject にワールドアンカーを適用すると、アンカーがオブジェクトの Transform を制御できるようになり、オブジェクトが正しい物理的位置に留まります (すなわち、実行時に 1 m ではなく 1.1 m 先へと更新されます)。[WorldAnchors](https://docs.unity3d.com/ScriptReference/XR.WSA.WorldAnchor.html) をアプリのセッション間で維持するために、開発者は [WorldAnchorStore](https://docs.unity3d.com/ScriptReference/XR.WSA.Persistence.WorldAnchorStore.html) を使って [WorldAnchors の保存とロード](https://docs.microsoft.com/ja-jp/windows/mixed-reality/persistence-in-unity) を行えます。

> [!NOTE]
> いったん WorldAnchor コンポーネントが GameObject に追加されると、その GameObject の Transform を変更する (すなわち、transform.position = x) ことはできません。Transform を編集するには、WorldAnchor を削除する必要があります。

```csharp
WorldAnchor m_anchor;

public void AddAnchor()
{
    this.m_anchor = this.gameObject.AddComponent<WorldAnchor>();
}

public void RemoveAnchor()
{
    DestroyImmediate(m_anchor);
}
```

## 関連項目

- [Performance](Performance/PerfGettingStarted.md)
- [HoloLens の環境に関する考慮事項](https://docs.microsoft.com/ja-jp/windows/mixed-reality/environment-considerations-for-hololens)
- [Windows Mixed Reality におけるホログラムの安定性](https://docs.microsoft.com/ja-jp/windows/mixed-reality/hologram-stability)
- [Unity のフォーカス ポイント](https://docs.microsoft.com/ja-jp/windows/mixed-reality/focus-point-in-unity)
- [Unity での座標系](https://docs.microsoft.com/ja-jp/windows/mixed-reality/coordinate-systems-in-unity)
- [Unity での永続化](https://docs.microsoft.com/ja-jp/windows/mixed-reality/persistence-in-unity)
- [Mixed Reality のパフォーマンスについて](https://docs.microsoft.com/ja-jp/windows/mixed-reality/understanding-performance-for-mixed-reality)
- [Unity のパフォーマンスに関する推奨事項](https://docs.microsoft.com/ja-jp/windows/mixed-reality/performance-recommendations-for-unity)
