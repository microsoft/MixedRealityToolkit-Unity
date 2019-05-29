# Debug Logging

Majority of the Spectator View related classes will have 'Debug Logging' flags that can be toggled on and off through the Unity inspector. Enabling these flags results in richer state information being output to log files as well as debug consoles. It's suggested to enable all of these flags when debugging Spectator View related issues.

# Show Debug Visuals

The [SpatialCoordinateSystemManager](Scripts/Sharing/SpatialCoordinateSystemManager.cs) has a 'Show Debug Visuals' flag. When enabled, debug visual game objects will be placed at each detected spatial coordinate. Viewing these markers across different devices can assist in understanding if transforms have been applied incorrectly to spectator cameras. It can also demonstrate how accurate different devices were when detecting spatial coordinates.
