// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

public class ShapeDefinition : Singleton<ShapeDefinition>
{
    // Properties
    public bool HasCreatedShapes { get; private set; }
    public ReadOnlyCollection<string> CustomShapeDefinitions { get { return customShapeDefinitions.AsReadOnly(); } }

    // Privates
    private List<string> customShapeDefinitions = new List<string>();

    // Functions
    private void Start()
    {
        if (SpatialUnderstanding.Instance != null)
        {
            SpatialUnderstanding.Instance.ScanStateChanged += OnScanStateChanged;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SpatialUnderstanding.Instance != null)
        {
            SpatialUnderstanding.Instance.ScanStateChanged -= OnScanStateChanged;
        }
    }

    public void CreateShapes()
    {
        if (HasCreatedShapes ||
            !SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }

        // Create definitions and analyze
        CreateCustomShapeDefinitions();
        SpatialUnderstandingDllShapes.ActivateShapeAnalysis();
    }

    private void OnScanStateChanged()
    {
        // If we are leaving the None state, go ahead and register shapes now
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {
            // Create definitions and analyze
            CreateShapes();
        }
    }

    private bool AddShape(
        string shapeName,
        List<SpatialUnderstandingDllShapes.ShapeComponent> shapeComponents)
    {
        return AddShape(shapeName, shapeComponents, null);
    }

    private bool AddShape(
        string shapeName, 
        List<SpatialUnderstandingDllShapes.ShapeComponent> shapeComponents,
        List<SpatialUnderstandingDllShapes.ShapeConstraint> shapeConstraints)
    {
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return false;
        }
        IntPtr shapeComponentsPtr = (shapeComponents == null) ? IntPtr.Zero : HoloToolkit.Unity.SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeComponents.ToArray());
        IntPtr shapeConstraintsPtr = (shapeConstraints == null) ? IntPtr.Zero : HoloToolkit.Unity.SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeConstraints.ToArray());
        if (SpatialUnderstandingDllShapes.AddShape(
                shapeName,
                (shapeComponents == null) ? 0 : shapeComponents.Count,
                shapeComponentsPtr,
                (shapeConstraints == null) ? 0 : shapeConstraints.Count,
                shapeConstraintsPtr) == 0)
        {
            Debug.LogError("Failed to create custom shape description");
            return false;
        }
        customShapeDefinitions.Add(shapeName);
        return true;
    }

    private void CreateCustomShapeDefinitions()
    {
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }

        List<SpatialUnderstandingDllShapes.ShapeComponent> shapeComponents;
        List<SpatialUnderstandingDllShapes.ShapeConstraint> shapeConstraints;

        // AllSurfaces
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.15f, 1.75f),
                }),
        };
        AddShape("All Surfaces", shapeComponents);

        // Sittable
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.2f, 0.6f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.035f),
                }),
        };
        AddShape("Sittable", shapeComponents);

        // Chair
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.25f, 0.6f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.035f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.1f, 0.5f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Between(0.1f, 0.4f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceNotPartOfShape("Couch"),
                }),
        };
        AddShape("Chair", shapeComponents);

        // LargeSurface
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.3f, 0.75f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.35f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Min(0.75f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Min(0.5f),
                }),
        };
        AddShape("Large Surface", shapeComponents);

        // EmptyTable
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.3f, 0.75f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.35f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Min(0.75f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Min(0.5f),
                }),
        };
        shapeConstraints = new List<SpatialUnderstandingDllShapes.ShapeConstraint>()
        {
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_NoOtherSurface(),
        };
        AddShape("Large Empty Surface", shapeComponents, shapeConstraints);

        // "Couch"
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    // Seat
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.2f, 0.6f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.3f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.4f, 3.0f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Min(0.3f),
                }),
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    // Back
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.6f, 1.0f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(0.3f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.4f, 3.0f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Min(0.05f),
                }),
        };
        shapeConstraints = new List<SpatialUnderstandingDllShapes.ShapeConstraint>()
        {
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesSameLength(0, 1, 0.6f),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesParallel(0, 1),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesAligned(0, 1, 0.3f),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_AtBackOf(1, 0),
        };
        AddShape("Couch", shapeComponents, shapeConstraints);

        // Mark it
        HasCreatedShapes = true;
    }
}
