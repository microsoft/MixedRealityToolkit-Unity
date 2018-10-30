// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Add to any Object to spawn ToolTips on tap or on focus, according to preference
    /// Applies its follow settings to the spawned ToolTip's ToolTipConnector component
    /// </summary>
    public class ToolTipSpawner : MonoBehaviour, IMixedRealityInputHandler, IMixedRealityFocusHandler
    {
        private enum VanishType
        {
            VanishOnFocusExit,
            VanishOnTap,
        }

        private enum AppearType
        {
            AppearOnFocusEnter,
            AppearOnTap,
        }

        public enum RemainType
        {
            Indefinite,
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
        private ConnnectorPivotModeType pivotMode = ConnnectorPivotModeType.Manual;

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
        private string ToolTipText = "New Tooltip";

        [SerializeField]
        private Transform Anchor = null;

        private float focusEnterTime = 0f;

        private float focusExitTime = 0f;

        private float tappedTime = 0f;

        private bool hasFocus;

        private ToolTip toolTip;

        /// <summary>
        /// methods associated with IInputHandler
        /// </summary>
        /// <param name="eventData"></param>
        public void OnFocusEnter(FocusEventData eventData)
        {
            focusEnterTime = Time.unscaledTime;
            hasFocus = true;
            if (toolTip == null || !toolTip.gameObject.activeSelf)
            {
                switch (appearType)
                {
                    case AppearType.AppearOnFocusEnter:
                        ShowToolTip();
                        break;

                    default:
                        break;
                }
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            focusExitTime = Time.unscaledTime;
            hasFocus = false;
        }

        public void OnBeforeFocusChange(FocusEventData eventData)
        { }

        public void OnFocusChanged(FocusEventData eventData)
        { }

        public void OnInputPressed(InputEventData<float> eventData)
        {
            if (eventData.InputData > .95f)
            {
                //tappedTime = Time.unscaledTime;
            }
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

        public void OnPositionInputChanged(InputEventData<Vector2> eventData)
        { }

        public void OnInputDown(InputEventData eventData)
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

        /// <summary>
        /// This Handler intentionally empty
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputUp(InputEventData eventData)
        {
        }

        private void ShowToolTip()
        {
            StartCoroutine(UpdateTooltip(focusEnterTime, tappedTime));
        }

        private IEnumerator UpdateTooltip(float focusEnterTimeOnStart, float tappedTimeOnStart)
        {
            if (toolTip == null)
            {
                GameObject toolTipGo = GameObject.Instantiate(toolTipPrefab) as GameObject;
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
                yield return new WaitForSeconds(appearDelay);
                // If we don't have focus any more, get out of here
                if (!hasFocus)
                {
                    yield break;
                }
            }

            toolTip.ToolTipText = ToolTipText;
            toolTip.gameObject.SetActive(true);
            ToolTipConnector connector = toolTip.GetComponent<ToolTipConnector>();
            connector.Target = (Anchor != null) ? Anchor.gameObject : gameObject;
            connector.PivotDirection = pivotDirection;
            connector.PivotDirectionOrient = pivotDirectionOrient;
            connector.ManualPivotLocalPosition = manualPivotLocalPosition;
            connector.ManualPivotDirection = manualPivotDirection;
            connector.FollowingType = followType;
            connector.PivotingMode = pivotMode;

            if (pivotMode == ConnnectorPivotModeType.Manual)
            {
                toolTip.PivotPosition = transform.TransformPoint(manualPivotLocalPosition);
            }

            while (toolTip.gameObject.activeSelf)
            {
                if (remainType == RemainType.Timeout)
                {
                    if (appearType == AppearType.AppearOnTap)
                    {
                        if (Time.unscaledTime - tappedTime >= lifetime)
                        {
                            toolTip.gameObject.SetActive(false);
                            yield break;
                        }
                    }
                    else if (appearType == AppearType.AppearOnFocusEnter)
                    {
                        if (Time.unscaledTime - focusEnterTime >= lifetime)
                        {
                            toolTip.gameObject.SetActive(false);
                            yield break;
                        }
                    }
                }
                //check whether we're suppose to disappear
                switch (vanishType)
                {
                    case VanishType.VanishOnFocusExit:
                        if (!hasFocus)
                        {
                            toolTip.gameObject.SetActive(false);
                        }
                        break;

                    case VanishType.VanishOnTap:
                        if (tappedTime != tappedTimeOnStart)
                        {
                            toolTip.gameObject.SetActive(false);
                        }
                        break;

                    default:
                        if (!hasFocus)
                        {
                            if (Time.time - focusExitTime > vanishDelay)
                            {
                                toolTip.gameObject.SetActive(false);
                            }
                        }
                        break;
                }
                yield return null;
            }
            yield break;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            if (gameObject == UnityEditor.Selection.activeGameObject)
            {
                Gizmos.color = Color.cyan;
                Transform relativeTo = null;
                switch (pivotDirectionOrient)
                {
                    case ConnectorOrientType.OrientToCamera:
                        relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                        break;

                    case ConnectorOrientType.OrientToObject:
                        relativeTo = (Anchor != null) ? Anchor.transform : transform;
                        break;
                }
                if (pivotMode == ConnnectorPivotModeType.Automatic)
                {
                    Vector3 targetPosition = (Anchor != null) ? Anchor.transform.position : transform.position;
                    Vector3 toolTipPosition = targetPosition + ToolTipConnector.GetDirectionFromPivotDirection(
                                    pivotDirection,
                                    manualPivotDirection,
                                    relativeTo) * pivotDistance;
                    Gizmos.DrawLine(targetPosition, toolTipPosition);
                    Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                    //Gizmos.DrawWireCube(toolTipPosition, defaultDimensions);
                }
                else
                {
                    Vector3 targetPosition = (Anchor != null) ? Anchor.transform.position : transform.position;
                    Vector3 toolTipPosition = transform.TransformPoint(manualPivotLocalPosition);
                    Gizmos.DrawLine(targetPosition, toolTipPosition);
                    Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                    //Gizmos.DrawWireCube(toolTipPosition, new Vector3(defaultDimensions.z, defaultDimensions.x, defaultDimensions.y));
                }
            }
        }
#endif
    }
}
