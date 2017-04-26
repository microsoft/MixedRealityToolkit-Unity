// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.Prototyping
{
    public class ObjectGrid : MonoBehaviour
    {
        //Creating a vector of ints for a better editor interface.
        [System.Serializable]
        public struct IntVec3
        {
            public int x;
            public int y;
            public int z;

            public IntVec3(int X, int Y, int Z)
            {
                this.x = X;
                this.y = Y;
                this.z = Z;
            }
        }

        public GameObject GameObjectPrefab;
        public IntVec3 Dimensions = new IntVec3(1, 1, 1);
        public Vector3 Offset = new Vector3(0f, 0f, 0f);

        private GameObject[,,] mArray;

        // Use this for initialization
        void Start()
        {
            mArray = new GameObject[Dimensions.x, Dimensions.y, Dimensions.z];
            for (int y = 0; y < Dimensions.y; ++y)
            {
                for (int z = 0; z < Dimensions.z; ++z)
                {
                    for (int x = 0; x < Dimensions.x; ++x)
                    {
                        GameObject go = SpawnObjectAt(new Vector3(x * GameObjectPrefab.transform.localScale.x + Offset.x * x,
                            y * GameObjectPrefab.transform.localScale.y + Offset.y * y,
                            z * GameObjectPrefab.transform.localScale.z + Offset.z * z));

                        mArray[x, y, z] = go;
                    }
                }
            }
        }

        GameObject SpawnObjectAt(Vector3 pos)
        {
            GameObject go = GameObject.Instantiate(GameObjectPrefab);
            go.transform.SetParent(this.gameObject.transform);

            //set the object's position relative to the parent's position
            go.transform.localPosition = pos;

            return go;
        }

        public GameObject GetObjectAt(int x, int y, int z)
        {
            if ((x >= 0 && x < Dimensions.x) && (y >= 0 && y < Dimensions.y) && (z >= 0 && z < Dimensions.z))
            {
                return mArray[x, y, z];
            }
            else
            {
                return null;
            }
        }

    }

}
