# 入力の概要

MRTK の Input System (入力システム)では、次のことが可能です。

- 入力イベントを介して、6 DOF コントローラー、多関節ハンドまたは音声など、さまざまな入力ソースからの入力を消費します。
- *Select* や *Menu* などの抽象アクションを定義し、それらを異なる入力に関連付けます。
- フォーカス イベントとポインター イベントを介して UI コンポーネントを駆動するためにコントローラーにアタッチされたポインターを設定します。

<img src="../../Documentation/Images/Input/MRTK_InputSystem.png" style="display:block;margin-left:auto;margin-right:auto;">
<sup>MRTK Input System (入力システム)</sup>

入力は [**Input Data Providers(Device Manager) (入力データ プロバイダー(デバイス マネージャー))**](InputProviders.md) によって生成されます。各プロバイダーは、Open VR、Windows Mixed Reality (WMR)、Unity Joystick、Windows Speech など、入力の特定のソースに対応しています。プロバイダーは、*Mixed Reality Toolkit* コンポーネントの **Registered Service Providers Profile** を介してプロジェクトに追加され、対応する入力ソースが利用可能な場合(例えば WMR のコントローラーが検出されたり、ゲームパッドが接続された場合)、自動的に [**Input Events (入力イベント)**](InputEvents.md) を生成します。

[**Input Actions (入力アクション)**](InputActions.md) は、入力を生成する特定の入力ソースからアプリケーション ロジックを分離するのに役立つ生の入力に対する抽象化です。たとえば、*Select* アクションを定義し、マウスの左ボタン、ゲームパッドのボタン、6 DOF コントローラーのトリガーにマップすると便利です。これにより、アプリケーション ロジックは、それを生成できるさまざまな入力をすべて意識する代わりに、*Select* 入力アクション イベントをリッスンすることができます。入力アクションは **Input Actions Profile** で定義され、*Mixed Reality Toolkit* コンポーネントの *Input System Profile* 内にあります。

[**Controllers (コントローラー)**](Controllers.md) は、入力デバイスが検出されたときに *input providers (入力プロバイダー)* によって作成され、入力デバイスが失われたり切断されたりすると破棄されます。たとえば、WMR 入力プロバイダーは、6 DOF デバイス用の *WMR controllers*と、 articulated hand (多関節ハンド) 用の *WMR articulated hand controllers* を作成します。コントローラー入力は、*Input System Profile* 内の **Controller Mapping Profile** を介して入力アクションにマッピングすることができます。コントローラーによって発生した入力イベントには、もし存在する場合、関連する入力アクションが含まれます。

コントローラーには、[**Pointers (ポインター)**](Pointers.md) をアタッチして、シーンを照会してフォーカスのあるゲーム オブジェクトを決定し、[**Pointer Events (ポインター イベント)**](Pointers.md#pointer-event-interfaces) を発生させることができます。例として、*line pointer (ライン ポインター)* はコントローラーのポーズを使用してシーンに対してレイキャストを実行し、レイの原点と方向を計算します。各コントローラーに対して作成されたポインターは、*Input System Profile* の下の **Pointer Profile** で設定されます。

<img src="../../Documentation/Images/Input/MRTK_Input_EventFlow.png" width="200px" style="display:block;margin-left:auto;margin-right:auto;">
<sup>イベント フロー</sup>

入力イベントは UI コンポーネントで直接処理できますが、ポインター イベントを使用して実装をデバイスに依存しないようにすることをお勧めします。
