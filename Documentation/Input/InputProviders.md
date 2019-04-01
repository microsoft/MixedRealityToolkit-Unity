# Input Providers

We currently support the following input providers with their corresponding controllers:

Input Provider | Controllers
--- | ---
Input Simulation Service | Simulated Hand
Mouse Device Manager | Mouse
OpenVR Device Manager | Generic OpenVR, Vive Wand, Vive Knuckles, Oculus Touch, Oculus Remote, Windows Mixed Reality OpenVR
Unity Joystick Manager | Generic Joystick
Unity Touch Device Manager | Unity Touch Controller
Windows Dictation Input Provider | *
Windows Mixed Reality Device Manager | WMR Articulated Hand, WMR Controller, WMR GGV Hand
Windows Speech Input Provider | *

\* Dictation and Speech providers don't create any controllers, they raise their own specialized input events directly.

<img src="../../External/ReadMeImages/Input/RegisteredServiceProviders.png" style="max-width:100%;">

<sup>Registered Service Providers Profile. Found in the Mixed Reality Toolkit component, is the place to go to add or remove input providers.</sup>