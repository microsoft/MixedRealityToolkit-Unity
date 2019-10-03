# プラットフォームの Capabilities（機能）を検出する

MRTK についてよくある質問は、アプリケーションを実行するのにどのデバイス
（例えば Microsoft HoloLens 2) が使われているのかを知る方法に関するものです。
正確なハードウェアを特定することは、プラットフォームによっては困難です。
代わりに、MRTK はシステムの特定の機能（例えば、システムが Articulated hand（多関節ハンド）の機能を持っているかどうか）を特定する方法を提供しています。

## Capabilities（機能）

Mixed Reality Toolkit は [MixedRealityCapability](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)
という列挙型を提供しており、これはアプリケーションがランタイムで問い合わせを行うための機能のセットを定義しています。
この列挙型は Mixed Reality Toolkit がチェック可能な機能の完全なセットを含んでいます。

### Input System capabilities（インプットシステムの機能）

インプットシステムは、以下の機能の問い合わせをサポートしています。

| 機能 | 説明 |
|---|---|
| ArticulatedHand | Articulated hand（多関節ハンド）のインプット |
| EyeTracking | Eye gaze（アイ ゲイズ）のターゲティング |
| GGVHand | Gaze-Gesture-Voice（ゲイズ、ジェスチャー、ボイス）のハンド インプット |
| MotionController | モーション コントローラーのインプット |
| VoiceCommand | アプリで定義したキーワードを使ったボイス コマンド |
| VoiceDictation | 音声によるテキスト ディクテーション |

以下の例は、多関節ハンドをサポートしたデータ プロバイダーをインプット システムがロードしたかどうかをチェックしています。

``` C#
// input system を取得する
IMixedRealityInputSystem inputSystem = null;
MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
if (inputSystem == null)
{
    // input system の取得に失敗
}

bool supportsArticulatedHands = false;

IMixedRealityCapabilityCheck capabilityCheck = inputSystem as IMixedRealityCapabilityCheck;
if (capabilityCheck != null)
{
    supportsArticulatedHands = capabilityCheck.CheckCapability(MixedRealityCapability.ArticulatedHand);
}
```

### Spatial Awareness capabilities（空間認識の機能）

空間認識システムは、以下の機能の問い合わせをサポートしています。

| 機能 | 説明 |
|---|---|
| SpatialAwarenessMesh | Spatial meshes（空間メッシュ） |
| SpatialAwarenessPlane | Spatial planes（空間プレーン） |
| SpatialAwarenessPoint | Spatial points（空間ポイント） |

以下の例は、空間メッシュをサポートしたデータ プロバイダーを空間認識システムがロードしたかどうかをチェックしています。

``` C#
// spatial awareness system を取得する
IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;
MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
if (spatialAwarenessSystem == null)
{
    // spatial awareness system の取得に失敗
}

bool supportsSpatialMesh = false;

IMixedRealityCapabilityCheck capabilityCheck = spatialAwarenessSystem as IMixedRealityCapabilityCheck;
if (capabilityCheck != null)
{
    supportsSpatialMesh = capabilityCheck.CheckCapability(MixedRealityCapability.SpatialAwarenessMesh);
}
```

## 関連項目

- [IMixedRealityCapabilityCheck API documentation](xref:Microsoft.MixedReality.Toolkit.IMixedRealityCapabilityCheck)
- [MixedRealityCapability enum documentation](xref:Microsoft.MixedReality.Toolkit.MixedRealityCapability)
