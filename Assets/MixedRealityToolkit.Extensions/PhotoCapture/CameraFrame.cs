// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// We have this CAN_USE_UNITY_TYPES macro definition so that this file can serve as an example
// for developers building C# windows camera applications outside of Unity.
// Unity specific types should be contained behind this macro with suitable counterparts
// defined when this macro is not available
#define CAN_USE_UNITY_TYPES

// The WINDOWS_UWP macro allows references to WinRT APIs within Unity
#if WINDOWS_UWP
#define CAN_USE_UWP_TYPES
#endif

using System;
using System.Threading;
using System.Threading.Tasks;

#if CAN_USE_UWP_TYPES
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture
{
    /// <summary>
    /// Represents a camera frame, with all format, resolution, properties and pixel data.
    /// When finished using a frame, call Release to return to pool.
    /// </summary>
    public class CameraFrame
    {
        /// <summary>
        /// Pixel format of this frame
        /// </summary>
        public PixelFormat PixelFormat { get; set; }

        /// <summary>
        /// Resolution settings of this frame
        /// </summary>
        public CameraResolution Resolution { get; set; }

        /// <summary>
        /// Exposure start time for this frame. This is a system relative value, and all 
        /// frames from a single session can be compared to this value.
        /// </summary>
        public double FrameTime { get; set; }

        /// <summary>
        /// Exposure duration in seconds for this frame
        /// </summary>
        public double Exposure { get; set; }

        /// <summary>
        /// Camera intrinsics for this frame
        /// </summary>
        public CameraIntrinsics Intrinsics { get; set; }

        /// <summary>
        /// Camera extrinsics (pose) for this frame
        /// </summary>
        public CameraExtrinsics Extrinsics { get; set; }

        /// <summary>
        /// Sensor gain for this frame
        /// </summary>
        public float Gain { get; set; }

        /// <summary>
        /// Pixel data for this frame in PixelFormat
        /// </summary>
        public byte[] PixelData { get; set; }

        /// <summary>
        /// The actual SoftwareBitmap that was returned from the camera frame.
        /// </summary>
#if CAN_USE_UWP_TYPES
        public SoftwareBitmap SoftwareBitmap { get; set; }
#endif

        protected int refCount;

        /// <summary>
        /// Current ref count for the frame
        /// </summary>
        public int RefCount
        {
            get
            {
                return refCount;
            }
        }

        /// <summary>
        /// Ensures only pools can create frames
        /// </summary>
        protected CameraFrame()
        {

        }

        /// <summary>
        /// Adds a reference to the camera frame
        /// </summary>
        public virtual void AddRef()
        {
            Interlocked.Increment(ref refCount);
        }

        /// <summary>
        /// This must be called when finished with the frame
        /// </summary>
        public virtual void Release()
        {
            if (Interlocked.Decrement(ref refCount) <= 0)
            {
#if CAN_USE_UWP_TYPES
                SoftwareBitmap?.Dispose();
                SoftwareBitmap = null;
#endif
                refCount = 0;
            }
        }

        /// <summary>
        /// Saves the camera frame's contents to a provided file path
        /// </summary>
        /// <param name="filePath"></param>
        public async void Save(string filePath)
        {
#if CAN_USE_UWP_TYPES
            if (SoftwareBitmap == null)
            {
                throw new NotSupportedException("Save currently only available if frame was captured with KeepSoftwareBitmap set to true on the camera.");
            }

            int extensionStartPos = filePath.LastIndexOf('.') + 1;
            int filenameStartPos = filePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1;

            if (extensionStartPos >= filePath.Length)
            {
                return;
            }

            string extension = filePath.Substring(extensionStartPos, filePath.Length - extensionStartPos);
            string folderPath = filePath.Substring(0, filenameStartPos);
            string filename = filePath.Substring(filenameStartPos);

            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            StorageFile outputFile = await folder.CreateFileAsync(filename);

            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                // Set the software bitmap
                SoftwareBitmap rgbBmp = SoftwareBitmap.Convert(SoftwareBitmap, BitmapPixelFormat.Bgra8);
                encoder.SetSoftwareBitmap(rgbBmp);

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            // If the encoder does not support writing a thumbnail, then try again
                            // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw;
                    }
                }
                finally
                {
                    rgbBmp?.Dispose();
                }
            }
#else
            await Task.CompletedTask;
#endif
        }
    }
}