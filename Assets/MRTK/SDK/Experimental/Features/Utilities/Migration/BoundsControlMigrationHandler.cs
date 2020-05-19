// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;

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
#if UNITY_EDITOR
            var boundingBox = gameObject.GetComponent<BoundingBox>();
            var boundsControl = gameObject.AddComponent<BoundsControl>();

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
            }

            // look in the scene for app bars and upgrade them too to point to the new component
            MigrateAppBar(boundingBox, boundsControl);
      
            {
                Undo.RecordObject(gameObject, "Removing obsolete BoundingBox component");
                // destroy obsolete component
                Object.DestroyImmediate(boundingBox);
            }
#endif
        }

#if UNITY_EDITOR
        virtual protected string GetBoundsControlConfigDirectory(BoundingBox boundingBox)
        {
            var scene = boundingBox.gameObject.scene;
            if (scene != null)
            {
                string scenePath = scene.path;             
                string sceneDir = System.IO.Path.GetDirectoryName(scenePath);
                // if empty we're creating the folder in the asset root.
                // This should only happen if we're trying to migrate a dynamically created
                // gameobject - which is usually only in test scenarios
                if (sceneDir == "")
                {
                    sceneDir = "Assets";
                }

                const string configDir = "BoundsControlConfigs";
                string configPath = System.IO.Path.Combine(sceneDir, configDir);
                if (AssetDatabase.IsValidFolder(configPath))
                {
                    return configPath;
                }
                else
                {
                    string guid = AssetDatabase.CreateFolder(sceneDir, configDir);
                    return AssetDatabase.GUIDToAssetPath(guid);
                }
            }

            return "";
        }
        private string GenerateUniqueConfigName(string directory, GameObject migratingFrom, string configName)
        {
            return directory + "/" + migratingFrom.name + migratingFrom.GetInstanceID() + configName + ".asset";
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

        private void MigrateBoxDisplay(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            BoxDisplayConfiguration config = ScriptableObject.CreateInstance<BoxDisplayConfiguration>();
            config.BoxMaterial = box.BoxMaterial;
            config.BoxGrabbedMaterial = box.BoxGrabbedMaterial;
            config.FlattenAxisDisplayScale = box.FlattenAxisDisplayScale;
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "BoxDisplayConfiguration"));

            control.BoxDisplayConfig = config;
        }

        private void MigrateLinks(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            LinksConfiguration config = ScriptableObject.CreateInstance<LinksConfiguration>();
            config.WireframeMaterial = box.WireframeMaterial;
            config.WireframeEdgeRadius = box.WireframeEdgeRadius;
            config.WireframeShape = MigrateWireframeShape(box.WireframeShape);
            config.ShowWireFrame = box.ShowWireFrame;
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "LinksConfiguration"));

            control.LinksConfig = config;

        }

        private void MigrateScaleHandles(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            ScaleHandlesConfiguration config = ScriptableObject.CreateInstance<ScaleHandlesConfiguration>();
            config.HandleSlatePrefab = box.ScaleHandleSlatePrefab;
            config.ShowScaleHandles = box.ShowScaleHandles;
            config.HandleMaterial = box.HandleMaterial;
            config.HandleGrabbedMaterial = box.HandleGrabbedMaterial;
            config.HandlePrefab = box.ScaleHandlePrefab;
            config.HandleSize = box.ScaleHandleSize;
            config.ColliderPadding = box.ScaleHandleColliderPadding;
            config.DrawTetherWhenManipulating = box.DrawTetherWhenManipulating;
            config.HandlesIgnoreCollider = box.HandlesIgnoreCollider;
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "ScaleHandlesConfiguration"));

            control.ScaleHandlesConfig = config;
        }

        private void MigrateRotationHandles(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            RotationHandlesConfiguration config = ScriptableObject.CreateInstance<RotationHandlesConfiguration>();
            config.RotationHandlePrefabColliderType = MigrateRotationHandleColliderType(box.RotationHandlePrefabColliderType);
            config.ShowRotationHandleForX = box.ShowRotationHandleForX;
            config.ShowRotationHandleForY = box.ShowRotationHandleForY;
            config.ShowRotationHandleForZ = box.ShowRotationHandleForZ;
            config.HandleMaterial = box.HandleMaterial;
            config.HandleGrabbedMaterial = box.HandleGrabbedMaterial;
            config.HandlePrefab = box.RotationHandlePrefab;
            config.HandleSize = box.RotationHandleSize;
            config.ColliderPadding = box.RotateHandleColliderPadding;
            config.DrawTetherWhenManipulating = box.DrawTetherWhenManipulating;
            config.HandlesIgnoreCollider = box.HandlesIgnoreCollider;
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "RotationHandlesConfiguration"));

            control.RotationHandlesConfig = config;
        }

        private void MigrateProximityEffect(BoundsControl control, BoundingBox box, string configAssetDirectory)
        {
            ProximityEffectConfiguration config = ScriptableObject.CreateInstance<ProximityEffectConfiguration>();
            config.ProximityEffectActive = box.ProximityEffectActive;
            config.ObjectMediumProximity = box.HandleMediumProximity;
            config.ObjectCloseProximity = box.HandleCloseProximity;
            config.FarScale = box.FarScale;
            config.MediumScale = box.MediumScale;
            config.CloseScale = box.CloseScale;
            config.FarGrowRate = box.FarGrowRate;
            config.MediumGrowRate = box.MediumGrowRate;
            config.CloseGrowRate = box.CloseGrowRate;            
            AssetDatabase.CreateAsset(config, GenerateUniqueConfigName(configAssetDirectory, box.gameObject, "ProximityEffectConfiguration"));

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
#endif

    }
}