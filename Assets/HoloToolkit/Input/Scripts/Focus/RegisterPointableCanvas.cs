// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Script to register a Canvas component so it's capable of being focused at for 'point and commit' scenarios.
    /// </summary>
    [Obsolete("You don't need to register canvases any longer")]
    public class RegisterPointableCanvas : MonoBehaviour
    {
        //private void Start()
        //{
        //    Canvas canvas = GetComponent<Canvas>();

        //    if (canvas == null)
        //    {
        //        Debug.LogErrorFormat("Object \"{0}\" is missing its canvas component.", name);
        //    }
        //    else
        //    {
        //        FocusManager.Instance.RegisterPointableCanvas(canvas);
        //    }
        //}
    }
}
