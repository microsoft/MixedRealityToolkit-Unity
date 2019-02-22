// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Static class providing useful functions for
    /// finding ToolTip Attach point information.
    /// </summary>
    public static class ToolTipUtility
    {
        private const int NumPivotLocations = 8;

        /// <summary>
        /// Avoid running this query in Update function because calculating Vector3.Distance requires sqr root calculation (expensive)
        /// Instead, find strategic moments to update nearest pivot (i.e. only once when ToolTip becomes active)
        /// </summary>
        /// <param name="anchor">Transform of object serving as anchor for tooltip</param>
        /// <param name="contentParent">Transform for the tooltip content</param>
        /// <param name="localPivotPositions">list of positions to find the closest</param>
        /// <param name="pivotType">pivot type needed for calculation of closest</param>
        /// <returns>Vector3 the point in localPivotPositions which is closest to the anchor position</returns>
        public static Vector3 FindClosestAttachPointToAnchor(Transform anchor, Transform contentParent, Vector3[] localPivotPositions, ToolTipAttachPoint pivotType)
        {
            Vector3 currentPivot;
            Vector3 nearPivot = Vector3.zero;
            Vector3 anchorPosition = anchor.position;
            float nearDist = Mathf.Infinity;

            if (localPivotPositions == null || localPivotPositions.Length < NumPivotLocations)
            {
                return nearPivot;
            }

            switch (pivotType)
            {
                case ToolTipAttachPoint.Center:
                    return nearPivot;

                // Search all pivots
                case ToolTipAttachPoint.Closest:
                    for (int i = 0; i < localPivotPositions.Length; i++)
                    {
                        currentPivot = localPivotPositions[i];
                        float sqrDist = (anchorPosition - contentParent.TransformPoint(currentPivot)).sqrMagnitude;
                        if (sqrDist < nearDist)
                        {
                            nearDist = sqrDist;
                            nearPivot = currentPivot;
                        }
                    }
                    break;

                // Search corner pivots
                case ToolTipAttachPoint.ClosestCorner:
                    for (int i = (int)ToolTipAttachPoint.BottomRightCorner; i < (int)ToolTipAttachPoint.TopLeftCorner; i++)
                    {
                        currentPivot = localPivotPositions[i];
                        float sqrDist = (anchorPosition - contentParent.TransformPoint(currentPivot)).sqrMagnitude;
                        if (sqrDist < nearDist)
                        {
                            nearDist = sqrDist;
                            nearPivot = currentPivot;
                        }
                    }
                    break;

                // Search middle pivots
                case ToolTipAttachPoint.ClosestMiddle:
                    for (int i = (int)ToolTipAttachPoint.BottomMiddle; i < (int)ToolTipAttachPoint.LeftMiddle; i++)
                    {
                        currentPivot = localPivotPositions[i];
                        float sqrDist = (anchorPosition - contentParent.TransformPoint(currentPivot)).sqrMagnitude;
                        if (sqrDist < nearDist)
                        {
                            nearDist = sqrDist;
                            nearPivot = currentPivot;
                        }
                    }
                    break;

                default:
                    // For all other types, just use the array position or contentParent
                    // position if there is no array provided.
                    nearPivot = localPivotPositions.Length == 0
                        ? contentParent.position
                        : localPivotPositions[(int)pivotType];
                    break;
            }

            return nearPivot;
        }

        /// <summary>
        /// gets an array of pivot positions
        /// </summary>
        /// <param name="pivotPositions">ref array that gets filled with positions</param>
        /// <param name="localContentSize">the xy scale of the tooltip</param>
        public static void GetAttachPointPositions(ref Vector3[] pivotPositions, Vector2 localContentSize)
        {
            if (pivotPositions == null || pivotPositions.Length < NumPivotLocations)
            {
                pivotPositions = new Vector3[NumPivotLocations];
            }

            //Get the extents of our content size
            localContentSize *= 0.5f;

            pivotPositions[(int)ToolTipAttachPoint.BottomMiddle] = new Vector3(0f, -localContentSize.y, 0f);
            pivotPositions[(int)ToolTipAttachPoint.TopMiddle] = new Vector3(0f, localContentSize.y, 0f);
            pivotPositions[(int)ToolTipAttachPoint.LeftMiddle] = new Vector3(-localContentSize.x, 0f, 0f); // was right
            pivotPositions[(int)ToolTipAttachPoint.RightMiddle] = new Vector3(localContentSize.x, 0f, 0f); // was left

            pivotPositions[(int)ToolTipAttachPoint.BottomLeftCorner] = new Vector3(-localContentSize.x, -localContentSize.y, 0f); // was right
            pivotPositions[(int)ToolTipAttachPoint.BottomRightCorner] = new Vector3(localContentSize.x, -localContentSize.y, 0f); // was left
            pivotPositions[(int)ToolTipAttachPoint.TopLeftCorner] = new Vector3(-localContentSize.x, localContentSize.y, 0f); // was right
            pivotPositions[(int)ToolTipAttachPoint.TopRightCorner] = new Vector3(localContentSize.x, localContentSize.y, 0f); // was left
        }
    }
}