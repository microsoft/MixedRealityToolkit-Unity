# Object collection (オブジェクトコレクション) #

![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Main.jpg)

Object collection はオブジェクトの列を事前定義された三次元形状にレイアウトすることを助けるスクリプトです。これは、平面、円柱、球、放射状を含む、さまざまな面タイプをサポートしています。半径、サイズ、アイテムの間のスペースは調整可能です。Unity のすべてのオブジェクトをサポートしているため、2D オブジェクトと 3D オブジェクトの両方のレイアウトに利用可能です。

# Object collection のスクリプト #
- [`GridObjectCollection.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/GridObjectCollection.cs) は、円柱、平面、球、放射状の面タイプをサポートしています。
- [`ScatterObjectCollection.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/ScatterObjectCollection.cs) は、点在するスタイルのコレクションをサポートしています。
- [`TileGridObjectCollection.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/TileGridObjectCollection.cs) は、GridObjectCollection にいくつかの追加オプションを提供しています。

|![Grid Object Collection - Cylinder](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionCylinder.png) Grid Object Collection - Cylinder | ![Grid Object Collection - Sphere](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionSphere.png) Grid Object Collection - Sphere |
|:--- | :--- |
|![Grid Object Collection - Radial](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionRadial.png) Grid Object Collection - Radial | ![Grid Object Collection - Plane](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionPlane.png) Grid Object Collection - Plane |
|![Scattered Object Collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionScattered.png) Scattered Object Collection | ![Tile Grid Object Collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionTileGrid.png) Tile Grid Object Collection |


## Object collection の使い方 ##

コレクションを作るためには、空のゲームオブジェクトを作り、それに Object collection スクリプトの1つをアサインします。
いかなるオブジェクトでも、このゲームオブジェクトの子供として追加できます。子オブジェクトの追加が終わったら、[Inspector] (インスペクター) パネルで *Update Collection* ボタンをクリックし、Object collection を生成します。オブジェクトは選択された面タイプにしたがって、シーンにレイアウトされます。Update Collection は、コードからもアクセスできます。




![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionScript.png)

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
