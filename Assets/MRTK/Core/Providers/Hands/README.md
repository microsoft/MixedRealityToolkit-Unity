# Hand devices

Base class for all hand devices, to act be implemented by all backends that want to provide hand input in MRTK. Simulated hands can be used if no hand tracking device is available.

## Simulated hands

Hand input can be simulated in Unity using the "In-Editor Input Simulation" service. This service is enabled by default.

When enabled, virtual left and/or right hands can be moved in the scene using the mouse. Clicking mouse buttons will perform gestures for interacting with objects.
Press shift to control the left hand and/or space to control the right hand. By pressing shift and space simultaneously both hands can be moved at the same time.

The hand simulation has two modes:

  1. Quick mode: Hands are shown only as long as they are moved by the mouse. This mode is useful for simple testing of buttons with a single hand.
  2. Persistent mode: Hands stay visible on the screen, even if they are not moved. This mode is useful for testing two-hand manipulation and accurate placement. Gestures are toggled on/off by mouse clicks.

The simulation mode can be switched for the left/right hand individually by pressing the T/Y keys respectively.

Detailed settings for hand simulation can be found in the SimulatedHandAPI prefab, which is instantiated through the hand tracking profile. This includes key bindings for hand movement and mouse button bindings for gestures.
