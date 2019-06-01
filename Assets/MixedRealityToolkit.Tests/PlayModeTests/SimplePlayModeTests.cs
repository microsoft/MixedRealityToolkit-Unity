// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SimplePlayModeTests
    {
        [UnityTest]
        public IEnumerator Test01_WhizzySphere()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);

            MixedRealityPlayspace.PerformTransformation(
                p =>
                {
                    p.position = new Vector3(1.0f, 1.5f, -2.0f);
                    p.LookAt(Vector3.zero);
                });

            var testLight = new GameObject("TestLight");
            var light = testLight.AddComponent<Light>();
            light.type = LightType.Directional;
            testLight.transform.position = new Vector3(-2.5f, 3.0f, -1.2f);
            testLight.transform.LookAt(Vector3.zero);

            var testObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            testObject.transform.localScale = Vector3.one * 0.1f;

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            while (stopwatch.Elapsed.TotalSeconds < 10.0f)
            {
                float t = (float)stopwatch.Elapsed.TotalSeconds;
                var pi2 = 2.0f*Mathf.PI;

                var a = 0.5f * t;
                var b = 0.75f * t;
                var c = 0.9f * t;
                testObject.transform.position = new Vector3(
                    Mathf.Sin((a + 0.0f) * pi2),
                    Mathf.Sin((b + 0.333f) * pi2),
                    Mathf.Sin((c + 0.666f) * pi2)
                );
                yield return new WaitForFixedUpdate();
            }
            stopwatch.Stop();

            GameObject.Destroy(testLight);
            GameObject.Destroy(testObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }
    }
}
#endif