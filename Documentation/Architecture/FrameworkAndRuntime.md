# Framework and runtime

## Changes to the scene

In order to use any of the MRTK functionality, you need to have a MixedRealityToolkit object in the
scene graph, whose script handles the initialization, runtime updates, and teardown of all of the
MRTK functionality. This is the object that gets added to the scene when you use the menu option:
Mixed Reality Toolkit -> Add to Scene and Configure.

Apart form adding the MRTK gameobject to the scene the menu option will also:

- Add the MixedRealityPlayspace, which is used by many other MRTK components to reason over
  world and local space transformations.
- Move the main Camera as a child of the MixedRealityPlayspace (and also adding some input and gaze
  related scripts to the main Camera, which help power UnityUI and gaze related input
  functionality).

## MixedRealityToolkit object and runtime

The MRTK contains a multitude of different services that spin up at runtime and talk with each other (or don't talk
with each other), and otherwise have some degree of coordination requirement in their startup, registration, update,
and teardown loops. [This medium post](https://medium.com/@stephen_hodgson/the-mixed-reality-framework-6fdb5c11feb2)
contains some of the background and motivation for why the system ended up written as it was. MRTK has a single
object that manages life and runtime of its services.

This entity ensures that:
- when the game starts, discovery and initialization of services happens in a pre-defined order.
- it provides a mechanism for services to register themselves (i.e. “I support this service!”) and for other
  callers to get a hold of those services.
- it provides the Update()/LateUpdate() calls and forwards them onto the various services
  (i.e. via UpdateAllServices/LateUpdateAllServices).
