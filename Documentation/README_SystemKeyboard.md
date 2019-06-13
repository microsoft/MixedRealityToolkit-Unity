# System keyboard #

![System keyboard](../Documentation/Images/SystemKeyboard/MRTK_SystemKeyboard_Main.png)

A Unity application can invoke the system keyboard at any time. Note that the system keyboard will behave according to the target platform's capabilities, for example the keyboard on HoloLens 2 would support direct hand interactions, while the keyboard on HoloLens (1st gen) would support GGV<sup>[1](https://docs.microsoft.com/en-us/windows/mixed-reality/gaze)</sup>.

## How to invoke the system keyboard ##

``` csharp
    public TouchScreenKeyboard keyboard;

    ...

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }
```

## How to read the input ##

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

## System keyboard example ##
You can see simple example of how to bring up system keyboard in 
[`OpenKeyboard.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/HandTracking/Script/OpenKeyboard.cs)
