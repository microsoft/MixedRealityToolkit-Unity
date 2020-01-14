// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
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
            var boundsControl = gameObject.AddComponent<BoundsControl>();

            // migrate logic settings
            boundsControl.Target = boundingBox.Target;
            boundsControl.BoundsOverride = boundingBox.BoundsOverride;
            boundsControl.CalculationMethod = MigrateCalculationMethod(boundingBox.CalculationMethod);
            boundsControl.BoundsControlActivation = MigrateActivationFlag(boundingBox.BoundingBoxActivation);

            // only carry over min max scaling values if user hasn't attached min max scale constraint component yet 
            if (gameObject.GetComponent<MinMaxScaleConstraint>() == null)
            {
                MinMaxScaleConstraint scaleConstraint = gameObject.AddComponent<MinMaxScaleConstraint>();
#pragma warning disable 0618
                scaleConstraint.ScaleMinimum = boundingBox.ScaleMinimum;
                scaleConstraint.ScaleMaximum = boundingBox.ScaleMaximum;
#pragma warning restore 0618
            }

            // migrate visuals
            boundsControl.DrawTetherWhenManipulating = boundingBox.DrawTetherWhenManipulating;
            boundsControl.HandlesIgnoreCollider = boundingBox.HandlesIgnoreCollider;
            boundsControl.FlattenAxis = MigrateFlattenAxis(boundingBox.FlattenAxis);
            boundsControl.BoxPadding = boundingBox.BoxPadding;
            string configDir = GetBoundsControlConfigDirectory(boundingBox);
            MigrateBoxDisplay(boundsControl, boundingBox, configDir);
            MigrateLinks(boundsControl, boundingBox, configDir);
            MigrateScaleHandles(boundsControl, boundingBox, configDir);
            MigrateRotationHandles(boundsControl, boundingBox, configDir);
            MigrateProximityEffect(boundsControl, boundingBox, configDir);

            // debug properties
            boundsControl.DebugText = boundingBox.debugText;
            boundsControl.HideElementsInInspector = boundingBox.HideElementsInInspector;

            // events
            boundsControl.RotateStarted = boundingBox.RotateStarted;
            boundsControl.RotateStopped = boundingBox.RotateStopped;
            boundsControl.ScaleStarted = boundingBox.ScaleStarted;
            boundsControl.ScaleStopped = boundingBox.ScaleStopped;

            // destroy obsolete component
            UnityEngine.Object.DestroyImmediate(boundingBox);
        }

        private string GetBoundsControlConfigDirectory(BoundingBox boundingBox)
        {
            // todo: this needs a better logic but will work for converting the scene now
            var scene = boundingBox.gameObject.scene;
            if (scene != null)
            {
                string scenePath = scene.path;
                string dirPath = System.IO.Path.GetDirectoryName(scenePath);
                string configPath = System.IO.Path.Combine(dirPath, "BoundsControlConfigs/");
                return configPath;
            }

            return "";
        }
        private string GenerateUniqueConfigName(string directory, GameObject migratingFrom, string configName)
        {
            return directory + migratingFrom.name + migratingFrom.GetInstanceID() + configName + ".asset";
        }

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

        WireframeType MigrateWireframeShape(BoundingBox.WireframeType wireframeType)
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

        private RotationHandlePrefabCollider MigrateRotationHandleColliderType(BoundingBox.RotationHandlePrefabCollider rotationHandlePrefabColliderType)
        {
            switch (rotationHandlePrefabColliderType)
            {
                case BoundingBox.RotationHandlePrefabCollider.Sphere:
                    return RotationHandlePrefabCollider.Sphere;
                case BoundingBox.RotationHandlePrefabCollider.Box:
                    return RotationHandlePrefabCollider.Box;
            }

            Debug.Assert(false, "Tried to migrate unsupported rotation handle collider type in bounding box / bounds control");
            return RotationHandlePrefabCollider.Sphere;
        }

        private void MigrateBoxDisplay(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            BoxDisplayConfiguration config = new BoxDisplayConfiguration();
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "BoxDisplayConfiguration"));

            config.BoxMaterial = box.BoxMaterial;
            config.BoxGrabbedMaterial = box.BoxGrabbedMaterial;
            config.FlattenAxisDisplayScale = box.FlattenAxisDisplayScale;

            control.BoxDisplayConfiguration = config;
        }

        private void MigrateLinks(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            LinksConfiguration config = new LinksConfiguration();
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "LinksConfiguration"));
            
            config.WireframeMaterial = box.WireframeMaterial;
            config.WireframeEdgeRadius = box.WireframeEdgeRadius;
            config.WireframeShape = MigrateWireframeShape(box.WireframeShape);
            config.ShowWireFrame = box.ShowWireFrame;

            control.LinksConfiguration = config;

        }

        private void MigrateScaleHandles(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            ScaleHandlesConfiguration config = new ScaleHandlesConfiguration();
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "ScaleHandlesConfiguration"));
            
            config.HandleSlatePrefab = box.ScaleHandleSlatePrefab;
            config.ShowScaleHandles = box.ShowScaleHandles;
            config.HandleMaterial = box.HandleMaterial;
            config.HandleGrabbedMaterial = box.HandleGrabbedMaterial;
            config.HandlePrefab = box.ScaleHandlePrefab;
            config.HandleSize = box.ScaleHandleSize;
            config.ColliderPadding = box.ScaleHandleColliderPadding;            

            control.ScaleHandlesConfiguration = config;
        }

        private void MigrateRotationHandles(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            RotationHandlesConfiguration config = new RotationHandlesConfiguration();
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "RotationHandlesConfiguration"));
            
            config.RotationHandlePrefabColliderType = MigrateRotationHandleColliderType(box.RotationHandlePrefabColliderType);
            config.ShowRotationHandleForX = box.ShowRotationHandleForX;
            config.ShowRotationHandleForY = box.ShowRotationHandleForY;
            config.ShowRotationHandleForZ = box.ShowRotationHandleForZ;
            config.HandleMaterial = box.HandleMaterial;
            config.HandleGrabbedMaterial = box.HandleGrabbedMaterial;
            config.HandlePrefab = box.RotationHandleSlatePrefab;
            config.HandleSize = box.RotationHandleSize;
            config.ColliderPadding = box.RotateHandleColliderPadding;

            control.RotationHandles = config;
        }

        private void MigrateProximityEffect(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            ProximityEffectConfiguration config = new ProximityEffectConfiguration();
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "ProximityEffectConfiguration"));
            
            config.ProximityEffectActive = box.ProximityEffectActive;
            config.ObjectMediumProximity = box.HandleMediumProximity;
            config.ObjectCloseProximity = box.HandleCloseProximity;
            config.FarScale = box.FarScale;
            config.MediumScale = box.MediumScale;
            config.CloseScale = box.CloseScale;
            config.FarGrowRate = box.FarGrowRate;
            config.MediumGrowRate = box.MediumGrowRate;
            config.CloseGrowRate = box.CloseGrowRate;            

            control.HandleProximityEffectConfiguration = config;
        }
    }
}