// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// controls the layout of content tiles
    /// </summary>
    public class GridLayout : MonoBehaviour
    {
        public Transform ParentTransform;
        public GameObject TilePrefab;
        public GridLayoutController LayoutController;
        public Vector3 OffsetVector;
        public int MaxColumns = 1;
        public int MaxRows;
        public float XGutter;
        public float YGutter;

        private Vector3 mPrefabSize;
        private float mXMagnitude;
        private Vector3 mStartPosition;
        private int mBuildGridCount = 0;

        private void Awake()
        {
            if (ParentTransform == null)
            {
                ParentTransform = this.gameObject.transform;
            }

            if (TilePrefab != null)
            {
                Renderer renderer = TilePrefab.GetComponent<Renderer>();
                if (renderer == null)
                {
                    renderer = TilePrefab.GetComponentInChildren<Renderer>();
                }

                mPrefabSize = renderer.bounds.size;
            }

            mXMagnitude = mPrefabSize.x * (MaxColumns - 1) + XGutter * (MaxColumns - 1);
            mStartPosition = OffsetVector + Vector3.left * (mXMagnitude / 2) + Vector3.down * (mPrefabSize.y / 2);

            if (mBuildGridCount > 0)
            {
                BuildGrid(mBuildGridCount);
            }
        }

        public void BuildGrid(int count)
        {
            mBuildGridCount = count;
            // clean up any existing tiles
            ClearExistingTiles();

            int row = 0, column = 0;
            // clear current grid

            for (int i = 0; i < count; ++i)
            {
                // create new prefab
                GameObject tile = GameObject.Instantiate(TilePrefab);
                LayoutController.SetupTile(tile, i, column, row);
                tile.transform.parent = ParentTransform;
                tile.transform.localPosition = GetTilePosition(column, row);
                tile.transform.localRotation = Quaternion.identity;

                ++column;
                if (column >= MaxColumns)
                {
                    column = 0;
                    ++row;
                    if (row >= MaxRows && MaxRows > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void ResetInteractiveToggles(int gridId)
        {
            int count = ParentTransform.childCount;
            for (int i = count - 1; i > -1; --i)
            {
                Transform item = ParentTransform.GetChild(i);
                GridLayoutRenderer renderer = item.gameObject.GetComponent<GridLayoutRenderer>();
                InteractiveToggle toggle = item.gameObject.GetComponentInChildren<InteractiveToggle>();
                toggle.HasSelection = renderer.GridId == gridId;
            }
        }

        private void ClearExistingTiles()
        {
            int count = ParentTransform.childCount;
            for (int i = count - 1; i > -1; --i)
            {
                Transform item = ParentTransform.GetChild(i);
                GameObject.Destroy(item.gameObject);
            }
        }

        private Vector3 GetTilePosition(int column, int row)
        {
            float xMagnitude = mPrefabSize.x * column + XGutter * column;
            float yMagnitude = mPrefabSize.y * row + YGutter * row;
            Vector3 newVector = mStartPosition + Vector3.right * xMagnitude + Vector3.down * yMagnitude;

            return newVector;
        }
    }
}
