# Mixed Reality Toolkit - SDK - UX - Tooltips support

As part of the Mixed Reality Toolkit SDK, we provide scripts / controls for managing and implementing tooltips in your Mixed Reality project.

Currently we provide components for:

## Tooltips

Manages the bulk of the tooltip display, content sizing and anchor behaviors. Also contains the text field for what a tooltip's text field should convey.

* Provides a field to show the connected Line Data Provider (see Lines section)

## Lines

Mixed Reality Toolkit has a line concept built on top of Unity's LineRenderer concept allowing for more expressive lines.
Begin with a LineDataProvider (Such as SimpleLine, Spline, or Parabola). A tooltip will look for a LineDataProvider automatically and then attempt to use it to connect the tooltip to the indicated Anchor.
The line appearance and behavior can be styled and controlled in a MixedRealityLineRenderer component.

Note: Not all LineDataProviders will work with Tooltips in a predictable manner.

* SimpleLine, Spline, ParabolaConstrained are the reasonable examples (and a prefab for each exists)
* Ellipse, Rectangle, ParabolaLine and ParabolaPhysicalLine tend to be less reliable or produce non-useful results.

### TooltipSpawner

A TooltipSpawner is a mostly standalone script that receives Focus or Input events (assuming it has a collider) that can be used to instantiate a tooltip at a particular location with certain behaviors.
You can control the appear and vanish conditions as well as how it responds to input.

If you are using a TooltipSpawner, customize the tooltip prefab in the [Tool Tip Prefab] field to change the line/tooltip appearance.
