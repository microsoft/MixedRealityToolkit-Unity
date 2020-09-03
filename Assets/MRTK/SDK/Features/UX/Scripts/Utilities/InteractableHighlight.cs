// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Adds or removes materials to target renderer for highlighting Focused <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s.
    /// </summary>
    /// <remarks>Useful with focusable <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s</remarks>
    [System.Obsolete("This component is no longer supported", true)]
    [AddComponentMenu("Scripts/MRTK/Obsolete/InteractableHighlight")]
    public class InteractableHighlight : BaseFocusHandler
    {
        [Flags]
        public enum HighlightedMaterialStyle
        {
            None = 0 << 0,
            /// <summary>
            /// A highlight to indicate focus.
            /// </summary>
            Highlight = 1 << 0,
            /// <summary>
            /// An overlay to indicate intent.
            /// </summary>
            Overlay = 1 << 1,
            /// <summary>
            /// Both highlight and overlay.
            /// </summary>
            Both = Highlight | Overlay,
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

        [SerializeField]
        private bool highlight = false;

        public HighlightedMaterialStyle Style
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

        /// <summary>
        /// The target renderers that will get the styles applied.
        /// </summary>
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
        [Tooltip("The target renderers that will get the styles applied.")]
        private Renderer[] targetRenderers = null;

        [SerializeField]
        [Tooltip("The material property that will be used for the highlight color.")]
        private string highlightColorProperty = "_Color";

        [SerializeField]
        [Tooltip("The material property that will be used for the outline color.")]
        private string outlineColorProperty = "_Color";

        [SerializeField]
        private Color highlightColor = Color.green;

        [SerializeField]
        private Color outlineColor = Color.white;

        [SerializeField]
        private Material highlightMaterial = null;

        [SerializeField]
        private Material overlayMaterial = null;

        [SerializeField]
        private HighlightedMaterialStyle targetStyle = HighlightedMaterialStyle.Highlight;

        private HighlightedMaterialStyle currentStyle = HighlightedMaterialStyle.None;

        private Dictionary<Renderer, List<Material>> materialsBeforeFocus;

        #region MonoBehaviour Implementation

        private void Awake()
        {
            Debug.LogError(this.GetType().Name + " is deprecated");
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

        #endregion MonoBehaviour Implementation

        private void Refresh()
        {
            if (isActiveAndEnabled && highlight)
            {
                AddHighlightMaterials();
            }
            else
            {
                RemoveHighlightMaterials();
            }
        }

        private void AddHighlightMaterials()
        {
            // If we've added our focus materials already, split
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
                preFocusMaterials.Remove(highlightMaterial);
                preFocusMaterials.Remove(overlayMaterial);
            }

            // If we're using a highlight
            if ((targetStyle & HighlightedMaterialStyle.Highlight) != 0)
            {
                // And we haven't added it yet
                if ((currentStyle & HighlightedMaterialStyle.Highlight) == 0)
                {
                    AddMaterialToRenderers(targetRenderers, highlightMaterial, highlightColorProperty, highlightColor);
                }
            }

            // If we're using an outline
            if ((targetStyle & HighlightedMaterialStyle.Overlay) != 0)
            {
                // And we haven't added it yet
                if ((currentStyle & HighlightedMaterialStyle.Overlay) == 0)
                {
                    AddMaterialToRenderers(targetRenderers, overlayMaterial, outlineColorProperty, outlineColor);
                }
            }

            currentStyle = targetStyle;
        }

        private void RemoveHighlightMaterials()
        {
            if (materialsBeforeFocus == null) { return; }

            foreach (KeyValuePair<Renderer, List<Material>> preFocusMats in materialsBeforeFocus)
            {
                preFocusMats.Key.sharedMaterials = preFocusMats.Value.ToArray();
            }

            materialsBeforeFocus.Clear();
            currentStyle = HighlightedMaterialStyle.None;
        }

        private static void AddMaterialToRenderers(Renderer[] renderers, Material material, string propName, Color color)
        {
            material.SetColor(propName, color);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) { continue; }

                var currentMaterials = new List<Material>(renderers[i].sharedMaterials);

                if (!currentMaterials.Contains(material))
                {
                    currentMaterials.Add(material);
                    renderers[i].sharedMaterials = currentMaterials.ToArray();
                }
            }
        }

        private static void RemoveMatFromRenderers(Renderer[] renderers, List<Material> materials)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                RemoveMatFromRenderers(renderers, materials[i]);
            }
        }

        private static void RemoveMatFromRenderers(Renderer[] renderers, Material material)
        {
            if (material == null) { return; }

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) { continue; }

                var currentMaterials = new List<Material>(renderers[i].sharedMaterials);

                // use the name because it may be instanced
                for (int j = currentMaterials.Count - 1; j >= 0; j--)
                {
                    if (currentMaterials[j] != null && currentMaterials[j].name == material.name)
                    {
                        currentMaterials.RemoveAt(j);
                    }
                }

                currentMaterials.Remove(material);
                renderers[i].sharedMaterials = currentMaterials.ToArray();
            }
        }

        #region IMixedRealityFocusHandler Implementation

        /// <inheritdoc />
        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            if (HasFocus)
            {
                Highlight = true;
            }
        }

        /// <inheritdoc />
        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            Highlight = false;
        }

        #endregion IMixedRealityFocusHandler Implementation
    }
}
