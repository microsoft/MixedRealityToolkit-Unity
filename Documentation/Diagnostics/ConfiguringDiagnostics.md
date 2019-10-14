# Configuring the Diagnostics System

## General Settings

![Diagnostics General Settings](../../Documentation/Images/Diagnostics/DiagnosticsGeneralSettings.png)

**Show Diagnostics**

Indicates whether or not the diagnostics system is to display the configured diagnostic options.

When disabled, all configured diagnostic options will be hidden.

## Profiler Settings

![Diagnostics Profiler Settings](../../Documentation/Images/Diagnostics/DiagnosticsProfilerSettings.png)

**Show Profiler**

Indicates whether or not the Visual Profiler is to be displayed.

**Frame Sample Rate**

The amount of time, in seconds to collect frames for frame rate calculation. The range is 0 to 5 seconds.

**Window Anchor**

To what portion of the view port should the profiler window be anchored. The default value is Lower Center.

**Window Offset**

The offset, from the center of the view port, to place the Visual Profiler. The offset will be in the direction of the *Window Anchor* property.

**Window Scale**

Size multiplier applied to the profiler window. For example, setting the value to 2 will double the window size.

**Window Follow Speed**

The speed at which to move the profiler window to maintain visibility within the view port.

## Programmatically controlling the Diagnostics System

It's also possible to toggle the visibility of the diagnostics system and the profiler at runtime. For example, the code below will hide the diagnostics system and profiler.

```C#
CoreServices.DiagnosticsSystem.ShowDiagnostics = false;

CoreServices.DiagnosticsSystem.ShowProfiler = false;
```

## See Also

- [Diagnostics System](DiagnosticsSystemGettingStarted.md)
- [Using the Visual Profiler](UsingVisualProfiler.md)
