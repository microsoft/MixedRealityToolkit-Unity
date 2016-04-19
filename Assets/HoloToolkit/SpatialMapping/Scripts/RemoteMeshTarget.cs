// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using System.Net;
using System.Net.Sockets;
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// RemoteMeshTarget will listen for meshes being sent from a remote system (HoloLens).
    /// It is intended to run in the Unity Editor with exactly one
    /// Hololens device sending data.
    /// </summary>
    public class RemoteMeshTarget : SpatialMappingSource
    {
        [Tooltip("The IPv4 Address of the machine running the Unity editor.")]
        public string ServerIP;

        [Tooltip("The connection port on the machine to use.")]
        public int ConnectionPort = 11000;

#if UNITY_EDITOR
        /// <summary>
        /// Listens for network connections over TCP.
        /// </summary> 
        private TcpListener networkListener;

        /// <summary>
        /// Keeps client information when a connection happens.
        /// </summary>
        private TcpClient networkClient;
 
        /// <summary>
        /// Tracks if a client is connected.
        /// </summary>
        private bool ClientConnected = false;

        // Use this for initialization.
        void Start()
        {
            // Setup the network listener.
            IPAddress localAddr = IPAddress.Parse(ServerIP.Trim());
            networkListener = new TcpListener(localAddr, ConnectionPort);
            networkListener.Start();

            // Request the network listener to wait for connections asynchronously.
            AsyncCallback callback = new AsyncCallback(OnClientConnect);
            networkListener.BeginAcceptTcpClient(callback, this);
        }

        // Update is called once per frame.
        void Update()
        {
            // If we have a connected client, presumably the client wants to send some meshes.
            if (ClientConnected)
            {
                // Get the clients stream.
                NetworkStream stream = networkClient.GetStream();

                // Make sure there is data in the stream.
                if (stream.DataAvailable)
                {
                    // The first 4 bytes will be the size of the data containing the mesh(es).
                    int datasize = ReadInt(stream);

                    // Allocate a buffer to hold the data.  
                    byte[] dataBuffer = new byte[datasize];

                    // Read the data.
                    // The data can come in chunks. 
                    int readsize = 0;

                    while (readsize != datasize)
                    {
                        readsize += stream.Read(dataBuffer, readsize, datasize - readsize);
                    }

                    if (readsize != datasize)
                    {
                        Debug.Log("reading mesh failed: " + readsize + " != " + datasize);
                    }

                    // Pass the data to the mesh serializer. 
                    List<Mesh> meshes = new List<Mesh>(SimpleMeshSerializer.Deserialize(dataBuffer));

                    // For each mesh, create a gameobject to render it.
                    for (int index = 0; index < meshes.Count; index++)
                    {
                        GameObject surface = AddSurfaceObject(meshes[index], string.Format("Beamed-{0}", surfaceObjects.Count), transform);
                        surface.transform.parent = SpatialMappingManager.Instance.transform;

                        if (SpatialMappingManager.Instance.DrawVisualMeshes == false)
                        {
                            surface.GetComponent<MeshRenderer>().enabled = false;
                        }

                        if(SpatialMappingManager.Instance.CastShadows == false)
                        {
                            surface.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }

                    // Finally disconnect.
                    ClientConnected = false;
                    networkClient.Close();

                    // And wait for the next connection.
                    AsyncCallback callback = new AsyncCallback(OnClientConnect);
                    networkListener.BeginAcceptTcpClient(callback, this);
                }
            }
        }

        /// <summary>
        /// Reads an int from the next 4 bytes of the supplied stream.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <returns>An integer representing the bytes.</returns>
        int ReadInt(Stream stream)
        {
            // The bytes arrive in the wrong order, so swap them.
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            byte t = bytes[0];
            bytes[0] = bytes[3];
            bytes[3] = t;

            t = bytes[1];
            bytes[1] = bytes[2];
            bytes[2] = t;

            // Then bitconverter can read the int32.
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Called when a client connects.
        /// </summary>
        /// <param name="result">The result of the connection.</param>
        void OnClientConnect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                networkClient = networkListener.EndAcceptTcpClient(result);
                if (networkClient != null)
                {
                    Debug.Log("Connected");
                    ClientConnected = true;
                }
            }
        }
#endif
    }
}