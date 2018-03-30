// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.EventData;

namespace MixedRealityToolkit.UX.ToolTips
{
    /// <summary>
    /// Add to any Object to spawn ToolTips on tap or on focus, according to preference
    /// Applies its follow settings to the spawned ToolTip's ToolTipConnector component
    /// </summary>
    public class ToolTipSpawner : MonoBehaviour, IInputHandler, IPointerSpecificFocusable
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
        private ToolTipConnector.ConnectorFollowType followType = ToolTipConnector.ConnectorFollowType.AnchorOnly;

        [SerializeField]
        private ToolTipConnector.ConnnectorPivotMode pivotMode = ToolTipConnector.ConnnectorPivotMode.Manual;

        [SerializeField]
        private ToolTipConnector.ConnectorPivotDirection pivotDirection = ToolTipConnector.ConnectorPivotDirection.North;

        [SerializeField]
        private ToolTipConnector.ConnectorOrientType pivotDirectionOrient = ToolTipConnector.ConnectorOrientType.OrientToObject;

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

        private bool hasFocus = false;

        private ToolTip toolTip = null;

        public void OnFocusEnter(PointerSpecificEventData eventData)
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
                }
            }
        }

        public void OnFocusExit(PointerSpecificEventData eventData)
        {
            focusExitTime = Time.unscaledTime;
            hasFocus = false;
        }

        public void OnInputDown(InputEventData eventData)
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

        public void OnInputUp(InputEventData eventData) { }

        private void ShowToolTip()
        {
            StartCoroutine(UpdateTooltip(tappedTime));
        }

        private IEnumerator UpdateTooltip(float tappedTimeOnStart)
        {
            if (toolTip == null)
            {
                GameObject toolTipGo = Instantiate(toolTipPrefab);
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

            toolTip.ToolTipText = toolTipText;
            toolTip.gameObject.SetActive(true);
            var connector = toolTip.GetComponent<ToolTipConnector>();
            connector.Target = (anchor != null) ? anchor.gameObject : gameObject;
            connector.PivotDirection = pivotDirection;
            connector.PivotDirectionOrient = pivotDirectionOrient;
            connector.ManualPivotLocalPosition = manualPivotLocalPosition;
            connector.ManualPivotDirection = manualPivotDirection;
            connector.FollowingType = followType;
            connector.PivotingMode = pivotMode;

            if (pivotMode == ToolTipConnector.ConnnectorPivotMode.Manual)
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
                        break;

                    case VanishType.VanishOnTap:
                        if (!tappedTime.Equals(tappedTimeOnStart))
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
                    case ToolTipConnector.ConnectorOrientType.OrientToCamera:
                        relativeTo = Camera.main.transform;
                        break;

                    case ToolTipConnector.ConnectorOrientType.OrientToObject:
                        relativeTo = (anchor != null) ? anchor.transform : transform;
                        break;
                }

                if (pivotMode == ToolTipConnector.ConnnectorPivotMode.Automatic)
                {
                    Vector3 targetPosition = (anchor != null) ? anchor.transform.position : transform.position;
                    Vector3 direction = ToolTipConnector.GetDirectionFromPivotDirection(pivotDirection, manualPivotDirection, relativeTo);
                    Vector3 toolTipPosition = targetPosition + direction * pivotDistance;
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
#endif
    }
}
