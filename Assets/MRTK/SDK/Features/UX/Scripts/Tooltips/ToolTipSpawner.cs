// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Add to any Object to spawn ToolTips on tap or on focus, according to preference
    /// Applies its follow settings to the spawned ToolTip's ToolTipConnector component
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ToolTipSpawner")]
    public class ToolTipSpawner :
        BaseFocusHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<float>
    {
        private enum SettingsMode
        {
            UseDefaults = 0,
            Override
        }

        private enum VanishType
        {
            VanishOnFocusExit = 0,
            VanishOnTap,
        }

        private enum AppearType
        {
            AppearOnFocusEnter = 0,
            AppearOnTap,
        }

        public enum RemainType
        {
            Indefinite = 0,
            Timeout,
        }

        [SerializeField]
        private GameObject toolTipPrefab = null;

        [Header("Input Settings")]
        [SerializeField]
        [Tooltip("The action that will be used for when to spawn or toggle the tooltip.")]
        private MixedRealityInputAction tooltipToggleAction = MixedRealityInputAction.None;

        [Header("Appear / Vanish Behavior Settings")]
        [SerializeField]
        private AppearType appearType = AppearType.AppearOnFocusEnter;
        [SerializeField]
        private VanishType vanishType = VanishType.VanishOnFocusExit;
        [SerializeField]
        private RemainType remainType = RemainType.Timeout;
        [SerializeField]
        [Range(0f, 5f)]
        private float appearDelay = 0.0f;
        [SerializeField]
        [Range(0f, 5f)]
        private float vanishDelay = 2.0f;
        [SerializeField]
        [Range(0.5f, 10.0f)]
        private float lifetime = 1.0f;

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

        private float focusEnterTime = 0f;
        private float focusExitTime = 0f;
        private float tappedTime = 0f;
        private ToolTip toolTip;

        /// <inheritdoc />
        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            HandleFocusEnter();
        }

        /// <inheritdoc />
        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            HandleFocusExit();
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler<float>.OnInputChanged(InputEventData<float> eventData)
        {
            if (eventData.InputData > .95f) 
            {
                HandleTap();
            }
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            if (tooltipToggleAction.Id == eventData.MixedRealityInputAction.Id)
            {
                HandleTap();
            }
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData) { }

        private void HandleTap()
        {
            tappedTime = Time.unscaledTime;

            if (toolTip == null || !toolTip.gameObject.activeSelf)
            {
                switch (appearType)
                {
                    case AppearType.AppearOnTap:
                        ShowToolTip();
                        break;
                }
            }
            else
            {
                switch (vanishType)
                {
                    case VanishType.VanishOnTap:
                        toolTip.gameObject.SetActive(false);
                        break;
                }
            }
        }

        private void HandleFocusEnter()
        {
            focusEnterTime = Time.unscaledTime;

            if (toolTip == null || !toolTip.gameObject.activeSelf)
            {
                switch (appearType)
                {
                    case AppearType.AppearOnFocusEnter:
                        ShowToolTip();
                        break;
                }
            }
        }

        private void HandleFocusExit()
        {
            focusExitTime = Time.unscaledTime;
        }

        private async void ShowToolTip()
        {
            await UpdateTooltip(focusEnterTime, tappedTime);
        }

        private async Task UpdateTooltip(float focusEnterTimeOnStart, float tappedTimeOnStart)
        {
            if (toolTip == null)
            {
                var toolTipGo = Instantiate(toolTipPrefab);
                toolTip = toolTipGo.GetComponent<ToolTip>();
                toolTip.gameObject.SetActive(false);
                toolTip.transform.position = transform.position;
                toolTip.transform.parent = transform;
            }

            if (appearType == AppearType.AppearOnFocusEnter)
            {
                // Wait for the appear delay
                await new WaitForSeconds(appearDelay);
                // If we don't have focus any more, get out of here

                if (!HasFocus)
                {
                    return;
                }
            }

            toolTip.ToolTipText = toolTipText;
            toolTip.gameObject.SetActive(true);
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

            while (toolTip.gameObject.activeSelf)
            {
                if (remainType == RemainType.Timeout)
                {
                    switch (appearType)
                    {
                        case AppearType.AppearOnTap:
                            if (Time.unscaledTime - tappedTime >= lifetime)
                            {
                                toolTip.gameObject.SetActive(false);
                                return;
                            }

                            break;
                        case AppearType.AppearOnFocusEnter:
                            if (Time.unscaledTime - focusEnterTime >= lifetime)
                            {
                                toolTip.gameObject.SetActive(false);
                                return;
                            }

                            break;
                    }
                }

                // Check whether we're suppose to disappear
                switch (vanishType)
                {
                    case VanishType.VanishOnFocusExit:
                        if (!HasFocus)
                        {
                            toolTip.gameObject.SetActive(false);
                        }

                        break;

                    case VanishType.VanishOnTap:
                        if (!tappedTime.Equals(tappedTimeOnStart))
                        {
                            toolTip.gameObject.SetActive(false);
                        }

                        break;

                    default:
                        if (!HasFocus)
                        {
                            if (Time.time - focusExitTime > vanishDelay)
                            {
                                toolTip.gameObject.SetActive(false);
                            }
                        }
                        break;
                }

                await new WaitForUpdate();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying) { return; }

            if (toolTipPrefab == null) { return; }

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
                        ToolTipConnector connector = toolTipPrefab.GetComponent<ToolTipConnector>();
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
