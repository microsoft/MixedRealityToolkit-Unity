# AdaptiveQuality
Use the AdaptiveQuality component to measure GPU time and dynamically lower or raise the quality level of the app, and reports any changes via events.
By listening to these events, the app can for instance choose to enable/disable expensive components based on the reported quality level to help maintain a steady framerate.

AdaptiveViewport listens to the AdaptiveQuality component for changes in quality and adjusts the viewport according to specified data.
