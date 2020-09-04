// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging;
using NUnit.Framework;
using System.Globalization;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    class UserInputRecorderTests
    {
        [Test]
        public void TestGetStringFormat()
        {
            // This test array is passed into UserInputRecorder.GetStringFormat and
            // its content doesn't matter, only its length. The length is arbitrarily
            // chosen to be relatively short.
            object[] testArray = new object[5];

            // en-US uses comma separated delimiters.
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);

            // In en-US, the generated format should be comma-delimited.
            Assert.AreEqual(
                "{0}, {1}, {2}, {3}, {4}",
                UserInputRecorder.GetStringFormat(testArray));

            // de-DE uses semicolon separated delimiters.
            CultureInfo.CurrentCulture = new CultureInfo("de-DE", false);

            // In de-DE, the generated format should be semicolon-delimited.
            Assert.AreEqual(
                "{0}; {1}; {2}; {3}; {4}",
                UserInputRecorder.GetStringFormat(testArray));
        }
    }
}
