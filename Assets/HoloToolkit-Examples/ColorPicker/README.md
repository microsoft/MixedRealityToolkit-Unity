# ColorPicker Example
This example shows how to implement a gazeable color picker component. 
It can be tested straight from Unity thanks to the ManualCameraControl compoment (use 'Shift' to move the camera and simulate a tap with 'Space').

#### GazeableColorPicker:
The GazeableColorPicker component expect you to set the renderer component to use (usually where you have the color picker material). 

Then you can define Unity events for:
- when we gaze at a color (in this example we change the color of the 'PreviewColor Canvas renderer')
- when we pick a color (in this example we change the color of the 'SelectedColor Canvas renderer')

This is also a good example of how you can define your own dynamic Unity Event, something that isn't very well explained on Unity's documentation.
Here we simply call the CanvasRenderer's SetColor methods but we could in fact call any methods that takes a Color argument. 
