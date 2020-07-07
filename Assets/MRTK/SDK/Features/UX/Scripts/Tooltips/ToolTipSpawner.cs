// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Add to any Object to spawn ToolTips on tap or on focus, according to preference
    /// Applies its follow settings to the spawned ToolTip's ToolTipConnector component
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ToolTipSpawner")]
    public class ToolTipSpawner : PrefabSpawner
    {
        private enum SettingsMode
        {
            UseDefaults = 0,
            Override
        }

        [Header("ToolTip Override Settings")]
        [Tooltip("Prefab's settings will be used unless this is set to Override")]
        [SerializeField]
        private SettingsMode settingsMode = SettingsMode.UseDefaults;
        [SerializeField]
        private bool showBackground = true;
        [SerializeField]
        private bool showOutline = false;
        [SerializeField]
        private bool showConnector = true;
        [SerializeField]
        private ConnectorFollowType followType = ConnectorFollowType.AnchorOnly;
        [SerializeField]
        private ConnectorPivotMode pivotMode = ConnectorPivotMode.Manual;
        [SerializeField]
        private ConnectorPivotDirection pivotDirection = ConnectorPivotDirection.North;
        [SerializeField]
        private ConnectorOrientType pivotDirectionOrient = ConnectorOrientType.OrientToObject;
        [SerializeField]
        private Vector3 manualPivotDirection = Vector3.up;
        [SerializeField]
        private Vector3 manualPivotLocalPosition = Vector3.up;
        [SerializeField]
        [Range(0f, 2f)]
        private float pivotDistance = 0.25f;

        [SerializeField]
        private string toolTipText = "New Tooltip";

        [SerializeField]
        private Transform anchor = null;

        protected override void SpawnableActivated(GameObject spawnable)
        {
            var toolTip = spawnable.GetComponent<ToolTip>();

            toolTip.ToolTipText = toolTipText;
            var connector = toolTip.GetComponent<ToolTipConnector>();
            connector.Target = (anchor != null) ? anchor.gameObject : gameObject;

            switch (settingsMode)
            {
                case SettingsMode.UseDefaults:
                    break;

                case SettingsMode.Override:
                    toolTip.ShowBackground = showBackground;
                    toolTip.ShowHighlight = showOutline;
                    toolTip.ShowConnector = showConnector;

                    connector.PivotDirection = pivotDirection;
                    connector.PivotDistance = pivotDistance;
                    connector.PivotDirectionOrient = pivotDirectionOrient;
                    connector.ManualPivotLocalPosition = manualPivotLocalPosition;
                    connector.ManualPivotDirection = manualPivotDirection;
                    connector.ConnectorFollowingType = followType;
                    connector.PivotMode = pivotMode;

                    if (connector.PivotMode == ConnectorPivotMode.Manual)
                    {
                        toolTip.PivotPosition = transform.TransformPoint(manualPivotLocalPosition);
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying) { return; }

            if (prefab == null) { return; }

            if (gameObject == UnityEditor.Selection.activeGameObject)
            {
                Gizmos.color = Color.cyan;
                Transform relativeTo = null;

                ConnectorFollowType followType = this.followType;
                ConnectorOrientType pivotDirectionOrient = this.pivotDirectionOrient;
                ConnectorPivotDirection pivotDirection = this.pivotDirection;
                ConnectorPivotMode pivotMode = this.pivotMode;
                Vector3 manualPivotDirection = this.manualPivotDirection;
                Vector3 manualPivotLocalPosition = this.manualPivotLocalPosition;
                float pivotDistance = this.pivotDistance;

                switch (settingsMode)
                {
                    case SettingsMode.UseDefaults:
                        ToolTipConnector connector = prefab.GetComponent<ToolTipConnector>();
                        followType = connector.ConnectorFollowingType;
                        pivotDirectionOrient = connector.PivotDirectionOrient;
                        pivotDirection = connector.PivotDirection;
                        pivotMode = connector.PivotMode;
                        manualPivotDirection = connector.ManualPivotDirection;
                        manualPivotLocalPosition = connector.ManualPivotLocalPosition;
                        pivotDistance = connector.PivotDistance;
                        break;
                }

                switch (pivotDirectionOrient)
                {
                    case ConnectorOrientType.OrientToCamera:
                        relativeTo = CameraCache.Main.transform;
                        break;

                    case ConnectorOrientType.OrientToObject:
                        relativeTo = (anchor != null) ? anchor.transform : transform;
                        break;
                }

                Vector3 targetPosition = (anchor != null) ? anchor.transform.position : transform.position;

                switch (followType)
                {
                    case ConnectorFollowType.AnchorOnly:
                        Gizmos.DrawLine(targetPosition, transform.TransformPoint(manualPivotLocalPosition));
                        Gizmos.DrawWireCube(transform.TransformPoint(manualPivotLocalPosition), Vector3.one * 0.05f);
                        break;

                    case ConnectorFollowType.Position:
                    case ConnectorFollowType.PositionAndXYRotation:
                    case ConnectorFollowType.PositionAndYRotation:
                        if (pivotMode == ConnectorPivotMode.Automatic)
                        {
                            Vector3 toolTipPosition = targetPosition + ToolTipConnector.GetDirectionFromPivotDirection(
                                            pivotDirection,
                                            manualPivotDirection,
                                            relativeTo) * pivotDistance;
                            Gizmos.DrawLine(targetPosition, toolTipPosition);
                            Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                        }
                        else
                        {
                            Vector3 toolTipPosition = transform.TransformPoint(manualPivotLocalPosition);
                            Gizmos.DrawLine(targetPosition, toolTipPosition);
                            Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                        }
                        break;
                }
            }
        }
#endif // UNITY_EDITOR
    }
}
