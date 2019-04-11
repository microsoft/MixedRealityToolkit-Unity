# System Keyboard
![System Keyboard](../External/ReadMeImages/SystemKeyboard/MRTK_SystemKeyboard_Main.png)

Inside Unity app, you can invoke the system keyboard. HoloLens2's system keyboard supports direct hand interactions.

## System Keyboard Example
You can see simple example of how to bring up system keyboard in `Assets\MixedRealityToolkit.Examples\Demos\HandTracking\Script\OpenKeyboard.cs`

## How to Invoke System Keyboard

    public TouchScreenKeyboard keyboard;
    ...

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

## How to read text typed

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