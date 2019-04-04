<!-- ![MRTK](/External/ReadMeImages/EyeTracking/MRTK_et_placeholder.png) -->
# Eye Tracking in the MixedRealityToolkit

The Mixed Reality Toolkit supports HoloLens 2 which offers Eye Tracking input. 
MRTK offers several examples for how to utilize Eye Tracking in your applications.
Eye Tracking enables users to quickly and effortlessly engage with holograms across their view.
Below you can find an overview of several powerful examples on how to use Eye Tracking in your app. 

You can directly build on the samples provided with MRTK which are stored in the following folder:
[\Assets\MixedRealityToolkit.Examples\Demos\EyeTracking](/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking)

If you want to start from a new Unity scene, check out the instructions on [Basic MRTK Setup to use Eye Tracking](/Documentation/EyeTracking/EyeTracking_BasicSetup.md).

<br>


## Overview of our Eye Tracking Input Tutorials

[**Eye-Supported Target Selection**](/Documentation/EyeTracking/EyeTracking_TargetSelection.md)

This tutorial showcases the ease of using GazeProvider to access smoothed eye gaze data and eye gaze specific events to select targets. Several examples are shown 
for subtle yet powerful feedback such as blending in/out visual highlights or holograms slowly turning towards the user when being looked at and notifications 
disappearing after they are read.

**Summary**: Fast and effortless target selections using a combination of Eyes+Voice and Eyes+Hands.

<br>


[**Eye-Supported Navigation**](/Documentation/EyeTracking/EyeTracking_Navigation.md)

Imagine you’re reading information on a slate and when you reach the end of the displayed text, the text automatically scrolls up to reveal more content. 
Or you can fluently zoom in where you’re looking at and that map automatically adjusts the content when you get closer to the border to keep your looked at content in view. 
These are some of the examples showcased in this tutorial about eye-supported navigation.
Another interesting application for hands-free observation of 3D holograms is automatically turning looked at aspects of your hologram to the front.  

**Summary**: Scroll, Pan, Zoom, 3D Rotation using Eyes+Voice and Eyes+Hands.

<br>


[**Eye-Supported Positioning**](/Documentation/EyeTracking/EyeTracking_Positioning.md)

In this tutorial, we showcase a popular input scenario called “Put that there” based on research work from Bolt in the early 1980s. 
The idea is simple: Benefit from your eyes for fast target selection and positioning. 
If refinement is required, use additional input from your hands, voice or controllers. 

**Summary**: Positioning holograms using Eyes+Voice & Eyes+Hands (*drag-and-drop*). Eye-supported sliders using Eyes+Hands. 

<br>


[**Visualization of Visual Attention**](/Documentation/EyeTracking/EyeTracking_Visualization.md)

Information about where users looked at is an immensely powerful tool to assess work streams and improve search patterns. 
In this tutorial, we discuss different eye tracking visualizations and how they fit different needs. 
We provide you with examples for logging and loading eye tracking data and examples for how to visualize them. 

**Summary**: Two-dimensional attention map (heatmaps) on slates. Recording & Replaying Eye Tracking data.


## To show or not to show an Eye Cursor?
For your HoloLens 2 apps, we recommend to *not* show an eye cursor, as this has shown to easily distract users and break the magic for instinctively having a system react to your intentions.
However, in some situations having the option to turn on an eye cursor is very helpful for identfying why the system is not reacting as expected. 

<br>
--- 
