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
    /// Encapsulates the object placement queries of the understanding DLL.
    /// These queries will not be valid until after scanning is finalized.
    /// </summary>
    public static class SpatialUnderstandingDllObjectPlacement
    {
        /// <summary>
        /// Defines an object placement query. A query consists of
        /// a type a name, type, set of rules, and set of constraints.
        /// 
        /// Rules may not be violated by the returned query. Possible 
        /// locations that satisfy the type and rules are selected
        /// by optimizing within the constraint list.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ObjectPlacementDefinition
        {
            /// <summary>
            /// Type of object placement. Each type has a custom set
            /// of parameter.
            /// </summary>
            public enum PlacementType
            {
                Place_OnFloor,
                Place_OnWall,
                Place_OnCeiling,
                Place_OnShape,
                Place_OnEdge,
                Place_OnFloorAndCeiling,
                Place_RandomInAir,
                Place_InMidAir,
                Place_UnderPlatformEdge,
            };

            /// <summary>
            /// Type of wall.
            /// External walls bound the playspace. Virtual walls are created
            /// at the edge of the playspace when an external wall does not
            /// exist.
            /// </summary>
            [FlagsAttribute]
            public enum WallTypeFlags
            {
                None = 0,
                Normal = (1 << 0),
                External = (1 << 1),
                Virtual = (1 << 2),
                ExternalVirtual = (1 << 3),
            };

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be placed on the floor.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_OnFloor(Vector3 halfDims)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_OnFloor;
                placement.HalfDims = halfDims;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be placed on a wall.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <param name="heightMin">Minimum height of the requested volume above the floor</param>
            /// <param name="heightMax">Maximum height of the requested volume above the floor</param>
            /// <param name="wallTypes">Bit mask of possible walls to consider, defined by WallTypeFlags</param>
            /// <param name="marginLeft">Required empty wall space to the left of the volume, as defined by facing the wall</param>
            /// <param name="marginRight">Required empty wall space to the right of the volume, as defined by facing the wall</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_OnWall(
                Vector3 halfDims,
                float heightMin,
                float heightMax,
                WallTypeFlags wallTypes = WallTypeFlags.External | WallTypeFlags.Normal,
                float marginLeft = 0.0f,
                float marginRight = 0.0f)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_OnWall;
                placement.HalfDims = halfDims;
                placement.PlacementParam_Float_0 = heightMin;
                placement.PlacementParam_Float_1 = heightMax;
                placement.PlacementParam_Float_2 = marginLeft;
                placement.PlacementParam_Float_3 = marginRight;
                placement.WallFlags = (int)wallTypes;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be place on the ceiling.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_OnCeiling(Vector3 halfDims)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_OnCeiling;
                placement.HalfDims = halfDims;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be placed on top of another object placed object.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <param name="shapeName">Name of the placed object</param>
            /// <param name="componentIndex">Index of the component within shapeName</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_OnShape(Vector3 halfDims, string shapeName, int componentIndex)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_OnShape;
                placement.HalfDims = halfDims;
                placement.PlacementParam_Str_0 = SpatialUnderstanding.Instance.UnderstandingDLL.PinString(shapeName);
                placement.PlacementParam_Int_0 = componentIndex;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be placed on the edge of a platform.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <param name="halfDimsBottom">Half size of the bottom part of the placement volume</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_OnEdge(Vector3 halfDims, Vector3 halfDimsBottom)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_OnEdge;
                placement.HalfDims = halfDims;
                placement.PlacementParam_Float_0 = halfDimsBottom.x;
                placement.PlacementParam_Float_1 = halfDimsBottom.y;
                placement.PlacementParam_Float_2 = halfDimsBottom.z;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be have space on the floor and ceiling within the same vertical space.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <param name="halfDimsBottom">Half size of the bottom part of the placement volume</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_OnFloorAndCeiling(Vector3 halfDims, Vector3 halfDimsBottom)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_OnFloorAndCeiling;
                placement.HalfDims = halfDims;
                placement.PlacementParam_Float_0 = halfDimsBottom.x;
                placement.PlacementParam_Float_1 = halfDimsBottom.y;
                placement.PlacementParam_Float_2 = halfDimsBottom.z;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be placed floating in space, within the playspace. Spaces visible from the
            /// center of the playspace are favored.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_RandomInAir(Vector3 halfDims)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_RandomInAir;
                placement.HalfDims = halfDims;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be placed floating in space, within the playspace. This query requires that
            /// other objects do not collide with the placement volume.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_InMidAir(Vector3 halfDims)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_InMidAir;
                placement.HalfDims = halfDims;
                return placement;
            }

            /// <summary>
            /// Constructs an object placement query definition requiring the object to
            /// be place under a platform edge.
            /// </summary>
            /// <param name="halfDims">Required half size of the requested bounding volume</param>
            /// <returns>Constructed object placement definition</returns>
            public static ObjectPlacementDefinition Create_UnderPlatformEdge(Vector3 halfDims)
            {
                ObjectPlacementDefinition placement = new ObjectPlacementDefinition();
                placement.Type = PlacementType.Place_UnderPlatformEdge;
                placement.HalfDims = halfDims;
                return placement;
            }

            public PlacementType Type;
            public int PlacementParam_Int_0;
            public float PlacementParam_Float_0;
            public float PlacementParam_Float_1;
            public float PlacementParam_Float_2;
            public float PlacementParam_Float_3;
            public IntPtr PlacementParam_Str_0;
            public int WallFlags;
            public Vector3 HalfDims;
        };

        /// <summary>
        /// Defines an object placement rule. Rules are one part of an object
        /// placement definition. Rules may not be violated by the returned query.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ObjectPlacementRule
        {
            /// <summary>
            /// Type of object placement rule. Each rule is defined by its
            /// type and a set of per-type parameters. The supplied static
            /// construction functions may be used to create rules.
            /// </summary>
            public enum ObjectPlacementRuleType
            {
                Rule_AwayFromPosition,
                Rule_AwayFromWalls,
                Rule_AwayFromOtherObjects,
            };

            /// <summary>
            /// Constructs an object placement rule requiring the placement volume to
            /// be placed a minimum distance away from the specified position.
            /// </summary>
            /// <param name="position">Defines the center position for the center of the invalid placement space.</param>
            /// <param name="minDistance">Defines the radius of the invalid placement space.</param>
            /// <returns>Constructed object placement rule</returns>
            public static ObjectPlacementRule Create_AwayFromPosition(Vector3 position, float minDistance)
            {
                ObjectPlacementRule rule = new ObjectPlacementRule();
                rule.Type = ObjectPlacementRuleType.Rule_AwayFromPosition;
                rule.RuleParam_Vec3_0 = position;
                rule.RuleParam_Float_0 = minDistance;
                return rule;
            }

            /// <summary>
            /// Constructs an object placement rule requiring the placement volume to
            /// be placed a minimum distance away from any wall
            /// </summary>
            /// <param name="minDistance">Minimum distance from a wall</param>
            /// <param name="minWallHeight">Minimum height of a wall to be considered by this rule</param>
            /// <returns>Constructed object placement rule</returns>
            public static ObjectPlacementRule Create_AwayFromWalls(float minDistance, float minWallHeight = 0.0f)
            {
                ObjectPlacementRule rule = new ObjectPlacementRule();
                rule.Type = ObjectPlacementRuleType.Rule_AwayFromWalls;
                rule.RuleParam_Float_0 = minDistance;
                rule.RuleParam_Float_1 = minWallHeight;
                return rule;
            }

            /// <summary>
            /// Constructs an object placement rule requiring the placement volume to
            /// be placed a minimum distance away from other placed objects
            /// </summary>
            /// <param name="minDistance">Minimum distance from other placed objects</param>
            /// <returns>Constructed object placement rule</returns>
            public static ObjectPlacementRule Create_AwayFromOtherObjects(float minDistance)
            {
                ObjectPlacementRule rule = new ObjectPlacementRule();
                rule.Type = ObjectPlacementRuleType.Rule_AwayFromOtherObjects;
                rule.RuleParam_Float_0 = minDistance;
                return rule;
            }

            public ObjectPlacementRuleType Type;
            public int RuleParam_Int_0;
            public float RuleParam_Float_0;
            public float RuleParam_Float_1;
            public Vector3 RuleParam_Vec3_0;
        };

        /// <summary>
        /// Defines an object placement constraint. Constraints are one part of an object
        /// placement definition. Possible object placement locations are picked by the 
        /// location that minimally violates the set of constraints.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ObjectPlacementConstraint
        {
            /// <summary>
            /// Types of object placement constraints
            /// </summary>
            public enum ObjectPlacementConstraintType
            {
                Constraint_NearPoint,
                Constraint_NearWall,
                Constraint_AwayFromWalls,
                Constraint_NearCenter,
                Constraint_AwayFromOtherObjects,
                Constraint_AwayFromPoint
            };

            /// <summary>
            /// Constructs an object placement constraint requesting that the placement volume
            /// be placed no closer than minDistance and no further than maxDistance from 
            /// the specified position.
            /// </summary>
            /// <param name="position">The center point from switch minDistance and maxDistance define their volumes</param>
            /// <param name="minDistance">The minimum distance from position to place the object</param>
            /// <param name="maxDistance">The maximum distance from position to place the object</param>
            /// <returns>Constructed object placement constraint</returns>
            public static ObjectPlacementConstraint Create_NearPoint(Vector3 position, float minDistance = 0.0f, float maxDistance = 0.0f)
            {
                ObjectPlacementConstraint constraint = new ObjectPlacementConstraint();
                constraint.Type = ObjectPlacementConstraintType.Constraint_NearPoint;
                constraint.RuleParam_Vec3_0 = position;
                constraint.RuleParam_Float_0 = minDistance;
                constraint.RuleParam_Float_1 = maxDistance;
                return constraint;
            }

            /// <summary>
            /// Constructs an object placement constraint requesting that the placement volume
            /// be placed no closer than minDistance and no further than maxDistance from 
            /// a wall.
            /// </summary>
            /// <param name="minDistance">The minimum distance from position to place the object</param>
            /// <param name="maxDistance">The maximum distance from position to place the object</param>
            /// <param name="minWallHeight">Minimum height of a wall to be considered by this rule</param>
            /// <param name="includeVirtualWalls">Indicates virtual walls should be considered in this query</param>
            /// <returns>Constructed object placement constraint</returns>
            public static ObjectPlacementConstraint Create_NearWall(float minDistance = 0.0f, float maxDistance = 0.0f, float minWallHeight = 0.0f, bool includeVirtualWalls = false)
            {
                ObjectPlacementConstraint constraint = new ObjectPlacementConstraint();
                constraint.Type = ObjectPlacementConstraintType.Constraint_NearWall;
                constraint.RuleParam_Float_0 = minDistance;
                constraint.RuleParam_Float_1 = maxDistance;
                constraint.RuleParam_Float_2 = minWallHeight;
                constraint.RuleParam_Int_0 = includeVirtualWalls ? 1 : 0;
                return constraint;
            }

            /// <summary>
            /// Constructs an object placement constraint requesting that the placement volume
            /// be placed away from all walls.
            /// </summary>
            /// <returns>Constructed object placement constraint</returns>
            public static ObjectPlacementConstraint Create_AwayFromWalls()
            {
                ObjectPlacementConstraint constraint = new ObjectPlacementConstraint();
                constraint.Type = ObjectPlacementConstraintType.Constraint_AwayFromWalls;
                return constraint;
            }

            /// <summary>
            /// Constructs an object placement constraint requesting that the placement volume
            /// be placed near the center of the playspace.
            /// </summary>
            /// <param name="minDistance">The minimum distance from the center to place the object</param>
            /// <param name="maxDistance">The maximum distance from the center to place the object</param>
            /// <returns>Constructed object placement constraint</returns>
            public static ObjectPlacementConstraint Create_NearCenter(float minDistance = 0.0f, float maxDistance = 0.0f)
            {
                ObjectPlacementConstraint constraint = new ObjectPlacementConstraint();
                constraint.Type = ObjectPlacementConstraintType.Constraint_NearCenter;
                constraint.RuleParam_Float_0 = minDistance;
                constraint.RuleParam_Float_1 = maxDistance;
                return constraint;
            }

            /// <summary>
            /// Constructs an object placement constraint requesting that the placement volume
            /// be placed away from other place objects.
            /// </summary>
            /// <returns>Constructed object placement constraint</returns>
            public static ObjectPlacementConstraint Create_AwayFromOtherObjects()
            {
                ObjectPlacementConstraint constraint = new ObjectPlacementConstraint();
                constraint.Type = ObjectPlacementConstraintType.Constraint_AwayFromOtherObjects;
                return constraint;
            }

            /// <summary>
            /// Constructs an object placement constraint requesting that the placement volume
            /// be placed away from the specified position.
            /// </summary>
            /// <param name="position">The center point from switch minDistance and maxDistance define their volumes</param>
            /// <returns>Constructed object placement constraint</returns>
            public static ObjectPlacementConstraint Create_AwayFromPoint(Vector3 position)
            {
                ObjectPlacementConstraint constraint = new ObjectPlacementConstraint();
                constraint.Type = ObjectPlacementConstraintType.Constraint_AwayFromPoint;
                constraint.RuleParam_Vec3_0 = position;
                return constraint;
            }

            public ObjectPlacementConstraintType Type;
            public int RuleParam_Int_0;
            public float RuleParam_Float_0;
            public float RuleParam_Float_1;
            public float RuleParam_Float_2;
            public Vector3 RuleParam_Vec3_0;
        };

        /// <summary>
        /// Object placement result. Defines an oriented bounding box result for the
        /// object placement query.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class ObjectPlacementResult : ICloneable
        {
            public object Clone()
            {
                return this.MemberwiseClone();
            }

            public Vector3 Position;
            public Vector3 HalfDims;
            public Vector3 Forward;
            public Vector3 Right;
            public Vector3 Up;
        };

        // Functions
        /// <summary>
        /// Initialized the object placement solver. This should be called after the
        /// scanning phase has finish and the playspace has been finalized.
        /// </summary>
        /// <returns></returns>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Solver_Init();

        /// <summary>
        /// Executes an object placement query.
        /// 
        /// A query consists of a type a name, type, set of rules, 
        /// and set of constraints.
        /// 
        /// Rules may not be violated by the returned query. Possible 
        /// locations that satisfy the type and rules are selected
        /// by optimizing within the constraint list.
        /// 
        /// Objects placed with Solver_PlaceObject persist until removed
        /// and are considered in subsequent queries by some rules and constraints.
        /// </summary>
        /// <param name="objectName">Name of the object placement query</param>
        /// <param name="placementDefinition">The placement definition, of type ObjectPlacementDefinition</param>
        /// <param name="placementRuleCount">Length of the provided placementRules array</param>
        /// <param name="placementRules">Array of ObjectPlacementRule structures, defining the rules</param>
        /// <param name="constraintCount">Length of the provided placementConstraints array</param>
        /// <param name="placementConstraints">Array of ObjectPlacementConstraint structures, defining the constraints</param>
        /// <param name="placementResult">Pointer to an ObjectPlacementResult structure to receive the result of the query </param>
        /// <returns>Zero on failure, one on success</returns>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Solver_PlaceObject(
            [In, MarshalAs(UnmanagedType.LPStr)] string objectName,
            [In] IntPtr placementDefinition,// ObjectPlacementDefinition
            [In] int placementRuleCount,
            [In] IntPtr placementRules,     // ObjectPlacementRule
            [In] int constraintCount,
            [In] IntPtr placementConstraints,// ObjectPlacementConstraint
            [Out] IntPtr placementResult);  // ObjectPlacementResult

        /// <summary>
        /// Removed a solved object. 
        /// 
        /// Objects placed with Solver_PlaceObject persist until removed
        /// and are considered in subsequent queries by some rules and constraints.
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Solver_RemoveObject(
            [In, MarshalAs(UnmanagedType.LPStr)] string objectName);

        /// <summary>
        /// Removed all solved object placements.
        /// 
        /// Objects placed with Solver_PlaceObject persist until removed
        /// and are considered in subsequent queries by some rules and constraints.
        /// </summary>
        [DllImport("SpatialUnderstanding", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Solver_RemoveAllObjects();
    }
}