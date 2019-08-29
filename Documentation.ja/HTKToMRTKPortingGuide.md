# HTK2017 から MRTK v2 への移植ガイド

## コントローラー入力およびハンド入力

### セットアップと設定

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| 型                      | 関連する入力タイプの情報を含む、ボタンに対する特定のイベント | イベントを介して渡される、動作 / ジェスチャ ベースの入力|
| セットアップ                     | シーン内に InputManager を配置します。 | [Configuration Profile](MixedRealityConfigurationGuide.md) 内の input system を有効にし、 input system type の詳細を設定します。|
| 設定             | シーン内の個々のスクリプトのインスペクタで設定されます。 | 以下に示す Mixed Reality Input System プロファイルおよび関連するプロファイルによって設定されます。 |

関連するプロファイル:

* Mixed Reality Controller Mapping Profile
* Mixed Reality Controller Visualization Profile
* Mixed Reality Gestures Profile
* Mixed Reality Input Actions Profile
* Mixed Reality Input Action Rules Profile
* Mixed Reality Pointer Profile

[Gaze Provider](xref:Microsoft.MixedReality.Toolkit.Input.GazeProvider) の設定はシーン内の Main Camera オブジェクト上で変更されています。

Windows Mixed Reality Device Manager などのプラットフォームサポートコンポーネントは、対応するサービスのデータプロバイダに追加する必要があります。

### インターフェースおよびイベントの対応付け

一部のイベントに関しては固有のイベントがなくなり、MRTK v2 では[MixedRealityInputAction](Input/InputActions.md) に含まれます。
これらの動作は Input Actions プロファイルで指定され、Controller Mapping profile 内で特定のコントローラーやプラットフォームに対応付けされます。`OnInputDown` のようなイベントは MixedRealityInputAction タイプをチェックする必要があります。

関連する input systems:

* [Input Overview](/Input/Overview.md)
* [Input Events](/Input/InputEvents.md)
* [Input Pointers](/Input/Pointers.md)

