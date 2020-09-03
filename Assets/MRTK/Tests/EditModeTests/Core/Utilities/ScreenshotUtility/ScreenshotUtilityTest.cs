// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Build.Editor
{
    class ScreenshotUtilityTest
    {
        /// <summary>
        /// Tests if a screenshot gets captured with various valid (and invalid) capture parameters.
        /// </summary>
        /// <returns>IEnumerator to allow waiting for async methods to complete.</returns>
        [UnityTest]
        public IEnumerator TestScreenshotCapture()
        {
            var path = ScreenshotUtility.GetScreenshotPath();

            try
            {
                Assert.False(string.IsNullOrEmpty(path), "GetScreenshotFileName returned a null or empty string.");

                // Tests which require a valid device context (such as taking screenshots) cannot be run in batch mode.
                if (!Application.isBatchMode)
                {
                    Assert.True(ScreenshotUtility.CaptureScreenshot(path, 1), "Failed to capture a 1x resolution screenshot.");

                    // Edit mode tests can only "yield return null" so we must roll our own "WaitForSeconds" implementation.
                    var waitRoutine = WaitForSeconds(1);
                    while (waitRoutine.MoveNext())
                    {
                        yield return null;
                    }

                    FileAssert.Exists(path, "A file was not written to disk during 1x resolution screenshot capture.");

                    Assert.True(ScreenshotUtility.CaptureScreenshot(path, 4), "Failed to capture a 4x resolution screenshot.");

                    waitRoutine = WaitForSeconds(4);
                    while (waitRoutine.MoveNext())
                    {
                        yield return null;
                    }

                    FileAssert.Exists(path, "A file was not written to disk during 4x resolution screenshot capture.");

                    Assert.True(ScreenshotUtility.CaptureScreenshot(path, 1, true), "Failed to capture a 1x resolution screenshot with a transparent clear color.");

                    waitRoutine = WaitForSeconds(1);
                    while (waitRoutine.MoveNext())
                    {
                        yield return null;
                    }

                    FileAssert.Exists(path, "A file was not written to disk during 1x resolution screenshot capture with a transparent clear color.");

                    Assert.True(ScreenshotUtility.CaptureScreenshot(path, 4, true), "Failed to capture a 4x resolution screenshot with a transparent clear color.");

                    waitRoutine = WaitForSeconds(4);
                    while (waitRoutine.MoveNext())
                    {
                        yield return null;
                    }

                    FileAssert.Exists(path, "A file was not written to disk during 4x resolution screenshot capture with a transparent clear color.");
                }

                Assert.False(ScreenshotUtility.CaptureScreenshot(null, 1), "A screenshot was captured with an invalid path.");
                Assert.False(ScreenshotUtility.CaptureScreenshot(ScreenshotUtility.GetScreenshotPath(), -1), "A screenshot was captured with an invalid super size.");
            }
            finally
            {
                // Clean up any screenshots created.
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private IEnumerator WaitForSeconds(float seconds)
        {
            float timer = seconds;

            while (timer >= 0.0f)
            {
                yield return null;

                timer -= Time.unscaledDeltaTime;
            }
        }
    }
}
