// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

public class CheckApiTest : MonoBehaviour
{
    public void CheckV4Api()
    {
        Debug.Log("Version 4 available? " + HoloToolkit.WindowsApiChecker.UniversalApiContractV4_IsAvailable);
    }

    public void CheckV5Api()
    {
        Debug.Log("Version 5 available? " + HoloToolkit.WindowsApiChecker.UniversalApiContractV5_IsAvailable);
    }
}
