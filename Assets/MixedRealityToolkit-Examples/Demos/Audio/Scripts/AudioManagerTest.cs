// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Audio;

public class AudioManagerTest : MonoBehaviour
{
    [SerializeField]
    private AudioEvent testEvent;

    private void Start()
    {
        AudioManager.PlayEvent(this.testEvent, this.gameObject);
    }
}