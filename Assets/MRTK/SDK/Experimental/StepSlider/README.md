# Step Slider
A Pinch Slider with set number of step points. 

## Example scene
You can find examples in the **StepSliderExample** scene under:
[MRTK/Examples/Experimental/StepSlider]

## How to use Step Slider
MRTK provides a StepSlider prefab located under: [MRTK/SDK/Experimental/StepSlider]

You can set the number of steps on the slider in the inspectors through the StepSliderDivisions field. 
When StepSliderDivisions is not set by the user, it defaults to 0 and the StepSlider behaves like a regular Pinch Slider.
If you set a custom slider value in the inspector, it will be adjusted to the closest step value.
For example, 1 step point would enable 0 and 1 as values
2 step points would enable 0, 0.5 and 1 as values

In the StepSlider scene:
The Red slider has a 7 divisions
The Green slider has 10 divisions
The Blue slider has 1 division

For more details, please see `StepSlider.cs` in StepSliderExample.unity scene.