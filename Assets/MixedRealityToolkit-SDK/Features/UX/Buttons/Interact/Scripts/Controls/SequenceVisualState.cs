// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Controls
{
    /// <summary>
    /// sets the index of CycleArray components on the prefab referenced by the SequenceVisualManager.
    /// </summary>
    [ExecuteInEditMode]
    public class SequenceVisualState : MonoBehaviour
    {
        public BlendStatus[] Blends;
        
        /// <summary>
        /// set the visual state (on/off)
        /// </summary>
        /// <param name="active"></param>
        public void SetState(bool active)
        {
            for (int i = 0; i < Blends.Length; i++)
            {
                if (!Blends[i].Disabled)
                {
                    Blends[i].Blender.Lerp(active ? 1 : 0);
                }
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                Blends = AbstractBlend.BlendDataList(GetComponents<AbstractBlend>());
            }
        }
    }
}
