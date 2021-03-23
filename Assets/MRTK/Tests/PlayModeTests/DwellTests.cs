// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Dwell;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for runtime behavior of dwell
    /// </summary>
    public class DwellTests : BasePlayModeTests
    {
        
        #region Utility methods

        private void InitializeTest(MixedRealityToolkitConfigurationProfile profile)
        {
            TestUtilities.InitializeMixedRealityToolkit(profile);
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        private MixedRealityToolkitConfigurationProfile LoadTestProfile(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<MixedRealityToolkitConfigurationProfile>(assetPath);
        }

        #endregion // Utility methods

        #region Test cases

        private const string DefaultHoloLens2ProfileGuid = "7e7c962b9eb9dfa44993d5b2f2576752";
        private static readonly string DefaultHoloLens2ProfilePath = AssetDatabase.GUIDToAssetPath(DefaultHoloLens2ProfileGuid);

        private const float allowedDelay = 0.01f;

        /// <summary>
        /// Test to verify that head based dwell works properly.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHeadDwell()
        {
            // Set up profile
            MixedRealityToolkitConfigurationProfile profile = LoadTestProfile(DefaultHoloLens2ProfilePath);
            yield return null;
            InitializeTest(profile);
            
            // Create a cube with DwellHandler. At the position below the cube is not in focus.
            LogAssert.Expect(LogType.Assert, "DwellProfile is null, creating default profile.");
            GameObject dwellTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dwellTarget.transform.position = new Vector3(2, 0, 2);
            DwellHandler dwellHandler = dwellTarget.AddComponent<DwellHandler>();
            
            // Configure the handle and set up listeners for events
            dwellHandler.DwellProfile.DwellPointerType = Toolkit.Input.InputSourceType.Head;
            dwellHandler.DwellProfile.DwellIntentDelay = new System.TimeSpan(0, 0, 1);
            bool dwellIntended = false;
            dwellHandler.DwellIntended.AddListener((_) => { dwellIntended = true; });
            bool dwellStarted = false;
            dwellHandler.DwellStarted.AddListener((_) => { dwellStarted = true; });
            bool dwellCompleted = false;
            dwellHandler.DwellCompleted.AddListener((_) => { dwellCompleted = true; });
            
            // Confirm initial states
            Assert.IsFalse(dwellIntended, "dwellIntended triggered too early!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCompleted, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.None, "Unexpected dwell state!");
            yield return null;
            
            // Move the cube to a position to make it in focus and verify the state
            dwellTarget.transform.position = new Vector3(0, 0, 2);
            yield return null;
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.FocusGained, "Unexpected dwell state!");
            
            // Verify we are in the dwell intended state
            yield return new WaitForSeconds((float)dwellHandler.DwellProfile.DwellIntentDelay.TotalSeconds + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCompleted, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellIntended, "Unexpected dwell state!");

            // Verify we are in the dwell started state
            yield return new WaitForSeconds((float)dwellHandler.DwellProfile.DwellStartDelay.TotalSeconds + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsFalse(dwellCompleted, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellStarted, "Unexpected dwell state!");

            // Verify we are in the dwell completed state
            yield return new WaitForSeconds((float)dwellHandler.DwellProfile.TimeToCompleteDwell.TotalSeconds + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsTrue(dwellCompleted, "dwellCompleted triggered too late!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellCompleted, "Unexpected dwell state!");
        }

        #endregion // Test cases
    }
}

#endif // !WINDOWS_UWP