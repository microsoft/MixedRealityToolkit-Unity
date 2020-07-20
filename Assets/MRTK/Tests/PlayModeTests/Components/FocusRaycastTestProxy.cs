// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Used by <see cref="FocusProviderRaycastTests"/> to represent a single raycast test.
    /// This is used to set any necessary values on the <see cref="TestPointer"/> for performing the test,
    /// including positioning (through <see cref="RayLineData"/>), as well as the GameObject that the test author expects to be selected
    /// as the pointer's new focus target after the values are set on the <see cref="TestPointer"/>, and it is updated by the <see cref="Toolkit.Input.FocusProvider"/>.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Tests/FocusRaycastTestProxy")]
    public class FocusRaycastTestProxy : MonoBehaviour
    {
        /// <summary>
        /// The physics layers used for the <see cref="Toolkit.Input.FocusProvider"/> raycasts and prioritization.
        /// Corresponds to <see cref="Toolkit.Input.IMixedRealityPointer.PrioritizedLayerMasksOverride"/> and is used to set that value on the <see cref="TestPointer"/>.
        /// </summary>
        public LayerMask[] PrioritizedLayerMasks = null;

        /// <summary>
        /// Represents the ray line along which the <see cref="Toolkit.Input.FocusProvider"/> will raycast.
        /// Corresponds to <see cref="Toolkit.Input.LinePointer.LineBase"/> and is used to set that value on the <see cref="TestPointer"/>.
        /// </summary>
        public BaseMixedRealityLineDataProvider RayLineData = null;

        /// <summary>
        /// The number of RaySteps to generate from <see cref="RayLineData"/>.
        /// Corresponds to <see cref="Toolkit.Input.CurvePointer.LineCastResolution"/> and is used to set that value on the <see cref="TestPointer"/>.
        /// </summary>
        public int LineCastResolution = 10;

        /// <summary>
        /// The object that is expected to be set for the <see cref="TestPointer"/>'s <see cref="Toolkit.Input.IPointerResult.CurrentPointerTarget"/> after <see cref="Toolkit.Input.FocusProvider.Update"/>.
        /// If the target object doesn't match, it will fail the test.
        /// </summary>
        public GameObject ExpectedHitObject = null;

        private void Awake()
        {
            if (RayLineData == null)
            {
                RayLineData = GetComponent<BaseMixedRealityLineDataProvider>();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.03f);
        }
    }
}
