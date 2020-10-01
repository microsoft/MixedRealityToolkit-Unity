// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes
{
    /// <summary>
    /// Enum which describes how an object's bounds control is to be flattened.
    /// </summary>
    public enum FlattenModeType
    {
        DoNotFlatten = 0,
        /// <summary>
        /// Flatten the X axis
        /// </summary>
        FlattenX,
        /// <summary>
        /// Flatten the Y axis
        /// </summary>
        FlattenY,
        /// <summary>
        /// Flatten the Z axis
        /// </summary>
        FlattenZ,
        /// <summary>
        /// Flatten the smallest relative axis if it falls below threshold
        /// </summary>
        FlattenAuto,
    }

    /// <summary>
    /// Enum describing the type of handle grabbed; can be a rotation (edge-mounted)
    /// handle, a scaling (corner-mounted) handle, or a translation (face-mounted)
    /// handle.
    /// </summary>
    public enum HandleType
    {
        None = 0,
        Rotation,
        Scale,
        Translation
    }

    /// <summary>
    /// This enum describes which primitive type the wireframe portion of the bounds control
    /// consists of. 
    /// </summary>
    /// <remarks>
    /// Wireframe refers to the thin linkage between the handles. When the handles are invisible
    /// the wireframe looks like an outline box around an object.
    /// </remarks> 
    public enum WireframeType
    {
        Cubic = 0,
        Cylindrical
    }

    /// <summary>
    /// This enum defines what volume type the bound calculation depends on and its priority
    /// for it.
    /// </summary>
    public enum BoundsCalculationMethod
    {
        /// <summary>
        /// Used Renderers for the bounds calculation and Colliders as a fallback
        /// </summary>
        RendererOverCollider = 0,
        /// <summary>
        /// Used Colliders for the bounds calculation and Renderers as a fallback
        /// </summary>
        ColliderOverRenderer,
        /// <summary>
        /// Omits Renderers and uses Colliders for the bounds calculation exclusively
        /// </summary>
        ColliderOnly,
        /// <summary>
        /// Omits Colliders and uses Renderers for the bounds calculation exclusively
        /// </summary>
        RendererOnly,
    }

    /// <summary>
    /// This enum defines how the bounds control gets activated
    /// </summary>
    public enum BoundsControlActivationType
    {
        ActivateOnStart = 0,
        ActivateByProximity,
        ActivateByPointer,
        ActivateByProximityAndPointer,
        ActivateManually
    }

    /// <summary>
    /// This enum defines the type of collider in use when no handle prefab is provided.
    /// </summary>
    public enum HandlePrefabCollider
    {
        Sphere,
        Box
    }

    /// <summary>
    /// This enum defines which of the axes a given rotation handle revolves about.
    /// </summary>
    public enum CardinalAxisType
    {
        X = 0,
        Y,
        Z
    }

    /// <summary>
    /// Scale mode that is used for scaling behavior of bounds control.
    /// </summary>
    public enum HandleScaleMode
    {
        /// <summary>
        /// Control will be scaled uniformly.
        /// </summary>
        Uniform,
        /// <summary>
        /// Scales non uniformly according to movement in 3d space.
        /// </summary>
        NonUniform
    }

    /// <summary>
    /// Enum describing faces of a bounds control box
    /// </summary>
    internal enum Face
    {
        ForwardX    = 0,
        BackwardX   = 1,
        ForwardY    = 2,
        BackwardY   = 3,
        ForwardZ    = 4,
        BackwardZ   = 5,
    }

    /// <summary>
    /// Enum describing edges / links of a bounds control box
    /// </summary>
    internal enum Edges
    {
        FrontBottom = 0,
        FrontLeft   = 1,
        FrontTop    = 2,
        FrontRight  = 3,
        BackBottom  = 4,
        BackLeft    = 5,
        BackTop     = 6,
        BackRight   = 7,
        BottomLeft  = 8,
        BottomRight = 9,
        TopLeft     = 10,
        TopRight    = 11,
    }
}
