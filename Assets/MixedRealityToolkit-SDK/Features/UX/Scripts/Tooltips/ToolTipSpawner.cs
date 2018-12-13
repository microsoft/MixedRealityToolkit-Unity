// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async.AwaitYieldInstructions;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Add to any Object to spawn ToolTips on tap or on focus, according to preference
    /// Applies its follow settings to the spawned ToolTip's ToolTipConnector component
    /// </summary>
    public class ToolTipSpawner : BaseFocusHandler, IMixedRealityInputHandler
    {
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
        private Vector3 defaultDimensions = new Vector3(0.182f, 0.028f, 1.0f);

        [SerializeField]
        private bool showBackground = true;

        [SerializeField]
        private bool showOutline = false;

        [SerializeField]
        private bool showConnector = true;

        [SerializeField]
        private AppearType appearType = AppearType.AppearOnFocusEnter;

        [SerializeField]
        private VanishType vanishType = VanishType.VanishOnFocusExit;

        [SerializeField]
        private RemainType remainType = RemainType.Timeout;

        [SerializeField]
        [Tooltip("The action that will be used for when to spawn or toggle the tooltip.")]
        private MixedRealityInputAction tooltipToggleAction = MixedRealityInputAction.None;

        [SerializeField]
        [Range(0f, 5f)]
        private float appearDelay = 0.0f;

        [SerializeField]
        [Range(0f, 5f)]
        private float vanishDelay = 2.0f;

        [SerializeField]
        [Range(0.5f, 10.0f)]
        private float lifetime = 1.0f;

        [SerializeField]
        private GameObject toolTipPrefab = null;

        [SerializeField]
        private ConnectorFollowType followType = ConnectorFollowType.AnchorOnly;

        [SerializeField]
        private ConnectorPivotModeType pivotMode = ConnectorPivotModeType.Manual;

        [SerializeField]
        private ConnectorPivotDirectionType pivotDirection = ConnectorPivotDirectionType.North;

        [SerializeField]
        private ConnectorOrientType pivotDirectionOrient = ConnectorOrientType.OrientToObject;

        [SerializeField]
        private Vector3 manualPivotDirection = Vector3.up;

        [SerializeField]
        private Vector3 manualPivotLocalPosition = Vector3.up;

#if UNITY_EDITOR
        [SerializeField]
        [Range(0f, 1f)]
        private float pivotDistance = 0.25f;
#endif

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

        /// <inheritdoc />
        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            focusExitTime = Time.unscaledTime;
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnInputPressed(InputEventData<float> eventData)
        {
            //if (eventData.InputData > .95f)
            //{
            //tappedTime = Time.unscaledTime;
            //}
            //if (toolTip == null || !toolTip.gameObject.activeSelf)
            //{
            //  switch (vanishType)
            //  {
            //      case VanishType.VanishOnTap:
            //          toolTip.gameObject.SetActive(false);
            //          break;

            //      default:
            //          break;
            //  }
            //  switch (appearType)
            //  {
            //      case AppearType.AppearOnTap:
            //          ShowToolTip();
            //          break;

            //      default:
            //          break;
            //  }
            //}
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnPositionInputChanged(InputEventData<Vector2> eventData) { }

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            if (tooltipToggleAction.Id == eventData.MixedRealityInputAction.Id)
            {
                tappedTime = Time.unscaledTime;

                if (toolTip == null || !toolTip.gameObject.activeSelf)
                {
                    if (appearType == AppearType.AppearOnTap)
                    {
                        ShowToolTip();
                    }
                }
            }
        }

        /// <inheritdoc />
        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData) { }

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
                toolTip.ShowBackground = showBackground;
                toolTip.ShowOutline = showOutline;
                toolTip.ShowConnector = showConnector;
                toolTip.transform.position = transform.position;
                toolTip.transform.parent = transform;
                toolTip.ContentParentTransform.localScale = defaultDimensions;
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
            connector.PivotDirection = pivotDirection;
            connector.PivotDirectionOrient = pivotDirectionOrient;
            connector.ManualPivotLocalPosition = manualPivotLocalPosition;
            connector.ManualPivotDirection = manualPivotDirection;
            connector.ConnectorFollowingType = followType;
            connector.PivotMode = pivotMode;

            if (pivotMode == ConnectorPivotModeType.Manual)
            {
                toolTip.PivotPosition = transform.TransformPoint(manualPivotLocalPosition);
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

                //check whether we're suppose to disappear
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

            if (gameObject == UnityEditor.Selection.activeGameObject)
            {
                Gizmos.color = Color.cyan;
                Transform relativeTo = null;
                switch (pivotDirectionOrient)
                {
                    case ConnectorOrientType.OrientToCamera:
                        relativeTo = CameraCache.Main.transform;
                        break;

                    case ConnectorOrientType.OrientToObject:
                        relativeTo = (anchor != null) ? anchor.transform : transform;
                        break;
                }
                if (pivotMode == ConnectorPivotModeType.Automatic)
                {
                    Vector3 targetPosition = (anchor != null) ? anchor.transform.position : transform.position;
                    Vector3 toolTipPosition = targetPosition + ToolTipConnector.GetDirectionFromPivotDirection(
                                    pivotDirection,
                                    manualPivotDirection,
                                    relativeTo) * pivotDistance;
                    Gizmos.DrawLine(targetPosition, toolTipPosition);
                    Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                }
                else
                {
                    Vector3 targetPosition = (anchor != null) ? anchor.transform.position : transform.position;
                    Vector3 toolTipPosition = transform.TransformPoint(manualPivotLocalPosition);
                    Gizmos.DrawLine(targetPosition, toolTipPosition);
                    Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                }
            }
        }
#endif // UNITY_EDITOR
    }
}
