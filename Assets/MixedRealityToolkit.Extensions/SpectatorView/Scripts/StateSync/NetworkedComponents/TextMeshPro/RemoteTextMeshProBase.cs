// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;

#if STATESYNC_TEXTMESHPRO
using TMPro;
#else
using System;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class RemoteTextMeshProBase : RemoteComponent
    {
#if STATESYNC_TEXTMESHPRO
        protected TMP_Text RemoteTextMesh
        {
            get;
            set;
        }
#else
        public override Type ComponentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }
#endif

        public static bool HasFlag(SynchronizedTextMeshProChangeType changeType, SynchronizedTextMeshProChangeType flag)
        {
            return (changeType & flag) == flag;
        }

        protected abstract void EnsureTextComponent();

        private static bool[] Unpack(byte value)
        {
            bool[] result = new bool[8];
            byte mask = 1;
            for (int i = 0; i < 8; i++)
            {
                result[i] = (value & mask) == mask;
                mask <<= 1;
            }

            return result;
        }

        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
#if STATESYNC_TEXTMESHPRO
            EnsureTextComponent();

            SynchronizedTextMeshProChangeType changeType = (SynchronizedTextMeshProChangeType)message.ReadByte();

            if (HasFlag(changeType, SynchronizedTextMeshProChangeType.Text))
            {
                RemoteTextMesh.SetText(message.ReadString());
            }

            if (HasFlag(changeType, SynchronizedTextMeshProChangeType.FontAndPlacement))
            {
                RemoteTextMesh.font = TextMeshProService.Instance.GetFont(message.ReadGuid());

                bool[] values = Unpack(message.ReadByte());
                RemoteTextMesh.autoSizeTextContainer = values[0];
                RemoteTextMesh.enableAutoSizing = values[1];
                RemoteTextMesh.enableCulling = values[2];
                RemoteTextMesh.enabled = values[3];
                RemoteTextMesh.enableKerning = values[4];
                RemoteTextMesh.enableWordWrapping = values[5];
                RemoteTextMesh.extraPadding = values[6];
                RemoteTextMesh.ignoreRectMaskCulling = values[7];

                values = Unpack(message.ReadByte());
                RemoteTextMesh.ignoreVisibility = values[0];
                RemoteTextMesh.isOrthographic = values[1];
                RemoteTextMesh.isOverlay = values[2];
                RemoteTextMesh.isRightToLeftText = values[3];
                RemoteTextMesh.isVolumetricText = values[4];
                RemoteTextMesh.maskable = values[5];
                RemoteTextMesh.overrideColorTags = values[6];
                RemoteTextMesh.parseCtrlCharacters = values[7];

                values = Unpack(message.ReadByte());
                RemoteTextMesh.richText = values[0];
                RemoteTextMesh.tintAllSprites = values[1];
                RemoteTextMesh.useMaxVisibleDescender = values[2];

                RemoteTextMesh.alignment = (TextAlignmentOptions)message.ReadInt32();
                RemoteTextMesh.alpha = message.ReadSingle();
                RemoteTextMesh.color = message.ReadColor();
                RemoteTextMesh.characterSpacing = message.ReadSingle();
                RemoteTextMesh.characterWidthAdjustment = message.ReadSingle();
                RemoteTextMesh.faceColor = message.ReadColor32();
                RemoteTextMesh.firstVisibleCharacter = message.ReadInt32();
                RemoteTextMesh.fontSize = message.ReadSingle();
                RemoteTextMesh.fontSizeMax = message.ReadSingle();
                RemoteTextMesh.fontSizeMin = message.ReadSingle();
                RemoteTextMesh.fontStyle = (FontStyles)message.ReadInt32();
                RemoteTextMesh.fontWeight = message.ReadInt32();
                RemoteTextMesh.horizontalMapping = (TextureMappingOptions)message.ReadByte();
                RemoteTextMesh.lineSpacing = message.ReadSingle();
                RemoteTextMesh.lineSpacingAdjustment = message.ReadSingle();
                RemoteTextMesh.mappingUvLineOffset = message.ReadSingle();
                RemoteTextMesh.margin = message.ReadVector4();
                RemoteTextMesh.maxVisibleCharacters = message.ReadInt32();
                RemoteTextMesh.maxVisibleLines = message.ReadInt32();
                RemoteTextMesh.maxVisibleWords = message.ReadInt32();
                RemoteTextMesh.outlineColor = message.ReadColor32();
                RemoteTextMesh.outlineWidth = message.ReadSingle();
                RemoteTextMesh.overflowMode = (TextOverflowModes)message.ReadByte();
                RemoteTextMesh.pageToDisplay = message.ReadInt32();
                RemoteTextMesh.paragraphSpacing = message.ReadSingle();
                RemoteTextMesh.renderMode = (TextRenderFlags)message.ReadByte();
                RemoteTextMesh.verticalMapping = (TextureMappingOptions)message.ReadByte();
                RemoteTextMesh.wordWrappingRatios = message.ReadSingle();
                RemoteTextMesh.wordSpacing = message.ReadSingle();
            }
#endif
        }
    }
}