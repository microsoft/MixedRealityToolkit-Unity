// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

public class DeleteLine : MonoBehaviour
{
#if UNITY_WSA
    /// <summary>
    /// when tip text is tapped, destroy this tip and relative objects.
    /// </summary>
	public void OnSelect()
    {
        var parent = gameObject.transform.parent.gameObject;
        if (parent != null)
        {
            Destroy(parent);
        }
    }
#endif
}
