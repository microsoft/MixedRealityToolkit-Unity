// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;

#if STATESYNC_TEXTMESHPRO
using System.IO;
using TMPro;
using UnityEngine;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class TextMeshProBroadcasterBase<TComponentService> : ComponentBroadcaster<TComponentService, TextMeshProBroadcasterChangeType>
        where TComponentService : Singleton<TComponentService>, IComponentBroadcasterService
    {
#if STATESYNC_TEXTMESHPRO

        private TMP_Text textMesh;
        private string previousText;
        private TextMeshProperties previousProperties;

        protected TMP_Text TextMesh
        {
            get { return textMesh; }
        }

        protected override void Awake()
        {
            base.Awake();

            textMesh = GetComponent<TMP_Text>();
        }

        protected override bool HasChanges(TextMeshProBroadcasterChangeType changeFlags)
        {
            return changeFlags != TextMeshProBroadcasterChangeType.None;
        }

        protected override TextMeshProBroadcasterChangeType CalculateDeltaChanges()
        {
            TextMeshProBroadcasterChangeType change = TextMeshProBroadcasterChangeType.None;
            string newText = this.textMesh.text;
            if (previousText != newText)
            {
                change |= TextMeshProBroadcasterChangeType.Text;
                previousText = newText;
            }
            if (!previousProperties.IsCached(textMesh))
            {
                change |= TextMeshProBroadcasterChangeType.FontAndPlacement;
                previousProperties = new TextMeshProperties(textMesh);
            }

            return change;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            SendDeltaChanges(endpoints, TextMeshProBroadcasterChangeType.FontAndPlacement | TextMeshProBroadcasterChangeType.Text);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, TextMeshProBroadcasterChangeType changeFlags)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                ComponentBroadcasterService.WriteHeader(message, this);

                message.Write((byte)changeFlags);
                WriteText(changeFlags, message);

                message.Flush();
                StateSynchronizationSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        public static bool HasFlag(TextMeshProBroadcasterChangeType changeType, TextMeshProBroadcasterChangeType flag)
        {
            return (changeType & flag) == flag;
        }

        protected virtual void WriteText(TextMeshProBroadcasterChangeType changeType, BinaryWriter message)
        {
            if (HasFlag(changeType, TextMeshProBroadcasterChangeType.Text))
            {
                message.Write(textMesh.text);
            }
            if (HasFlag(changeType, TextMeshProBroadcasterChangeType.FontAndPlacement))
            {
                previousProperties.Write(message);
            }
        }

        private static byte Pack(params bool[] values)
        {
            if (values.Length > 8)
            {
                throw new InvalidOperationException();
            }

            byte result = 0;
            byte mask = 1;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    result |= mask;
                }
                mask <<= 1;
            }

            return result;
        }

        private struct TextMeshProperties
        {
            public TextMeshProperties(TMP_Text textMesh)
            {
                alignment = textMesh.alignment;
                alpha = textMesh.alpha;
                autoSizeTextContainer = textMesh.autoSizeTextContainer;
                color = textMesh.color;
                characterSpacing = textMesh.characterSpacing;
                characterWidthAdjustment = textMesh.characterWidthAdjustment;
                enableAutoSizing = textMesh.enableAutoSizing;
                enableCulling = textMesh.enableCulling;
                enabled = textMesh.enabled;
                enableKerning = textMesh.enableKerning;
                enableWordWrapping = textMesh.enableWordWrapping;
                extraPadding = textMesh.extraPadding;
                faceColor = textMesh.faceColor;
                firstVisibleCharacter = textMesh.firstVisibleCharacter;
                font = textMesh.font;
                fontSize = textMesh.fontSize;
                fontSizeMax = textMesh.fontSizeMax;
                fontSizeMin = textMesh.fontSizeMin;
                fontStyle = textMesh.fontStyle;
                fontWeight = textMesh.fontWeight;
                horizontalMapping = textMesh.horizontalMapping;
                ignoreRectMaskCulling = textMesh.ignoreRectMaskCulling;
                ignoreVisibility = textMesh.ignoreVisibility;
                isOrthographic = textMesh.isOrthographic;
                isOverlay = textMesh.isOverlay;
                isRightToLeftText = textMesh.isRightToLeftText;
                isVolumetricText = textMesh.isVolumetricText;
                lineSpacing = textMesh.lineSpacing;
                lineSpacingAdjustment = textMesh.lineSpacingAdjustment;
                mappingUvLineOffset = textMesh.mappingUvLineOffset;
                margin = textMesh.margin;
                maskable = textMesh.maskable;
                maxVisibleCharacters = textMesh.maxVisibleCharacters;
                maxVisibleLines = textMesh.maxVisibleLines;
                maxVisibleWords = textMesh.maxVisibleWords;
                outlineColor = textMesh.outlineColor;
                outlineWidth = textMesh.outlineWidth;
                overflowMode = textMesh.overflowMode;
                overrideColorTags = textMesh.overrideColorTags;
                pageToDisplay = textMesh.pageToDisplay;
                paragraphSpacing = textMesh.paragraphSpacing;
                parseCtrlCharacters = textMesh.parseCtrlCharacters;
                renderMode = textMesh.renderMode;
                richText = textMesh.richText;
                tintAllSprites = textMesh.tintAllSprites;
                useMaxVisibleDescender = textMesh.useMaxVisibleDescender;
                verticalMapping = textMesh.verticalMapping;
                wordSpacing = textMesh.wordSpacing;
                wordWrappingRatios = textMesh.wordWrappingRatios;
            }

            public bool autoSizeTextContainer { get; set; }
            public bool enableAutoSizing { get; set; }
            public bool enableCulling { get; set; }
            public bool enabled { get; private set; }
            public bool enableKerning { get; set; }
            public bool enableWordWrapping { get; set; }
            public bool extraPadding { get; set; }
            public bool ignoreRectMaskCulling { get; private set; }
            public bool ignoreVisibility { get; private set; }
            public bool isOrthographic { get; set; }
            public bool isOverlay { get; set; }
            public bool isRightToLeftText { get; set; }
            public bool isVolumetricText { get; set; }
            public bool maskable { get; set; }
            public bool overrideColorTags { get; set; }
            public bool parseCtrlCharacters { get; set; }
            public bool richText { get; set; }
            public bool tintAllSprites { get; set; }
            public bool useMaxVisibleDescender { get; set; }



            public TextAlignmentOptions alignment { get; set; }
            public float alpha { get; set; }
            public Color color { get; set; }
            public float characterSpacing { get; set; }
            public float characterWidthAdjustment { get; set; }
            public Color32 faceColor { get; set; }
            public int firstVisibleCharacter { get; set; }
            public TMP_FontAsset font { get; set; }
            public float fontSize { get; set; }
            public float fontSizeMax { get; set; }
            public float fontSizeMin { get; set; }
            public FontStyles fontStyle { get; set; }
            public int fontWeight { get; set; }
            public TextureMappingOptions horizontalMapping { get; set; }
            public float lineSpacing { get; set; }
            public float lineSpacingAdjustment { get; set; }
            public float mappingUvLineOffset { get; set; }
            public Vector4 margin { get; set; }
            public int maxVisibleCharacters { get; set; }
            public int maxVisibleLines { get; set; }
            public int maxVisibleWords { get; set; }
            public Color32 outlineColor { get; set; }
            public float outlineWidth { get; set; }
            public TextOverflowModes overflowMode { get; set; }
            public int pageToDisplay { get; set; }
            public float paragraphSpacing { get; set; }
            public TextRenderFlags renderMode { get; set; }
            public TextureMappingOptions verticalMapping { get; set; }
            public float wordWrappingRatios { get; set; }
            public float wordSpacing { get; set; }


            static bool SameColor(Color32 c1, Color32 c2) { return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a; }

            public bool IsCached(TMP_Text other)
            {
                return autoSizeTextContainer == other.autoSizeTextContainer &&
                enableAutoSizing == other.enableAutoSizing &&
                enableCulling == other.enableCulling &&
                enabled == other.enabled &&
                enableKerning == other.enableKerning &&
                enableWordWrapping == other.enableWordWrapping &&
                extraPadding == other.extraPadding &&
                ignoreRectMaskCulling == other.ignoreRectMaskCulling &&
                ignoreVisibility == other.ignoreVisibility &&
                isOrthographic == other.isOrthographic &&
                isOverlay == other.isOverlay &&
                isRightToLeftText == other.isRightToLeftText &&
                isVolumetricText == other.isVolumetricText &&
                maskable == other.maskable &&
                overrideColorTags == other.overrideColorTags &&
                parseCtrlCharacters == other.parseCtrlCharacters &&
                richText == other.richText &&
                useMaxVisibleDescender == other.useMaxVisibleDescender &&
                alignment == other.alignment &&
                alpha == other.alpha &&
                color == other.color &&
                characterSpacing == other.characterSpacing &&
                characterWidthAdjustment == other.characterWidthAdjustment &&
                SameColor(faceColor, other.faceColor) &&
                firstVisibleCharacter == other.firstVisibleCharacter &&
                font == other.font &&
                fontSize == other.fontSize &&
                fontSizeMax == other.fontSizeMax &&
                fontSizeMin == other.fontSizeMin &&
                fontStyle == other.fontStyle &&
                fontWeight == other.fontWeight &&
                horizontalMapping == other.horizontalMapping &&
                lineSpacing == other.lineSpacing &&
                lineSpacingAdjustment == other.lineSpacingAdjustment &&
                mappingUvLineOffset == other.mappingUvLineOffset &&
                margin == other.margin &&
                maxVisibleCharacters == other.maxVisibleCharacters &&
                maxVisibleLines == other.maxVisibleLines &&
                maxVisibleWords == other.maxVisibleWords &&
                SameColor(outlineColor, other.outlineColor) &&
                outlineWidth == other.outlineWidth &&
                overflowMode == other.overflowMode &&
                pageToDisplay == other.pageToDisplay &&
                paragraphSpacing == other.paragraphSpacing &&
                renderMode == other.renderMode &&
                tintAllSprites == other.tintAllSprites &&
                verticalMapping == other.verticalMapping &&
                wordWrappingRatios == other.wordWrappingRatios &&
                wordSpacing == other.wordSpacing;
            }


            public void Write(BinaryWriter message)
            {
                message.Write(TextMeshProService.Instance.GetFontId(font));

                message.Write(Pack(
                    autoSizeTextContainer,
                    enableAutoSizing,
                    enableCulling,
                    enabled,
                    enableKerning,
                    enableWordWrapping,
                    extraPadding,
                    ignoreRectMaskCulling));
                message.Write(Pack(
                    ignoreVisibility,
                    isOrthographic,
                    isOverlay,
                    isRightToLeftText,
                    isVolumetricText,
                    maskable,
                    overrideColorTags,
                    parseCtrlCharacters));
                message.Write(Pack(
                    richText,
                    tintAllSprites,
                    useMaxVisibleDescender));

                message.Write((int)alignment);
                message.Write(alpha);
                message.Write(color);
                message.Write(characterSpacing);
                message.Write(characterWidthAdjustment);
                message.Write(faceColor);
                message.Write(firstVisibleCharacter);
                message.Write(fontSize);
                message.Write(fontSizeMax);
                message.Write(fontSizeMin);
                message.Write((int)fontStyle);
                message.Write(fontWeight);
                message.Write((byte)horizontalMapping);
                message.Write(lineSpacing);
                message.Write(lineSpacingAdjustment);
                message.Write(mappingUvLineOffset);
                message.Write(margin);
                message.Write(maxVisibleCharacters);
                message.Write(maxVisibleLines);
                message.Write(maxVisibleWords);
                message.Write(outlineColor);
                message.Write(outlineWidth);
                message.Write((byte)overflowMode);
                message.Write(pageToDisplay);
                message.Write(paragraphSpacing);
                message.Write((byte)renderMode);
                message.Write(tintAllSprites);
                message.Write((byte)verticalMapping);
                message.Write(wordWrappingRatios);
                message.Write(wordSpacing);
            }
        }
#else
        protected override bool HasChanges(TextMeshProBroadcasterChangeType changeFlags)
        {
            throw new NotImplementedException();
        }

        protected override TextMeshProBroadcasterChangeType CalculateDeltaChanges()
        {
            throw new NotImplementedException();
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            throw new NotImplementedException();
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, TextMeshProBroadcasterChangeType changeFlags)
        {
            throw new NotImplementedException();
        }
#endif
    }
}