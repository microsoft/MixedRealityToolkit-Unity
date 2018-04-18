// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    [RequireComponent(typeof(InteractiveSet))]
    public class InteractiveGroup : MonoBehaviour
    {
        [Tooltip("Gameobject containing GridLayoutGroup")]
        public GameObject Grid;

        [Tooltip("Prefab for your interactive.")]
        public GameObject InteractivePrefab;

        [Tooltip("scale for new instance of InteractivePrefab")]
        public Vector3 PrefabScale = new Vector3(2000, 2000, 2000);

        [Tooltip("Data to fill the InteractiveSet.")]
        public List<string> Titles = new List<string>();

        private void Start()
        {
            // note that simplifying this with the ??-operator does not work in Unity.
            Grid = Grid == null ? transform.Find("Canvas/Grid").gameObject : Grid;

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
        private void CreateInteractives()
        {
            for (int i = Interactives.Count; i < Titles.Count; i++)
            {
                GameObject PrefabInst = Instantiate(InteractivePrefab, Grid.transform) as GameObject;
                PrefabInst.transform.localScale = PrefabScale;
                InteractiveToggle InterInst = PrefabInst.GetComponent<InteractiveToggle>();
                if (InterInst == null)
                {
                    Debug.LogWarning("Please add an InteractiveToggle for your prefab " +
                        gameObject.name + " to use it in an InteractiveGroup.");
                }
                else
                { 
                    Interactives.Add(InterInst);
                }
            }
        }

        /// <summary>
        /// position interactives and set title text.
        /// </summary>
        public void UpdateData()
        {
            RemoveInteractives(Titles.Count);
            CreateInteractives();

            for (int i = 0; i < Interactives.Count; i++)
            {
                // Set title
                string title = Titles[i];
                Interactive interactive = Interactives[i];
                interactive.SetTitle(title);
                interactive.Keyword = title;
            }
            GetInteractiveSet().SelectedIndices.Clear();
            GetInteractiveSet().UpdateInteractives();
        }

        private void OnDestroy()
        {
            RemoveInteractives();
        }
 

        /// <summary>
        /// Remove unused Interactives from scene
        /// </summary>
        /// <param name="keep">Number of Interactives that will NOT be deleted</param>
        private void RemoveInteractives(int keep = 0)
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