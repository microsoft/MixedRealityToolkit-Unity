# Introduction 
Blend is a collection of lerping scripts that effect the transform or visual propterties of an object, such as: color, position, rotation and/or scale. 

# Getting Started
With the default setting, starting a blend requires two lines of code.
## Properties
    TargetValue (float, Color, Vector3, Vector4)
	Example: GetComponent<BlendColor>().TargetValue = Color.Red;
	
## Methods
    Play()
	Example: GetComponent<BlendColor>().Play();
	
## For more advanced settings
1.	Add the Blend package to your project
2.  Set the TargetValue in the inspector or through code: TargetValue = {float, Color, Vector3, Vector4}
3.  Set the LerpType
	-  Timed: Maually start the Blend which strictly adheres to the LerpTime and the EaseCurve specified.
	-  Free: Automatically starts lerping whenever the TargetValue changes, loosely adhering to the LerpTime with an exagerated EaseOut
4.	Choose an EaseCurve or create a custom curve. Curves should cover a range of 0-1.
    -  Default is a linear lerp, which is good for fades, color changes and continuous looping rotation.
	-  EaseOut is used most of the time, easpecially for showing or assembling elements, quick in the beginning and slow in the end, **the curve looks like a hill or mound**.
	-  EaseIn is used for a rocket effect, slow at the start and accellorates over time. These are good for deconstructing. **The curve looks like a ramp**.
	-  EaseInOut curve looks like an "S" shape and is slow in the beginning and end of the lerp.
5.  Setting IsPlaying to True in the inspector will make the blend auto start. **This method should only be used to auto-start**.
    -  Use Play() to start the blend during run-time.
6.  Select the LoopType if one is desired.
    -  None: default, no looping
	-  Repeat: the Loop starts at the beginning
	-  PingPong: the Loop is reversed each time
7.  Add an event for OnComplete.