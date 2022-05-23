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
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for runtime behavior of dwell
    /// </summary>
    public class DwellTests : BasePlayModeTests
    {
        #region Test cases

        private const float allowedDelay = 0.01f;

        /// <summary>
        /// Test to verify that head based dwell works properly.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHeadDwell()
        {
            // Initialize
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a cube with DwellHandler. At the position below the cube is not in focus.
            LogAssert.Expect(LogType.Assert, "DwellProfile is null, creating default profile.");
            GameObject dwellTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dwellTarget.transform.position = new Vector3(2, 0, 2);
            DwellHandler dwellHandler = dwellTarget.AddComponent<DwellHandler>();

            // Configure the handle and set up listeners for events
            dwellHandler.DwellProfile.DwellPointerType = Toolkit.Input.InputSourceType.Head;
            dwellHandler.DwellProfile.DwellIntentDelay = 1f;
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
            yield return null;
            yield return null;
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.FocusGained, "Unexpected dwell state!");

            // Verify we are in the dwell intended state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellIntentDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCompleted, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellIntended, "Unexpected dwell state!");

            // Verify we are in the dwell started state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellStartDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsFalse(dwellCompleted, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellStarted, "Unexpected dwell state!");

            // Verify we are in the dwell completed state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToCompleteDwell + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsTrue(dwellCompleted, "dwellCompleted triggered too late!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellCompleted, "Unexpected dwell state!");
        }

        /// <summary>
        /// Test to verify that hand based dwell works properly.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandDwell()
        {
            // Initialize
            TestUtilities.PlayspaceToOriginLookingForward();

            Vector3 focusOnCubeHandPosition = new Vector3(0.2f, 0.1f, 0.8f);

            LogAssert.Expect(LogType.Assert, "DwellProfile is null, creating default profile.");
            GameObject dwellTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dwellTarget.transform.position = new Vector3(2, 0, 2);
            DwellHandler dwellHandler = dwellTarget.AddComponent<DwellHandler>();

            // Configure the handle and set up listeners for events
            dwellHandler.DwellProfile.DwellPointerType = Toolkit.Input.InputSourceType.Hand;
            dwellHandler.DwellProfile.DwellIntentDelay = 1f;
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

            // Move the cube to a position to make it in focus (via head gaze) and verify the head gaze does not change the state
            dwellTarget.transform.position = new Vector3(0, 0, 2);
            yield return null;
            yield return null;
            yield return null;
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.None, "Unexpected dwell state!");

            // Show the hand to make the cube in focus (via hand ray) and verify the state
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(focusOnCubeHandPosition);
            yield return null;
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.FocusGained, "Unexpected dwell state!");

            // Verify we are in the dwell intended state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellIntentDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCompleted, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellIntended, "Unexpected dwell state!");

            // Verify we are in the dwell started state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellStartDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsFalse(dwellCompleted, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellStarted, "Unexpected dwell state!");

            // Verify we are in the dwell completed state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToCompleteDwell + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsTrue(dwellCompleted, "dwellCompleted triggered too late!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellCompleted, "Unexpected dwell state!");
        }

        /// <summary>
        /// Test to verify that dwell was canceled with decay
        /// </summary>
        [UnityTest]
        public IEnumerator TestDwellDecayCanceled()
        {
            // Initialize
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a cube with DwellHandler. At the position below the cube is not in focus.
            LogAssert.Expect(LogType.Assert, "DwellProfile is null, creating default profile.");
            GameObject dwellTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dwellTarget.transform.position = new Vector3(2, 0, 2);
            DwellHandler dwellHandler = dwellTarget.AddComponent<DwellHandler>();

            // Configure the handle and set up listeners for events
            dwellHandler.DwellProfile.DwellPointerType = Toolkit.Input.InputSourceType.Head;
            dwellHandler.DwellProfile.DwellIntentDelay = 1f;
            dwellHandler.DwellProfile.TimeToAllowDwellResume = 1.0f;
            dwellHandler.DwellProfile.TimeToCompleteDwell = dwellHandler.DwellProfile.TimeToAllowDwellResume + 1.0f;
            dwellHandler.DwellProfile.DecayDwellOverTime = true;
            bool dwellIntended = false;
            dwellHandler.DwellIntended.AddListener((_) => { dwellIntended = true; });
            bool dwellStarted = false;
            dwellHandler.DwellStarted.AddListener((_) => { dwellStarted = true; });
            bool dwellCanceled = false;
            dwellHandler.DwellCanceled.AddListener((_) => { dwellCanceled = true; });

            // Confirm initial states
            Assert.IsFalse(dwellIntended, "dwellIntended triggered too early!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCanceled, "dwellCanceled triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.None, "Unexpected dwell state!");
            yield return null;

            // Move the cube to a position to make it in focus and verify the state
            dwellTarget.transform.position = new Vector3(0, 0, 2);
            yield return null;
            yield return null;
            yield return null;
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.FocusGained, "Unexpected dwell state!");

            // Verify we are in the dwell intended state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellIntentDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCanceled, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellIntended, "Unexpected dwell state!");

            // Verify we are in the dwell started state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellStartDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsFalse(dwellCanceled, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellStarted, "Unexpected dwell state!");

            // Let the dwell timer increase a bit
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume + allowedDelay);

            // Move the cube out of the way
            dwellTarget.transform.position = new Vector3(0, 0, -2);

            // check that the dwell progress decreased
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume * 0.25f + allowedDelay);
            float currentProgress = dwellHandler.DwellProgress;
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume * 0.25f + allowedDelay);
            Debug.Log(currentProgress);

            Assert.IsTrue(dwellHandler.DwellProgress < currentProgress, "Dwell progress didn't decay!");

            // verify that the dwell progress is cancelled
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume * 0.5f + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsTrue(dwellCanceled, "dwellCanceled triggered too early!");
        }

        /// <summary>
        /// Test to verify that dwell was canceled without any decay
        /// </summary>
        [UnityTest]
        public IEnumerator TestDwellCanceled()
        {
            // Initialize
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a cube with DwellHandler. At the position below the cube is not in focus.
            LogAssert.Expect(LogType.Assert, "DwellProfile is null, creating default profile.");
            GameObject dwellTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dwellTarget.transform.position = new Vector3(2, 0, 2);
            DwellHandler dwellHandler = dwellTarget.AddComponent<DwellHandler>();

            // Configure the handle and set up listeners for events
            dwellHandler.DwellProfile.DwellPointerType = Toolkit.Input.InputSourceType.Head;
            dwellHandler.DwellProfile.DwellIntentDelay = 1f;
            dwellHandler.DwellProfile.TimeToAllowDwellResume = 1.0f;
            dwellHandler.DwellProfile.TimeToCompleteDwell = dwellHandler.DwellProfile.TimeToAllowDwellResume + 1.0f;
            dwellHandler.DwellProfile.DecayDwellOverTime = false;
            bool dwellIntended = false;
            dwellHandler.DwellIntended.AddListener((_) => { dwellIntended = true; });
            bool dwellStarted = false;
            dwellHandler.DwellStarted.AddListener((_) => { dwellStarted = true; });
            bool dwellCanceled = false;
            dwellHandler.DwellCanceled.AddListener((_) => { dwellCanceled = true; });

            // Confirm initial states
            Assert.IsFalse(dwellIntended, "dwellIntended triggered too early!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCanceled, "dwellCanceled triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.None, "Unexpected dwell state!");
            yield return null;

            // Move the cube to a position to make it in focus and verify the state
            dwellTarget.transform.position = new Vector3(0, 0, 2);
            yield return null;
            yield return null;
            yield return null;
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.FocusGained, "Unexpected dwell state!");

            // Verify we are in the dwell intended state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellIntentDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsFalse(dwellStarted, "dwellStarted triggered too early!");
            Assert.IsFalse(dwellCanceled, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellIntended, "Unexpected dwell state!");

            // Verify we are in the dwell started state
            yield return new WaitForSeconds(dwellHandler.DwellProfile.DwellStartDelay + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsFalse(dwellCanceled, "dwellCompleted triggered too early!");
            Assert.IsTrue(dwellHandler.CurrentDwellState == DwellStateType.DwellStarted, "Unexpected dwell state!");

            // Let the dwell timer increase a bit
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume + allowedDelay);

            // Move the cube out of the way
            dwellTarget.transform.position = new Vector3(0, 0, -2);

            // check that the dwell progress stayed the same
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume * 0.25f + allowedDelay);
            float currentProgress = dwellHandler.DwellProgress;
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume * 0.25f + allowedDelay);
            Assert.IsTrue(dwellHandler.DwellProgress == currentProgress, "Dwell progress decayed!");

            // verify that the dwell progress is cancelled
            yield return new WaitForSeconds(dwellHandler.DwellProfile.TimeToAllowDwellResume * 0.5f + allowedDelay);
            Assert.IsTrue(dwellIntended, "dwellIntended triggered too late!");
            Assert.IsTrue(dwellStarted, "dwellStarted triggered too late!");
            Assert.IsTrue(dwellCanceled, "dwellCanceled triggered too early!");
        }

        #endregion // Test cases
    }
}

#endif // !WINDOWS_UWP