// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Utilities.Editor
{
    public class ProjectPreferencesTest : MonoBehaviour
    {
        [TearDown]
        public void CleanUp()
        {
            AssetDatabase.DeleteAsset(DummyProjectPreferences.PreferencesFilePath);
        }

        /// <summary>
        /// TODO
        /// </summary>
        [UnityTest]
        public void TestNewPreferences()
        {
            const string BaseKey = "ProjectPreferencesTest_";

            var TestData = new[]{
                new { Key=BaseKey + typeof(bool).Name, Value=(object)true, Type=typeof(bool) },
                new { Key=BaseKey + typeof(float).Name, Value = (object)2.3f, Type = typeof(float) },
                new { Key=BaseKey + typeof(int).Name, Value = (object)5, Type = typeof(int) },
                new { Key=BaseKey + typeof(string).Name, Value = (object)"TEST", Type = typeof(string)},
            };

            foreach (var test in TestData)
            {
                if (test.Value is float)
                {
                    var value = (float)test.Value;
                    DummyProjectPreferences.Set(test.Key, value);
                    Assert.AreEqual(value, DummyProjectPreferences.Get(test.Key, 0f));
                }
                else if (test.Value is bool)
                {
                    var value = (bool)test.Value;
                    DummyProjectPreferences.Set(test.Key, value);
                    Assert.AreEqual(value, DummyProjectPreferences.Get(test.Key, 0f));
                }
                else if (test.Value is int)
                {
                    var value = (int)test.Value;
                    DummyProjectPreferences.Set(test.Key, value);
                    Assert.AreEqual(value, DummyProjectPreferences.Get(test.Key, 0f));
                }
                else if (test.Value is string)
                {
                    var value = (string)test.Value;
                    DummyProjectPreferences.Set(test.Key, value);
                    Assert.AreEqual(value, DummyProjectPreferences.Get(test.Key, 0f));
                }
            }
        }
    }
}