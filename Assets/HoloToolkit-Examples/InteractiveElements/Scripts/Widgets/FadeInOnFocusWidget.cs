// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class FadeInOnFocusWidget : InteractiveWidget
    {

        public FadeMaterialColor FadeController;

        private void Awake()
        {
            if (FadeController == null)
            {
                FadeController = GetComponent<FadeMaterialColor>();
            }

            if (FadeController == null)
            {
                Debug.LogError("FadeMaterialColor:FadeController is not set in FadeInOnFocusWidget!");
                Destroy(this);
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            if (state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Focus || state == Interactive.ButtonStateEnum.Press || state == Interactive.ButtonStateEnum.PressSelected)
            {
                if (FadeController != null)
                {
                    FadeController.FadeIn();
                }
            }
            else
            {
                if (FadeController != null)
                {
                    FadeController.FadeOut();
                }
            }
        }
    }
}
