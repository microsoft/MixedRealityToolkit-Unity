// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabViewManager.cs" company="Exit Games GmbH">
//   Part of: PunCockpit
// </copyright>
// <summary>
//  Simple Management for Tabs, it requires a ToggleGroup, and then for each Tab, a Unique Name, the related Toggle and its associated RectTransform View 
// this manager handles Tab views activation and deactivation, and provides a Unity Event Callback when a tab was selected.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Tab view manager. Handles Tab views activation and deactivation, and provides a Unity Event Callback when a tab was selected.
    /// </summary>
    public class TabViewManager : MonoBehaviour
    {

        /// <summary>
        /// Tab change event.
        /// </summary>
        [System.Serializable]
        public class TabChangeEvent : UnityEvent<string> { }

        [Serializable]
        public class Tab
        {
            public string ID = "";
            public Toggle Toggle;
            public RectTransform View;
        }

        /// <summary>
        /// The toggle group component target.
        /// </summary>
        public ToggleGroup ToggleGroup;

        /// <summary>
        /// all the tabs for this group
        /// </summary>
        public Tab[] Tabs;

        /// <summary>
        /// The on tab changed Event.
        /// </summary>
        public TabChangeEvent OnTabChanged;

        protected Tab CurrentTab;

        Dictionary<Toggle, Tab> Tab_lut;

        void Start()
        {

            Tab_lut = new Dictionary<Toggle, Tab>();

            foreach (Tab _tab in this.Tabs)
            {

                Tab_lut[_tab.Toggle] = _tab;

                _tab.View.gameObject.SetActive(_tab.Toggle.isOn);

                if (_tab.Toggle.isOn)
                {
                    CurrentTab = _tab;
                }
                _tab.Toggle.onValueChanged.AddListener((isSelected) =>
                {
                    if (!isSelected)
                    {
                        return;
                    }
                    OnTabSelected(_tab);
                });
            }


        }

        /// <summary>
        /// Selects a given tab.
        /// </summary>
        /// <param name="id">Tab Id</param>
        public void SelectTab(string id)
        {
            foreach (Tab _t in Tabs)
            {
                if (_t.ID == id)
                {
                    _t.Toggle.isOn = true;
                    return;
                }
            }
        }


        /// <summary>
        /// final method for a tab selection routine
        /// </summary>
        /// <param name="tab">Tab.</param>
        void OnTabSelected(Tab tab)
        {
            CurrentTab.View.gameObject.SetActive(false);

            CurrentTab = Tab_lut[ToggleGroup.ActiveToggles().FirstOrDefault()];

            CurrentTab.View.gameObject.SetActive(true);

            OnTabChanged.Invoke(CurrentTab.ID);

        }
    }
}