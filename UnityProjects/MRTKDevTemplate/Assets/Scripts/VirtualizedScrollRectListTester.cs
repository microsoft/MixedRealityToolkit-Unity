// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;
using Microsoft.MixedReality.Toolkit.UX.Experimental;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("MRTK/Examples/Virtualized Scroll Rect List Tester")]
    public class VirtualizedScrollRectListTester : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Auto-scrolls the list up and down in a sin wave.")]
        private bool sinScroll = true;

        private VirtualizedScrollRectList list;
        private float destScroll;
        private bool animate;

        private string[] words = { "one", "two", "three", "zebra", "keyboard", "rabbit", "graphite", "ruby", };
        // Start is called before the first frame update
        private void Start()
        {
            list = GetComponent<VirtualizedScrollRectList>();
            list.OnVisible = (go, i) =>
            {
                foreach (var text in go.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    if (text.gameObject.name == "Text")
	                    text.text = $"{i} {words[i%words.Length]}";
                }
            };
        }

        // Update is called once per frame
        private void Update()
        {
            if (sinScroll)
            {
                list.Scroll = (Mathf.Sin(Time.time * 0.5f - (Mathf.PI / 2)) * 0.5f + 0.5f) * list.MaxScroll;
                destScroll  = list.Scroll;
                animate     = false;
            }

            if (animate)
            {
                float newScroll = Mathf.Lerp(list.Scroll, destScroll, 8 * Time.deltaTime);
                list.Scroll = newScroll;
                if (Mathf.Abs(list.Scroll - destScroll) < 0.02f)
                {
                    list.Scroll = destScroll;
                    animate     = false;
                }
            }
        }

        /// <summary>Scrolls the VirtualizedScrollRect to the next page.</summary>
        public void Next()
        {
            sinScroll  = false;
            animate    = true;
            destScroll = Mathf.Min(list.MaxScroll, Mathf.Floor(list.Scroll / list.RowsOrColumns) * list.RowsOrColumns + list.TotallyVisibleCount);
        }
        /// <summary>Scrolls the VirtualizedScrollRect to the previous page.</summary>
        public void Prev()
        {
            sinScroll  = false;
            animate    = true;
            destScroll = Mathf.Max(0, Mathf.Floor(list.Scroll / list.RowsOrColumns) * list.RowsOrColumns - list.TotallyVisibleCount);
        }

        /// <summary>Testing function for adjusting the number of items during
        /// runtime.</summary>
        [ContextMenu("Set Item Count 50")]
        public void TestItemCount1() => list.SetItemCount(50);
        /// <summary>Testing function for adjusting the number of items during
        /// runtime.</summary>
        [ContextMenu("Set Item Count 200")]
        public void TestItemCount2() => list.SetItemCount(200);
    }
}
