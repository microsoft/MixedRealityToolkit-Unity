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
            { BaseKey + typeof(bool).Name, true },
            { BaseKey + typeof(float).Name, 2.3f },
            { BaseKey + typeof(int).Name, 5 },
            { BaseKey + typeof(string).Name, "TEST"},
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
                    if (test.Value is float floatValue)
                    {
                        ProjectPreferences.Set(test.Key, floatValue);
                        Assert.AreEqual(floatValue, ProjectPreferences.Get(test.Key, 0f));
                    }
                    else if (test.Value is bool boolValue)
                    {
                        ProjectPreferences.Set(test.Key, boolValue);
                        Assert.AreEqual(boolValue, ProjectPreferences.Get(test.Key, false));
                    }
                    else if (test.Value is int intValue)
                    {
                        ProjectPreferences.Set(test.Key, intValue);
                        Assert.AreEqual(intValue, ProjectPreferences.Get(test.Key, 0));
                    }
                    else if (test.Value is string stringValue)
                    {
                        ProjectPreferences.Set(test.Key, stringValue);
                        Assert.AreEqual(stringValue, ProjectPreferences.Get(test.Key, string.Empty));
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
                    if (test.Value is float floatValue)
                    {
                        Assert.AreEqual(floatValue, ProjectPreferences.Get(test.Key, floatValue));
                    }
                    else if (test.Value is bool boolValue)
                    {
                        Assert.AreEqual(boolValue, ProjectPreferences.Get(test.Key, boolValue));
                    }
                    else if (test.Value is int intValue)
                    {
                        Assert.AreEqual(intValue, ProjectPreferences.Get(test.Key, intValue));
                    }
                    else if (test.Value is string stringValue)
                    {
                        Assert.AreEqual(stringValue, ProjectPreferences.Get(test.Key, stringValue));
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