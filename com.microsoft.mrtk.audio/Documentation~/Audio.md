# Microsoft Mixed Reality Toolkit Audio Effects

The audio effects package contains features that enable developers to enhance the immersion of their experiences by applying
audio effects, such as occlusion.

## Features

### BandPass Effect

Adding `AudioBandPassEffect` to a sound emitting object provides a simple mechanism to customize the range of allowed frequencies. One
use of this effect is to simulate the the sound of an AM Radio.

### Occlusion

Audio occlusion is the effect of being outside of a space in which audio is playing. In the physical world, different materials (ex: wood, concrete, etc.) will limit the maximum frequency that can be transmitted through them. They also can impact the perceived volume of the sound. By applying the `AudioOcclusion` script to an object that is between the user and the sound emitter will simulate these effects.
