﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.InputSystem;
//using HoloToolkit.Unity.InputModule;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
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
		private ConnnectorPivotMode pivotMode = ConnnectorPivotMode.Manual;

		[SerializeField]
		private ConnectorPivotDirection pivotDirection = ConnectorPivotDirection.North;

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
		/// Does this object currently have focus by any <see cref="IMixedRealityPointer"/>?
		/// </summary>
		bool HasFocus { get; }

		/// <summary>
		/// Is Focus capabilities enabled for this <see cref="UnityEngine.Component"/>?
		/// </summary>
		bool FocusEnabled { get; set; }

		/// <summary>
		/// The list of <see cref="IMixedRealityPointer"/>s that are currently focused on this <see cref="UnityEngine.GameObject"/>
		/// </summary>
		List<IMixedRealityPointer> Focusers { get; }

		bool IMixedRealityFocusHandler.HasFocus
		{
			get
			{
				return hasFocus;
			}
		}

		private bool focusEnabled = true;
		bool IMixedRealityFocusHandler.FocusEnabled
		{
			get { return focusEnabled; }
			set { focusEnabled = value; }
		}

		List<IMixedRealityPointer> IMixedRealityFocusHandler.Focusers => new List<IMixedRealityPointer>(0);

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
		{
			//throw new System.NotImplementedException();
		}

		public void OnFocusChanged(FocusEventData eventData)
		{
			//throw new System.NotImplementedException();
		}

		public void OnInputPressed(InputEventData<float> eventData)
		{
			Debug.Log("OnInputPressed Input Data: " + eventData.InputData, this);
			if (eventData.InputData > .95f)
			{
				Debug.Log("OnInputPressed HIT  =================================== ", this);
				//tappedTime = Time.unscaledTime;
			}
			//if (toolTip == null || !toolTip.gameObject.activeSelf)
			//{
			//	switch (vanishType)
			//	{
			//		case VanishType.VanishOnTap:
			//			toolTip.gameObject.SetActive(false);
			//			break;

			//		default:
			//			break;
			//	}
			//	switch (appearType)
			//	{
			//		case AppearType.AppearOnTap:
			//			ShowToolTip();
			//			break;

			//		default:
			//			break;
			//	}
			//}
		}

		public void OnPositionInputChanged(InputEventData<Vector2> eventData)
		{
			//We do nothing with this.
			//Could drive the position of the tooltip? But that's ridiculous.
		}

		public void OnInputDown(InputEventData eventData)
		{
			Debug.Log("InInputDown: " + eventData.currentInputModule.ToString() + "   selName: " + eventData.selectedObject.name + "   Handedness: " + eventData.Handedness + "  InputSrc: " + eventData.InputSource.SourceName + "\n  MRInputAction.Desc: " + eventData.MixedRealityInputAction.Description + "    MRInputAction.ID " + eventData.MixedRealityInputAction.Id + "   used: " + eventData.used, this);
			tappedTime = Time.unscaledTime;
			if (toolTip == null || !toolTip.gameObject.activeSelf)
			{
				if (appearType == AppearType.AppearOnTap)
				{
					ShowToolTip();
				}
			}
		}

		/// <summary>
		/// this Handler intentionally empty
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

			if (pivotMode == ConnnectorPivotMode.Manual)
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
							Debug.Log("Vanish from Tap ==" + tappedTime.ToString() + "  " + tappedTimeOnStart, this);

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
				if (pivotMode == ConnnectorPivotMode.Automatic)
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
