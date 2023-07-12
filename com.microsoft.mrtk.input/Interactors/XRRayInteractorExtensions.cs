// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{

	public static class XRRayInteractorExtensions
	{
		/// <summary>
		/// Used to locate and lock the raycast hit data on a select.
		/// </summary>
		/// <param name="rayInteractor">The XRRayInteractor responsible for the raycast hit.</param>
		/// <param name="interactableObject"> The IXRSelectInteractable which has been selected. </param>
		/// <returns>The local position and normal of the hit target, the hit target transform, and a reference point to calculate hit distance, contained in a TargetHitInfo struct.</returns>
		public static TargetHitInfo LocateTargetHitPoint(this XRRayInteractor rayInteractor, IXRSelectInteractable interactableObject)
		{
			TargetHitInfo hitInfo = new TargetHitInfo();
			bool hitPointAndTransformUpdated = false;
			bool hitNormalUpdated = false;

			// In the case of affordances/handles, we can stick the ray right on to the handle.
			if (interactableObject is ISnapInteractable snappable)
			{
				hitInfo.HitTargetTransform = snappable.HandleTransform;
				hitInfo.TargetLocalHitPoint = Vector3.zero;
				hitInfo.TargetLocalHitNormal = Vector3.up;
				hitPointAndTransformUpdated = true;
				hitNormalUpdated = true;
			}

			// In the case of an IScrollable being selected, ensure that the reticle locks to the
			// scroller and not to the a list item within the scroller, such as a button.
			if (interactableObject is IScrollable scrollable &&
				scrollable.IsScrolling &&
				scrollable.ScrollingInteractor == (IXRInteractor)rayInteractor)
			{
				hitInfo.HitTargetTransform = scrollable.ScrollableTransform;
				hitInfo.TargetLocalHitPoint = scrollable.ScrollingLocalAnchorPosition;
				hitPointAndTransformUpdated = true;
			}

			// If no hit, abort.
			if (!rayInteractor.TryGetCurrentRaycast(
				  out RaycastHit? raycastHit,
				  out _,
				  out UnityEngine.EventSystems.RaycastResult? raycastResult,
				  out _,
				  out bool isUIHitClosest))
			{
				return hitInfo;
			}

			// Align the reticle with a UI hit if applicable
			if (raycastResult.HasValue && isUIHitClosest)
			{
				hitInfo.HitTargetTransform = raycastResult.Value.gameObject.transform;
				hitInfo.TargetLocalHitPoint = hitInfo.HitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
				hitInfo.TargetLocalHitNormal = hitInfo.HitTargetTransform.InverseTransformDirection(raycastResult.Value.worldNormal);
				hitInfo.HitDistanceReferencePoint = raycastResult.Value.worldPosition;
			}
			// Otherwise, calculate the reticle pose based on the raycast hit.
			else if (raycastHit.HasValue)
			{
				if (!hitPointAndTransformUpdated)
				{
					hitInfo.HitTargetTransform = raycastHit.Value.collider.transform;
					hitInfo.TargetLocalHitPoint = hitInfo.HitTargetTransform.InverseTransformPoint(raycastHit.Value.point);
				}

				if (!hitNormalUpdated)
				{
					hitInfo.TargetLocalHitNormal = hitInfo.HitTargetTransform.InverseTransformDirection(raycastHit.Value.normal);
				}

				hitInfo.HitDistanceReferencePoint = hitInfo.HitTargetTransform.TransformPoint(hitInfo.TargetLocalHitPoint);
			}
			return hitInfo;
		}

		/// <summary>
		/// A data container for managing the position, normal, and transform of a target hit point. 
		/// </summary>
		public struct TargetHitInfo
		{
			/// <summary>
			/// The position of the target hit.
			/// </summary>
			public Vector3 TargetLocalHitPoint;

			/// <summary>
			/// The normal of the target hit. 
			/// </summary>
			public Vector3 TargetLocalHitNormal;

			/// <summary>
			/// The Transform of the selected target. 
			/// </summary>
			public Transform HitTargetTransform;

			/// <summary>
			/// The position used to calculate hit distance from a given ray position.
			/// </summary>
			public Vector3 HitDistanceReferencePoint;
		}
	}
}