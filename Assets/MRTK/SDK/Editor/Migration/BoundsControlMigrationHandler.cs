// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Migration handler for migrating bounding box gameobjects to bounds control gameobjects.
    /// </summary>
    public class BoundsControlMigrationHandler : IMigrationHandler
    {
        /// <inheritdoc />
        public bool CanMigrate(GameObject gameObject)
        {
            return gameObject.GetComponent<BoundingBox>() != null;
        }

        /// <inheritdoc />
        public void Migrate(GameObject gameObject)
        {
            var boundingBox = gameObject.GetComponent<BoundingBox>();
            var boundsControl = gameObject.EnsureComponent<BoundsControl>();

            boundsControl.enabled = boundingBox.enabled;

            {
                Undo.RecordObject(gameObject, "BoundsControl migration: swapping BoundingBox with BoundsControl.");

                // migrate logic settings
                boundsControl.Target = boundingBox.Target;
                boundsControl.BoundsOverride = boundingBox.BoundsOverride;
                boundsControl.CalculationMethod = MigrateCalculationMethod(boundingBox.CalculationMethod);
                boundsControl.BoundsControlActivation = MigrateActivationFlag(boundingBox.BoundingBoxActivation);

                // only carry over min max scaling values if user hasn't attached min max scale constraint component yet 
                if (gameObject.GetComponent<MinMaxScaleConstraint>() == null)
                {
#pragma warning disable 0618
                    // create a minmaxscaleconstraint in case there's a min max scale set up
                    if (boundingBox.ScaleMinimum != 0.0f || boundingBox.ScaleMaximum != 0.0f)
                    {
                        MinMaxScaleConstraint scaleConstraint = gameObject.AddComponent<MinMaxScaleConstraint>();
                        scaleConstraint.ScaleMinimum = boundingBox.ScaleMinimum;
                        scaleConstraint.ScaleMaximum = boundingBox.ScaleMaximum;
                    }
#pragma warning restore 0618
                }

                // migrate visuals
                boundsControl.FlattenAxis = MigrateFlattenAxis(boundingBox.FlattenAxis);
                boundsControl.BoxPadding = boundingBox.BoxPadding;
                MigrateBoxDisplay(boundsControl, boundingBox);
                MigrateLinks(boundsControl, boundingBox);
                MigrateScaleHandles(boundsControl, boundingBox);
                MigrateRotationHandles(boundsControl, boundingBox);
                MigrateProximityEffect(boundsControl, boundingBox);

                // debug properties
                boundsControl.DebugText = boundingBox.debugText;
                boundsControl.HideElementsInInspector = boundingBox.HideElementsInInspector;

                // events
                boundsControl.RotateStarted = boundingBox.RotateStarted;
                boundsControl.RotateStopped = boundingBox.RotateStopped;
                boundsControl.ScaleStarted = boundingBox.ScaleStarted;
                boundsControl.ScaleStopped = boundingBox.ScaleStopped;
            }

            // look in the scene for app bars and upgrade them too to point to the new component
            MigrateAppBar(boundingBox, boundsControl);

            {
                Undo.RecordObject(gameObject, "Removing obsolete BoundingBox component");
                // destroy obsolete component
                Object.DestroyImmediate(boundingBox);
            }
        }

        #region Flags Migration

        private BoundsCalculationMethod MigrateCalculationMethod(BoundingBox.BoundsCalculationMethod calculationMethod)
        {
            switch (calculationMethod)
            {
                case BoundingBox.BoundsCalculationMethod.RendererOverCollider:
                    return BoundsCalculationMethod.RendererOverCollider;
                case BoundingBox.BoundsCalculationMethod.ColliderOverRenderer:
                    return BoundsCalculationMethod.ColliderOverRenderer;
                case BoundingBox.BoundsCalculationMethod.ColliderOnly:
                    return BoundsCalculationMethod.ColliderOnly;
                case BoundingBox.BoundsCalculationMethod.RendererOnly:
                    return BoundsCalculationMethod.RendererOnly;
            }

            Debug.Assert(false, "Tried to migrate unsupported bounds calculation method in bounding box / bounds control");
            return BoundsCalculationMethod.RendererOverCollider;
        }

        private BoundsControlActivationType MigrateActivationFlag(BoundingBox.BoundingBoxActivationType activationFlag)
        {
            switch (activationFlag)
            {
                case BoundingBox.BoundingBoxActivationType.ActivateOnStart:
                    return BoundsControlActivationType.ActivateOnStart;
                case BoundingBox.BoundingBoxActivationType.ActivateByProximity:
                    return BoundsControlActivationType.ActivateByProximity;
                case BoundingBox.BoundingBoxActivationType.ActivateByPointer:
                    return BoundsControlActivationType.ActivateByPointer;
                case BoundingBox.BoundingBoxActivationType.ActivateByProximityAndPointer:
                    return BoundsControlActivationType.ActivateByProximityAndPointer;
                case BoundingBox.BoundingBoxActivationType.ActivateManually:
                    return BoundsControlActivationType.ActivateManually;
            }

            Debug.Assert(false, "Tried to migrate unsupported activation flag in bounding box / bounds control");
            return BoundsControlActivationType.ActivateOnStart;
        }

        private FlattenModeType MigrateFlattenAxis(BoundingBox.FlattenModeType flattenAxisType)
        {
            switch (flattenAxisType)
            {
                case BoundingBox.FlattenModeType.DoNotFlatten:
                    return FlattenModeType.DoNotFlatten;
                case BoundingBox.FlattenModeType.FlattenX:
                    return FlattenModeType.FlattenX;
                case BoundingBox.FlattenModeType.FlattenY:
                    return FlattenModeType.FlattenY;
                case BoundingBox.FlattenModeType.FlattenZ:
                    return FlattenModeType.FlattenZ;
                case BoundingBox.FlattenModeType.FlattenAuto:
                    return FlattenModeType.FlattenAuto;
            }

            Debug.Assert(false, "Tried to migrate unsupported flatten axis type in bounding box / bounds control");
            return FlattenModeType.DoNotFlatten;
        }

        private WireframeType MigrateWireframeShape(BoundingBox.WireframeType wireframeType)
        {
            switch (wireframeType)
            {
                case BoundingBox.WireframeType.Cubic:
                    return WireframeType.Cubic;
                case BoundingBox.WireframeType.Cylindrical:
                    return WireframeType.Cylindrical;
            }

            Debug.Assert(false, "Tried to migrate unsupported wireframe type in bounding box / bounds control");
            return WireframeType.Cubic;
        }

        private HandlePrefabCollider MigrateRotationHandleColliderType(BoundingBox.RotationHandlePrefabCollider rotationHandlePrefabColliderType)
        {
            switch (rotationHandlePrefabColliderType)
            {
                case BoundingBox.RotationHandlePrefabCollider.Sphere:
                    return HandlePrefabCollider.Sphere;
                case BoundingBox.RotationHandlePrefabCollider.Box:
                    return HandlePrefabCollider.Box;
            }

            Debug.Assert(false, "Tried to migrate unsupported rotation handle collider type in bounding box / bounds control");
            return HandlePrefabCollider.Sphere;
        }

        #endregion Flags Migration

        #region Visuals Configuration Migration

        private T EnsureConfiguration<T>(GameObject owner, T config, bool enforceScriptableCreation) where T : ScriptableObject
        {
            var instance = config;
            if (instance == null || enforceScriptableCreation)
            {
                instance = ScriptableObject.CreateInstance<T>();
                // scriptables in prefabs need to be added into their asset file as serializedobject, else they won't get stored
                string objectPath = owner.scene.path;
                if (objectPath.EndsWith(".prefab"))
                {
                    instance.name = instance.GetType().Name;
                    AssetDatabase.AddObjectToAsset(instance, objectPath);
                }
            }

            return instance;
        }

        /// <summary>
        /// This method checks if the given object has a property override for any of the given property names.
        /// It's used to figure out if scriptable objects have to be created for the current prefab / gameobject instance level.
        /// </summary>
        /// <param name="target">The target object (can be prefab component or component instance).</param>
        /// <param name="propertyNames">Property names to be checked.</param>
        /// <returns>Returns true if any of the passed property names have an override.</returns>
        bool HasPropertyOverrides(Object target, List<string> propertyNames)
        {
            var propertyModifications = PrefabUtility.GetPropertyModifications(target);
            if (propertyModifications != null)
            {
                foreach (var propertyMod in propertyModifications)
                {
                    if (propertyNames.Contains(propertyMod.propertyPath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void MigrateBoxDisplay(BoundsControl control, BoundingBox box)
        {
            List<string> propertyPaths = new List<string>{ "boxMaterial", "boxGrabbedMaterial", "flattenAxisDisplayScale" };
            BoxDisplayConfiguration config = EnsureConfiguration(control.gameObject, control.BoxDisplayConfig, HasPropertyOverrides(box, propertyPaths));
            config.BoxMaterial = box.BoxMaterial;
            config.BoxGrabbedMaterial = box.BoxGrabbedMaterial;
            config.FlattenAxisDisplayScale = box.FlattenAxisDisplayScale;
            control.BoxDisplayConfig = config;
        }

        private void MigrateLinks(BoundsControl control, BoundingBox box)
        {
            List<string> propertyPaths = new List<string> { "wireframeMaterial", "wireframeEdgeRadius", "wireframeShape", "showWireFrame" };
            LinksConfiguration config = EnsureConfiguration(control.gameObject, control.LinksConfig, HasPropertyOverrides(box, propertyPaths));
            config.WireframeMaterial = box.WireframeMaterial;
            config.WireframeEdgeRadius = box.WireframeEdgeRadius;
            config.WireframeShape = MigrateWireframeShape(box.WireframeShape);
            config.ShowWireFrame = box.ShowWireFrame;
            control.LinksConfig = config;
        }

        private void MigrateScaleHandles(BoundsControl control, BoundingBox box)
        {
            List<string> propertyPaths = new List<string> { "scaleHandleSlatePrefab", "showScaleHandles", "handleMaterial", "handleGrabbedMaterial", 
            "scaleHandlePrefab", "scaleHandleSize", "scaleHandleColliderPadding", "drawTetherWhenManipulating", "handlesIgnoreCollider"};
            ScaleHandlesConfiguration config = EnsureConfiguration(control.gameObject, control.ScaleHandlesConfig, HasPropertyOverrides(box, propertyPaths));
            config.HandleSlatePrefab = box.ScaleHandleSlatePrefab;
            config.ShowScaleHandles = box.ShowScaleHandles;
            config.HandleMaterial = box.HandleMaterial;
            config.HandleGrabbedMaterial = box.HandleGrabbedMaterial;
            config.HandlePrefab = box.ScaleHandlePrefab;
            config.HandleSize = box.ScaleHandleSize;
            config.ColliderPadding = box.ScaleHandleColliderPadding;
            config.DrawTetherWhenManipulating = box.DrawTetherWhenManipulating;
            config.HandlesIgnoreCollider = box.HandlesIgnoreCollider;
            control.ScaleHandlesConfig = config;
        }

        private void MigrateRotationHandles(BoundsControl control, BoundingBox box)
        {
            List<string> propertyPaths = new List<string> { "rotationHandlePrefabColliderType", "showRotationHandleForX", "showRotationHandleForY",
            "showRotationHandleForZ", "handleMaterial", "handleGrabbedMaterial", "rotationHandlePrefab", "rotationHandleSize", "rotateHandleColliderPadding", 
            "drawTetherWhenManipulating", "handlesIgnoreCollider"};
            RotationHandlesConfiguration config = EnsureConfiguration(control.gameObject, control.RotationHandlesConfig, HasPropertyOverrides(box, propertyPaths));
            config.HandlePrefabColliderType = MigrateRotationHandleColliderType(box.RotationHandlePrefabColliderType);
            config.ShowHandleForX = box.ShowRotationHandleForX;
            config.ShowHandleForY = box.ShowRotationHandleForY;
            config.ShowHandleForZ = box.ShowRotationHandleForZ;
            config.HandleMaterial = box.HandleMaterial;
            config.HandleGrabbedMaterial = box.HandleGrabbedMaterial;
            config.HandlePrefab = box.RotationHandlePrefab;
            config.HandleSize = box.RotationHandleSize;
            config.ColliderPadding = box.RotateHandleColliderPadding;
            config.DrawTetherWhenManipulating = box.DrawTetherWhenManipulating;
            config.HandlesIgnoreCollider = box.HandlesIgnoreCollider;
            control.RotationHandlesConfig = config;
        }

        private void MigrateProximityEffect(BoundsControl control, BoundingBox box)
        {
            List<string> propertyPaths = new List<string> { "proximityEffectActive", "handleMediumProximity", "handleCloseProximity", "farScale", 
            "mediumScale", "closeScale", "farGrowRate", "mediumGrowRate", "closeGrowRate"};
            ProximityEffectConfiguration config = EnsureConfiguration(control.gameObject, control.HandleProximityEffectConfig, HasPropertyOverrides(box, propertyPaths));
            config.ProximityEffectActive = box.ProximityEffectActive;
            config.ObjectMediumProximity = box.HandleMediumProximity;
            config.ObjectCloseProximity = box.HandleCloseProximity;
            config.FarScale = box.FarScale;
            config.MediumScale = box.MediumScale;
            config.CloseScale = box.CloseScale;
            config.FarGrowRate = box.FarGrowRate;
            config.MediumGrowRate = box.MediumGrowRate;
            config.CloseGrowRate = box.CloseGrowRate;
            control.HandleProximityEffectConfig = config;
        }

        #endregion Visuals Configuration Migration

        private void MigrateAppBar(BoundingBox boundingBox, BoundsControl boundsControl)
        {
            // note: this might be expensive for larger scenes but we don't know where the appbar is 
            // placed in the hierarchy so we have to search the scene for it
            AppBar[] appBars = Object.FindObjectsOfType<AppBar>();
            for (int i = 0; i < appBars.Length; ++i)
            {
                AppBar appBar = appBars[i];
                if (appBar.Target == boundingBox)
                {
                    Undo.RecordObject(appBar, "BoundsControl migration: changed target of app bar.");
                    appBar.Target = boundsControl;
                    EditorUtility.SetDirty(appBar);
                }
            }
        }
    }
}