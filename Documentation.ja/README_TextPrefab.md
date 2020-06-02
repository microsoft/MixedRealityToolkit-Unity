# Text Prefab (テキスト プレハブ)

これらのプレハブは、Windows Mixed Reality のレンダリング品質向けに最適化されています。詳細については、Microsoft Windows Dev Center のガイドライン [Text in Unity](https://docs.microsoft.com/windows/mixed-reality/text-in-unity) を参照してください。

## Prefab (プレハブ)

### 3DTextPrefab

2 メートルの距離で最適化されたスケーリング係数を持つ 3D Text Mesh プレハブ (Assets/MRTK/SDK/StandardAssets/Prefabs/Text) (以下の手順をお読みください)。

### UITextPrefab

2 メートルの距離で最適化されたスケーリング係数を持つ UI Text Mesh プレハブ (Assets/MRTK/SDK/StandardAssets/Prefabs/Text) (以下の手順をお読みください)。

### Fonts

Mixed Reality Toolkit に含まれるオープンソース フォント (Assets/MRTK/Core/StandardAssets/Fonts)。

> [!IMPORTANT]
> テキスト プレハブはオープン ソース フォント 'Selawik' を使用しています。別のフォントでテキスト プレハブを使用するには、フォント ファイルをインポートして、以下の手順に従ってください。以下の例は、Text Prefab で 'Segoe UI' フォントを使用する方法を示しています。

![Importing Segoe UI font file](../Documentation/Images/TextPrefab/TextPrefabInstructions01.png)

1. フォント テクスチャを 3DTextSegoeUI.mat マテリアルに割り当てます。

    ![Assigning font texture](../Documentation/Images/TextPrefab/TextPrefabInstructions02.png)

1. 3DTextSegoeUI.mat マテリアルで、Custom/3DTextShader.shader シェーダーを選択します。

    ![Assigning shader](../Documentation/Images/TextPrefab/TextPrefabInstructions03.png)

1. Segoe UI フォントと 3DTextSegoeUI マテリアルをプレハブのテキスト コンポーネントに割り当てます。

    ![Assigning font file and material](../Documentation/Images/TextPrefab/TextPrefabInstructions04.png)

### Unity でフォントを扱う

Unity のシーンに新しい 3D TextMesh を追加するとき、視覚的に明らかな 2 つの問題があります。 1 つ目は、フォントが非常に大きく表示され、2 つ目は、フォントが非常にぼやけて表示されます。また、インスペクターでデフォルトのフォント サイズの値がゼロに設定されていることにも注目してください。このゼロ値を 13 に置き換えても、実際には 13 がデフォルト値であるため、サイズに違いはありません。

Unity は、シーンに追加されるすべての新しい要素のサイズが 1 Unity 単位、または 100% の Transform スケール (これは HoloLens で約 1 メートルに変換される) だと仮定します。フォントの場合、3D TextMesh のバウンディング ボックスが、デフォルトで高さが約 1 メートルとなります。

### フォント スケールとフォント サイズ

ほとんどのビジュアル デザイナー、そしてデザイン プログラムは、Points を使用して現実世界のフォント サイズを定義します。1 メートルは約 2835 (2,834.645666399962) ポイントになります。ポイント システムの 1 メートルへの変換と Unity のデフォルトの TextMesh フォント サイズ 13 に基づいて、13 を 2835 で割った単純計算の 0.0046 (正確には 0.004586111116) という値は、初めの標準スケールとして良い値となります。ただし、0.005 に丸めたい人もいるかもしれません。

いずれにしても、Text オブジェクトまたはコンテナーをこれらの値にスケールすると、デザイン プログラムからフォント サイズの 1：1 変換が可能になるだけでなく、アプリケーションまたはゲーム全体で一貫性を維持するための標準も提供されます。

### UI Text

UI またはキャンバス ベースの Text 要素をシーンに追加すると、サイズの不均衡はさらに大きくなります。 2つのサイズの違いは約1000％で、UI ベースのテキスト コンポーネントのスケール係数は 0.00046 (正確には 0.0004586111116) または丸められた値の場合は 0.0005 になります。

**免責事項**: 任意のフォントのデフォルト値は、そのフォントのテクスチャ サイズまたはフォントが Unity にインポートされた方法によって影響を受ける場合があります。これらのテストは、Unity のデフォルトの Arial フォントと、インポートされた他の 1 つのフォントに基づいて実行されました。

![Font size with scaling factors](../Documentation/Images/TextPrefab/TextPrefabInstructions07.png)

### [Text3DSelawik.mat](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/mrtk_development/Assets/MRTK/Core/StandardAssets/Materials)

オクルージョンをサポートする 3DTextPrefab のマテリアル。3DTextShader.shader が必要です。

![Default Font material vs 3DTextSegoeUI material](../Documentation/Images/TextPrefab/TextPrefabInstructions06.png)

### [Text3DShader.shader](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/mrtk_development/Assets/MRTK/Core/StandardAssets/Shaders)

オクルージョンをサポートする 3DTextPrefab 用シェーダー。
