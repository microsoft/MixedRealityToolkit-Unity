// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

public class CheckApiTest : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void CheckV5Api()
    {
        text.text = "UniversalApiContract\nVersion 5 available? " + HoloToolkit.WindowsApiChecker.UniversalApiContractV5_IsAvailable;
        Debug.Log(text.text);
    }

    public void CheckV4Api()
    {
        text.text = "UniversalApiContract\nVersion 4 available? " + HoloToolkit.WindowsApiChecker.UniversalApiContractV4_IsAvailable;
        Debug.Log(text.text);
    }

    public void CheckV3Api()
    {
        text.text = "UniversalApiContract\nVersion 3 available? " + HoloToolkit.WindowsApiChecker.UniversalApiContractV3_IsAvailable;
        Debug.Log(text.text);
    }
}
