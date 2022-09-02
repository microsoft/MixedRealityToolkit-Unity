# Unified UX

## Why can't I preview my animations on the object?

This is because StateVisualizer does not use AnimatorControllers, for performance + resilience. Instead, it uses the Unity Playables API to directly inject animation data into the Animator from code. Think of `StateVisualizer` as a more performant + code-driven + maintainable AnimatorController replacement.

If you would like to author/edit animations in Edit mode and see them previewed on the Object, simply assign the provided `TestAuthoringController` to the Animator. Author your animations, and then remove the `TestAuthoringController` from the Animator. Assign your animations to the relevant slots in the `StateVisualizer`, and enjoy performant non-Animator-Controller-based state feedback!

## Why can't StateVisualizer also do this previewing behavior?

The Playables graph is constructed at runtime. A future update may implement edit-time Playable graph construction, but Unity may limit the usefulness of this feature.

## Can I still use my own AnimatorController?

Right now, `StateVisualizer` overrides the control data flowing to the Animator. In a future update, it will be configurable to blend with existing AnimatorController behaviors.