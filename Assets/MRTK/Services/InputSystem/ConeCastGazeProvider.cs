// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class ConeCastGazeProvider : GazeProvider
    {
        public override IMixedRealityPointer GazePointer => coneCastPointer ?? InitializeGazePointer();
        private GazeConePointer coneCastPointer = null;

        /// <inheritdoc />
        public override IMixedRealityInputSource GazeInputSource
        {
            get
            {
                if (gazeInputSource == null)
                {
                    gazeInputSource = new BaseGenericInputSource("Gaze", sourceType: InputSourceType.Head);
                    coneCastPointer.SetGazeInputSourceParent(gazeInputSource);

                }
                return gazeInputSource;
            }
        }

        private static readonly ProfilerMarker UpdateConeCastPerfMarker = new ProfilerMarker("[MRTK] ConeCastGazeProvider.Update");

        private void Update()
        {
            using (UpdateConeCastPerfMarker.Auto())
            {
                if (MixedRealityRaycaster.DebugEnabled && gazeTransform != null)
                {
                    Debug.DrawRay(GazeOrigin, (HitPosition - GazeOrigin), Color.white);
                }

                // If flagged to do so (setCursorInvisibleWhenFocusLocked) and active (IsInteractionEnabled), set the visibility to !IsFocusLocked,
                // but don't touch the visibility when not active or not flagged.
                if (setCursorInvisibleWhenFocusLocked && coneCastPointer != null &&
                    coneCastPointer.IsInteractionEnabled && GazeCursor != null && coneCastPointer.IsFocusLocked == GazeCursor.IsVisible)
                {
                    GazeCursor.SetVisibility(!coneCastPointer.IsFocusLocked);
                }

                // Handle toggling the input source's SourceType based on the current eyetracking mode 
                if (IsEyeTrackingEnabledAndValid)
                {
                    gazeInputSource.SourceType = InputSourceType.Eyes;
                }
                else
                {
                    gazeInputSource.SourceType = InputSourceType.Head;
                }
            }
        }

        private static readonly ProfilerMarker InitializeConeCastGazePointerPerfMarker = new ProfilerMarker("[MRTK] GazeProvider.InitializeGazePointer");
        internal override IMixedRealityPointer InitializeGazePointer()
        {
            using (InitializeConeCastGazePointerPerfMarker.Auto())
            {
                if (gazeTransform == null)
                {
                    gazeTransform = CameraCache.Main.transform;
                }

                Debug.Assert(gazeTransform != null, "No gaze transform to raycast from!");

                coneCastPointer = new GazeConePointer(this, "Gaze Pointer", null, raycastLayerMasks, maxGazeCollisionDistance, gazeTransform, stabilizer);

                if ((GazeCursor == null) &&
                    (GazeCursorPrefab != null))
                {
                    GameObject cursor = Instantiate(GazeCursorPrefab);
                    MixedRealityPlayspace.AddChild(cursor.transform);
                    SetGazeCursor(cursor);
                }

                coneCastPointer.IsTargetPositionLockedOnFocusLock = lockCursorWhenFocusLocked;

                return coneCastPointer;
            }
        }

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == GazePointer.PointerId)
                {
                    coneCastPointer.RaisePointerUp(eventData.MixedRealityInputAction, eventData.Handedness, eventData.InputSource);
                    return;
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == GazePointer.PointerId)
                {
                    coneCastPointer.RaisePointerDown(eventData.MixedRealityInputAction, eventData.Handedness, eventData.InputSource);
                    return;
                }
            }
        }

    }
}
