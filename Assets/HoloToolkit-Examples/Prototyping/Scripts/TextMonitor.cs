// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{

    public class TextMonitor : MonoBehaviour
    {

        public TextMesh TextToMonitor;
        public TextMesh TargetTextMesh;

        // Use this for initialization
        void Start()
        {
            if (TargetTextMesh == null)
            {
                TargetTextMesh = this.gameObject.GetComponent<TextMesh>();

                if (TargetTextMesh == null)
                {
                    TargetTextMesh = this.gameObject.GetComponentInChildren<TextMesh>();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (TextToMonitor != null)
            {
                TargetTextMesh.text = TextToMonitor.text;
            }
        }
    }
}
