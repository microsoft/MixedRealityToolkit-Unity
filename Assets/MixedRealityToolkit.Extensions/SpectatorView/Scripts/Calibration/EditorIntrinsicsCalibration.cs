// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class EditorIntrinsicsCalibration : MonoBehaviour
    {
        public RawImage feedImage;
        public RawImage lastCornersImage;
        public RawImage heatmapImage;
        public RawImage cornersImage;

        private int nextChessboardImageId = 0;
        private CalibrationAPI calibration = null;
        int boardWidth = 16;
        int boardHeight = 11;
        float chessSquareSize = 0.0236f;
        Texture2D chessboardCorners = null;
        Texture2D chessboardHeatmap = null;
        private int cornerScale = 3;
        private int heatmapWidth = 12;
        private List<CalculatedCameraIntrinsics> intrinsics;

#if UNITY_EDITOR
        private void Start()
        {
            CalibrationDataHelper.Initialize(out nextChessboardImageId, out var nextArUcoImageId);
            calibration = new CalibrationAPI();

            for (int i = 0; i < nextChessboardImageId; i++)
            {
                var texture = CalibrationDataHelper.LoadChessboardImage(i);
                if(texture == null ||
                   !ProcessChessboardImage(texture))
                {
                    Debug.LogWarning($"Failed to process/locate chessboard in dataset: {i}");
                }
                else
                {
                    CalibrationDataHelper.SaveChessboardDetectedImage(texture, i);
                }
            }

            if (chessboardHeatmap)
                CalibrationDataHelper.SaveImage(chessboardHeatmap, "ChessboardHeatmap");

            if (chessboardCorners)
                CalibrationDataHelper.SaveImage(chessboardCorners, "ChessboardCorners");
        }

        private void Update()
        {
            if (feedImage != null &&
                feedImage.texture == null)
                feedImage.texture = CompositorWrapper.GetDSLRFeed();

            if (cornersImage)
                cornersImage.texture = chessboardCorners;

            if (heatmapImage)
                heatmapImage.texture = chessboardHeatmap;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var dslrTexture = CompositorWrapper.GetDSLRTexture();
                CalibrationDataHelper.SaveChessboardImage(dslrTexture, nextChessboardImageId);

                if(!ProcessChessboardImage(dslrTexture))
                {
                    Debug.LogWarning($"Failed to process/locate chessboard in dataset: {nextChessboardImageId}");
                }
                else
                {
                    CalibrationDataHelper.SaveChessboardDetectedImage(dslrTexture, nextChessboardImageId);
                    CalibrationDataHelper.SaveImage(chessboardHeatmap, "ChessboardHeatmap");
                    CalibrationDataHelper.SaveImage(chessboardCorners, "ChessboardCorners");
                }

                nextChessboardImageId++;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Starting Camera Intrinsics calculation.");
                intrinsics = calibration.CalculateChessboardIntrinsics(chessSquareSize);
                foreach(var intrinsic in intrinsics)
                {
                    Debug.Log($"Chessboard intrinsics reprojection error: {intrinsic.ToString()}");
                    var file = CalibrationDataHelper.SaveCameraIntrinsics(intrinsic);
                    Debug.Log($"Camera Intrinsics saved to file: {file}");
                }
            }
        }

        private bool ProcessChessboardImage(Texture2D texture)
        {
            if (texture == null ||
                texture.format != TextureFormat.RGB24)
            {
                return false;
            }

            int imageWidth = texture.width;
            int imageHeight = texture.height;

            if (chessboardCorners == null)
            {
                chessboardCorners = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
                for (int y = 0; y < chessboardCorners.height; y++)
                {
                    for (int x = 0; x < chessboardCorners.width; x++)
                    {
                        chessboardCorners.SetPixel(x, y, new Color(0, 0, 0));
                    }
                }
                chessboardCorners.Apply();
            }

            if (chessboardHeatmap == null)
            {
                chessboardHeatmap = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
                for (int y = 0; y < chessboardHeatmap.height; y++)
                {
                    for (int x = 0; x < chessboardHeatmap.width; x++)
                    {
                        chessboardHeatmap.SetPixel(x, y, new Color(0, 0, 0));
                    }
                }
                chessboardHeatmap.Apply();
            }

            var unityPixels = texture.GetRawTextureData<byte>();
            var pixels = unityPixels.ToArray();
            var unityCorners = chessboardCorners.GetRawTextureData<byte>();
            var cornersPixels = unityCorners.ToArray();
            var unityHeatmap = chessboardHeatmap.GetRawTextureData<byte>();
            var heatmapPixels = unityHeatmap.ToArray();

            if (!calibration.ProcessChessboardImage(pixels, imageWidth, imageHeight, boardWidth, boardHeight, cornersPixels, heatmapPixels, cornerScale, heatmapWidth))
            {
                return false;
            }

            for (int i = 0; i < unityPixels.Length; i++)
            {
                unityPixels[i] = pixels[i];
            }

            for(int i = 0; i < unityCorners.Length; i++)
            {
                unityCorners[i] = cornersPixels[i];
            }

            for (int i = 0; i < unityHeatmap.Length; i++)
            {
                unityHeatmap[i] = heatmapPixels[i];
            }

            texture.Apply();
            chessboardCorners.Apply();
            chessboardHeatmap.Apply();

            if (lastCornersImage)
                lastCornersImage.texture = texture;

            if (cornersImage)
                cornersImage.texture = chessboardCorners;

            if (heatmapImage)
                heatmapImage.texture = chessboardHeatmap;

            return true;
        }
#endif
    }
}
