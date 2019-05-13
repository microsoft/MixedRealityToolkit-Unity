// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedTextMesh : SynchronizedMeshRenderer<TextMeshService>
    {
        public static class TextMeshChangeType
        {
            public const byte Text = 0x8;
            public const byte FontAndPlacement = 0x10;
        }

        private TextMesh textMesh;
        private string previousText;
        private TextMeshProperties previousFontAndPlacement;

        public TextMesh TextMesh
        {
            get { return this.textMesh; }
        }

        public Guid FontAssetId
        {
            get { return TextMesh.font == null ? Guid.Empty : TextMeshService.Instance.GetFontId(TextMesh.font); }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            textMesh = GetComponent<TextMesh>();
        }

        protected override byte InitialChangeType
        {
            get
            {
                return (byte)(base.InitialChangeType | TextMeshChangeType.FontAndPlacement | TextMeshChangeType.Text);
            }
        }

        protected override byte CalculateDeltaChanges()
        {
            byte changeType = base.CalculateDeltaChanges();

            string newText = this.textMesh.text;
            TextMeshProperties newFontAndPlacement = new TextMeshProperties(this.textMesh);
            if (previousText != newText)
            {
                changeType |= TextMeshChangeType.Text;
                previousText = newText;
            }
            if (previousFontAndPlacement != newFontAndPlacement)
            {
                changeType |= TextMeshChangeType.FontAndPlacement;
                previousFontAndPlacement = newFontAndPlacement;
            }

            return changeType;
        }

        protected override bool ShouldSynchronizeMaterialProperty(MaterialPropertyAsset materialProperty)
        {
            if (materialProperty.propertyType == MaterialPropertyType.Texture && (textMesh.font == null || textMesh.font.dynamic))
            {
                // Font generate textures on each end and we shouldn't attempt to sync that texture over the network
                return false;
            }

            return true;
        }

        protected override void WriteRenderer(BinaryWriter message, byte changeType)
        {
            if (HasFlag(changeType, TextMeshChangeType.Text))
            {
                message.Write(TextMesh.text);
            }
            if (HasFlag(changeType, TextMeshChangeType.FontAndPlacement))
            {
                message.Write((byte)TextMesh.alignment);
                message.Write((byte)TextMesh.anchor);
                message.Write(TextMesh.characterSize);
                message.Write(TextMesh.color);
                message.Write(TextMesh.fontSize);
                message.Write((byte)TextMesh.fontStyle);
                message.Write(TextMesh.lineSpacing);
                message.Write(TextMesh.offsetZ);
                message.Write(TextMesh.richText);
                message.Write(TextMesh.tabSize);
                message.Write(FontAssetId);
            }

            base.WriteRenderer(message, changeType);
        }

        private struct TextMeshProperties
        {
            public TextMeshProperties(TextMesh textMesh)
            {
                this.alignment = textMesh.alignment;
                this.anchor = textMesh.anchor;
                this.characterSize = textMesh.characterSize;
                this.color = textMesh.color;
                this.fontSize = textMesh.fontSize;
                this.fontStyle = textMesh.fontStyle;
                this.lineSpacing = textMesh.lineSpacing;
                this.offsetZ = textMesh.offsetZ;
                this.richText = textMesh.richText;
                this.tabSize = textMesh.tabSize;
            }

            public TextAlignment alignment;
            public TextAnchor anchor;
            public float characterSize;
            public Color color;
            public int fontSize;
            public FontStyle fontStyle;
            public float lineSpacing;
            public float offsetZ;
            public bool richText;
            public float tabSize;

            public static bool operator ==(TextMeshProperties first, TextMeshProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(TextMeshProperties first, TextMeshProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TextMeshProperties))
                {
                    return false;
                }

                TextMeshProperties other = (TextMeshProperties)obj;
                return other.alignment == alignment &&
                    other.anchor == anchor &&
                    other.characterSize == characterSize &&
                    other.color == color &&
                    other.fontSize == fontSize &&
                    other.fontStyle == fontStyle &&
                    other.lineSpacing == lineSpacing &&
                    other.offsetZ == offsetZ &&
                    other.richText == richText &&
                    other.tabSize == tabSize;
            }

            public override int GetHashCode()
            {
                return (int)(fontSize + lineSpacing + offsetZ + tabSize + characterSize);
            }
        }
    }
}