// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using System;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Profiles
{
    public class ButtonTextProfile : ButtonProfile
    {
        [Header("Text Mesh Settings")]
        public TextAlignment Alignment = TextAlignment.Center;
        public TextAnchor Anchor = TextAnchor.MiddleCenter;
        public FontStyle Style = FontStyle.Normal;
        public int Size = 72;
        public Color Color = Color.white;
        public Font Font;

        // Used to reposition the text mesh object in addition to setting its anchor
        // This is useful when button text position will change dramatically based on the presence of other elements
        // e.g., bottom anchor will move the text out of the way of an icon
        [Header("Anchor Settings")]
        [HideInMRTKInspector]
        public Vector3 AnchorLowerCenterOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorLowerLeftOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorLowerRightOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorMiddleCenterOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorMiddleLeftOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorMiddleRightOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorUpperCenterOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorUpperLeftOffset = Vector3.zero;
        [HideInMRTKInspector]
        public Vector3 AnchorUpperRightOffset = Vector3.zero;

        /// <summary>
        /// Convenience function for getting the offset from an anchor setting
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public Vector3 GetOffset (TextAnchor anchor)
        {
            Vector3 offset;

            switch (anchor)
            {
                case TextAnchor.LowerCenter:
                    offset = AnchorLowerCenterOffset;
                    break;

                case TextAnchor.LowerLeft:
                    offset = AnchorLowerLeftOffset;
                    break;

                case TextAnchor.LowerRight:
                    offset = AnchorLowerRightOffset;
                    break;

                case TextAnchor.MiddleCenter:
                    offset = AnchorMiddleCenterOffset;
                    break;

                case TextAnchor.MiddleLeft:
                    offset = AnchorMiddleLeftOffset;
                    break;

                case TextAnchor.MiddleRight:
                    offset = AnchorMiddleRightOffset;
                    break;

                case TextAnchor.UpperCenter:
                    offset = AnchorUpperCenterOffset;
                    break;

                case TextAnchor.UpperLeft:
                    offset = AnchorUpperLeftOffset;
                    break;

                case TextAnchor.UpperRight:
                    offset = AnchorUpperRightOffset;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("anchor", anchor, null);
            }

            return offset;
        }
    }
}