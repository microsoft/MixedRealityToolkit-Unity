// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    [ExecuteInEditMode]
    public class HoldWidget : InteractiveWidget
    {
        public BlendStatus[] Blends;

        protected bool isHolding;
        protected bool wasHolding;
        protected bool setBlends;

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            isHolding = state == Interactive.ButtonStateEnum.Press || state == Interactive.ButtonStateEnum.PressSelected;

        }

        protected override void Update()
        {
            base.Update();

            if (!Application.isPlaying)
            {
                Blends = AbstractBlend.BlendDataList(GetComponents<AbstractBlend>());
            }
            else
            {
                setBlends = false;

                if (wasHolding != isHolding)
                {
                    setBlends = true;
                }
                else
                {
                    setBlends = isHolding;
                }

                if (setBlends && InteractiveHost != null)
                {
                    float percentage = InteractiveHost.GetHoldPercentage();

                    for (int i = 0; i < Blends.Length; i++)
                    {
                        if (!Blends[i].Disabled)
                        {
                            Blends[i].Blender.Lerp(float.IsNaN(percentage) ? 0 : percentage);
                        }
                    }
                }

                wasHolding = isHolding;
            }


        }
    }
}
