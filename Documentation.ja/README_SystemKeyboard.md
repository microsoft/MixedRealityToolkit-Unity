# System keyboard (システム キーボード)

![System keyboard](../Documentation/Images/SystemKeyboard/MRTK_SystemKeyboard_Main.png)

Unity アプリケーションならば、いつでもシステム キーボードを呼び出すことができます。システム キーボードはターゲット プラットフォームの機能に応じて動作し、たとえば HoloLens 2 のキーボードはダイレクト ハンド インタラクションをサポートしますが、HoloLens (第 1 世代) のキーボードは GGV (Gaze, Gesture, and Voice)<sup>[1](https://docs.microsoft.com/windows/mixed-reality/gaze)</sup> をサポートすることに注意してください。さらに、[Unity Remoting](Tools/HolographicRemoting.md) によってエディターから HoloLens に対し実行している場合は、システム キーボードは表示されません。

## System keyboard (システム キーボード) の呼び出し方

```c#
public TouchScreenKeyboard keyboard;

...

public void OpenSystemKeyboard()
{
    keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
}
```

## 入力の読み込み方

```c#
public TouchScreenKeyboard keyboard;

...

private void Update()
{
    if (keyboard != null)
    {
        keyboardText = keyboard.text;
        // Do stuff with keyboardText
    }
}
```

## System keyboard (システム キーボード) のサンプル

システム キーボードを表示する簡単な例は、`MixedRealityKeyboard.cs` (Assets/MRTK/SDK/Experimental/Features/UX/MixedRealityKeyboard.cs) にて見ることができます。

## 関連項目

- [Mixed Reality Keyboard Helper Classes](../Assets/MRTK/SDK/Experimental/MixedRealityKeyboard/README_MixedRealityKeyboard.md)
