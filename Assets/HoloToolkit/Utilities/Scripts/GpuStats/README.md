# GpuStats
Use the GpuStats component measure (on non-tiled GPUs) time of GPU calls.
Add the GpuTimingCamera component to the camera you want to track the total GPU frame time of.
The GpuTimingCamera.NewGpuFrameTime event will fire before the next frame rendering begins to provide the GPU timing of the previous frame.
