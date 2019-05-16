// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RectTransformBroadcaster
    {
        public Vector2 offsetMax { get; set; }
        public Vector2 offsetMin { get; set; }
        public Vector2 pivot { get; set; }
        public Vector2 sizeDelta { get; set; }
        public Vector2 anchoredPosition { get; set; }
        public Vector2 anchorMax { get; set; }
        public Vector3 anchoredPosition3D { get; set; }
        public Vector2 anchorMin { get; set; }


        public bool IsEqual(RectTransform rectTrans)
        {
            return rectTrans.offsetMax == offsetMax &&
                rectTrans.offsetMin == offsetMin &&
                rectTrans.pivot == pivot &&
                rectTrans.sizeDelta == sizeDelta &&
                rectTrans.anchoredPosition == anchoredPosition &&
                rectTrans.anchorMax == anchorMax &&
                rectTrans.anchoredPosition3D == anchoredPosition3D &&
                rectTrans.anchorMin == anchorMin;
        }


        public void Copy(RectTransform rectTrans)
        {
            offsetMax = rectTrans.offsetMax;
            offsetMin = rectTrans.offsetMin;
            pivot = rectTrans.pivot;
            sizeDelta = rectTrans.sizeDelta;
            anchoredPosition = rectTrans.anchoredPosition;
            anchorMax = rectTrans.anchorMax;
            anchoredPosition3D = rectTrans.anchoredPosition3D;
            anchorMin = rectTrans.anchorMin;
        }

        public void Apply(RectTransform rectTrans)
        {
            rectTrans.offsetMax = offsetMax;
            rectTrans.offsetMin = offsetMin;
            rectTrans.pivot = pivot;
            rectTrans.sizeDelta = sizeDelta;
            rectTrans.anchoredPosition = anchoredPosition;
            rectTrans.anchorMax = anchorMax;
            rectTrans.anchoredPosition3D = anchoredPosition3D;
            rectTrans.anchorMin = anchorMin;
        }

        public bool UpdateChange(RectTransform rectTrans)
        {
            if (IsEqual(rectTrans))
                return false;
            Copy(rectTrans);
            return true;
        }


        static public void Send(RectTransform rectTrans, BinaryWriter message)
        {
            message.Write(rectTrans.offsetMax);
            message.Write(rectTrans.offsetMin);
            message.Write(rectTrans.pivot);
            message.Write(rectTrans.sizeDelta);
            message.Write(rectTrans.anchoredPosition);
            message.Write(rectTrans.anchorMax);
            message.Write(rectTrans.anchoredPosition3D);
            message.Write(rectTrans.anchorMin);
        }


        static public void Read(RectTransform rectTrans, BinaryReader message)
        {
            rectTrans.offsetMax = message.ReadVector2();
            rectTrans.offsetMin = message.ReadVector2();
            rectTrans.pivot = message.ReadVector2();
            rectTrans.sizeDelta = message.ReadVector2();
            rectTrans.anchoredPosition = message.ReadVector2();
            rectTrans.anchorMax = message.ReadVector2();
            rectTrans.anchoredPosition3D = message.ReadVector3();
            rectTrans.anchorMin = message.ReadVector2();
        }
    }
}
