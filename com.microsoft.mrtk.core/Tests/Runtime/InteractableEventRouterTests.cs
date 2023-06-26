// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the <see cref="InteractableEventRouter"/> class.
    /// </summary>
    public class InteractableEventRouterTests : MonoBehaviour
    {
        private GameObject level0 = null;
        private GameObject interactorObject = null;
        private GameObject level1 = null;
        private GameObject level2 = null;

        private XRRayInteractor interactor = null;

        private InteractableEventRouter level0_router = null;
        private InteractableEventRouterChildSource level1_routerChildSource = null;

        private StatefulInteractable level1_statefulInteractableParent = null;
        private TestInteractableParent level1_testInteractableParent = null;

        private StatefulInteractable level2_statefulInteractableChild = null;
        private TestInteractableChild level2_testInteractableChild = null;

        private XRInteractionManager cachedInteractionManager = null;

        /// <summary>
        /// A cached reference to the <see cref="MRTKInteractionManager"/> on the XR rig.
        /// Cleared during <see cref="TearDown"/> at the end of each test.
        /// </summary>
        private XRInteractionManager CachedInteractionManager
        {
            get
            {
                if (cachedInteractionManager == null)
                {
                    cachedInteractionManager = FindObjectOfType<XRInteractionManager>();
                }
                return cachedInteractionManager;
            }
        }

        [SetUp]
        public void Init()
        {
            CreateTestObjectsWithEventRouter();
        }

        [TearDown]
        public void Teardown()
        {
            if (level0 != null)
            {
                Object.Destroy(level0);
            }

            interactorObject = null;
            interactor = null;
            cachedInteractionManager = null;

            level0 = null;
            level1 = null;
            level2 = null;

            level0_router = null;
            level1_routerChildSource = null;
            level1_statefulInteractableParent = null;
            level1_testInteractableParent = null;
            level2_statefulInteractableChild = null;
            level2_testInteractableChild = null;
        }

        [UnityTest]
        public IEnumerator ComponentCreationTest()
        {
            Assert.IsNotNull(level0, "Level 0 game object should not be null");
            Assert.IsNotNull(interactorObject, "interactor game object should not be null");
            Assert.IsNotNull(level1, "Level 1 game object should not be null");
            Assert.IsNotNull(level2, "Level 2 game object should not be null");
            Assert.IsNotNull(level1_statefulInteractableParent, "The `StatusInteractable` parent should not be null.");
            Assert.IsNotNull(level2_statefulInteractableChild, "The `StatusInteractable` child should not be null.");
            Assert.IsNotNull(level1_testInteractableParent, "The `TestInteractableParent` should not be null.");
            Assert.IsNotNull(level2_testInteractableChild, "The `TestInteractableChild` should not be null.");
            Assert.IsNotNull(level1_routerChildSource, "The `InteractableEventRouterChildSource` should not be null.");
            Assert.IsNotNull(level0_router, "The `InteractableEventRouter` should not be null.");
            Assert.IsNotNull(interactor, "The `XRRayInteractor` should not be null.");
            Assert.IsNotNull(CachedInteractionManager, "The `StatusInteractables` should have created an interaction manager.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsTest()
        {
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred yet.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectExitedCount, "The child select exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleHoverEventsTest()
        {
            Assert.AreEqual(0, level1_testInteractableParent.ChildHoverEnteredCount, "No child hover entered events should have occurred yet.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverEnter((IXRHoverInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverExit((IXRHoverInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(1, level1_testInteractableParent.ChildHoverExitedCount, "The child hover exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TrickleSelectEventsTest()
        {
            Assert.AreEqual(0, level2_testInteractableChild.ParentSelectEnteredCount, "No parent select entered events should have occurred yet.");
            Assert.AreEqual(0, level2_testInteractableChild.ParentSelectExitedCount, "No parent select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, level1_statefulInteractableParent);

            Assert.AreEqual(1, level2_testInteractableChild.ParentSelectEnteredCount, "The parent select entered event should have occurred once.");
            Assert.AreEqual(0, level2_testInteractableChild.ParentSelectExitedCount, "No parent select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, level1_statefulInteractableParent);

            Assert.AreEqual(1, level2_testInteractableChild.ParentSelectEnteredCount, "The parent select entered event should have occurred once.");
            Assert.AreEqual(1, level2_testInteractableChild.ParentSelectExitedCount, "The parent select exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TrickleHoverEventsTest()
        {
            Assert.AreEqual(0, level2_testInteractableChild.ParentHoverEnteredCount, "No parent hover entered events should have occurred yet.");
            Assert.AreEqual(0, level2_testInteractableChild.ParentHoverExitedCount, "No parent hover exited events should have occurred yet.");

            CachedInteractionManager.HoverEnter((IXRHoverInteractor)interactor, level1_statefulInteractableParent);

            Assert.AreEqual(1, level2_testInteractableChild.ParentHoverEnteredCount, "The parent hover entered event should have occurred once.");
            Assert.AreEqual(0, level2_testInteractableChild.ParentHoverExitedCount, "No parent hover exited events should have occurred yet.");

            CachedInteractionManager.HoverExit((IXRHoverInteractor)interactor, level1_statefulInteractableParent);

            Assert.AreEqual(1, level2_testInteractableChild.ParentHoverEnteredCount, "The parent hover entered event should have occurred once.");
            Assert.AreEqual(1, level2_testInteractableChild.ParentHoverExitedCount, "The parent hover exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsFromNewlyAddedChildTest()
        {
            var newChild = new GameObject("new level 2 child");
            var newStatefulInteractableChild = newChild.AddComponent<StatefulInteractable>();
            newChild.transform.SetParent(level1.transform, worldPositionStays: true);

            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred yet.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectExitedCount, "The child select exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleHoverEventsFromNewlyAddedChildTest()
        {
            var newChild = new GameObject("new level 2 child");
            var newStatefulInteractableChild = newChild.AddComponent<StatefulInteractable>();
            newChild.transform.SetParent(level1.transform, worldPositionStays: true);

            Assert.AreEqual(0, level1_testInteractableParent.ChildHoverEnteredCount, "No child hover entered events should have occurred yet.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverEnter((IXRHoverInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverExit((IXRHoverInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(1, level1_testInteractableParent.ChildHoverExitedCount, "The child hover exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsDisabledAfterRemovalOrSelectEventRouteTest()
        {
            level0_router.RemoveEventRoute<BubbleChildSelectEvents>();

            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred, since `BubbleChildSelectEvents` was removed.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred, since `BubbleChildSelectEvents` was removed.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred, since `BubbleChildSelectEvents` was removed.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsDisabledAfterRemovalOfHoverEventRouteTest()
        {
            level0_router.RemoveEventRoute<BubbleChildHoverEvents>();

            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred yet.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(0, level1_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(1, level1_testInteractableParent.ChildSelectExitedCount, "The child select exited event should have occurred once.");

            yield return null;
        }


        [UnityTest]
        public IEnumerator MultipleInteractableEventRoutersOnlyOneBubbledEventTest()
        {
            var levelA = new GameObject("level a");
            var levelB = new GameObject("level b");
 
            // Setup level b 
            levelB.AddComponent<StatefulInteractable>();
            var levelB_testInteractableParent = levelB.AddComponent<TestInteractableParent>();
            levelB.transform.SetParent(levelA.transform, worldPositionStays: true);

            // Setup level a
            var levelA_router = level0.AddComponent<InteractableEventRouter>();
            levelA.transform.SetParent(level0.transform, worldPositionStays: true);
            levelA_router.AddEventRoute<BubbleChildHoverEvents>();
            levelA_router.AddEventRoute<BubbleChildSelectEvents>();
            levelA_router.AddEventRoute<TrickleChildHoverEvents>();
            levelA_router.AddEventRoute<TrickleChildSelectEvents>();

            // Refersh level 0 router, to ensure it doesn't pickup levelB_testInteractableParent
            level0_router.Refresh();


            Assert.AreEqual(0, levelB_testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred yet.");
            Assert.AreEqual(0, levelB_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, levelB_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(0, levelB_testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, level2_statefulInteractableChild);

            Assert.AreEqual(1, levelB_testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(1, levelB_testInteractableParent.ChildSelectExitedCount, "The child select exited event should have occurred once.");


            Destroy(levelA);
            level0 = null;
            level1 = null;
            level2 = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator MultipleInteractableEventRoutersOnlyOneTrickledEventTest()
        {
            var levelA = new GameObject("level a");
            var levelB = new GameObject("level b");

            // Setup level b 
            var levelB_statefulInteractableParent = levelB.AddComponent<StatefulInteractable>();
            levelB.AddComponent<TestInteractableParent>();
            levelB.transform.SetParent(levelA.transform, worldPositionStays: true);

            // Setup level a
            var levelA_router = level0.AddComponent<InteractableEventRouter>();
            levelA.transform.SetParent(level0.transform, worldPositionStays: true);
            levelA_router.AddEventRoute<BubbleChildHoverEvents>();
            levelA_router.AddEventRoute<BubbleChildSelectEvents>();
            levelA_router.AddEventRoute<TrickleChildHoverEvents>();
            levelA_router.AddEventRoute<TrickleChildSelectEvents>();

            // Refersh level 0 router, to ensure it doesn't pickup levelB_testInteractableParent
            level0_router.Refresh();

            Assert.AreEqual(0, level2_testInteractableChild.ParentSelectEnteredCount, "No parent select entered events should have occurred yet.");
            Assert.AreEqual(0, level2_testInteractableChild.ParentSelectExitedCount, "No parent select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, levelB_statefulInteractableParent);

            Assert.AreEqual(1, level2_testInteractableChild.ParentSelectEnteredCount, "The parent select entered event should have occurred once.");
            Assert.AreEqual(0, level2_testInteractableChild.ParentSelectExitedCount, "No parent select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, levelB_statefulInteractableParent);

            Assert.AreEqual(1, level2_testInteractableChild.ParentSelectEnteredCount, "The parent select entered event should have occurred once.");
            Assert.AreEqual(1, level2_testInteractableChild.ParentSelectExitedCount, "The parent select exited event should have occurred once.");


            Destroy(levelA);
            level0 = null;
            level1 = null;
            level2 = null;

            yield return null;
        }

        private void CreateTestObjectsWithEventRouter()
        {
            interactorObject = new GameObject("Interactor");
            level0 = new GameObject("level 0");
            level1 = new GameObject("level 1");
            level2 = new GameObject("level 2");

            // Setup interactor
            interactorObject.AddComponent<XRController>();
            interactor = interactorObject.AddComponent<XRRayInteractor>();
            interactorObject.transform.SetParent(level0.transform, worldPositionStays: true);

            // Setup level 1
            level1_statefulInteractableParent = level1.AddComponent<StatefulInteractable>();
            level1_routerChildSource = level1.AddComponent<InteractableEventRouterChildSource>();
            level1_testInteractableParent = level1.AddComponent<TestInteractableParent>();
            level1.transform.SetParent(level0.transform, worldPositionStays: true);

            // Setup level 2
            level2_statefulInteractableChild = level2.AddComponent<StatefulInteractable>();
            level2_testInteractableChild = level2.AddComponent<TestInteractableChild>();
            level2.transform.SetParent(level1.transform, worldPositionStays: true);

            // Setup level 0
            level0_router = level0.AddComponent<InteractableEventRouter>();
            level0_router.AddEventRoute<BubbleChildHoverEvents>();
            level0_router.AddEventRoute<BubbleChildSelectEvents>();
            level0_router.AddEventRoute<TrickleChildHoverEvents>();
            level0_router.AddEventRoute<TrickleChildSelectEvents>();
        }
    }

    /// <summary>
    /// A test class for validating usages of <see cref="IXRHoverInteractableParent"/> and <see cref="IXRSelectInteractableParent"/>.
    /// </summary>
    internal class TestInteractableParent : MonoBehaviour, IXRHoverInteractableParent, IXRSelectInteractableParent
    {
        /// <summary>
        /// Get the number of times <see cref="OnChildHoverEntered"/> was called.
        /// </summary>
        public int ChildHoverEnteredCount { get; private set; } = 0;

        /// <summary>
        /// Get the number of times <see cref="OnChildHoverExited"/> was called.
        /// </summary>
        public int ChildHoverExitedCount { get; private set; } = 0;

        /// <summary>
        /// Get the number of times <see cref="OnChildSelectEntered"/> was called.
        /// </summary>
        public int ChildSelectEnteredCount { get; private set; } = 0;

        /// <summary>
        /// Get the number of times <see cref="OnChildSelectExited"/> was called.
        /// </summary>
        public int ChildSelectExitedCount { get; private set; } = 0;

        public void ResetCounts()
        {
            ChildHoverEnteredCount = 0;
            ChildHoverExitedCount = 0;
            ChildSelectEnteredCount = 0;
            ChildSelectExitedCount = 0;
        }

        /// <inheritdoc/>
        public void OnChildHoverEntered(HoverEnterEventArgs args)
        {
            ChildHoverEnteredCount++;
        }

        /// <inheritdoc/>
        public void OnChildHoverExited(HoverExitEventArgs args)
        {
            ChildHoverExitedCount++;
        }

        /// <inheritdoc/>
        public void OnChildSelectEntered(SelectEnterEventArgs args)
        {
            ChildSelectEnteredCount++;
        }

        /// <inheritdoc/>
        public void OnChildSelectExited(SelectExitEventArgs args)
        {
            ChildSelectExitedCount++;
        }
    }


    /// <summary>
    /// A test class for validating usages of <see cref="IXRHoverInteractableChild"/> and <see cref="IXRSelectInteractableChild"/>.
    /// </summary>
    internal class TestInteractableChild : MonoBehaviour, IXRHoverInteractableChild, IXRSelectInteractableChild
    {
        /// <summary>
        /// Get the number of times <see cref="OnParentHoverEntered"/> was called.
        /// </summary>
        public int ParentHoverEnteredCount { get; private set; } = 0;

        /// <summary>
        /// Get the number of times <see cref="OnParentHoverExited"/> was called.
        /// </summary>
        public int ParentHoverExitedCount { get; private set; } = 0;

        /// <summary>
        /// Get the number of times <see cref="OnParentSelectEntered"/> was called.
        /// </summary>
        public int ParentSelectEnteredCount { get; private set; } = 0;

        /// <summary>
        /// Get the number of times <see cref="OnParentSelectExited"/> was called.
        /// </summary>
        public int ParentSelectExitedCount { get; private set; } = 0;

        public void ResetCounts()
        {
            ParentHoverEnteredCount = 0;
            ParentHoverExitedCount = 0;
            ParentSelectEnteredCount = 0;
            ParentSelectExitedCount = 0;
        }

        /// <inheritdoc/>
        public void OnParentHoverEntered(HoverEnterEventArgs args)
        {
            ParentHoverEnteredCount++;
        }

        /// <inheritdoc/>
        public void OnParentHoverExited(HoverExitEventArgs args)
        {
            ParentHoverExitedCount++;
        }

        /// <inheritdoc/>
        public void OnParentSelectEntered(SelectEnterEventArgs args)
        {
            ParentSelectEnteredCount++;
        }

        /// <inheritdoc/>
        public void OnParentSelectExited(SelectExitEventArgs args)
        {
            ParentSelectExitedCount++;
        }
    }
}
