# Solver

![Solver](../Documentation/Images/Solver/MRTK_Solver_Main.png)

TODO: Reword intro. Calculate position & orientation and to also define determinaticaly order of operations

At this time, there is no easy, reliable way to specify the update order for Unity components. This can make it difficult to implement deterministic compound transformations. The solver system addresses this issue.

Building on this, solvers offer a range of behaviors to attach objects to other objects or systems. One example would be a tag-along object that hovers in front of the user (based on the camera). A solver could also be attached to a controller and an object to make the object tag-along the controller. All solvers can be safely stacked, for example a tag-along behavior + surface magnetism + momentum.

## How to use a solver

First, add the desired behavior to an object by using the respective solver type. 

The Solver system consists of three categories of scripts:

* [`Solver`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.Solver): The base abstract class that all solvers derive from. It provides state tracking, smoothing parameters and implementation, automatic solver system integration, and update order.
* [`SolverHandler`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.SolverHandler): Sets the reference object to track against (ex: the main camera transform, hand ray, etc.), handles gathering of solver components, and executes updating them in the proper order.

The third category is the solver itself. The following solvers provide the building blocks for basic behavior:

* [`Orbital`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.Orbital): Locks to a specified position and offset from the referenced object.
* [`ConstantViewSize`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.ConstantViewSize): Scales to maintain a constant size relative to the view of the referenced object.
* [`RadialView`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.RadialView): Keeps the object within a view cone cast by the referenced object.
* [`SurfaceMagnetism`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.SurfaceMagnetism): casts rays to surfaces in the world, and align the object to that surface.
* [`Momentum`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.Momentum): Applies acceleration/velocity/friction to simulate momentum and springiness for an object being moved by other solvers/components.
* [`InBetween`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.InBetween): Keeps an object in between two tracked objects.

## How to change tracking reference

The `TrackedTargetType` of the [`SolverHandler`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.SolverHandler) component defines the point of reference all solvers will use to calculate their algorithms. For example, a value type of `Head` (xref:Microsoft.MixedReality.Toolkit.Utilities.TrackedObjectType.Head) with a simple [`SurfaceMagnetism`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.SurfaceMagnetism) component will result in a raycast from the head and in the direction of the user's gaze for solving what surface is hit. Potential values for the `TrackedTargetType` property are:

- *Head* : Point of reference from the camera
- *MotionController*: Point of reference from motion controller. Use `TrackedHandedness` property to define details
- *HandJoint*: Point of reference from hand. Use `TrackedHandJoint` and `TrackedHandedness` properties to define details
- *CustomOverride*: Point of reference from assigned `TransformOverride`

> [!NOTE]
> For both *MotionController* and *HandJoint*, the solver handler will attempt to provide the left controller/hand transform first and then the right if the former is not available or unless the `TrackedHandedness` property specifies otherwise.

// ADD gif of changing solverhandler

## How to use chain solvers

When *UpdateLinkedTransform* is true, the solver will calculate position & orientation, but will not apply it. This lets other components use the transform values.

## How to use the InBetween


## Common misteps


### SurfaceMagnetism

When adding a [`SurfaceMagnetism`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.SurfaceMagnetism) component to a GameObject, it is important to consider the layer of the GameObject and it's children, if any have colliders. The component works by performing various types of raycast to determine what surface to "magnet" itself against. If the GameObject has a collider on one of the layers listed in the `MagneticSurfaces` property of SurfaceMagnetism, then the raycast will likely hit itself resulting in the GameObject attaching to it's own collider point. This odd behavior can be avoided by setting the main GameObject and all children to the *Ignore Raycast* layer or modifying the `MagneticSurfaces` LayerMask array appropriately. 

Conversely, a [`SurfaceMagnetism`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.SurfaceMagnetism) GameObject will not collide with surfaces on a layer not listed in the `MagneticSurfaces` property. 

Finally, surfaces farther than the `MaxRaycastDistance` property setting will be ignored by the Surface Magnetism raycasts. 



When a solver is used, the [`SolverHandler`](xref:Microsoft.MixedReality.Toolkit.Utilities.Solvers.SolverHandler) will be added automatically. It has two fields for setting the reference object. You can choose a tracked object (such as the user camera or L/R motion controllers), or instead use the **TransformTarget** field which overrides any set tracked object. This enables you to have solvers reference any scene object. That means objects can have tag alongs and cast surface magnetism as well as tracked objects.

The surface magnetism scripts rely on a *LayerMask* for raycasting. As a recommendation, create a custom layer mask (the example uses *Surface*). Note that using *default* or *everything* will result in UI components or cursors contributing to the solver. Note that this can produce weird and unexpected behavior, for example objects lurching toward the user and so on.

<img src="../Documentation/Images/Solver/MRTK_Solver_Orbital.png" width="450">

*Example of using Orbital solver in the [Slate](README_Slate.md) prefab.*

## Expectations for extending or adding to the solver system

To create a new solver script, you will want to extend from the abstract base class, *Solver*. This will ensure your new solver will tie into the state tracking and execution from both Solver and SolverHandler. The power with solvers comes from them being modular in nature, so as you extend Solver, keep this in mind. Its better to have many small solver scripts vs. one big one.

## Known Issues

Sometimes solvers behave differently than one may expect based on the order in which they're executed. Previous solvers can change or even neutralize the behavior of earlier solvers. Try re-arranging their execution order if the settings on a particular solver aren't having the desired effect.
