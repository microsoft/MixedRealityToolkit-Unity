# Configuring the Diagnostics System

The diagnostics system profile provides options for configuring:
- [General Settings](#general-settings)
- [Profiler Settings](#profiler-settings)

## General Settings

![Diagnostics General Settings](../../External/ReadMeImages/Diagnostics/DiagnosticsGeneralSettings.png)

### Show Diagnostics

Indicates whether or not the diagnostics system is to display the configured diagnostic options.

> When disabled, all configured diagnostic options will be hidden.

## Profiler Settings

![Diagnostics Profiler Settings](../../External/ReadMeImages/Diagnostics/DiagnosticsProfilerSettings.png)

### Show Profiler

Indicates whether or not the Visual Profiler is to be displayed.

### Frame Sample Rate

The amount of time, in seconds to collect frames for frame rate calculation. The range is 0 to 5 seconds.

### Window Anchor

To what portion of the view port should the profiler window be anchored. The default value is Lower Center.

### Window Offset

The offset, from the center of the view port, to place the Visual Profiler. The offset will be in the direction
of the [Window Anchor](#window-anchor).

### Window Scale

Size multiplier applied to the profiler window. For example, setting the value to 2 will double the window size.

### Window Follow Speed

The speed at which to move the profiler window to maintain visibility within the view port.

## See Also

- [Diagnostics System](DiagnosticsSystemGettingStarted.md)
- [Using the Visual Profiler](UsingVisualProfiler.md)
