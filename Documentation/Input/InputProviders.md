# Input Providers

Input providers are registered in the **Registered Service Providers Profile**, found in the Mixed Reality Toolkit component:

<img src="../../External/ReadMeImages/Input/RegisteredServiceProviders.png" style="max-width:100%;">

These are the input providers available out of the box, together with their corresponding controllers:

Input Provider | Controllers
--- | ---
Input Simulation Service | Simulated Hand
Mouse Device Manager | Mouse
OpenVR Device Manager | Generic OpenVR, Vive Wand, Vive Knuckles, Oculus Touch, Oculus Remote, Windows Mixed Reality OpenVR
Unity Joystick Manager | Generic Joystick
Unity Touch Device Manager | Unity Touch Controller
Windows Dictation Input Provider | *None* *
Windows Mixed Reality Device Manager | WMR Articulated Hand, WMR Controller, WMR GGV Hand
Windows Speech Input Provider | *None* *

\* Dictation and Speech providers don't create any controllers, they raise their own specialized input events directly.

Custom input providers can be created implementing the [`IMixedRealityInputDeviceManager`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputDeviceManager) interface.