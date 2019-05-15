// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedText : SynchronizedComponent<TextService, SynchronizedText.ChangeType>
    {
        private Text textComp;
        private string previousText;
        private TextProperties previousFontAndPlacement;
        private SynchronizedMaterials synchronizedMaterials = new SynchronizedMaterials();

        [Flags]
        public enum ChangeType : byte
        {
            None = 0x0,
            Text = 0x1,
            FontAndPlacement = 0x2,
            Materials = 0x4,
            MaterialProperty = 0x8
        }

        public Guid FontAssetId
        {
            get { return textComp.font == null ? Guid.Empty : TextService.Instance.GetFontId(textComp.font); }
        }

        protected override void Awake()
        {
            base.Awake();

            textComp = GetComponent<Text>();
        }

        protected override bool HasChanges(ChangeType changeFlags)
        {
            return changeFlags != ChangeType.None;
        }

        protected override ChangeType CalculateDeltaChanges()
        {
            ChangeType change = ChangeType.None;
            string newText = textComp.text;
            TextProperties newFontAndPlacement = new TextProperties(textComp);
            if (previousText != newText)
            {
                previousText = newText;
                change |= ChangeType.Text;
            }
            if (previousFontAndPlacement != newFontAndPlacement)
            {
                previousFontAndPlacement = newFontAndPlacement;
                change |= ChangeType.FontAndPlacement;
            }

            bool areMaterialsDifferent;
            synchronizedMaterials.UpdateMaterials(null, SynchronizedTransform.PerformanceParameters, new Material[] { textComp.materialForRendering }, out areMaterialsDifferent);
            if (areMaterialsDifferent)
            {
                change |= ChangeType.Materials;
            }

            if (!HasFlag(change, ChangeType.Materials))
            {
                change |= ChangeType.MaterialProperty;
            }

            return change;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            SendDeltaChanges(endpoints, ChangeType.FontAndPlacement | ChangeType.Text | ChangeType.Materials);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, ChangeType changeFlags)
        {
            if (changeFlags != ChangeType.MaterialProperty)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter message = new BinaryWriter(memoryStream))
                {
                    SynchronizedComponentService.WriteHeader(message, this);

                    message.Write((byte)(changeFlags & ~ChangeType.MaterialProperty));

                    if (HasFlag(changeFlags, ChangeType.Text))
                    {
                        message.Write(textComp.text);
                    }
                    if (HasFlag(changeFlags, ChangeType.FontAndPlacement))
                    {
                        message.Write((byte)textComp.alignment);
                        message.Write(textComp.color);
                        message.Write(textComp.fontSize);
                        message.Write((byte)textComp.fontStyle);
                        message.Write(textComp.lineSpacing);
                        message.Write((byte)textComp.horizontalOverflow);
                        message.Write((byte)textComp.verticalOverflow);
                        message.Write(FontAssetId);
                    }
                    if (HasFlag(changeFlags, ChangeType.Materials))
                    {
                        synchronizedMaterials.SendMaterials(message, null, ShouldSynchronizeMaterialProperty);
                    }

                    message.Flush();
                    SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
                }
            }

            if (HasFlag(changeFlags, ChangeType.MaterialProperty))
            {
                synchronizedMaterials.SendMaterialPropertyChanges(endpoints, null, SynchronizedTransform.PerformanceParameters, message =>
                {
                    TextService.Instance.WriteHeader(message, this);
                    message.Write((byte)ChangeType.MaterialProperty);
                }, ShouldSynchronizeMaterialProperty);
            }
        }

        private bool ShouldSynchronizeMaterialProperty(MaterialPropertyAsset materialProperty)
        {
            return true;
        }

        public static bool HasFlag(ChangeType changeType, ChangeType flag)
        {
            return (changeType & flag) == flag;
        }
        
        private struct TextProperties
        {
            public TextProperties(Text text)
            {
                alignment = text.alignment;
                color = text.color;
                fontSize = text.fontSize;
                fontStyle = text.fontStyle;
                lineSpacing = text.lineSpacing;
                horizontalOverflow = text.horizontalOverflow;
                verticalOverflow = text.verticalOverflow;
            }

            public TextAnchor alignment;
            public Color color;
            public int fontSize;
            public FontStyle fontStyle;
            public float lineSpacing;
            public HorizontalWrapMode horizontalOverflow;
            public VerticalWrapMode verticalOverflow;

            public static bool operator ==(TextProperties first, TextProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(TextProperties first, TextProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TextProperties))
                {
                    return false;
                }

                TextProperties other = (TextProperties)obj;
                return
                    other.alignment == alignment &&
                    other.color == color &&
                    other.fontSize == fontSize &&
                    other.fontStyle == fontStyle &&
                    other.lineSpacing == lineSpacing &&
                    other.horizontalOverflow == horizontalOverflow &&
                    other.verticalOverflow == verticalOverflow;
            }

            public override int GetHashCode()
            {
                return (int)(fontSize + lineSpacing + fontSize);
            }
        }
    }
}