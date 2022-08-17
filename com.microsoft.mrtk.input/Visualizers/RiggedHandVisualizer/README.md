## Why are there three different shaders + materials for the rigged hand?

URP doesn't support multipass shaders, unfortunately! So we have to split the three passes
we need into three separate shaders + materials, and then use material chaining on the
rigged hand to run all three passes, even on URP!

The alternative solution on URP is to create special render passes in the pipeline object,
but that's a project-specific change and we'd need to somehow automate that, which is nasty.

Three materials/shaders is a bit cluttered, but it works nicely across all platforms and
render pipelines!

## Where did this hand mesh come from?

It's a model that we've had internally for a while (since the early days of MRTK!) It's been
re-rigged and painted with the correct vertex colors for MRTK3.

## I want hands with a different shader/color/material/model/etc!

Great! You can build your own hand model prefab with your own custom mesh or materials, and
still use the RiggedHandMeshVisualizer.cs script. The script expects joints/armatures in the
default Blender format, following OpenXR joint conventions.