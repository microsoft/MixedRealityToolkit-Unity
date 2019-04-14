# Speech

Speech input providers, like *Windows Speech Input*, don't create any controllers but instead allow you to define keywords that will raise speech input events when recognized. The **Speech Commands Profile** in the *Input System Profile* is where you configure the keywords to recognize. For each command you can also:

- Select an **input action** to map it to. This way you can for example use the keyword *Select* to have the same effect as a left mouse click, by mapping both to the same action.
- Specify a **key code** that will produce the same speech event when pressed.
- Add a **localization key** that will be used in UWP apps to obtain the localized keyword from the app resources.

<img src="../../External/ReadMeImages/Input/SpeechCommands.png" style="max-width:100%;">

You can use the [**`Speech Input Handler`**](xref:Microsoft.MixedReality.Toolkit.Input.SpeechInputHandler) script to handle speech commands using [**UnityEvents**](https://docs.unity3d.com/Manual/UnityEvents.html). The *Speech* demo, in `MixedRealityToolkit.Examples\Demos\Input\Scenes\Speech`, shows how to do this. You can also listen to speech command events directly in your own script by implementing [`IMixedRealitySpeechHandler`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealitySpeechHandler) (see table of [event handlers](InputEvents.md)).