// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System.Net;
using System.Net.Sockets;
#endif

#if UNITY_WSA && !UNITY_EDITOR
using System.Collections.Concurrent;
#endif

namespace HoloToolkit.Unity.SpatialMapping
{
    /// <summary>
    /// RemoteMeshTarget will listen for meshes being sent from a remote system (HoloLens).
    /// It is intended to run in the Unity Editor with exactly one
    /// HoloLens device sending data.
    /// </summary>
    public class RemoteMeshTarget : SpatialMappingSource
    {
        [Tooltip("The IPv4 Address of the machine running the Unity editor.")]
        public string ServerIP;

        [Tooltip("The connection port on the machine to use.")]
        public int ConnectionPort = 11000;

        /// <summary>
        /// Reads an int from the next 4 bytes of the supplied stream.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <returns>An integer representing the bytes.</returns>
        private int ReadInt(Stream stream)
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

// Remote mapping for Immmersive Windows MR and UWP Apps
#if UNITY_WSA && !UNITY_EDITOR

        /// <summary>
        /// Listens for network connections over TCP in an UWP app
        /// </summary> 
        private Windows.Networking.Sockets.StreamSocketListener streamSocketListener;

        /// <summary>
        /// Buffers the received meshes to process and display
        /// </summary>
        private ConcurrentQueue<byte[]> ReceivedMeshes;

        // Use this for initialization.
        void Start()
        {
            // Initiate the queue for storing the mesh data until the game objects are created
            ReceivedMeshes = new ConcurrentQueue<byte[]>();

            // Start the server for receiving mesh data
            StartServer();
        }

        // Update is called once per frame.
        void Update()
        {
            if (ReceivedMeshes.TryDequeue(out byte[] result))
            {
                // Pass the data to the mesh serializer. 
                List<Mesh> meshes = new List<Mesh>(SimpleMeshSerializer.Deserialize(result));

                if (meshes.Count > 0 && SpatialMappingManager.Instance.Source != this)
                {
                    // Use the network-based mapping source to receive meshes in the Unity editor.
                    SpatialMappingManager.Instance.SetSpatialMappingSource(this);
                    System.Diagnostics.Debug.WriteLine("Set the network-based mapping source.");
                }

                // For each mesh, create a GameObject to render it.
                for (int index = 0; index < meshes.Count; index++)
                {
                    int meshID = SurfaceObjects.Count;

                    SurfaceObject surface = CreateSurfaceObject(
                        mesh: meshes[index],
                        objectName: "Beamed-" + meshID,
                        parentObject: transform,
                        meshID: meshID
                        );

                    surface.Object.transform.parent = SpatialMappingManager.Instance.transform;

                    AddSurfaceObject(surface);
                    System.Diagnostics.Debug.WriteLine("Added object " + surface.Object.name);
                }
            }
        }


        /// <summary>
        /// Used to start the stream socket listener and attach the event handler for incoming connections
        /// </summary>
        private async void StartServer()
        {
            try
            {
                streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += this.StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListener.BindServiceNameAsync(ConnectionPort.ToString());

                System.Diagnostics.Debug.WriteLine("Listener started on port " + ConnectionPort);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't open socket listener.");
            }
        }

        /// <summary>
        /// Used to dispose of the stream socket listener
        /// </summary>
        private async void StopServer()
        {
            if (streamSocketListener != null)
            {
                streamSocketListener.Dispose();
                System.Diagnostics.Debug.WriteLine("Listener stopped.");
            }
        }

        /// <summary>
        /// Event handler for handling incoming connections and save the mesh data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            using (var stream = args.Socket.InputStream.AsStreamForRead())
            {
                // TODO Make sure there is data in the stream.

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

                System.Diagnostics.Debug.WriteLine("Received:" + datasize + "bytes");

                ReceivedMeshes.Enqueue(dataBuffer);
            }
        }
#endif

// Remote mapping inside the Unity Editor
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        /// <summary>
        /// Listens for network connections over TCP.
        /// </summary> 
        private TcpListener networkListener;

        /// <summary>
        /// Keeps client information when a connection happens.
        /// </summary>
        private TcpClient networkClient;

        private bool clientConnected = false;

        // Use this for initialization.
        private void Start()
        {
            // Setup the network listener.
            IPAddress localAddr = IPAddress.Parse(ServerIP.Trim());
            networkListener = new TcpListener(localAddr, ConnectionPort);
            networkListener.Start();

            // Request the network listener to wait for connections asynchronously.
            AsyncCallback callback = OnClientConnect;
            networkListener.BeginAcceptTcpClient(callback, this);
        }

        // Update is called once per frame.
        private void Update()
        {
            // If we have a connected client, presumably the client wants to send some meshes.
            if (clientConnected)
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

                    if (meshes.Count > 0)
                    {
                        // Use the network-based mapping source to receive meshes in the Unity editor.
                        SpatialMappingManager.Instance.SetSpatialMappingSource(this);
                    }

                    // For each mesh, create a GameObject to render it.
                    for (int index = 0; index < meshes.Count; index++)
                    {
                        int meshID = SurfaceObjects.Count;

                        SurfaceObject surface = CreateSurfaceObject(
                            mesh: meshes[index],
                            objectName: "Beamed-" + meshID,
                            parentObject: transform,
                            meshID: meshID
                            );

                        surface.Object.transform.parent = SpatialMappingManager.Instance.transform;

                        AddSurfaceObject(surface);
                    }

                    // Finally disconnect.
                    clientConnected = false;
                    networkClient.Close();

                    // And wait for the next connection.
                    AsyncCallback callback = OnClientConnect;
                    networkListener.BeginAcceptTcpClient(callback, this);
                }
            }
        }

        /// <summary>
        /// Called when a client connects.
        /// </summary>
        /// <param name="result">The result of the connection.</param>
        private void OnClientConnect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                networkClient = networkListener.EndAcceptTcpClient(result);
                if (networkClient != null)
                {
                    Debug.Log("Connected");
                    clientConnected = true;
                }
            }
        }
#endif

    }
}