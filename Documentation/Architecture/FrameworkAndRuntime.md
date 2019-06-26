# Framework and runtime

## Changes to the scene

In order to use any of the MRTK functionality, you need to have a MixedRealityToolkit object in the
scene graph, whose script handles the initialization, runtime updates, and teardown of all of the
MRTK functionality. This is the object that gets added to the scene when you use the menu option:
Mixed Reality Toolkit -> Add to Scene and Configure.

The addition of the MRTK to the scene will also make a few more changes, beyond adding the
MixedRealityToolkit scene object:

- The addition of the MixedRealityPlayspace, which is used by many other MRTK components to reason over
  world and local space transformations.
- Moves the main Camera as a child of the MixedRealityPlayspace (and also adding some input and gaze
  related scripts to the main Camera, which help power UnityUI and gaze related input
  functionality).

## MixedRealityToolkit Object and Runtime

The MRTK contains a multitude of different services that spin up at runtime, that talk with each other (or don't talk
with each other), and otherwise have some degree of coordination requirement in their startup, registration, update,
and teardown loops. [This medium post](https://medium.com/@stephen_hodgson/the-mixed-reality-framework-6fdb5c11feb2)
contains some of the background and motivation for why the system ended up written as it was. Rather than deal with
the potential chaos of managing the startup, registration, and update dependencies between multiple components that
each have their own update loop, the MRTK ended up with a single object that would manage the lifetime and runtime
of each its services.

This entity ensures that:
- When the game starts, discovery and initialization of services happens in a pre-defined order.
- Providing mechanisms for services to register themselves (i.e. “I support this service!”) and for other callers to get a hold of those services.
- Wrapping the Update()/LateUpdate() calls and forwarding them onto the various services (i.e. via UpdateAllServices/LateUpdateAllServices)

Note that design may not have a lot of other examples with past Unity work, though it was also written this way with
the hopes that it could be more easily integrated in with future ECS/DOTS work that Unity is doing.

Note that there have been downsides to doing things this way, which have come in feedback around:

- Customer confusion about why there's a single 'monolithic' object that controls the lifetime of all objects
- Not all of the services actually talk to each other - coordiation can be great when it's required, but in a lot
  of our cases, they are relatively independent things that could have been spun off as separate MonoBehaviours.
- Some of the rationale for ensuring a single Update() loop was based on performance concerns (i.e. ensuring a
  single Update() MonoBehaviour call, instead of 6 different ones) where given the small number of services,
  the benefits were minor. While there may be a benefit to doing things in this way, there has been
  a corresponding increase to complexity.


