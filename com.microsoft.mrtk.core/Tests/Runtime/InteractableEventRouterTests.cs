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
        private GameObject root = null;
        private GameObject interactorObject = null;
        private GameObject level1 = null;
        private GameObject lever2 = null;

        private XRRayInteractor interactor = null;

        private InteractableEventRouter router = null;
        private InteractableEventRouterChildSource routerChildSource = null;

        private StatefulInteractable statefulInteractableParent = null;
        private TestInteractableParent testInteractableParent = null;

        private StatefulInteractable statefulInteractableChild = null;
        private TestInteractableChild testInteractableChild = null;

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
            if (root != null)
            {
                Object.Destroy(root);
            }

            root = null;
            interactorObject = null;
            interactor = null;
            cachedInteractionManager = null;
            level1 = null;
            lever2 = null;
            router = null;
            routerChildSource = null;
            statefulInteractableParent = null;
            testInteractableParent = null;
            statefulInteractableChild = null;
            testInteractableChild = null;
        }

        [UnityTest]
        public IEnumerator ComponentCreationTest()
        {
            Assert.IsNotNull(root, "Root game object should not be null");
            Assert.IsNotNull(interactorObject, "interactor game object should not be null");
            Assert.IsNotNull(level1, "Level 1 game object should not be null");
            Assert.IsNotNull(lever2, "Level 2 game object should not be null");
            Assert.IsNotNull(statefulInteractableParent, "The `StatusInteractable` parent should not be null.");
            Assert.IsNotNull(statefulInteractableChild, "The `StatusInteractable` child should not be null.");
            Assert.IsNotNull(testInteractableParent, "The `TestInteractableParent` should not be null.");
            Assert.IsNotNull(testInteractableChild, "The `TestInteractableChild` should not be null.");
            Assert.IsNotNull(routerChildSource, "The `InteractableEventRouterChildSource` should not be null.");
            Assert.IsNotNull(router, "The `InteractableEventRouter` should not be null.");
            Assert.IsNotNull(interactor, "The `XRRayInteractor` should not be null.");
            Assert.IsNotNull(CachedInteractionManager, "The `StatusInteractables` should have created an interaction manager.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsTest()
        {
            Assert.AreEqual(0, testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred yet.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(1, testInteractableParent.ChildSelectExitedCount, "The child select exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleHoverEventsTest()
        {
            Assert.AreEqual(0, testInteractableParent.ChildHoverEnteredCount, "No child hover entered events should have occurred yet.");
            Assert.AreEqual(0, testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverEnter((IXRHoverInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(0, testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverExit((IXRHoverInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(1, testInteractableParent.ChildHoverExitedCount, "The child hover exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TrickleSelectEventsTest()
        {
            Assert.AreEqual(0, testInteractableChild.ParentSelectEnteredCount, "No parent select entered events should have occurred yet.");
            Assert.AreEqual(0, testInteractableChild.ParentSelectExitedCount, "No parent select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, statefulInteractableParent);

            Assert.AreEqual(1, testInteractableChild.ParentSelectEnteredCount, "The parent select entered event should have occurred once.");
            Assert.AreEqual(0, testInteractableChild.ParentSelectExitedCount, "No parent select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, statefulInteractableParent);

            Assert.AreEqual(1, testInteractableChild.ParentSelectEnteredCount, "The parent select entered event should have occurred once.");
            Assert.AreEqual(1, testInteractableChild.ParentSelectExitedCount, "The parent select exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TrickleHoverEventsTest()
        {
            Assert.AreEqual(0, testInteractableChild.ParentHoverEnteredCount, "No parent hover entered events should have occurred yet.");
            Assert.AreEqual(0, testInteractableChild.ParentHoverExitedCount, "No parent hover exited events should have occurred yet.");

            CachedInteractionManager.HoverEnter((IXRHoverInteractor)interactor, statefulInteractableParent);

            Assert.AreEqual(1, testInteractableChild.ParentHoverEnteredCount, "The parent hover entered event should have occurred once.");
            Assert.AreEqual(0, testInteractableChild.ParentHoverExitedCount, "No parent hover exited events should have occurred yet.");

            CachedInteractionManager.HoverExit((IXRHoverInteractor)interactor, statefulInteractableParent);

            Assert.AreEqual(1, testInteractableChild.ParentHoverEnteredCount, "The parent hover entered event should have occurred once.");
            Assert.AreEqual(1, testInteractableChild.ParentHoverExitedCount, "The parent hover exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsFromNewlyAddedChildTest()
        {
            var newChild = new GameObject("new level 2 child");
            var newStatefulInteractableChild = newChild.AddComponent<StatefulInteractable>();
            newChild.transform.SetParent(level1.transform, worldPositionStays: true);

            Assert.AreEqual(0, testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred yet.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(1, testInteractableParent.ChildSelectExitedCount, "The child select exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleHoverEventsFromNewlyAddedChildTest()
        {
            var newChild = new GameObject("new level 2 child");
            var newStatefulInteractableChild = newChild.AddComponent<StatefulInteractable>();
            newChild.transform.SetParent(level1.transform, worldPositionStays: true);

            Assert.AreEqual(0, testInteractableParent.ChildHoverEnteredCount, "No child hover entered events should have occurred yet.");
            Assert.AreEqual(0, testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverEnter((IXRHoverInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(0, testInteractableParent.ChildHoverExitedCount, "No child hover exited events should have occurred yet.");

            CachedInteractionManager.HoverExit((IXRHoverInteractor)interactor, newStatefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildHoverEnteredCount, "The child hover entered event should have occurred once.");
            Assert.AreEqual(1, testInteractableParent.ChildHoverExitedCount, "The child hover exited event should have occurred once.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsDisabledAfterRemovalOrSelectEventRouteTest()
        {
            router.RemoveEventRoute<BubbleChildSelectEvents>();

            Assert.AreEqual(0, testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(0, testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred, since `BubbleChildSelectEvents` was removed.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(0, testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred, since `BubbleChildSelectEvents` was removed.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred, since `BubbleChildSelectEvents` was removed.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BubbleSelectEventsDisabledAfterRemovalOfHoverEventRouteTest()
        {
            router.RemoveEventRoute<BubbleChildHoverEvents>();

            Assert.AreEqual(0, testInteractableParent.ChildSelectEnteredCount, "No child select entered events should have occurred yet.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectEnter((IXRSelectInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(0, testInteractableParent.ChildSelectExitedCount, "No child select exited events should have occurred yet.");

            CachedInteractionManager.SelectExit((IXRSelectInteractor)interactor, statefulInteractableChild);

            Assert.AreEqual(1, testInteractableParent.ChildSelectEnteredCount, "The child select entered event should have occurred once.");
            Assert.AreEqual(1, testInteractableParent.ChildSelectExitedCount, "The child select exited event should have occurred once.");

            yield return null;
        }

        private void CreateTestObjectsWithEventRouter()
        {
            root = new GameObject("root");
            interactorObject = new GameObject("Interactor");
            level1 = new GameObject("level 1");
            lever2 = new GameObject("level 2");

            // Setup interactor
            interactorObject.AddComponent<XRController>();
            interactor = interactorObject.AddComponent<XRRayInteractor>();
            interactorObject.transform.SetParent(root.transform, worldPositionStays: true);

            // Setup first level
            statefulInteractableParent = level1.AddComponent<StatefulInteractable>();
            routerChildSource = level1.AddComponent<InteractableEventRouterChildSource>();
            testInteractableParent = level1.AddComponent<TestInteractableParent>();
            level1.transform.SetParent(root.transform, worldPositionStays: true);

            // Setup second level
            statefulInteractableChild = lever2.AddComponent<StatefulInteractable>();
            testInteractableChild = lever2.AddComponent<TestInteractableChild>();
            lever2.transform.SetParent(level1.transform, worldPositionStays: true);

            // Setup router
            router = root.AddComponent<InteractableEventRouter>();
            router.AddEventRoute<BubbleChildHoverEvents>();
            router.AddEventRoute<BubbleChildSelectEvents>();
            router.AddEventRoute<TrickleChildHoverEvents>();
            router.AddEventRoute<TrickleChildSelectEvents>();
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
