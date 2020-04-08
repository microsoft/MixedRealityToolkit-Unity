# Hand Menu

![Hand Menu UX Example](Images/Solver/MRTK_UX_HandMenu.png)

Hand menus allow users to quickly bring up hand-attached UI for frequently used functions. 

## Example Scene
**HandMenuExamples.unity** scene under ``MRTK/Examples/Demos/HandTracking/Scenes`` folder
![Hand Menu UX Example](Images/HandMenu/MRTK_HandMenu_ExampleScene.png)

## Hand Menu prefabs in MRTK
Examples of the hand menu prefabs are under ``MRTK/Examples/Demos/HandTracking/Prefabs`` folder

### Unity UI Image/Graphic based buttons

* `HandMenu_Small_HideOnHandDrop.prefab`
* `HandMenu_Medium_HideOnHandDrop.prefab`
* `HandMenu_Large_WorldLock_On_GrabAndPull.prefab`
* `HandMenu_Large_AutoWorldLock_On_HandDrop.prefab`

## Scripts

The [`HandConstraint`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.HandConstraint) behavior provides a solver that constrains the tracked object to a region safe for hand constrained content (such as hand UI, menus, etc). Safe regions are considered areas that don't intersect with the hand. A derived class of [`HandConstraint`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.HandConstraint) called [`HandConstraintPalmUp`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.HandConstraintPalmUp) is also included to demonstrate a common behavior of activating the solver tracked object when the palm is facing the user. For example use of this behavior please see the HandBasedMenuExample scene under `MRTK/Examples/Demos/HandTracking/Scenes/`.

Please see the tool tips available for each [`HandConstraint`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.HandConstraint) property for additional documentation. A few properties are defined in more detail below.

<img src="Images/Solver/MRTK_Solver_HandConstraintPalmUp.png" width="450">

* **Safe Zone**: The safe zone specifies where on the hand to constrain content. It is recommended that content be placed on the Ulnar Side to avoid overlap with the hand and improved interaction quality. Safe zones are calculated by taking the hands orientation projected into a plane orthogonal to the camera's view and raycasting against a bounding box around the hands. Safe zones are defined to work with [`IMixedRealityHand`](xref:Microsoft.MixedReality.Toolkit.Input.IMixedRealityHand) but also works with other controller types. It is recommended to explore what each safe zone represents on different controller types.

* **Follow Hand Until Facing Camera** With this active, solver will follow hand rotation until the menu is sufficiently aligned with the gaze, at which point it faces the camera. This works by changing the SolverRotationBehavior in the HandConstraintSolver, from LookAtTrackedObject to LookAtMainCamera as the GazeAlignment angle with the solver varies.

<img src="Images/Solver/MRTK_Solver_HandConstraintSafeZones.png" width="450">

* **Activation Events**: Currently the [`HandConstraint`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.HandConstraint) triggers four activation events. These events can be used in many different combinations to create unique [`HandConstraint`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.HandConstraint) behaviors, please see the HandBasedMenuExample scene under `MRTK/Examples/Demos/HandTracking/Scenes/` for examples of these behaviors.

    * *OnHandActivate*: triggers when a hand satisfies the IsHandActive method
    * *OnHandDeactivate*: triggers when the IsHandActive method is no longer satisfied.
    * *OnFirstHandDetected*: occurs when the hand tracking state changes from no hands in view, to the first hand in view.
    * *OnLastHandLost*: occurs when the hand tracking state changes from at least one hand in view, to no hands in view.

## See also

* [Button](README_Button.md)
* [Near Menu](README_NearMenu.md)
