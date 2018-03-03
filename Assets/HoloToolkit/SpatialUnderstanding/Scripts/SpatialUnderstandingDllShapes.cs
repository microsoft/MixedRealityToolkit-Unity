// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Encapsulates the shape detection queries of the understanding DLL.
    /// Shapes are defined by the user with AddShape and the analysis is 
    /// initiated with ActivateShapeAnalysis. These queries will not be 
    /// valid until after scanning is finalized.
    /// 
    /// Shape definitions are composed of a list of components and a list
    /// of shape constraints which defining requirements between the 
    /// components. Each component is defined by a list of its own shape 
    /// component constraints.
    /// </summary>
    public static class SpatialUnderstandingDllShapes
    {
        /// <summary>
        /// Result structure returned by shape queries
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ShapeResult
        {
            public Vector3 position;
            public Vector3 halfDims;
        };

        /// <summary>
        /// Types of shape component constraints
        /// </summary>
        public enum ShapeComponentConstraintType
        {
            SurfaceNotPartOfShape,

            SurfaceHeight_Min,
            SurfaceHeight_Max,
            SurfaceHeight_Between,
            SurfaceHeight_Is,

            SurfaceCount_Min,
            SurfaceCount_Max,
            SurfaceCount_Between,
            SurfaceCount_Is,

            SurfaceArea_Min,
            SurfaceArea_Max,
            SurfaceArea_Between,
            SurfaceArea_Is,

            IsRectangle,
            RectangleSize_Min,
            RectangleSize_Max,
            RectangleSize_Between,
            RectangleSize_Is,

            RectangleLength_Min,
            RectangleLength_Max,
            RectangleLength_Between,
            RectangleLength_Is,

            RectangleWidth_Min,
            RectangleWidth_Max,
            RectangleWidth_Between,
            RectangleWidth_Is,

            IsSquare,
            SquareSize_Min,
            SquareSize_Max,
            SquareSize_Between,
            SquareSize_Is,

            IsCircle,
            CircleRadius_Min,
            CircleRadius_Max,
            CircleRadius_Between,
            CircleRadius_Is,
        };

        /// <summary>
        /// A shape component constraint. This includes its type enum and 
        /// its type specific parameters.
        /// 
        /// Static construction functions contained in this class can be used
        /// to construct a list of component constraints.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ShapeComponentConstraint
        {
            /// <summary>
            /// Constructs a constraint requiring the component to not be a part of a specified shape
            /// </summary>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceNotPartOfShape(string shapeName)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceNotPartOfShape;
                constraint.Param_Str_0 = SpatialUnderstanding.Instance.UnderstandingDLL.PinString(shapeName);
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be a minimum height above the floor
            /// </summary>
            /// <param name="minHeight">Minimum height above the floor</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceHeight_Min(float minHeight)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceHeight_Min;
                constraint.Param_Float_0 = minHeight;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be a maximum height above the floor
            /// </summary>
            /// <param name="maxHeight">Maximum height above the floor</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceHeight_Max(float maxHeight)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceHeight_Max;
                constraint.Param_Float_0 = maxHeight;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be within a height range above the floor
            /// </summary>
            /// <param name="minHeight">Minimum height above the floor</param>
            /// <param name="maxHeight">Maximum height above the floor</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceHeight_Between(float minHeight, float maxHeight)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceHeight_Between;
                constraint.Param_Float_0 = minHeight;
                constraint.Param_Float_1 = maxHeight;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be a specific height above the floor
            /// </summary>
            /// <param name="height">Required height above the floor</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceHeight_Is(float height)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceHeight_Is;
                constraint.Param_Float_0 = height;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be a minimum number of discrete flat surfaces
            /// </summary>
            /// <param name="minCount">Minimum number of discrete surfaces</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceCount_Min(int minCount)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceCount_Min;
                constraint.Param_Int_0 = minCount;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be a maximum number of discrete flat surfaces
            /// </summary>
            /// <param name="maxCount">Maximum number of discrete surfaces</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceCount_Max(int maxCount)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceCount_Max;
                constraint.Param_Int_0 = maxCount;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be a composed of a number of 
            /// discrete flat surfaces between a specified range
            /// </summary>
            /// <param name="minCount">Minimum number of discrete surfaces</param>
            /// <param name="maxCount">Maximum number of discrete surfaces</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceCount_Between(int minCount, int maxCount)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceCount_Between;
                constraint.Param_Int_0 = minCount;
                constraint.Param_Int_1 = maxCount;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be a composed of a number of 
            /// discrete flat surfaces of the count specified
            /// </summary>
            /// <param name="count">Number of discrete surfaces</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceCount_Is(int count)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceCount_Is;
                constraint.Param_Int_0 = count;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to contain a minimum surface area
            /// </summary>
            /// <param name="minArea">Minimum surface area</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceArea_Min(float minArea)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceArea_Min;
                constraint.Param_Float_0 = minArea;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to contain a maximum surface area
            /// </summary>
            /// <param name="maxArea">Maximum surface area</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceArea_Max(float maxArea)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceArea_Max;
                constraint.Param_Float_0 = maxArea;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to contain a surface area
            /// between the range specified
            /// </summary>
            /// <param name="minArea">Minimum surface area</param>
            /// <param name="maxArea">Maximum surface area</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceArea_Between(float minArea, float maxArea)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceArea_Between;
                constraint.Param_Float_0 = minArea;
                constraint.Param_Float_1 = maxArea;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to contain a specific surface area
            /// </summary>
            /// <param name="area">Required surface area</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SurfaceArea_Is(float area)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SurfaceArea_Is;
                constraint.Param_Float_0 = area;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to shaped like a rectangle. 
            /// 
            /// The rectangles similarity is the percent of the surface that matches the 
            /// containing rectangular component shape.
            /// </summary>
            /// <param name="similarityMin">Minimum similarity to a rectangle</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_IsRectangle(float similarityMin = 0.5f)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.IsRectangle;
                constraint.Param_Float_0 = similarityMin;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a minimum length and width of the surface rectangle. 
            /// Length is the longer of the two bounding edges and width the shorter edge.
            /// </summary>
            /// <param name="minLength">Minimum length</param>
            /// <param name="minWidth">Minimum width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleSize_Min(float minLength, float minWidth)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleSize_Min;
                constraint.Param_Float_0 = minLength;
                constraint.Param_Float_1 = minWidth;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a maximum length and width of the surface rectangle. 
            /// Length is the longer of the two bounding edges and width the shorter edge.
            /// </summary>
            /// <param name="maxLength">Maximum length</param>
            /// <param name="maxWidth">Maximum width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleSize_Max(float maxLength, float maxWidth)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleSize_Max;
                constraint.Param_Float_0 = maxLength;
                constraint.Param_Float_1 = maxWidth;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a the length and width of the surface rectangle
            /// to be within a specified range. Length is the longer of the two bounding edges and 
            /// width the shorter edge.
            /// </summary>
            /// <param name="minLength">Minimum length</param>
            /// <param name="minWidth">Minimum width</param>
            /// <param name="maxLength">Maximum length</param>
            /// <param name="maxWidth">Maximum width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleSize_Between(float minLength, float minWidth, float maxLength, float maxWidth)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleSize_Between;
                constraint.Param_Float_0 = minLength;
                constraint.Param_Float_1 = minWidth;
                constraint.Param_Float_2 = maxLength;
                constraint.Param_Float_3 = maxWidth;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a specified length and width.
            /// Length is the longer of the two bounding edges and width the shorter edge.
            /// </summary>
            /// <param name="length">Required surface length</param>
            /// <param name="width">Required surface width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleSize_Is(float length, float width)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleSize_Is;
                constraint.Param_Float_0 = length;
                constraint.Param_Float_1 = width;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a minimum length.
            /// Length is the longer of the two bounding edges.
            /// </summary>
            /// <param name="minLength">Minimum surface length</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleLength_Min(float minLength)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleLength_Min;
                constraint.Param_Float_0 = minLength;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a maximum length.
            /// Length is the longer of the two bounding edges.
            /// </summary>
            /// <param name="maxLength">Maximum surface length</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleLength_Max(float maxLength)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleLength_Max;
                constraint.Param_Float_0 = maxLength;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the surface length to be between the given range.
            /// Length is the longer of the two bounding edges.
            /// </summary>
            /// <param name="minLength">Minimum surface length</param>
            /// <param name="maxLength">Maximum surface length</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleLength_Between(float minLength, float maxLength)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleLength_Between;
                constraint.Param_Float_0 = minLength;
                constraint.Param_Float_1 = maxLength;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a specific surface length.
            /// Length is the longer of the two bounding edges.
            /// </summary>
            /// <param name="length">Required surface length</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleLength_Is(float length)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleLength_Is;
                constraint.Param_Float_0 = length;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a minimum width.
            /// Width is the shorter of the two bounding edges.
            /// </summary>
            /// <param name="minWidth">Minimum surface width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleWidth_Min(float minWidth)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleWidth_Min;
                constraint.Param_Float_0 = minWidth;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a maximum width.
            /// Width is the shorter of the two bounding edges.
            /// </summary>
            /// <param name="maxWidth">Maximum surface width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleWidth_Max(float maxWidth)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleWidth_Max;
                constraint.Param_Float_0 = maxWidth;
                return constraint;
            }
            /// <summary>
            /// Constructs a constraint requiring the surface width to be between the given range.
            /// Width is the shorter of the two bounding edges.
            /// </summary>
            /// <param name="minWidth">Minimum surface width</param>
            /// <param name="maxWidth">Maximum surface width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleWidth_Between(float minWidth, float maxWidth)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleWidth_Between;
                constraint.Param_Float_0 = minWidth;
                constraint.Param_Float_1 = maxWidth;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring a specific surface width.
            /// Width is the shorter of the two bounding edges.
            /// </summary>
            /// <param name="width">Required surface width</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_RectangleWidth_Is(float width)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.RectangleWidth_Is;
                constraint.Param_Float_0 = width;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be shaped like a square 
            /// 
            /// The squares similarity is the percent of the surface that matches the 
            /// containing square component shape.
            /// </summary>
            /// <param name="similarityMin">Minimum similarity to a square</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_IsSquare(float similarityMin = 0.5f)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.IsSquare;
                constraint.Param_Float_0 = similarityMin;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to have a minimum area
            /// </summary>
            /// <param name="minSize">Minimum size in meters squared</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SquareSize_Min(float minSize)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SquareSize_Min;
                constraint.Param_Float_0 = minSize;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to have a maximum area
            /// </summary>
            /// <param name="maxSize">Maximum size in meters squared</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SquareSize_Max(float maxSize)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SquareSize_Max;
                constraint.Param_Float_0 = maxSize;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to have a surface area
            /// between the given range
            /// </summary>
            /// <param name="minSize">Minimum size in meters squared</param>
            /// <param name="maxSize">Maximum size in meters squared</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SquareSize_Between(float minSize, float maxSize)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SquareSize_Between;
                constraint.Param_Float_0 = minSize;
                constraint.Param_Float_1 = maxSize;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to have a specific surface area
            /// </summary>
            /// <param name="size">Required size in meters squared</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_SquareSize_Is(float size)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.SquareSize_Is;
                constraint.Param_Float_0 = size;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the component to be shaped like a circle
            /// 
            /// The squares similarity is the percent of the surface that matches the 
            /// containing circular component shape
            /// </summary>
            /// <param name="similarityMin">Minimum similarity to a circle</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_IsCircle(float similarityMin = 0.5f)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.IsCircle;
                constraint.Param_Float_0 = similarityMin;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the circle shaped component to have
            /// a minimum radius
            /// </summary>
            /// <param name="minRadius">Minimum radius in meters</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_CircleRadius_Min(float minRadius)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.CircleRadius_Min;
                constraint.Param_Float_0 = minRadius;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the circle shaped component to have
            /// a maximum radius
            /// </summary>
            /// <param name="maxRadius">Maximum radius in meters</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_CircleRadius_Max(float maxRadius)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.CircleRadius_Max;
                constraint.Param_Float_0 = maxRadius;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the circle shaped component to have
            /// a radius between the given range
            /// </summary>
            /// <param name="minRadius">Minimum radius in meters</param>
            /// <param name="maxRadius">Maximum radius in meters</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_CircleRadius_Between(float minRadius, float maxRadius)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.CircleRadius_Between;
                constraint.Param_Float_0 = minRadius;
                constraint.Param_Float_1 = maxRadius;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the circle shaped component to have
            /// a specific radius
            /// </summary>
            /// <param name="radius">Required radius in meters</param>
            /// <returns>Constructed component constraint</returns>
            public static ShapeComponentConstraint Create_CircleRadius_Is(float radius)
            {
                ShapeComponentConstraint constraint = new ShapeComponentConstraint();
                constraint.Type = ShapeComponentConstraintType.CircleRadius_Is;
                constraint.Param_Float_0 = radius;
                return constraint;
            }

            public ShapeComponentConstraintType Type;
            public float Param_Float_0;
            public float Param_Float_1;
            public float Param_Float_2;
            public float Param_Float_3;
            public int Param_Int_0;
            public int Param_Int_1;
            public IntPtr Param_Str_0;
        };

        /// <summary>
        /// A shape component definition. Contains a list of component constraints.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ShapeComponent
        {
            public ShapeComponent(List<ShapeComponentConstraint> componentConstraints)
            {
                ConstraintCount = componentConstraints.Count;
                Constraints = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(componentConstraints.ToArray());
            }

            public int ConstraintCount;
            public IntPtr Constraints;  // ShapeComponentConstraint
        };

        /// <summary>
        /// Lists the types of shape constraints. Each defines a requirement
        /// between shape components.
        /// </summary>
        public enum ShapeConstraintType
        {
            NoOtherSurface,
            AwayFromWalls,

            RectanglesParallel,
            RectanglesPerpendicular,
            RectanglesAligned,
            RectanglesSameLength,

            AtFrontOf,
            AtBackOf,
            AtLeftOf,
            AtRightOf,
        };

        /// <summary>
        /// A shape constraint definition. Composed of a type and 
        /// type specific parameters
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ShapeConstraint
        {
            /// <summary>
            /// Constructs a constraint required no other surfaces be included in this shape
            /// </summary>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_NoOtherSurface()
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.NoOtherSurface;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the shape to be away from all walls
            /// </summary>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_AwayFromWalls()
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.AwayFromWalls;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the components shapes longer edges
            /// to have parallel alignment.
            /// </summary>
            /// <param name="componentIndexA">Zero based index of the first component constraint</param>
            /// <param name="componentIndexB">Zero based index of the second component constraint</param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_RectanglesParallel(int componentIndexA, int componentIndexB)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.RectanglesParallel;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the components shapes longer edges
            /// to have perpendicular alignment.
            /// </summary>
            /// <param name="componentIndexA">Zero based index of the first component constraint</param>
            /// <param name="componentIndexB">Zero based index of the second component constraint</param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_RectanglesPerpendicular(int componentIndexA, int componentIndexB)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.RectanglesPerpendicular;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the components shapes to be either aligned
            /// with parallel or parallel alignment. The difference is the defined as the cosine of the angle
            /// between the best aligned axis (i.e. the dot product)
            /// </summary>
            /// <param name="componentIndexA">Zero based index of the first component constraint</param>
            /// <param name="componentIndexB">Zero based index of the second component constraint</param>
            /// <param name="maxDifference">Maximum difference</param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_RectanglesAligned(int componentIndexA, int componentIndexB, float maxDifference = 0.1f)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.RectanglesAligned;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                constraint.Param_Float_0 = maxDifference;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring the components shapes longest edges to
            /// have the same length, within the difference parameter. 
            /// 
            /// The difference is defined as the ratio of the longest edges of the two components.
            /// </summary>
            /// <param name="componentIndexA">Zero based index of the first component constraint</param>
            /// <param name="componentIndexB">Zero based index of the second component constraint</param>
            /// <param name="similarityMin">Maximum similarity</param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_RectanglesSameLength(int componentIndexA, int componentIndexB, float similarityMin = 0.8f)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.RectanglesSameLength;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                constraint.Param_Float_0 = similarityMin;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring component B to be immediately in front of component A.
            /// </summary>
            /// <param name="componentIndexA"></param>
            /// <param name="componentIndexB"></param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_AtFrontOf(int componentIndexA, int componentIndexB)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.AtFrontOf;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring component B to be immediately in back of component A.
            /// </summary>
            /// <param name="componentIndexA"></param>
            /// <param name="componentIndexB"></param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_AtBackOf(int componentIndexA, int componentIndexB)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.AtBackOf;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring component B to be immediately to the left of component A.
            /// </summary>
            /// <param name="componentIndexA"></param>
            /// <param name="componentIndexB"></param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_AtLeftOf(int componentIndexA, int componentIndexB)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.AtLeftOf;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                return constraint;
            }

            /// <summary>
            /// Constructs a constraint requiring component B to be immediately to the right of component A.
            /// </summary>
            /// <param name="componentIndexA"></param>
            /// <param name="componentIndexB"></param>
            /// <returns>Constructed shape constraint</returns>
            public static ShapeConstraint Create_AtRightOf(int componentIndexA, int componentIndexB)
            {
                ShapeConstraint constraint = new ShapeConstraint();
                constraint.Type = ShapeConstraintType.AtRightOf;
                constraint.Param_Int_0 = componentIndexA;
                constraint.Param_Int_1 = componentIndexB;
                return constraint;
            }

            public ShapeConstraintType Type;
            public float Param_Float_0;
            public int Param_Int_0;
            public int Param_Int_1;
        };

        // Functions
        /// <summary>
        /// Finds the set of available positions on the set of found shapes 
        /// of the type specified by shapeName.
        /// </summary>
        /// <param name="shapeName">Name of the shape</param>
        /// <param name="minRadius">Defines the minimum space requirement for a returned position</param>
        /// <param name="shapeCount">Length of the array passed in shapeData, the return value will never exceed this value</param>
        /// <param name="shapeData">An array of ShapeResult structures to receive the results of the query</param>
        /// <returns>Number of positions found. This number will never exceed shapeCount (the space provided for the results in shapeData).</returns>
        // Queries (shapes)
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QueryShape_FindPositionsOnShape(
            [In, MarshalAs(UnmanagedType.LPStr)] string shapeName,          // char*
            [In] float minRadius,
            [In] int shapeCount,                                            // Pass in the space allocated in shapeData
            [In, Out] IntPtr shapeData);                                    // ShapeResult

        /// <summary>
        /// Finds the set of found shapes of the type specified by shapeName. 
        /// Returns the bounding rectangle's half dimensions.
        /// </summary>
        /// <param name="shapeName">Name of the shape</param>
        /// <param name="shapeCount">Length of the array passed in shapeData, the return value will never exceed this value</param>
        /// <param name="shapeData">An array of ShapeResult structures to receive the results of the query</param>
        /// <returns>Number of shapes found. This number will never exceed shapeCount (the space provided for the results in shapeData).</returns>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern int QueryShape_FindShapeHalfDims(
            [In, MarshalAs(UnmanagedType.LPStr)] string shapeName,         // char*
            [In] int shapeCount,                                            // Pass in the space allocated in shapeData
            [In, Out] IntPtr shapeData);                                    // ShapeResult

        /// <summary>
        /// Add a shape definition. A shape is defined by a list of components and a 
        /// set of component constraints. Each component is of type ShapeComponent and
        /// is defined by a set of component constraint.
        /// </summary>
        /// <param name="shapeName">Name of the shaped</param>
        /// <param name="componentCount">Length of the component array pass in the components parameter</param>
        /// <param name="components">Array of ShapeComponent structures</param>
        /// <param name="shapeConstraints">Length of the shape constraint array passed in the constraints parameter</param>
        /// <param name="constraints">Array of ShapeConstraint structures</param>
        /// <returns></returns>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddShape(
            [In, MarshalAs(UnmanagedType.LPStr)] string shapeName,
            [In] int componentCount,
            [In] IntPtr components,         // ShapeComponent
            [In] int shapeConstraints,
            [In] IntPtr constraints);       // ShapeConstraint

        /// <summary>
        /// Runs the shape analysis. This should be called after scanning has been 
        /// finalized and shapes have been defined with AddShape.
        /// </summary>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ActivateShapeAnalysis();

        /// <summary>
        /// Removes all shapes defined by AddShape.
        /// </summary>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RemoveAllShapes();
    }
}