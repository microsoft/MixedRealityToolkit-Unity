// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Preview.SpectatorView;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Example NetworkManager implementation. It simply starts the server if the HoloLens is running the app.
/// This component represents the NetworkManager of an entire UNET multilens application.
/// </summary>
public class SpectatorViewNetworkManager : NetworkManager
{
    /// <summary>
    /// Component that manages the main flow of spectator view
    /// </summary>
    [SerializeField]
    [Tooltip("Component that manages the main flow, events and the main contact point with UNET multilens")]
    private SpectatorView spectatorView = null;

    // Use this for initialization
    private void Start()
    {
        if (spectatorView == null)
        {
            Debug.LogError("SpectatorView reference null on SpectatorViewNetworkManager");
            return;
        }

        if (spectatorView.IsHost)
        {
            StartHost();
        }
    }
}
