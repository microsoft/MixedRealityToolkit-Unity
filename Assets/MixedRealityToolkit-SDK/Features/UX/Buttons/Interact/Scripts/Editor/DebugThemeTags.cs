// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Interact.Themes;
using Interact.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interact
{
    public class DebugThemeTags
    {
        private struct Orphan
        {
            public InteractiveThemeWidget Widget;
            public string Tag;
            public Interactive InteractiveHost;
        }

        private struct ThemeTags
        {
            public string Tag;
            public AbstractInteractiveTheme Theme;
            public int Count;
        }

        [MenuItem("Interact/Debug Interactive Theme Tags %#I")]
        public static void Execute()
        {
            AbstractInteractiveTheme[] themes = GameObject.FindObjectsOfType<AbstractInteractiveTheme>();
            InteractiveThemeWidget[] widgets = GameObject.FindObjectsOfType<InteractiveThemeWidget>();

            List<Orphan> orphans = new List<Orphan>();
            
            for (int i = 0; i < widgets.Length; i++)
            {
                List<string> tags = widgets[i].GetTags();
                
                for (int n = 0; n < tags.Count; n++)
                {
                    if (!TagHasMatchingThemeTag(tags[n], themes))
                    {
                        Orphan orphan = new Orphan();
                        orphan.Tag = tags[n];
                        orphan.Widget = widgets[i];
                        orphan.InteractiveHost = widgets[i].gameObject.GetComponentInParent<Interactive>();
                        orphans.Add(orphan);
                    }
                }
            }

            string orphanList = "";
            for (int i = 0; i < orphans.Count; i++)
            {
                Interactive host = orphans[i].InteractiveHost;
                string name = "null";
                if (host != null)
                {
                    name = host.gameObject.name;
                }
                
                orphanList += orphans[i].Tag + " [" + name + "/" + orphans[i].Widget.name + "]  \n";
            }

            bool clickedOk = EditorUtility.DisplayDialog(
                            "Orphan Widget Tags",
                            "Orphan Tags Found: " + orphans.Count + "\n" + orphanList,
                            "OK");

            if (clickedOk)
            {
                // nothing to do
            }

        }

        public static bool TagHasMatchingThemeTag(string tag, AbstractInteractiveTheme[] themes)
        {
            for (int i = 0; i < themes.Length; i++)
            {
                if (themes[i].Tag == tag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
