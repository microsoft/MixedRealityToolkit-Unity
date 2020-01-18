# MixedRealityServiceRegistry と IMixedRealityServiceRegistrar とは何か?

Mixed Reality Toolkit には、関連するタスクを行う2つの非常に似た名前のコンポーネントがあります。
MixedRealityServiceRegistry と IMixedRealityServiceRegistrar です。

## MixedRealityServiceRegistry

[MixedRealityServiceRegistry](xref:Microsoft.MixedReality.Toolkit.MixedRealityServiceRegistry) は
登録されたサービス（コア システムとエクステンション サービス）のそれぞれのインスタンスを含むコンポーネントです。

> [!NOTE]
> MixedRealityServiceRegistry は、[IMixedRealityExtensionService](xref:Microsoft.MixedReality.Toolkit.IMixedRealityExtensionService) を含む [IMixedRealityService](xref:Microsoft.MixedReality.Toolkit.IMixedRealityService) インターフェイスを実装したオブジェクトのインスタンスを含んでいます。
>
> [IMixedRealityDataProvider](xref:Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider) (IMixedRealityService のサブクラス) を実装しているオブジェクトは、明示的には MixedRealityServiceRegistry に登録されていません。これらのオブジェクトは、個々のサービス（例えば、Spatial Awareness）によって管理されています。

MixedRealityServiceRegistry は静的な C# クラスとして実装されており、
アプリケーション コードでサービスのインスタンスを取得するのに使われる推奨パターンです。

以下のスニペットは、IMixedRealityInputSystem のインスタンスを取得するデモです。

```
IMixedRealityInputSystem inputSystem = null;

if (!MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem))
{
    // input system の取得に失敗。登録されていないかもしれない。
}
```

## IMixedRealityServiceRegistrar

[IMixedRealityServiceRegistrar](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar)
は、1つまたは複数のサービスの登録を管理するコンポーネントで実装される機能を定義したインターフェイスです。
IMixedRealityServiceRegistrar を実装したコンポーネントは、MixedRealityServiceRegistry 内のデータの追加と削除を行う責務があります。
[MixedRealityToolkit](xref:Microsoft.MixedReality.Toolkit.MixedRealityToolkit) オブジェクトはそのようなコンポーネントの一つです。

その他の registrars は MixedRealityToolkit.SDK.Experimental.Features フォルダーにて見つけることができます。
これらのコンポーネントは、単一のサービス（例えば、Spatial Awareness）のサポートをアプリケーションに追加するのに使うことができます。
これらの、単一サービスのマネージャーは以下の通りです。

- [BoundarySystemManager](xref:Microsoft.MixedReality.Toolkit.Experimental.Boundary.BoundarySystemManager)
- [CameraSystemManager](xref:Microsoft.MixedReality.Toolkit.Experimental.CameraSystem.CameraSystemManager)
- [DiagnosticsSystemManager](xref:Microsoft.MixedReality.Toolkit.Experimental.Diagnostics.DiagnosticsSystemManager)
- [InputSystemManager](xref:Microsoft.MixedReality.Toolkit.Experimental.Input.InputSystemManager)
- [SpatialAwarenessSystemManager](xref:Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness.SpatialAwarenessSystemManager)
- [TeleportSystemManager](xref:Microsoft.MixedReality.Toolkit.Experimental.Teleport.TeleportSystemManager)

上記コンポーネントの内 InputSystemManager 以外のものは、単一のサービス タイプの登録と状態を管理する責務があります。
InputSystem はいくつかの追加のサポート サービス（例えば、FocusProvider）を必要とし、それらもまた InputSystemManager によって管理されます。

一般的に、IMixedRealityServiceRegistrar によって定義されたメソッドはサービス マネジメント コンポーネントによって内部的に呼ばれる、もしくは、正しく機能するために追加のサービス コンポーネントを必要とするサービスによって呼ばれます。
アプリケーション コードは、一般的に、これらのメソッドを呼ぶべきではありません。
アプリケーションが予想外の動きをする（例えば、キャッシュされたサービス インスタンスが無効になるかもしれない）可能性があるためです。

## 関連項目

- [IMixedRealityServiceRegistrar API documentation](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar)
- [MixedRealityServiceRegistry API documentation](xref:Microsoft.MixedReality.Toolkit.MixedRealityServiceRegistry)
