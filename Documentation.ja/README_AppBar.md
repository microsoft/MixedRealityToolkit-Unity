# App bar

![App bar](../Documentation/Images/AppBar/MRTK_AppBar_Main.png)

App bar は、[バウンズ コントロール](README_BoundsControl.md) スクリプトとともに使用される UI コンポーネントです。オブジェクトを操作するためのボタン コントロールをオブジェクトに追加します。[Adjust] ボタンを使用すると、オブジェクトのバウンズ コントロール インターフェイスを非アクティブ/アクティブにすることができます。[Remove] ボタンをクリックすると、シーンからオブジェクトが削除されます。

## App bar の使い方
`AppBar` (Assets/MRTK/SDK/Features/UX/Prefabs/AppBar/AppBar.prefab) を scene hierarchy (シーン ヒエラルキー) の中にドラッグ アンド ドロップします。コンポーネントの [Inspector](インスペクター) パネルで、バウンズ コントロールを持つ任意のオブジェクトを  *Target Bounding Box* として割り当て、そこに App bar を追加します。

**重要:** ターゲット オブジェクトのバウンズ コントロール アクティブ化オプションは [Activate Manually] である必要があります。

<img src="../Documentation/Images/AppBar/MRTK_AppBar_Setup1.png" width="450">

<img src="../Documentation/Images/AppBar/MRTK_AppBar_Setup2.png" width="450">


