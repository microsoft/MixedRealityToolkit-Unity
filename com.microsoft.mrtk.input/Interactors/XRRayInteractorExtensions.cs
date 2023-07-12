// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
	/// <summary>
	/// Extensions to the XRRayInteractor and associated structs.
	/// </summary>
	public static class XRRayInteractorExtensions
	{
        /// <summary>
        /// Used to locate and lock the raycast hit data on a select.
        /// </summary>
        /// <param name="rayInteractor">The XRRayInteractor responsible for the raycast hit.</param>
        /// <param name="interactableObject"> The IXRSelectInteractable which has been selected. </param>
        /// <param name="hitDetails"> The local position and normal of the hit target, the hit target transform, and a reference point to calculate hit distance, contained in a TargetHitDetails struct. </param>
        /// <returns> Returns true if there was a raycast hit and false otherwise. </returns>
        public static bool TryLocateTargetHitPoint(this XRRayInteractor rayInteractor, IXRSelectInteractable interactableObject, out TargetHitDetails hitDetails)
		{
			hitDetails = new TargetHitDetails();
			bool hitPointAndTransformUpdated = false;
			bool hitNormalUpdated = false;

			// In the case of affordances/handles, we can stick the ray right on to the handle.
			if (interactableObject is ISnapInteractable snappable)
			{
				hitDetails.HitTargetTransform = snappable.HandleTransform;
				hitDetails.TargetLocalHitPoint = Vector3.zero;
				hitDetails.TargetLocalHitNormal = Vector3.up;
				hitPointAndTransformUpdated = true;
				hitNormalUpdated = true;
			}

			// In the case of an IScrollable being selected, ensure that the reticle locks to the
			// scroller and not to the a list item within the scroller, such as a button.
			if (interactableObject is IScrollable scrollable &&
				scrollable.IsScrolling &&
				scrollable.ScrollingInteractor == (IXRInteractor)rayInteractor)
			{
				hitDetails.HitTargetTransform = scrollable.ScrollableTransform;
				hitDetails.TargetLocalHitPoint = scrollable.ScrollingLocalAnchorPosition;
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
				return false;
			}

			// Align the reticle with a UI hit if applicable
			if (raycastResult.HasValue && isUIHitClosest)
			{
				hitDetails.HitTargetTransform = raycastResult.Value.gameObject.transform;
				hitDetails.TargetLocalHitPoint = hitDetails.HitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
				hitDetails.TargetLocalHitNormal = hitDetails.HitTargetTransform.InverseTransformDirection(raycastResult.Value.worldNormal);
				hitDetails.HitDistanceReferencePoint = raycastResult.Value.worldPosition;
			}
			// Otherwise, calculate the reticle pose based on the raycast hit.
			else if (raycastHit.HasValue)
			{
				if (!hitPointAndTransformUpdated)
				{
					hitDetails.HitTargetTransform = raycastHit.Value.collider.transform;
					hitDetails.TargetLocalHitPoint = hitDetails.HitTargetTransform.InverseTransformPoint(raycastHit.Value.point);
				}

				if (!hitNormalUpdated)
				{
					hitDetails.TargetLocalHitNormal = hitDetails.HitTargetTransform.InverseTransformDirection(raycastHit.Value.normal);
				}

				hitDetails.HitDistanceReferencePoint = hitDetails.HitTargetTransform.TransformPoint(hitDetails.TargetLocalHitPoint);
			}
			return true;
		}

		/// <summary>
		/// A data container for managing the position, normal, and transform of a target hit point. 
		/// </summary>
		public struct TargetHitDetails
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
