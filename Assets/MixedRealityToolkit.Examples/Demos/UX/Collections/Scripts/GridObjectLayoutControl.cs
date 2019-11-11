using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Provides functions to control layout of GridObjectCollection as well
    /// as to output positions of child controls to help with building
    /// GridObjectCollectionTests.
    /// </summary>
    public class GridObjectLayoutControl : MonoBehaviour
    {
        [Tooltip("Point this at the GridObjectCollection to control.")]
        public GridObjectCollection grid;
        [Tooltip("Optional text field to output the layout of control.")]
        public TMPro.TextMeshPro text;

        /// <summary>
        /// Change the grid collection's layout to the next one in order.
        /// </summary>
        public void NextLayout()
        {
            GridObjectCollection.AnchorType cL = grid.Anchor;
            var anchorTypeCount = Enum.GetNames(typeof(GridObjectCollection.AnchorType)).Length;
            int n = (((int)cL) + 1) % anchorTypeCount;
            grid.Anchor = (GridObjectCollection.AnchorType)n;
            UpdateUI();
        }

        /// <summary>
        /// Change the grid's layout to the previous one in order
        /// </summary>
        public void PreviousLayout()
        {
            GridObjectCollection.AnchorType cL = grid.Anchor;
            var anchorTypeCount = Enum.GetNames(typeof(GridObjectCollection.AnchorType)).Length;
            int n = (((int)cL) - 1) % anchorTypeCount;
            grid.Anchor = (GridObjectCollection.AnchorType)n;
            UpdateUI();
        }

        /// <summary>
        /// Use this to run mock tests in grid object collection tests
        /// and print the resulting positions of child objects to a file.
        /// Used to get expected values for GridObjectCollectionTests.
        /// </summary>
        public void RunTest()
        {
            StartCoroutine(TestAnchors());
        }


        void Start()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            if (text != null)
            {
                text.text = "Anchor: " + grid.Anchor;
            }
            grid.UpdateCollection();
        }

        IEnumerator TestAnchors()
        {
            var go = new GameObject();
            go.name = "grid";
            var grid = go.AddComponent<GridObjectCollection>();
            grid.Distance = 0.75f;
            grid.CellWidth = 0.15f;
            grid.CellHeight = 0.15f;

            for (int i = 0; i < 3; i++)
            {
                var child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.parent = go.transform;
                child.transform.localScale = Vector3.one * 0.1f;
            }
            grid.Layout = LayoutOrder.Horizontal;


            var filename = "printgrid-" + DateTime.UtcNow.ToString("yyMMdd-HHmmss") + ".txt";
            string path = Path.Combine(Application.persistentDataPath, filename);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Vector3[] expectedPositions = new Vector3[] {");
            foreach (GridObjectCollection.AnchorType et in Enum.GetValues(typeof(GridObjectCollection.AnchorType)))
            {
                grid.Anchor = et;
                grid.UpdateCollection();
                PrintGrid(grid, et.ToString(), path, sb);
                yield return new WaitForSeconds(0.5f);
            }
            sb.AppendLine("}");
            using (var writer = new StreamWriter(path))
            {
                writer.Write(sb.ToString());
                Debug.Log("Wrote to: " + path);
            }
            yield return null;
        }

        /// <summary>
        /// Prints the coordinates of every child in the grid
        /// </summary>
        private void PrintGrid(GridObjectCollection grid, string prefix, string path, StringBuilder sb)
        {

            Debug.Log(prefix);
            var i = 0;
            foreach (Transform child in grid.gameObject.transform)
            {
                sb.AppendLine($"    new Vector3({child.localPosition.x:F2}f, {child.localPosition.y:F2}f, {child.localPosition.z:F2}f), // {prefix} index {i}");
                i++;
            }
        }
    }
}
