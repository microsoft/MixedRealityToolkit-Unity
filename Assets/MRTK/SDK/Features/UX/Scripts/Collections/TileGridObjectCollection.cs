// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A basic grid layout for game objects of a consistent size
    /// </summary>
    /// 
    [ExecuteAlways]
    [AddComponentMenu("Scripts/MRTK/SDK/TileGridObjectCollection")]
    public class TileGridObjectCollection : MonoBehaviour
    {
        /// <summary>
        /// structure elements of a grid layout
        /// </summary>
        public enum GridDivisions { Rows, Columns };

        /// <summary>
        /// How many columns should the grid have
        /// </summary>
        [Tooltip("How many rows max in each column")]
        [SerializeField]
        protected int Columns = 4;

        /// <summary>
        /// Tile size in pixels
        /// </summary>
        [Tooltip("The size of the tile or grid cell")]
        [SerializeField]
        protected Vector3 TileSize = new Vector3(0.1f, 0.1f, 0);

        /// <summary>
        /// The space between tiles in pixels
        /// </summary>
        [Tooltip("Additional space between tiles")]
        [SerializeField]
        protected Vector3 Gutters = new Vector3(0.005f, 0.005f, 0.005f);

        /// <summary>
        /// The layout direction as a normalized vector
        /// </summary>
        [Tooltip("The direction the layout should be distributed - normalized vector")]
        [SerializeField]
        protected Vector3 LayoutDireciton = new Vector3(1, -1, 0);

        /// <summary>
        /// The starting position of the grid - an offset value
        /// </summary>
        [Tooltip("The starting position offset")]
        [SerializeField]
        protected Vector3 StartPosition = Vector3.zero;

        /// <summary>
        /// Will the grid be centered or start in the top corner
        /// </summary>
        [Tooltip("Should the layout center itself on the startPosition?")]
        [SerializeField]
        protected bool Centered = false;

        /// <summary>
        /// The depth or z uses the rows if true or columns if false
        /// </summary>
        [Tooltip("Depth values (z) is applied using rows or columns position")]
        [SerializeField]
        protected GridDivisions DepthCalculatedBy = GridDivisions.Rows;

        /// <summary>
        /// Should this update during run-time
        /// </summary>
        [Tooltip("Should the update run in Edit Mode only or should this be responsive during run-time?")]
        [SerializeField]
        protected bool OnlyInEditMode = false;

        protected Vector3 offSet;
        protected bool editorUpdated = false;

        /// <summary>
        /// Load the settings of the grid with code
        /// </summary>
        /// <param name="columns">the amount of columns</param>
        /// <param name="tileSize">grid tile size in pixels</param>
        /// <param name="gutters">gutter size in pixels</param>
        /// <param name="layouDirection">normalized vector flow direction</param>
        /// <param name="startPosition">start position offset</param>
        /// <param name="centered">center the grid or layout from edge</param>
        public virtual void ConfigureGrid(int columns, Vector3 tileSize, Vector3 gutters, Vector3 layouDirection, Vector3 startPosition, bool centered)
        {
            Columns = columns;
            TileSize = tileSize;
            Gutters = gutters;
            LayoutDireciton = layouDirection;
            StartPosition = startPosition;
            Centered = centered;
        }

        protected virtual void OnValidate()
        {
            editorUpdated = true;
        }

        protected virtual void Start()
        {
            editorUpdated = true;
        }

        /// <summary>
        /// Set the item position by index
        /// </summary>
        public virtual Vector3 GetListPosition(int index)
        {
            int column = index % Columns;
            int row = Columns > 0 ? Mathf.FloorToInt(index / Columns) : index;

            Vector3 size = Vector3.Scale(TileSize + Gutters, LayoutDireciton);

            float xPos = size.x * column;
            float yPos = size.y * row;
            float zPos = DepthCalculatedBy == GridDivisions.Rows ? size.z * row : size.z * column;

            return new Vector3(xPos, yPos, zPos);
        }

        protected virtual void Update()
        {
            // restrict update unless we need this to be responsive
            if ((Application.isPlaying || !OnlyInEditMode) || editorUpdated)
            {
                int childCount = transform.childCount;

                if (Centered)
                {
                    offSet = GetListPosition(Mathf.CeilToInt(childCount / Columns) * Columns - 1) * -0.5f + Vector3.Scale(TileSize, LayoutDireciton) * -0.5f;
                }
                else
                {
                    offSet = Vector3.zero;
                }

                for (int i = 0; i < childCount; i++)
                {
                    Transform item = transform.GetChild(i);
                    item.localPosition = StartPosition + offSet + (Vector3.Scale(TileSize, LayoutDireciton) * 0.5f) + GetListPosition(i);
                }

                editorUpdated = false;
            }
        }
    }
}
