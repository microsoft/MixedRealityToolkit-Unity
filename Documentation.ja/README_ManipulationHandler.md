# Manipulation handler #

![Manipulation handler](../Documentation/Images/ManipulationHandler/MRTK_Manipulation_Main.png)

*ManipulationHandler* スクリプトを使用することで、オブジェクトを 片手か両手を使って、移動、スケール調整、および回転を可能にすることができます。特定の種類の移動のみを許可することにより、操作を制限できます。このスクリプトは、HoloLens 2 多関節ハンド入力、手の Ray 、HoloLens (第 1 世代) ジェスチャ入力、没入型ヘッドセット モーション コントローラー入力など、さまざまな種類の入力で動作します。

## Manipulation handler の使用方法 ##

[`ManipulationHandler.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/Input/Handlers/ManipulationHandler.cs) コンポーネントをゲームオブジェクトに追加します。

また、オブジェクトに衝突可能な境界を合わせて追加してください。オブジェクトが近い多関節ハンドに応答するようにするには、[NearInteractionGrabbable.cs](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Services/InputSystem/NearInteractionGrabbable.cs) スクリプトも追加します。

オブジェクトのスケールの最小値または最大値を設定する場合は、[TransformScaleHandler](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/Input/Handlers/TransformScaleHandler.cs) スクリプトも追加してください。

![Manipulation Handler](../Documentation/Images/ManipulationHandler/MRTK_ManipulationHandler_Howto.png)

## インスペクタのプロパティ ##

<img src="../Documentation/Images/ManipulationHandler/MRTK_ManipulationHandler_Structure.png" width="450">

**ホスト Transform**
ドラッグされる変換。既定では、コンポーネントのオブジェクトが設定されます。

**操作タイプ**
片手、両手、または両方を使用してオブジェクトを操作できるかどうかを指定します。

* *片手のみ*
* *両手のみ*
* *片手と両手両方*

**両手操作タイプ**

* *スケール*: スケーリングのみ可能です。
* *回転*:回転のみが許可されています。
* *移動スケール*: 移動とスケーリングが許可されています。
* *回転を移動*:移動および回転が許可される。
* *回転スケール*: 回転とスケーリングが可能です。
* *回転スケールを移動*:移動、回転およびスケーリングが許可されます。

![Manipulation Handler](../Documentation/Images/ManipulationHandler/MRTK_ManipulationHandler_TwoHanded.jpg)

**遠い操作**
ポインターとの遠い相互作用を使用して操作を実行できるかどうかを指定します。

**片手回転モード近く**
オブジェクトが片手/コントローラーの近くでつかまれているときにオブジェクトがどのように動作するかを指定します。

**片手回転モード遠い**
オブジェクトが距離で片手/コントローラーでつかまれているときにオブジェクトがどのように動作するかを指定します。

**片手回転モードオプション**
オブジェクトが片手でつかまれているときにオブジェクトを回転させる方法を指定します。

* *元の回転を維持*: 移動中にオブジェクトを回転させません
* *ユーザへの回転を維持する*: X/Y 軸のオブジェクトの元の回転をユーザに維持する
* *重力整列は、ユーザーに回転を維持*: オブジェクトの元の回転をユーザに維持しますが、オブジェクトを垂直にします。境界ボックスに便利です。
* *顔のユーザー*: オブジェクトが常にユーザーに直面していることを確認します。スレート/パネルに便利です。
* *ユーザーから離れて顔*: オブジェクトが常にユーザーから離れて顔を保ちます。後方に設定されたスレート/パネルに便利です。
* *オブジェクトの中心を中心に回転*: 多関節ハンド/コントローラでのみ動作します。ハンド/コントローラの回転を使用してオブジェクトを回転しますが、オブジェクトの中心点を中心に回転します。遠くから検査する場合に便利です。
* *グラブポイント*について回転:関節の手/コントローラのためにのみ動作します。オブジェクトを手/コントローラーで保持しているかのように回転します。検査に役立ちます。

**リリース動作**
オブジェクトが解放されるとき、その物理的な動きの動作を指定します。そのオブジェクト上に Rigidbody コンポーネントが必要です。

* *何も*
* *すべて*
* *キープ速度*
* *角度速度を保つ*

**回転の制約**
操作時にオブジェクトが回転する軸を指定します。

* *なし*
* *X軸のみ*
* *Y軸のみ*
* *Z軸のみ*

**動きの制約**
* *なし*
* *ヘッドからの距離を修正*

**スムージングアクティブ**
スムージングをアクティブにするかどうかを指定します。

**平滑量片手**
移動、スケール、回転に適用するスムージングの量。0 のスムージングは、スムージングがないことを意味します。最大値は、値に変更がないことを意味します。

## イベント ##
Manipulation handlerは、次のイベントを提供します。

* *OnManipulationStarted*: 操作が開始されたときに起動されます。
* *OnManipulationEnded*: 操作が終了すると起動します。
* *OnHoverStarted*: ハンド/コントローラーが操作可能な、近くまたは遠くにホバーしたときに起動します。
* *OnHoverEnded*: ハンド/コントローラーが操作可能な、近くまたは遠くにホバリング解除すると起動します。
