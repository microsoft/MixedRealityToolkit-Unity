# System keyboard (システム キーボード) #

![System keyboard](../Documentation/Images/SystemKeyboard/MRTK_SystemKeyboard_Main.png)

Unity アプリケーションならば、いつでもシステム キーボードを呼び出すことができます。システム キーボードはターゲット プラットフォームの機能に応じて動作し、たとえば HoloLens 2 のキーボードはダイレクト ハンド インタラクションをサポートしますが、HoloLens (第 1 世代) のキーボードは GGV<sup>[1](https://docs.microsoft.com/en-us/windows/mixed-reality/gaze)</sup> をサポートすることに注意してください。

## System keyboard (システム キーボード) の呼び出し方 ##

``` csharp
    public TouchScreenKeyboard keyboard;

    ...

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }
```

## 入力の読み込み方 ##

``` csharp

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

## System keyboard (システム キーボード) のサンプル ##
システム キーボードを表示する簡単な例は、[`OpenKeyboard.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/HandTracking/Script/OpenKeyboard.cs) にて見ることができます。
