# Spectator View Debugging

## Troubleshooting the [MarkerSpatialCoordinateService](xref:Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Sharing.MarkerSpatialCoordinateService)

If Unity scene content feels poorly aligned in the physical world across devices, there is some built in functionality within the Marker Spatial Coordinate Service for showing debug visuals. Check 'Show Debug Visuals' in the unity editor for [MarkerSpatialCoordinateService](xref:Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Sharing.MarkerSpatialCoordinateService) and recompile to enable this functionality.

### Main User (HoloLens) Debug Visuals
On the HoloLens device, debug visuals will be shown at the following locations:
1. At the shared scene origin
2. At all locations markers are originally detected
3. At all spectator device (secondary device) Unity camera locations

### Spectator (secondary device) Debug Visuals
On a spectator device (secondary device), debug visuals will be shown at the following locations:
1. At the shared scene origin
2. At the User HoloLens's Unity camera location

### Interpreting Debug Visuals
Observing the shared scene origin on both the HoloLens and spectator device can provide a quick understanding of how well aligned scenes are across devices. Because devices only calibrate their location with one another during marker detection, there can be some drift. 

Using the HoloLens to view where markers have been detected when initially calibrating can also provide insights into any marker detection regressions.

>Note: Spectator devices and HoloLenses may have different Unity camera definitions. Debug visuals tracking HoloLens and spectator devices should only be used for general position and orientation information.
