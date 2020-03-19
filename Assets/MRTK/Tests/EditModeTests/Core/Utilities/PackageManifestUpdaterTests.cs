// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Core.Utilities.Editor
{
    /// <summary>
    /// Tests that verify the correct behavior of the PackageManifestUpdater.
    /// </summary>
    public class PackageManifestUpdaterTests
    {
        /// <summary>
        /// Verifies parsing of a properly formatted value returns the correct results.
        /// </summary>
        [Test]
        public void TryGetValidVersion()
        {
            Version version;
            float prerelease;

            bool success = PackageManifestUpdater.TryGetVersionComponents("17.27.43", out version, out prerelease);
            Assert.IsTrue(success);
            Assert.AreEqual(version, new Version(17, 27, 43));
            Assert.AreEqual(prerelease, 0f);

            success = PackageManifestUpdater.TryGetVersionComponents("0.9.1-20200131.12", out version, out prerelease);
            Assert.IsTrue(success);
            Assert.AreEqual(version, new Version(0, 9, 1));
            Assert.AreEqual(prerelease, 20200131.12f);
        }

        /// <summary>
        /// Verifies parsing of an improperly formatted string returns the correct failure results.
        /// </summary>
        [Test]
        public void TryGetInvalidVersion()
        {
            Version version;
            float prerelease;

            bool success = PackageManifestUpdater.TryGetVersionComponents("x.2.3", out version, out prerelease);
            Assert.IsFalse(success);
            Assert.IsNull(version);
            Assert.AreEqual(prerelease, float.NaN);

            // Setting arbitrary values to ensure the function modifies them appropriately.
            version = new Version(5, 6, 7);
            prerelease = 17f;
            success = PackageManifestUpdater.TryGetVersionComponents("1.2.3-v20200417.19", out version, out prerelease);
            Assert.IsFalse(success);
            Assert.IsNull(version);
            Assert.AreEqual(prerelease, float.NaN);

            // Setting arbitrary values to ensure the function modifies them appropriately.
            version = new Version(5, 6, 7);
            prerelease = 17f;
            success = PackageManifestUpdater.TryGetVersionComponents("", out version, out prerelease);
            Assert.IsFalse(success);
            Assert.IsNull(version);
            Assert.AreEqual(prerelease, float.NaN);
        }

        /// <summary>
        /// Verifies that an outdated MSBuild for Unity version string will return the correct result.
        /// </summary>
        [Test]
        public void OutdatedMSBuild()
        {
            string minVersion = "1.5.22-20200919.28";
            string currentVersion = "0.7.15";

            bool isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);

            minVersion = "9.4.2-20200622.4";
            currentVersion = "9.4.2-20200530.9";

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);

            minVersion = "1.0.0";
            currentVersion = "0.7.15";

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);

            minVersion = "0.9.19";
            currentVersion = "0.9.19-20200101.55";

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);
        }

        /// <summary>
        /// Verifies that an exactly matching MSBuild for Unity version returns the correct result.
        /// </summary>
        [Test]
        public void MatchingMSBuild()
        {
            string minVersion = "28.32.44";
            string currentVersion = "28.32.44";

            bool isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);

            minVersion = "1.5.22-20200919.28";
            currentVersion = "1.5.22-20200919.28";

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);
        }

        /// <summary>
        /// Verifies that a more recent MSBuild for Unity version returns the correct result.
        /// </summary>
        [Test]
        public void NewerMSBuild()
        {
            string minVersion = "28.32.44";
            string currentVersion = "28.32.45";

            bool isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);

            minVersion = "1.5.22-20200919.28";
            currentVersion = "1.5.22-20200919.29";

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);

            minVersion = "1.5.22-20200919.28";
            currentVersion = "1.6.30-20201031.6";

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);
        }
    }
}
