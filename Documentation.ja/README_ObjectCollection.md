# Object collection (オブジェクトコレクション) #

![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Main.jpg)

Object collection はオブジェクトの列を事前定義された三次元形状にレイアウトすることを助けるスクリプトです。これは、平面、円柱、球、放射状を含む、さまざまな面タイプをサポートしています。Unity のすべてのオブジェクトをサポートしているため、2D オブジェクトと 3D オブジェクトの両方のレイアウトに利用可能です。

# Object collection のスクリプト #
- [`GridObjectCollection`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/GridObjectCollection.cs) は、円柱、平面、球、放射状の面タイプをサポートしています。
- [`ScatterObjectCollection`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/ScatterObjectCollection.cs) は、点在するスタイルのコレクションをサポートしています。
- [`TileGridObjectCollection`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/TileGridObjectCollection.cs) は、GridObjectCollection にいくつかの追加オプションを提供しています。 **注意:** TileGridObjectCollection は [`GridObjectCollection`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/GridObjectCollection.cs) を拡張しておらず、いくつかのバグがあります ([issue 6237](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6237) をご覧ください)。そのため、[`GridObjectCollection`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/GridObjectCollection.cs) を使うことが推奨されます。

|![Grid Object Collection - Cylinder](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionCylinder.png) Grid Object Collection - Cylinder | ![Grid Object Collection - Sphere](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionSphere.png) Grid Object Collection - Sphere |
|:--- | :--- |
|![Grid Object Collection - Radial](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionRadial.png) Grid Object Collection - Radial | ![Grid Object Collection - Plane](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionPlane.png) Grid Object Collection - Plane |
|![Scattered Object Collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionScattered.png) Scattered Object Collection | ![Tile Grid Object Collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionTileGrid.png) Tile Grid Object Collection |


## Object collection の使い方 ##

コレクションを作るためには、空のゲームオブジェクトを作り、それに Object collection スクリプトの1つをアサインします。
いかなるオブジェクトでも、このゲームオブジェクトの子供として追加できます。子オブジェクトの追加が終わったら、[Inspector] (インスペクター) パネルで *Update Collection* ボタンをクリックし、Object collection を生成します。オブジェクトはコレクションのパラメーターにしたがって、シーンにレイアウトされます。Update Collection は、コードからもアクセスできます。


![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionScript.png)

## `GridObjectCollection` の中身のアラインメント
GridObjectCollection の中身はアラインすることができ、親オブジェクトはコレクションの top/middle/bottom と left/center/right に固定されます。中身のアラインメントを指定するには **anchor** プロパティを使用してください。

## `GridObjectCollection` のレイアウト順序
子要素がレイアウトされる行、列の順序を指定するには、**Layout** フィールドを使用してください。

**Column Then Row** - 子要素はまず横に (列に) レイアウトされ、それから縦に (行に) レイアウトされます。**Num Columns** (またはコードでは Columns プロパティ) を使って、グリッドの列の数を指定してください。

![Column then row layout](../Documentation/Images/ObjectCollection/MRTK_ColumnThenRow.png)

**Row Then Column** - 子要素はまず縦に (行に) レイアウトされ、それから横に (列に) レイアウトされます。**Num Rows** (またはコードでは Rows プロパティ) を使って、グリッドの行の数を指定してください。

![Row then column layout](../Documentation/Images/ObjectCollection/MRTK_RowThenColumn.png)


**Horizontal** - 子要素は列だけを使い、1つの行にレイアウトされます。

**Vertical** - 子要素は行だけを使い、1つの列にレイアウトされます。

## Object collection のサンプル ##

[ObjectCollectionExamples.unity](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/UX/Collections/Scenes/ObjectCollectionExamples.unity) のサンプル シーンには、Object collection のさまざまなタイプの例が含まれています。

[Periodic table of the elements](https://github.com/Microsoft/MRDesignLabs_Unity_PeriodicTable) は、Object collection がどのように動くかをデモンストレーションするサンプル アプリです。
3D の原子の箱を異なる形状にレイアウトするために、Object collection を使っています。

## Object collection のタイプ ##

**3D オブジェクト**
Object collection は、インポートされた 3D オブジェクトのレイアウトに利用可能です。以下の例は、Object collection を使った 3D の椅子モデル オブジェクトの平面レイアウトと円柱状のレイアウトを示しています。

![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_3DObjects.jpg)

**2D オブジェクト**

Object collection は 2D 画像からも作ることができます。例えば、複数の画像をグリッド上に配置することができます。

![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Layout_2DImages.jpg)
