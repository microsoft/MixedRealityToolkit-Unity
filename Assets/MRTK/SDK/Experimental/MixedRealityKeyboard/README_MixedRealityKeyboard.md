# Mixed Reality and HoloLens Keyboard Helper Classes

MRTK provides several experimental helper components to assist with launching and reading text from the [System Keyboard](../../../../../Documentation/README_SystemKeyboard.md). 

Note that the system keyboard will behave according to the target platform's capabilities, for example the keyboard on HoloLens 2 would support direct hand interactions, while the keyboard on HoloLens (1st gen) would support GGV<sup>[1](https://docs.microsoft.com/windows/mixed-reality/gaze)</sup>. Additionally, the system keyboard will not show up when performing [Unity Remoting](../../../../../Documentation/Tools/HolographicRemoting.md) from the editor to a HoloLens.


## MixedRealityKeyboard
[`MixedRealityKeyboard`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.MixedRealityKeyboard) is a component that provides methods for launching and closing a system keyboard, as well as interacting with text entered by the keyboard.  

### How to Use
1. Attach the [`MixedRealityKeyboard`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.MixedRealityKeyboard) component to any object.
2. Call `Show()` `Hide()` to show and hide the keyboard, and handle the `OnShowKeyboard`, `OnHideKeyboard` and `OnCommitText` events to handle when the keyboard is shown, hidden, and when the enter key is pressed.

## Input fields TMP_KeyboardInputField, and UI_KeyboardInputField
The [`TMP_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.TMP_KeyboardInputField) and [`UI_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.UI_KeyboardInputField) classes are components that can be added to text input fields to automatically invoke the system keyboard when clicked and update the text input field contents as the user enters text.

### How to use
1. Create an input field for either UnityUI or TextMeshPro.
2. Add the corresponding [`TMP_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.TMP_KeyboardInputField) or [`UI_KeyboardInputField`](xref:Microsoft.MixedReality.Toolkit.Experimental.UI.UI_KeyboardInputField) component to the input field game object.

Prefabs for both UnityUI input fields and TextMeshPro (TMPro) input fields are available at "Assets\MRTK\Experimental\MixedRealityKeyboard\Prefabs"

An example of how the to use TMP_KeyboardInputField and UI_KeyboardInputField is at "Assets\MRTK\Examples\Experimental\MixedRealityKeyboard\Scenes\MixedRealityKeyboardExample.unity"
