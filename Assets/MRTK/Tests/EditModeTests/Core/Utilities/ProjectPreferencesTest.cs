// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Core.Utilities.Editor
{
    /// <summary>
    /// Test structure to test <see cref="Microsoft.MixedReality.Toolkit.Utilities.Editor.ProjectPreferences"/>
    /// </summary>
    public class ProjectPreferencesTest
    {
        private const string BaseKey = "ProjectPreferencesTest_";
        private static readonly Dictionary<string, object> TestData = new Dictionary<string, object>
        {
            { BaseKey + typeof(bool).Name, (object)true },
            { BaseKey + typeof(float).Name, (object)2.3f },
            { BaseKey + typeof(int).Name, (object)5 },
            { BaseKey + typeof(string).Name, (object)"TEST"},
        };

        /// <summary>
        /// Test basic ProjectPreferences uses of setting a key and then retrieving that item for various data types
        /// </summary>
        [Test]
        public void TestNewPreferences()
        {
            try
            {
                foreach (var test in TestData)
                {
                    if (test.Value is float)
                    {
                        var value = (float)test.Value;
                        ProjectPreferences.Set(test.Key, value);
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, 0f));
                    }
                    else if (test.Value is bool)
                    {
                        var value = (bool)test.Value;
                        ProjectPreferences.Set(test.Key, value);
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, false));
                    }
                    else if (test.Value is int)
                    {
                        var value = (int)test.Value;
                        ProjectPreferences.Set(test.Key, value);
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, 0));
                    }
                    else if (test.Value is string)
                    {
                        var value = (string)test.Value;
                        ProjectPreferences.Set(test.Key, value);
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, string.Empty));
                    }
                }
            }
            finally
            {
                foreach (var test in TestData)
                {
                    Cleanup(test.Key, test.Value);
                }
            }
        }

        /// <summary>
        /// Validate that the default value provided with Get is returned for new preference entries
        /// </summary>
        [Test]
        public void TestDefaultValues()
        {
            try
            {
                foreach (var test in TestData)
                {
                    if (test.Value is float)
                    {
                        var value = (float)test.Value;
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, value));
                    }
                    else if (test.Value is bool)
                    {
                        var value = (bool)test.Value;
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, value));
                    }
                    else if (test.Value is int)
                    {
                        var value = (int)test.Value;
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, value));
                    }
                    else if (test.Value is string)
                    {
                        var value = (string)test.Value;
                        Assert.AreEqual(value, ProjectPreferences.Get(test.Key, value));
                    }
                }
            }
            finally
            {
                foreach (var test in TestData)
                {
                    Cleanup(test.Key, test.Value);
                }
            }
        }

        private void Cleanup(string key, object value)
        {
            if (value is float)
            {
                ProjectPreferences.RemoveFloat(key);
            }
            else if (value is bool)
            {
                ProjectPreferences.RemoveBool(key);
            }
            else if (value is int)
            {
                ProjectPreferences.RemoveInt(key);
            }
            else if (value is string)
            {
                ProjectPreferences.RemoveString(key);
            }
        }
    }
}