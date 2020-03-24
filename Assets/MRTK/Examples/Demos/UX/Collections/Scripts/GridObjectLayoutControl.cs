// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Provides functions to control layout of GridObjectCollection as well
    /// as to output positions of child controls to help with building
    /// GridObjectCollectionTests.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/GridObjectLayoutControl")]
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
            LayoutAnchor currentLayout = grid.Anchor;
            var anchorTypeCount = Enum.GetNames(typeof(LayoutAnchor)).Length;
            int nextLayout = (((int)currentLayout) + 1) % anchorTypeCount;
            grid.Anchor = (LayoutAnchor)nextLayout;
            UpdateUI();
        }

        /// <summary>
        /// Change the grid's layout to the previous one in order.
        /// </summary>
        public void PreviousLayout()
        {
            LayoutAnchor currentLayout = grid.Anchor;
            var anchorTypeCount = Enum.GetNames(typeof(LayoutAnchor)).Length;
            int nextLayout = (((int)currentLayout) - 1) % anchorTypeCount;
            if (nextLayout < 0)
            {
                nextLayout += anchorTypeCount;
            }
            grid.Anchor = (LayoutAnchor)nextLayout;
            UpdateUI();
        }

        /// <summary>
        /// Use this to run mock tests in grid object collection tests
        /// and print the resulting positions of child objects to a file.
        /// Used to get expected values for GridObjectCollectionTests.
        /// When you run the comand, look in the Debug log for where the file is output,
        /// it will be of form "printgrid-yyMMdd-HHmmss.txt"
        /// </summary>
        public void RunTest()
        {
            StartCoroutine(TestAnchors());
        }


        private void Start()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (text != null)
            {
                text.text = "Anchor: " + grid.Anchor;
            }
            grid.UpdateCollection();
        }

        private IEnumerator TestAnchors()
        {
            var gridRoot = new GameObject();
            gridRoot.name = "grid";
            var gridCollection = gridRoot.AddComponent<GridObjectCollection>();
            gridCollection.Distance = 0.75f;
            gridCollection.CellWidth = 0.15f;
            gridCollection.CellHeight = 0.15f;

            for (int i = 0; i < 3; i++)
            {
                var child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.parent = gridRoot.transform;
                child.transform.localScale = Vector3.one * 0.1f;
            }
            gridCollection.Layout = LayoutOrder.Horizontal;


            var filename = "printgrid-" + DateTime.UtcNow.ToString("yyMMdd-HHmmss") + ".txt";
            string path = Path.Combine(Application.persistentDataPath, filename);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Vector3[] expectedPositions = new Vector3[] {");
            foreach (LayoutAnchor value in Enum.GetValues(typeof(LayoutAnchor)))
            {
                gridCollection.Anchor = value;
                gridCollection.UpdateCollection();
                PrintGrid(gridCollection, value.ToString(), path, stringBuilder);
                // Wait for 5 seconds so dev / user can see the layout 
                yield return new WaitForSeconds(0.5f);
            }
            stringBuilder.AppendLine("}");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(stringBuilder.ToString());
                    Debug.Log("Wrote to: " + path);
                }
            }
            yield return null;
        }

        /// <summary>
        /// Prints the coordinates of every child in the grid
        /// </summary>
        private void PrintGrid(GridObjectCollection grid, string prefix, string path, StringBuilder builder)
        {
            Debug.Log(prefix);
            var i = 0;
            foreach (Transform child in grid.gameObject.transform)
            {
                builder.AppendLine($"    new Vector3({child.localPosition.x:F2}f, {child.localPosition.y:F2}f, {child.localPosition.z:F2}f), // {prefix} index {i}");
                i++;
            }
        }
    }
}