| HTK 2017 |  MRTK v2  | Action への対応付け |
|----------|-----------|----------------|
| `IControllerInputHandler` | [`IMixedRealityInputHandler<Vector2>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | タッチパッドあるいはジョイスティックに対応付けされる |
| `IControllerTouchpadHandler` | [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler) | タッチパッドに対応付けされる |
| `IFocusable` | [`IMixedRealityFocusHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusHandler) | |
| `IGamePadHandler` | [`IMixedRealitySourceStateHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySourceStateHandler) | |
| `IHoldHandler` | [`IMixedRealityGestureHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler) | Gestures Profile 内で hold に対して対応付けされる |
| `IInputClickHandler` | [`IMixedRealityPointerHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointerHandler) |
| `IInputHandler` | [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler) | コントローラーのボタンやハンドでのタップ入力に対応付けされる |
| `IManipulationHandler` | [`IMixedRealityGestureHandler<Vector3>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) | Gestures Profile 内の manipulation に対応付けされる|
| `INavigationHandler` | [`IMixedRealityGestureHandler<Vector3>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGestureHandler`1) | Gestures Profile 内の navigation に対応付けされる |
| `IPointerSpecificFocusable` | [`IMixedRealityFocusChangedHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityFocusChangedHandler) | |
| `ISelectHandler` | [`IMixedRealityInputHandler<float>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | trigger position に対応付けされる |
| `ISourcePositionHandler` | [`IMixedRealityInputHandler<Vector3>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) あるいは [`IMixedRealityInputHandler<MixedRealityPose>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | pointer position あるいは grip position 対応付けされる |
| `ISourceRotationHandler` | [`IMixedRealityInputHandler<Quaternion>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) あるいは [`IMixedRealityInputHandler<MixedRealityPose>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | pointer position あるいは grip position に対応付けされる |
| `ISourceStateHandler` | [`IMixedRealitySourceStateHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySourceStateHandler) | |
| `IXboxControllerHandler` | [`IMixedRealityInputHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler) と [`IMixedRealityInputHandler<Vector2>`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler`1) | 多くのコントローラーのボタンやジョイスティックに対応付けされる |

## カメラ

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | Main Camera を削除し、MixedRealityCameraParent / MixedRealityCamera / HoloLensCamera プレハブをシーンに追加します。 **あるいは**  Mixed Reality Toolkit > Configure > Apply Mixed Reality Scene Settings というメニューを使用します。 | Main camera オブジェクトの親オブジェクトが MixedRealityPlayspace となるように、 Mixed Reality Toolkit > Add to Scene and Configure... を使用します。 |
| 設定             | Camera の設定は、プレハブインスタンスにて行います。 | [Mixed Reality Camera Profile](xref:Microsoft.MixedReality.Toolkit.MixedRealityCameraProfile) にて Camera の設定を行います。 |

## 音声認識 (Speech)

### キーワード認識

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | SpeechInputSource をシーンに追加してください。 | Windows Speech Input Manager などのキーワードサービスが入力システムのデータプロバイダに追加されている必要があります。|
| 設定             | 認識するキーワードをSpeechInputSource のインスペクタで設定します。| キーワードは [Mixed Reality Speech Commands Profile](Input/Speech.md) で設定します。 |
| イベントハンドラ            | `ISpeechHandler` | [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) |

### 音声ディクテーション (Dictation)

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | DictationInputManager をシーンに追加してください。 | Dictation のサポートには、Windows Dictation Input Manager などのサービスを、input system のデータプロバイダに追加する必要があります。 |
| イベントハンドラ            | `IDictationHandler` | `IMixedRealityDictationHandler`[`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) |

## 空間認識・空間マッピング (Spatial awareness / mapping)

### メッシュ (Mesh)

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | SpatialMapping プレハブをシーンに追加します。 | Configuration Profile 内の Spatial Awareness System を有効にし、 Windows Mixed Reality Spatial Mesh Observer などの空間オブザーバーを Spatial Awareness System のデータプロバイダに追加します。|
| 設定             | インスペクタ内でシーンインスタンスを設定します。| それぞれの空間オブザーバーのプロファイルを設定します。 |

### 平面 (Plane)

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | `SurfaceMeshesToPlanes` スクリプトを使用します。 | 未実装の状態です。 |

### 空間理解 (Spatial Awareness)

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | SpatialUnderstanding プレハブをシーンに追加します。 | 未実装の状態です。 |
| 設定             | インスペクタ内でシーンインスタンスを設定します。 | 未実装の状態です。 |

## 移動境界 (Boundary)

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | `BoundaryManager` スクリプトをシーンに追加します。 | Configuration Profile 内の Boundary Systemを有効にします。|
| 設定             | インスペクタ内でシーンインスタンスを設定します。 | Boundary Visualization profile 内にて設定します。 |

## シェアリング (Sharing)

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| セットアップ                     | Sharing service の場合: Sharing プレハブをシーンに追加します。<br>UNet の場合: SharingWithUNET example を使用してください。 | 更新中の状態です。 |
| 設定             | インスペクタ内でシーンインスタンスを設定します。 | 更新中の状態です。 |

## UX

|                           | HTK 2017 |  MRTK v2  |
|---------------------------|----------|-----------|
| ボタン (Button)                     | [Interactable Objects](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_InteractableObjectExample.md) | [Button](README_Button.md) |
| Interactable                     | [Interactable Objects](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_InteractableObjectExample.md) | [Interactable](README_Interactable.md) |
| バウンディングボックス (Bounding Box)             | [Bounding Box](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_BoundingBoxGizmoExample.md) | [Bounding Box](README_BoundingBox.md) |
| App Bar             | [App Bar](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_BoundingBoxGizmoExample.md) | [App Bar](README_AppBar.md) |
| 片手操作 (掴む (Grab)、 移動 (Move))   | [HandDraggable](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit/Input/Scripts/Utilities/Interactions/HandDraggable.cs) | [Manipulation Handler](README_ManipulationHandler.md) |
| 両手操作 (掴む (Grab)/移動 (Move)/回転 (Rotate)/拡大縮小 (Scale))             | [TwoHandManipulatable](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit/Input/Scripts/Utilities/Interactions/TwoHandManipulatable.cs) | [Manipulation Handler](README_ManipulationHandler.md) |
| キーボード             | [Keyboard prefab]() | [System Keyboard](README_SystemKeyboard.md) |
| ツールチップ (Tooltip)             | [Tooltip](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_TooltipExample.md) | [Tooltip](README_Tooltip.md) |
| オブジェクト選択             | [Object Collection](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/UX/Readme/README_ObjectCollection.md) | [Object Collection](README_ObjectCollection.md) |
| ソルバー            | [Solver](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/htk_release/Assets/HoloToolkit-Examples/Utilities/Readme/README_SolverSystem.md) | [Solver](README_Solver.md) |

## ユーティリティ (Utilities)

一部のユーティリティは、ソルバーシステムと重複するように調整されています。 必要なスクリプトが見つからない場合は、問題を報告してください。

| HTK 2017 |  MRTK v2  |
|----------|-----------|
| ビルボード (Billboard) | [`Billboard`](xref:Microsoft.MixedReality.Toolkit.UI.Billboard) |
| タグアロング (Tagalong) | [`RadialView`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.RadialView) あるいは [`Orbital`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.Orbital) [Solver](README_Solver.md) |
| FixedAngularSize | [`ConstantViewSize`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.ConstantViewSize) [Solver](README_Solver.md) |
| FPS表示 (FpsDisplay) | [Diagnostics System](Diagnostics/DiagnosticsSystemGettingStarted.md) (Configuration Profile 内にて) |
| NearFade | [Mixed Reality Toolkit Standard shader](README_MRTKStandardShader.md) に含まれています。|
