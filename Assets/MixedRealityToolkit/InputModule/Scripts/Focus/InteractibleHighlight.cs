// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Focus
{
    /// <summary>
    /// Adds or removes materials to target renderer for highlighting Interactable
    /// Useful with focus targets
    /// </summary>
    public class InteractableHighlight : FocusTarget
    {
        [Flags]
        public enum MatStyleEnum
        {
            None = 0,
            Highlight = 1,  // A highlight to indicate focus
            Overlay = 2,    // An overlay to indicate intent
            Both = Highlight | Overlay,
        }

        public virtual void OnEnable()
        {
            if (targetRenderers == null || targetRenderers.Length == 0)
            {
                targetRenderers = GetComponentsInChildren<Renderer>();
            }

            Refresh();
        }

        public virtual void OnDisable()
        {
            Highlight = false;
            Refresh();
        }

        public bool Highlight
        {
            get
            {
                return highlight;
            }
            set
            {
                if (value != highlight)
                {
                    highlight = value;
                    Refresh();
                }
            }
        }

        public MatStyleEnum Style
        {
            set
            {
                if (targetStyle != value)
                {
                    targetStyle = value;
                    Refresh();
                }
            }
        }

        public Renderer[] TargetRenderers
        {
            set
            {
                if (targetRenderers != value)
                {
                    targetRenderers = value;
                    Refresh();
                }
            }
        }

        [SerializeField]
        private string highlightColorProp = "_Color";

        [SerializeField]
        private string outlineColorProp = "_Color";

        [SerializeField]
        private Color highlightColor = Color.green;

        [SerializeField]
        private Color outlineColor = Color.white;

        [SerializeField]
        private Renderer[] targetRenderers;

        [SerializeField]
        private Material highlightMat;

        [SerializeField]
        private Material overlayMat;

        [SerializeField]
        private MatStyleEnum targetStyle = MatStyleEnum.Highlight;

        [SerializeField]
        private bool highlight = false;

        private MatStyleEnum currentStyle = MatStyleEnum.None;

        private void Refresh()
        {
            if (isActiveAndEnabled && highlight)
            {
                AddHighlightMats();
            }
            else
            {
                RemoveHighlightMats();
            }
        }

        private void AddHighlightMats()
        {
            // If we've added our focus mats already, split
            if ((currentStyle & targetStyle) != 0) { return; }

            if (materialsBeforeFocus == null)
            {
                materialsBeforeFocus = new Dictionary<Renderer, List<Material>>();
            }

            for (int i = 0; i < targetRenderers.Length; i++)
            {
                List<Material> preFocusMaterials;

                if (!materialsBeforeFocus.TryGetValue(targetRenderers[i], out preFocusMaterials))
                {
                    preFocusMaterials = new List<Material>();
                    materialsBeforeFocus.Add(targetRenderers[i], preFocusMaterials);
                }
                else
                {
                    preFocusMaterials.Clear();
                }

                preFocusMaterials.AddRange(targetRenderers[i].sharedMaterials);
                // Remove any references to outline and highlight materials
                preFocusMaterials.Remove(highlightMat);
                preFocusMaterials.Remove(overlayMat);
            }

            // If we're using a highlight
            if ((targetStyle & MatStyleEnum.Highlight) != 0)
            {
                // And we haven't added it yet
                if ((currentStyle & MatStyleEnum.Highlight) == 0)
                {
                    AddMatToRenderers(targetRenderers, highlightMat, highlightColorProp, highlightColor);
                }
            }

            // If we're using an outline
            if ((targetStyle & MatStyleEnum.Overlay) != 0)
            {
                // And we haven't added it yet
                if ((currentStyle & MatStyleEnum.Overlay) == 0)
                {
                    AddMatToRenderers(targetRenderers, overlayMat, outlineColorProp, outlineColor);
                }
            }

            currentStyle = targetStyle;
        }

        private void RemoveHighlightMats()
        {
            if (materialsBeforeFocus == null) { return; }

            foreach (KeyValuePair<Renderer, List<Material>> preFocusMats in materialsBeforeFocus)
            {
                preFocusMats.Key.sharedMaterials = preFocusMats.Value.ToArray();
            }

            materialsBeforeFocus.Clear();
            currentStyle = MatStyleEnum.None;
        }

        private static Material AddMatToRenderers(Renderer[] renderers, Material mat, string propName, Color color)
        {
            mat.SetColor(propName, color);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) { continue; }

                var currentMaterials = new List<Material>(renderers[i].sharedMaterials);

                if (!currentMaterials.Contains(mat))
                {
                    currentMaterials.Add(mat);
                    renderers[i].sharedMaterials = currentMaterials.ToArray();
                }
            }

            return mat;
        }

        private static void RemoveMatFromRenderers(Renderer[] renderers, List<Material> mats)
        {
            for (int i = 0; i < mats.Count; i++)
            {
                RemoveMatFromRenderers(renderers, mats[i]);
            }
        }

        private static void RemoveMatFromRenderers(Renderer[] renderers, Material mat)
        {
            if (mat == null) { return; }

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) { continue; }

                var currentMaterials = new List<Material>(renderers[i].sharedMaterials);

                //use the name because it may be instanced
                for (int j = currentMaterials.Count - 1; j >= 0; j--)
                {
                    if (currentMaterials[j] != null && currentMaterials[j].name == mat.name)
                    {
                        currentMaterials.RemoveAt(j);
                    }
                }

                currentMaterials.Remove(mat);
                renderers[i].sharedMaterials = currentMaterials.ToArray();
            }
        }

        private Dictionary<Renderer, List<Material>> materialsBeforeFocus;

        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            if (HasFocus)
            {
                Highlight = true;
            }
        }

        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            Highlight = false;
        }
    }
}
