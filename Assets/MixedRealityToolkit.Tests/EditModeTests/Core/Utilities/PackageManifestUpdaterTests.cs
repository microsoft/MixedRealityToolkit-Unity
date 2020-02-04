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
            Debug.Log("Try to get version from properly formatted version with no prerelease.");

            Version v;
            float f;

            bool success = PackageManifestUpdater.TryGetVersionComponents("17.27.43", out v, out f);
            Assert.IsTrue(success);
            Assert.AreEqual(v, new Version(17, 27, 43));
            Assert.AreEqual(f, 0f);

            Debug.Log("Try to get version from properly formatted version with prerelease.");

            success = PackageManifestUpdater.TryGetVersionComponents("0.9.1-20200131.12", out v, out f);
            Assert.IsTrue(success);
            Assert.AreEqual(v, new Version(0, 9, 1));
            Assert.AreEqual(f, float.Parse("20200131.12"));
        }

        /// <summary>
        /// Verifies parsing of an improperly formatted string returns the correct failure results.
        /// </summary>
        [Test]
        public void TryGetInvalidVersion()
        {
            Debug.Log("Try to get version from improperly formatted version.");
            Version v;
            float f;

            bool success = PackageManifestUpdater.TryGetVersionComponents("x.2.3", out v, out f);
            Assert.IsFalse(success);
            Assert.IsNull(v);
            Assert.AreEqual(f, float.NaN);

            Debug.Log("Try to get version from improperly formatted prerelease.");

            v = new Version(5, 6, 7);
            f = 17f;
            success = PackageManifestUpdater.TryGetVersionComponents("1.2.3-v20200417.19", out v, out f);
            Assert.IsFalse(success);
            Assert.IsNull(v);
            Assert.AreEqual(f, float.NaN);

            Debug.Log("Try to get version from an empty string.");

            v = new Version(5, 6, 7);
            f = 17f;
            success = PackageManifestUpdater.TryGetVersionComponents("", out v, out f);
            Assert.IsFalse(success);
            Assert.IsNull(v);
            Assert.AreEqual(f, float.NaN);
        }

        /// <summary>
        /// Verifies that an outdated MSBuild for Unity version string will return the correct result.
        /// </summary>
        [Test]
        public void OutdatedMSBuild()
        {
            Debug.Log("Upgrade to new verion / prerelease");
            string minVersion = "1.5.22-20200919.28";
            string currentVersion = "0.7.15";

            bool isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);

            Debug.Log("Upgrade to new prerelease");
            minVersion = "9.4.2-20200622.4";
            currentVersion = "9.4.2-20200530.9";
            isAppropriate = true;

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);

            Debug.Log("Upgrade to new version");
            minVersion = "1.0.0";
            currentVersion = "0.7.15";
            isAppropriate = true;

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);

            Debug.Log("Upgrade prerelease to final");
            minVersion = "0.9.19";
            currentVersion = "0.9.19-20200101.55";
            isAppropriate = true;

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsFalse(isAppropriate);
        }

        /// <summary>
        /// Verifies that an exactly matching MSBuild for Unity version returns the correct result.
        /// </summary>
        [Test]
        public void MatchingMSBuild()
        {
            Debug.Log("Compare version");
            string minVersion = "28.32.44";
            string currentVersion = "28.32.44";

            bool isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);

            Debug.Log("Compare version / prerelease");
            minVersion = "1.5.22-20200919.28";
            currentVersion = "1.5.22-20200919.28";
            isAppropriate = false;

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);
        }

        /// <summary>
        /// Verifies that a more recent MSBuild for Unity version returns the correct result.
        /// </summary>
        [Test]
        public void NewerMSBuild()
        {
            Debug.Log("Newer version");
            string minVersion = "28.32.44";
            string currentVersion = "28.32.45";

            bool isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);

            Debug.Log("Newer prerelease");
            minVersion = "1.5.22-20200919.28";
            currentVersion = "1.5.22-20200919.29";
            isAppropriate = false;

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);

            Debug.Log("Newer version / prerelease");
            minVersion = "1.5.22-20200919.28";
            currentVersion = "1.6.30-20201031.6";
            isAppropriate = false;

            isAppropriate = PackageManifestUpdater.IsAppropriateMBuildVersion(minVersion, currentVersion);
            Assert.IsTrue(isAppropriate);
        }
    }
}
