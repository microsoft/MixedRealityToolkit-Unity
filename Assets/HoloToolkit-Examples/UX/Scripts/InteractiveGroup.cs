using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    [RequireComponent(typeof(InteractiveSet))]
    public class InteractiveGroup : MonoBehaviour
    {
        [Tooltip("Prefab for your interactive.")]
        public GameObject InteractivePrefab;

        [Tooltip("Data to fill the InteractiveSet.")]
        public List<string> Titles = new List<string>();

        [Tooltip("number of elements after which we start a new column")]
        public int Rows = 3;

        public Vector2 Offsets = new Vector2(0.00f, 0.00f);

        private InteractiveSet Set;

        void Start()
        {
            Set = GetComponent<InteractiveSet>();
            Interactive interactive = InteractivePrefab.GetComponent<Interactive>();
            if (interactive == null)
            {
                Debug.LogError("No interactive attached to Prefab, " +
                    "please attach one of the Interactive scripts " +
                    "to your InteractiveGroup Prefab!");
            }
            else
            {
                UpdateData();
            }
        }

        /// <summary>
        /// create new Prefab-instance and fill with given data
        /// </summary>
        void CreateInteractives()
        {
            for (int i = Set.Interactives.Count; i < Titles.Count; i++)
            {
                GameObject PrefabInst = Instantiate(InteractivePrefab, gameObject.transform) as GameObject;
                InteractiveToggle InterInst = PrefabInst.GetComponent<InteractiveToggle>();
                Set.Interactives.Add(InterInst);
            }
        }

        /// <summary>
        /// position interactives and set title text.
        /// </summary>
        public void UpdateData()
        {
            RemoveInteractives(Titles.Count);
            CreateInteractives();

            int rows = System.Math.Min(Rows, Titles.Count);
            int columns = (Titles.Count - 1) / Rows + 1;

            for (int i = 0; i < Set.Interactives.Count; i++)
            {
                // set title
                string title = Titles[i];
                LabelTheme lblTheme = Set.Interactives[i].gameObject.GetComponent<LabelTheme>();
                Set.Interactives[i].Keyword = title;
                lblTheme.Default = title;

                Set.Interactives[i].gameObject.GetComponentInChildren<TextMesh>().text = title;
                // layouting
                int j = i % rows;
                Collider collider = Set.Interactives[i].gameObject.GetComponent<Collider>();
                Vector2 Distance = new Vector2(
                    collider.bounds.size.x + Offsets.x,
                    collider.bounds.size.y + Offsets.y
                );
                Set.Interactives[i].gameObject.transform.localPosition = new Vector3(
                    ((i / rows) - ((columns - 1) / 2f)) * Distance.x,
                    -(j - (rows - 1) / 2f) * Distance.y);
            }
            Set.SelectedIndices.Clear();
            Set.UpdateInteractives();
        }

        private void OnDestroy()
        {
            RemoveInteractives();
        }
 

        /// <summary>
        /// remove unused Interactives from scene
        /// </summary>
        /// <param name="keep">number of Iteractives that will NOT be deleted</param>
        void RemoveInteractives(int keep = 0)
        {
            for (int i = Set.Interactives.Count - 1; i >= keep; i--)
            {
                Interactive interactive = Set.Interactives[i];
                Set.RemoveInteractive(i);
                DestroyImmediate(interactive.gameObject);
            }
        }
    }
}