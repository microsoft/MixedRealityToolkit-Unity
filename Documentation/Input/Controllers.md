# Controllers

Controllers are created and destroyed automatically by [**input providers**](InputProviders.md). Each controller type has a number of *physical inputs* defined by an *axis type*, telling us the data type of the input value (Digital, Single Axis, Dual Axis, Six Dof, ...), and an *input type* (Button Press, Trigger, Thumb Stick, Spatial Pointer, ...) describing the origin of the input. Physical inputs are mapped to *input actions* via in the **Controller Input Mapping Profile**, under the *Input System Profile* in the Mixed Reality Toolkit component.

<img src="../../External/ReadMeImages/Input/ControllerInputMapping.png" style="max-width:100%;">