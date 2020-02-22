# Input System (入力システム)

入力システムは、MRTK が提供するすべての機能の中で最大のシステムの 1 つです。
ツールキット内の非常に多くのものがその上に構築されます(ポインター、フォーカス、プレハブ)。
入力システム内のコードは、プラットフォーム間でのグラブや回転などの自然なインタラクションを可能にします。

入力システムには、定義する価値のある独自の用語がいくつかあります:

- **Data providers (データ プロバイダー)**

    Input Profile (入力プロファイル) の入力設定には、データ プロバイダー (別名デバイス マネージャー) と呼ばれるエンティティへの参照があります。
    これらは、特定の基礎となるシステムとのインターフェイスとなって MRTK の入力システムを拡張することを役割とするコンポーネントです。
    プロバイダーの例は、Windows Mixed Reality プロバイダーです。その役割は、基礎となる Windows Mixed Reality API と通信し、
    それらの API からのデータを以下の MRTK 固有の入力概念に変換することです。
    別の例は、OpenVR プロバイダーです (その役割は、Unity で抽象化されたバージョンの OpenVR API と通信し、そのデータを MRTK 入力概念に変換することです)。

- **Controller (コントローラー)**

    物理コントローラーを表したものです (6 自由度コントローラー、ジェスチャ サポート付きの HoloLens 1 スタイルの手、
    多関節ハンド、Leap Motion コントローラーなど)。コントローラーは、デバイス マネージャーによって生成されます
    (つまり、WMR デバイス マネージャーは、多関節ハンドの存在を確認すると、コントローラーを生成し、そのライフタイムを管理します)。

- **Pointer (ポインター)**

    コントローラーはポインターを使用してゲームオブジェクトとやり取りします。例えば、ニア インタラクション ポインターは、
    (コントローラーである) 手が ‘near interaction’ をサポートしていると宣伝するオブジェクトに近づいたことを検出する責務を負います。
    ポインターの他の例としては、テレポーテーションまたはファー ポインター (シェル ハンド レイ ポインター) があり、
    ファー レイキャストを使用して、ユーザーからの腕の長さよりも遠いコンテンツを処理します。

    ポインターはデバイス マネージャーによって作成され、入力ソースにアタッチされます。
    コントローラーのすべてのポインターを取得するには、次のようにします: `controller.InputSource.Pointers`

    コントローラーは同時に多くの異なるポインターに関連付けることができることに注意してください。
    これが混乱に陥らないように、どのポインターをアクティブにするかを制御する Pointer Mediator (ポインター メディエーター) があります
    (例えば、メディエーターはニア インタラクションが検出された場合、ファー インタラクション ポインターを無効にします)。

- **Focus (フォーカス)**

    ポインター イベントは、**フォーカス**でオブジェクトに送信されます。 フォーカスの選択は、ポインターの種類によって異なります。ハンド レイ ポインターは
    レイキャストを使用し、ポーク ポインターはスフィアキャストを使用します。 オブジェクトは、フォーカスを受け取るために
    IMixedRealityFocusHandler を実装する必要があります。 オブジェクトをグローバルに登録して、
    フィルター処理されていないポインター イベントを受け取ることは可能ですが、この方法はお勧めしません。

    どのオブジェクトがフォーカスされているかを更新するコンポーネントは [FocusProvider](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_development/Assets/MixedRealityToolkit.Services/InputSystem/FocusProvider.cs) です。

- **Cursor (カーソル)**

    ポインター インタラクションの周りに追加の視覚的効果を与える、ポインターに関連付けられたエンティティです。
    例えば、FingerCursor は指の周りにリングを描画し、指が ‘near interactable’ オブジェクトの近くにあるときにそのリングを回転させる場合があります。
    ポインターは、一度に 1 つのカーソルに関連付けることができます。

- **Interaction and Manipulation (インタラクションとマニピュレーション)**

    オブジェクトは、インタラクションまたはマニピュレーション スクリプトでタグ付けできます。
    これは [`Interactable`](xref:Microsoft.MixedReality.Toolkit.UI.Interactable)、または  [`NearInteractionGrabbable`](xref:Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable)/[`ManipulationHandler`](xref:Microsoft.MixedReality.Toolkit.UI.ManipulationHandler) のようなものを介する場合があります。

    例えば、NearInteractionGrabbable と NearInteractionTouchable を使用すると、特定のポインター (特にニア インタラクション ポインター) で、
    どのオブジェクトにフォーカスできるかを知ることができます。

    Interactable および ManipulationHandler は、ポインター イベントをリッスンして UI の見た目を変更したり、
    ゲームオブジェクトを移動/スケーリング/回転するコンポーネントの例です。

以下の画像は、MRTK 入力スタックの高レベルのビルドアップ (下から上) を示しています:

![Input System Diagram](../../../Documentation/Images/Input/MRTK_InputSystem.png)
