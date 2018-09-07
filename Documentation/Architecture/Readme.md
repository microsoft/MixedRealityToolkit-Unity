# Architecture Overview of the Mixed Reality Toolkit for Unity

The Mixed Reality Toolkit is intended to abstract the specific implementation details of platforms, such as the Microsoft HoloLens, that provides platform agnostic support for common systems.

Where possible, we are designing for common functionality well as access to the lowest level of available data.

It is expected that many platforms may not support one or more of the interfaces defined herein. In fact, some may not support some feature areas at all. On those platforms, the system must gracefully fail and provide the developer with appropriate data (null, empty collections, etc.) in return.

Each interface defined will implement one or more Properties, Methods and/or Events (PMEs) that can be accessed by application code.

TODO Add high level image of MRTK system architecture here

- [Core System (Mixed Reality Manager)](./Core_System/Core.md)
- [Input System](./Input_System/Input_System.md)
- [Boundary System](./Boundary_System/Boundary_System.md)
- [Teleport System](./Teleport_System/Teleport_System.md)