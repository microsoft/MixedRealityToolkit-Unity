// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    [ExecuteInEditMode]
    public class SizeToRectTransform : MonoBehaviour
    {
        public RectTransform ParentRectTransform;
        public float ScaleFactor = 2048;
        public float Depth = 10;
        public Vector2 EdgeOffset;

        private RectTransform mRectTransform;

        private void Awake()
        {
            if (ParentRectTransform == null)
            {
                ParentRectTransform = transform.parent.GetComponent<RectTransform>();
            }

            if (ParentRectTransform == null)
            {
                Debug.LogError("The parent of " + name + "does not have a RectTransform!");
            }

            mRectTransform = GetComponent<RectTransform>();

            if (mRectTransform == null)
            {
                //Debug.LogError("This GameObject: " + name + ", does not have a RectTransform!");
            }
        }

        private void UpdateScale()
        {
            if (ParentRectTransform != null)
            {
                if (mRectTransform == null)
                {
                    transform.localScale = new Vector3((ParentRectTransform.rect.width - EdgeOffset.x) / ScaleFactor, (ParentRectTransform.rect.height - EdgeOffset.y) / ScaleFactor, Depth / ScaleFactor);
                }
                else
                {
                    mRectTransform.sizeDelta = new Vector2(ParentRectTransform.rect.width - EdgeOffset.x, ParentRectTransform.rect.height - EdgeOffset.y);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateScale();
        }
    }
}
