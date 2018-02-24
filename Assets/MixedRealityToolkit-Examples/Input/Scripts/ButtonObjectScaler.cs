// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class ButtonObjectScaler : MonoBehaviour
    {
        [SerializeField]
        private TestButton button = null;

        [SerializeField]
        private GameObject ObjectToScale = null;

        [SerializeField]
        private float ScaleIncrement = 1.0f;

        [SerializeField]
        private ButtonAction buttonAction = ButtonAction.Reset;

        private enum ButtonAction { Reset, Grow, Shrink };

        private Vector3 InitialScale;

        private void Start()
        {
            if (ObjectToScale)
            {
                InitialScale = ObjectToScale.transform.localScale;
            }
        }

        private void OnEnable()
        {
            button.Activated += OnButtonPressed;
        }

        private void OnDisable()
        {
            button.Activated -= OnButtonPressed;
        }

        private void OnButtonPressed(TestButton source)
        {
            if (ObjectToScale)
            {
                switch (buttonAction)
                {
                    case ButtonAction.Reset:
                        Debug.Log(InitialScale);
                        ObjectToScale.transform.localScale = InitialScale;
                        break;
                    case ButtonAction.Grow:
                        ObjectToScale.transform.localScale = new Vector3((ObjectToScale.transform.localScale.x + ScaleIncrement), (ObjectToScale.transform.localScale.y + ScaleIncrement), (ObjectToScale.transform.localScale.z + ScaleIncrement));
                        break;
                    case ButtonAction.Shrink:
                        if ((ObjectToScale.transform.localScale.x - ScaleIncrement) < 0.0f)
                        {
                            ObjectToScale.transform.localScale = new Vector3(0, 0, 0);
                        }
                        else
                        {
                            ObjectToScale.transform.localScale = new Vector3((ObjectToScale.transform.localScale.x - ScaleIncrement), (ObjectToScale.transform.localScale.y - ScaleIncrement), (ObjectToScale.transform.localScale.z - ScaleIncrement));
                        }
                        break;
                }
            }

            button.Selected = false;
        }
    }
}