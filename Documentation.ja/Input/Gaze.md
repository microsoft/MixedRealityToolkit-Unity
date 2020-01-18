# Gaze (ゲイズ)

[Gaze](https://docs.microsoft.com/ja-jp/windows/mixed-reality/gaze) は、ユーザーがどこを見ているかに基づいて世界とインタラクションする入力方式です。ゲイズには、異なる2つの種類があります。

## ヘッド ゲイズ

このタイプのゲイズは、頭/カメラが向いている方向に基づきます。ヘッド ゲイズは、アイ ゲイズをサポートしていないシステムや、ハードウェアはアイ ゲイズをサポートしている可能性があるが [アクセス許可とセットアップ](../EyeTracking/EyeTracking_BasicSetup.md#eye-tracking-requirements-checklist) が正しく実行されていない場合において、アクティブになります。

ヘッド ゲイズは、通常、HoloLens 1 スタイルのインタラクションに関連しています。
それは、オブジェクトがホログラフィック フレームの中央に配置されるようにオブジェクトを見て、エア タップのジェスチャを実行するというものです。

## アイ ゲイズ (視線)

このタイプのゲイズは、ユーザーの目がどこを見ているかに基づいています。アイ ゲイズはアイ トラッキングをサポートするシステムにのみ存在します。アイ ゲイズの使用方法の詳細については、[アイ トラッキングのドキュメント](../EyeTracking/EyeTracking_Main.md) を参照してください。

## GazeProvider

ゲイズの機能 (ヘッドと視線の両方) は、[GazeProvider](xref:Microsoft.MixedReality.Toolkit.Input.GazeProvider) によって提供されます。このプロバイダーは入力システム プロファイルの *Pointer* セクションで設定できます:

![Gaze Configuration Entrypoint](../../Documentation/Images/Input/GazeConfigurationEntrypoint.png)

他の入力ソースと同様に、GazeProvider はポインター [(ポインターに関する情報はこのドキュメントをご覧ください)](../Architecture/InputSystem/ControllersPointersAndFocus.md) を使用してシーン内のオブジェクトとインタラクションします。
GazeProvider の場合、ポインターは `InternalGazePointer` によって実装され、プロファイルによって設定されていません。

*Gaze Provider Type* を [IMixedRealityGazeProvider](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityGazeProvider) と [IMixedRealityEyeGazeProvider](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityEyeGazeProvider) を実装する別のクラスを参照するように変更することで、標準の GazeProvider を別の実装に置き換えることができます。
GazeProvider の再実装は簡単ではないので、通常は標準の GazeProvider を使う (バグを発見した場合は問題を報告する) ことをお勧めします。

## 使用方法

### 現在の gaze target (ゲイズ ターゲット) を取得する方法

このサンプルは、ユーザーのゲイズのターゲットとなっている現在のゲームオブジェクトを取得する方法を示します。

```csharp
void LogCurrentGazeTarget()
{
    if (CoreServices.InputSystem.GazeProvider.GazeTarget)
    {
        Debug.Log("User gaze is currently over game object: "
            + CoreServices.InputSystem.GazeProvider.GazeTarget)
    }
}
```

### 現在のゲイズの方向と原点を取得する方法

このサンプルは、ユーザのゲイズの方向と原点 (ゲイズの方向がどの点から出ているか) を表す Vector3 を取得する方法を示します。

```csharp
void LogGazeDirectionOrigin()
{
    Debug.Log("Gaze is looking in direction: "
        + CoreServices.InputSystem.GazeProvider.GazeDirection);

    Debug.Log("Gaze origin is: "
        + CoreServices.InputSystem.GazeProvider.GazeOrigin);
}
```
