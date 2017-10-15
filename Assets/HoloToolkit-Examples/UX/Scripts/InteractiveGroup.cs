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

        void Start()
        {
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

        private List<InteractiveToggle> Interactives {
            get 
            {
                return GetInteractiveSet().Interactives;
            }
            set 
            {
                GetInteractiveSet().Interactives = value;
            }
        }

        public InteractiveSet GetInteractiveSet()
        {
            return GetComponent<InteractiveSet>();
        }

        /// <summary>
        /// create new Prefab-instance and fill with given data
        /// </summary>
        void CreateInteractives()
        {
            for (int i = Interactives.Count; i < Titles.Count; i++)
            {
                GameObject PrefabInst = Instantiate(InteractivePrefab, gameObject.transform) as GameObject;
                InteractiveToggle InterInst = PrefabInst.GetComponent<InteractiveToggle>();
                Interactives.Add(InterInst);
            }
        }

        /// <summary>
        /// shortcut to set title
        /// (assuming this Interactive has a LabelTheme attached to it)
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            LabelTheme lblTheme = gameObject.GetComponent<LabelTheme>();
            if (lblTheme == null)
            {
                Debug.LogError("No LabelTheme attached to this Interactive");
            }
            else
            {
                lblTheme.Default = title;
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

            for (int i = 0; i < Interactives.Count; i++)
            {
                // set title
                string title = Titles[i];
                Interactive interactive = Interactives[i];
                interactive.SetTitle(title);
                interactive.Keyword = title;
                
                // layouting
                int j = i % rows;
                Collider collider = interactive.gameObject.GetComponent<Collider>();
                Vector2 Distance = new Vector2(
                    collider.bounds.size.x + Offsets.x,
                    collider.bounds.size.y + Offsets.y
                );
                interactive.gameObject.transform.localPosition = new Vector3(
                    ((i / rows) - ((columns - 1) / 2f)) * Distance.x,
                    -(j - (rows - 1) / 2f) * Distance.y);
            }
            GetInteractiveSet().SelectedIndices.Clear();
            GetInteractiveSet().UpdateInteractives();
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
            for (int i = Interactives.Count - 1; i >= keep; i--)
            {
                Interactive interactive = Interactives[i];
                GetInteractiveSet().RemoveInteractive(i);
                DestroyImmediate(interactive.gameObject);
            }
        }
    }
}