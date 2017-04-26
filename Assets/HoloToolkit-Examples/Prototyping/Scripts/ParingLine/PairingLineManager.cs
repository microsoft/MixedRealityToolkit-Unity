// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Linq;
using System;

    public class PairingLineManager
    {
        public static PairingLineManager Instance { get; private set; }

        static PairingLineManager()
        {
            if (Instance == null)
            {
                Instance = new PairingLineManager();
            }
        }

        public GameObject PairingLinePrefab;
        
        public PairingLine SpawnToObjectAndPosition(Vector3 start, GameObject end, Color color)
        {
            
            GameObject linesObj = GameObject.Instantiate(PairingLinePrefab);
            var line = linesObj.GetComponent<PairingLine>();
            line.StartPosition = start;
            line.EndObject = end;
            line.Color = color;

            return line;
        }
    }
