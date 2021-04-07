// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ScrollViewTests : BasePlayModeTests
    {
        private Material mrtkMaterial = new Material(StandardShaderUtility.MrtkStandardShader);

        // Assets/MRTK/SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2_32x96_NoLabel.prefab
        private const string PressableHololens2_32x96_PrefabGuid = "eb36a4319b6be77409716f5a41e6da51";

        // Assets/MRTK/SDK/Features/UX/Interactable/Prefabs/PressableButtonHoloLens2_NoLabel.prefab
        private const string PressableHololens2PrefabGuid = "b20573eb9bf8a914882fa4a571d2e8dc";

        [UnitySetUp]
        public override IEnumerator Setup()
        {
            yield return base.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;
        }

        #region Tests

        /// <summary>
        /// Tests if near interaction with a pressable button item is reset after the user engages in a scroll drag.
        /// User should be able to interact with other buttons right after scroll engage is finished.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsNearInteractionWithChildren()
        {
            // Setting up a vertical 1x2 scroll view with three pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 3);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         2,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();
            PressableButton button2Component = contentItems[1].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);
            Assert.IsNotNull(button2Component);

            bool button1TouchBegin = false;
            button1Component.TouchBegin.AddListener(() =>
            {
                button1TouchBegin = true;
            });

            bool button1TouchEnd = false;
            button1Component.TouchEnd.AddListener(() =>
            {
                button1TouchEnd = true;
            });

            bool button1PressBegin = false;
            button1Component.ButtonPressed.AddListener(() =>
            {
                button1PressBegin = true;
            });

            bool button1PressCompleted = false;
            button1Component.ButtonReleased.AddListener(() =>
            {
                button1PressCompleted = true;
            });

            bool button2TouchBegin = false;
            button2Component.TouchBegin.AddListener(() =>
            {
                button2TouchBegin = true;
            });

            bool button2TouchEnd = false;
            button2Component.TouchEnd.AddListener(() =>
            {
                button2TouchEnd = true;
            });

            bool button2PressBegin = false;
            button2Component.ButtonPressed.AddListener(() =>
            {
                button2PressBegin = true;
            });

            bool button2PressCompleted = false;
            button2Component.ButtonReleased.AddListener(() =>
            {
                button2PressCompleted = true;
            });

            bool scrollDragBegin = false;
            scrollView.OnMomentumStarted.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonTouchPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset);

            // Interaction with child button should behave normally if scroll drag not yet engaged
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(initialPos);

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.IsTrue(button1TouchBegin, "Button1 touch begin did not trigger.");
            Assert.IsTrue(button1PressBegin, "Button1 press begin did not trigger.");
            Assert.IsTrue(button1PressCompleted, "Button1 press release did not trigger.");
            Assert.IsTrue(button1TouchEnd, "Button1 touch end did not trigger.");

            scrollDragBegin = false;
            button1TouchBegin = false;
            button1PressBegin = false;
            button1PressCompleted = false;
            button1TouchEnd = false;

            // Scroll drag engage should halt interaction with child button                
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);

            Assert.IsTrue(button1TouchBegin, "Button1 touch begin did not trigger.");
            Assert.IsTrue(button1PressBegin, "Button1 press begin did not trigger.");

            yield return hand.MoveTo(scrollEngagedPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin did not trigger.");
            Assert.IsTrue(button1TouchEnd, "Button1 touch end did not trigger.");
            Assert.IsTrue(button1PressCompleted, "Button1 press release did not trigger.");

            yield return hand.MoveTo(initialPos);

            Assert.IsFalse(scrollView.IsEngaged);

            // Interaction with other children buttons should behave normally after scroll drag engage is finished
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(initialPos);

            Assert.IsTrue(button2TouchBegin, "Button2 touch begin did not trigger.");
            Assert.IsTrue(button2PressBegin, "Button2 press begin did not trigger.");
            Assert.IsTrue(button2PressCompleted, "Button2 press release did not trigger.");
            Assert.IsTrue(button2TouchEnd, "Button2 touch end did not trigger.");
        }

        /// <summary>
        /// Tests if far interaction with a pressable button item is reset after the user engages in a scroll drag.
        /// User should be able to interact with other buttons right after scroll engage is finished.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsFarInteractionWithChildren()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            float scale = 10f;
            scrollView.transform.localScale *= scale;

            Interactable interactable1 = contentItems[0].GetComponent<Interactable>();
            Interactable interactable2 = contentItems[1].GetComponent<Interactable>();

            Assert.IsNotNull(interactable1);
            Assert.IsNotNull(interactable2);

            bool scrollDragBegin = false;
            scrollView.OnMomentumStarted.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = new Vector3(0.13f, -0.17f, 0.5f); // Far pointer focus is on button       
            Vector3 scrollEngagedPos = initialPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight * scale + offset);

            // Interaction with child button should behave normally if scroll drag not yet engaged
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.IsTrue(interactable1.HasFocus, "Interactable1 does not have far pointer focus.");
            Assert.IsTrue(interactable1.HasPress, "Interactable1 did not get press from far interaction.");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered");
            Assert.IsTrue(interactable1.HasFocus, "Interactable1 does not have far pointer focus.");
            Assert.IsFalse(interactable1.HasPress, "Interactable1 still have press from far interaction.");

            // Scroll drag engage should halt interaction with child button 
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.IsTrue(interactable1.HasFocus, "Interactable1 does not have far pointer focus.");
            Assert.IsTrue(interactable1.HasPress, "Interactable1 did not get press from far interaction.");

            yield return hand.MoveTo(scrollEngagedPos);
            yield return new WaitForSeconds(interactable1.RollOffTime); // Wait for interactable has press roll off

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was not triggered");
            Assert.IsFalse(interactable1.HasFocus, "Interactable1 still have far pointer focus.");
            Assert.IsFalse(interactable1.HasPress, "Interactable1 still have press from far interaction.");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Assert.IsFalse(scrollView.IsEngaged);

            // Interaction with other children buttons should behave normally after scroll drag engage is finished
            yield return hand.MoveTo(initialPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            Assert.IsTrue(interactable2.HasFocus, "Interactable2 does not have far pointer focus.");
            Assert.IsTrue(interactable2.HasPress, "Interactable2 did not get press from far interaction.");
        }

        /// <summary>
        /// Tests if interaction with a pressable button child triggers an undesired jump or scroll drag.
        /// </summary>
        [UnityTest]
        public IEnumerator NoJumpsWhenInteractingWithChildren()
        {
            // Setting up a vertical 1x2 scroll view with three pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 3);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         2,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            float scale = 10f;
            scrollView.transform.localScale *= scale;

            bool scrollDragBegin = false;
            scrollView.OnMomentumStarted.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = new Vector3(0.13f, -0.17f, 0.5f); // Far pointer focus is on button       
            Vector3 scrollEngagedPos = initialPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset);

            // Interaction with child button should behave normally if scroll drag not yet engaged
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForSeconds(1.0f); // Waiting for possible timed drag trigger

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.AreEqual(0, scrollView.ScrollContainerPosition.y, "Scroll container has moved.");
        }

        /// <summary>
        /// Tests scroll drag engage by interacting with the background empty space of a scroll view 
        /// </summary>
        [UnityTest]
        public IEnumerator InteractionWithBackgroundEmptySpace()
        {
            // Setting up a vertical 2x1 scroll view with three pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 3);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                2,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(2,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            bool scrollDragBegin = false;
            scrollView.OnMomentumStarted.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 scrollTouchPos = contentItems[1].transform.position + Vector3.forward * 0.015f; // Touching scroll second column slot        
            Vector3 scrollEngagedUpPos = scrollTouchPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset); // Scrolls up one row
            Vector3 scrollEngagedDownPos = scrollTouchPos - Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset); // Scrolls down one row

            // Scrolls up from button to reveal second row with empty slot 
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(scrollTouchPos);
            yield return hand.MoveTo(scrollEngagedUpPos);
            yield return hand.MoveTo(initialPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.AreEqual(scrollView.ScrollContainerPosition.y, scrollView.CellHeight, 0.001, "Scroll container has not moved to second row.");

            // Reset drag and scrolls down from empty space
            scrollDragBegin = false;

            yield return hand.MoveTo(scrollTouchPos);
            yield return hand.MoveTo(scrollEngagedDownPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.AreEqual(scrollView.ScrollContainerPosition.y, 0, 0.001, "Scroll container has not moved to first row.");
        }

        /// <summary>
        /// Tests if adding or deleting children items while scroll is engaged in a drag work as expected. 
        /// </summary>
        [UnityTest]
        public IEnumerator ChildrenCanBeAddedAndDeleted()
        {
            // Setting up a vertical 1x1 scroll view with three pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 3);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            // This button will be added later to the scroll collection
            GameObject button4 = InstantiatePrefab(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid));

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();
            PressableButton button3Component = contentItems[2].GetComponentInChildren<PressableButton>();
            PressableButton button4Component = button4.GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);
            Assert.IsNotNull(button3Component);
            Assert.IsNotNull(button4Component);

            bool scrollDragBegin = false;
            scrollView.OnMomentumStarted.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            bool button1TouchBegin = false;
            button1Component.TouchBegin.AddListener(() =>
            {
                button1TouchBegin = true;
            });

            bool button3TouchBegin = false;
            button3Component.TouchBegin.AddListener(() =>
            {
                button3TouchBegin = true;
            });

            bool button4TouchBegin = false;
            button4Component.TouchBegin.AddListener(() =>
            {
                button4TouchBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonTouchPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedHalfPageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight / 2 + offset);
            Vector3 scrollEngagedOnePageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset);
            Vector3 scrollEngagedTwoPageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight * 2 + offset);

            // Scrolling half of the row width
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was not triggered.");
            Assert.IsTrue(button1TouchBegin, "Button1 touch begin was not triggered.");
            Assert.IsFalse(button3TouchBegin, "Button3 touch begin was triggered.");
            Assert.IsFalse(button4TouchBegin, "Button4 touch begin was triggered.");

            // Removing scroll item from collection while scroll is engaged
            scrollView.RemoveItem(contentItems[1]);
            GameObject.Destroy(contentItems[1]);
            objectCollection.UpdateCollection();

            // Scrolling to second row
            scrollDragBegin = false;
            button1TouchBegin = false;
            button3TouchBegin = false;
            button4TouchBegin = false;

            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedOnePageUpPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was not triggered."); // both colliders disabled need to go deeper
            Assert.IsTrue(button1TouchBegin, "Button1 touch begin was not triggered.");
            Assert.IsFalse(button3TouchBegin, "Button3 touch begin was triggered.");
            Assert.IsFalse(button4TouchBegin, "Button4 touch begin was triggered.");

            scrollDragBegin = false;
            button1TouchBegin = false;
            button3TouchBegin = false;
            button4TouchBegin = false;

            // Button 3 should be visible and interaction should allow scroll drag
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was not triggered.");
            Assert.IsFalse(button1TouchBegin, "Button1 touch begin was triggered.");
            Assert.IsTrue(button3TouchBegin, "Button3 touch begin was not triggered.");
            Assert.IsFalse(button4TouchBegin, "Button4 touch begin was triggered.");

            // Adding scroll item while scroll is engaged
            button4.transform.parent = objectCollection.transform;
            objectCollection.UpdateCollection();
            scrollView.Reset();

            // Scrolling to third row
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedTwoPageUpPos);

            scrollDragBegin = false;
            button1TouchBegin = false;
            button3TouchBegin = false;
            button4TouchBegin = false;

            // Button 4 should be visible and interaction should allow scroll drag
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedTwoPageUpPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was not triggered.");
            Assert.IsFalse(button1TouchBegin, "Button1 touch begin was triggered.");
            Assert.IsFalse(button3TouchBegin, "Button3 touch begin was triggered.");
            Assert.IsTrue(button4TouchBegin, "Button4 touch begin was not triggered.");
        }

        /// <summary>
        /// Tests if scroll engage triggered by a near interaction is reset if pointer crosses outside boundaries threshold.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsWhenOutOfBoundaryThreshold()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.002f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonTouchPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedHalfPageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight / 2 + offset);
            Vector3 scrollEngagedInsideTopBound = pastButtonPressPos + Vector3.up * (scrollView.CellHeight / 2 + scrollView.ReleaseThresholdTopBottom - offset);
            Vector3 scrollEngagedOutsideTopBound = pastButtonPressPos + Vector3.up * (scrollView.CellHeight / 2 + scrollView.ReleaseThresholdTopBottom + offset);
            Vector3 scrollEngagedInsideBottomBound = pastButtonPressPos - Vector3.up * (scrollView.CellHeight / 2 + scrollView.ReleaseThresholdTopBottom - offset);
            Vector3 scrollEngagedOutsideBottomBound = pastButtonPressPos - Vector3.up * (scrollView.CellHeight / 2 + scrollView.ReleaseThresholdTopBottom + offset);
            Vector3 scrollEngagedInsideRightBound = pastButtonPressPos + Vector3.right * (scrollView.CellWidth / 2 + scrollView.ReleaseThresholdLeftRight - offset);
            Vector3 scrollEngagedOutsideRightBound = pastButtonPressPos + Vector3.right * (scrollView.CellWidth / 2 + scrollView.ReleaseThresholdLeftRight + offset);
            Vector3 scrollEngagedInsideLeftBound = pastButtonPressPos - Vector3.right * (scrollView.CellWidth / 2 + scrollView.ReleaseThresholdLeftRight - offset);
            Vector3 scrollEngagedOutsideLeftBound = pastButtonPressPos - Vector3.right * (scrollView.CellWidth / 2 + scrollView.ReleaseThresholdLeftRight + offset);
            Vector3 scrollEngagedInsideBackBound = pastButtonPressPos + Vector3.forward * (scrollView.CellHeight / 4 + scrollView.ReleaseThresholdBack - offset);
            Vector3 scrollEngagedOutsideBackBound = pastButtonPressPos + Vector3.forward * (scrollView.CellHeight / 4 + scrollView.ReleaseThresholdBack + offset);
            Vector3 scrollEngagedInsideFrontBound = pastButtonPressPos - Vector3.forward * (scrollView.CellHeight / 4 + scrollView.ReleaseThresholdFront - offset);
            Vector3 scrollEngagedOutsideFrontBound = pastButtonPressPos - Vector3.forward * (scrollView.CellHeight / 4 + scrollView.ReleaseThresholdFront + offset);

            // Moving hand outside top boundary should halt scroll drag engagement
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedInsideTopBound);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");
            Assert.IsTrue(scrollView.IsEngaged, "Scroll view is not engaged.");

            yield return hand.MoveTo(scrollEngagedOutsideTopBound);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand outside bottom boundary should halt scroll drag engagement            
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedInsideBottomBound);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");
            Assert.IsTrue(scrollView.IsEngaged, "Scroll view is not engaged.");

            yield return hand.MoveTo(scrollEngagedOutsideBottomBound);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand outside left boundary should halt scroll drag engagement
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);
            yield return hand.MoveTo(scrollEngagedInsideLeftBound);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");
            Assert.IsTrue(scrollView.IsEngaged, "Scroll view is not engaged.");

            yield return hand.MoveTo(scrollEngagedOutsideLeftBound);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand outside right boundary should halt scroll drag engagement
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);
            yield return hand.MoveTo(scrollEngagedInsideRightBound);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");
            Assert.IsTrue(scrollView.IsEngaged, "Scroll view is not engaged.");

            yield return hand.MoveTo(scrollEngagedOutsideRightBound);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand outside front boundary should halt scroll drag engagement
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);
            yield return hand.MoveTo(scrollEngagedInsideFrontBound);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");
            Assert.IsTrue(scrollView.IsEngaged, "Scroll view is not engaged.");

            yield return hand.MoveTo(scrollEngagedOutsideFrontBound);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand outside back boundary should halt scroll drag engagement
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);
            yield return hand.MoveTo(scrollEngagedInsideBackBound);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");
            Assert.IsTrue(scrollView.IsEngaged, "Scroll view is not engaged.");

            yield return hand.MoveTo(scrollEngagedOutsideBackBound);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");
        }

        /// <summary>
        /// Tests if scroll engage is only triggered by a near interaction if pointer comes from the front plane.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageOnlyFromFrontInteraction()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity,
                                                                         ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                         ScrollingObjectCollection.VelocityType.FalloffPerItem);
            scrollView.AddContent(objectCollection.gameObject);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.002f;
            Vector3 initialPos = Vector3.zero;
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 offFrontPos = pastButtonPressPos - Vector3.forward * scrollView.CellHeight;
            Vector3 offBackPos = pastButtonPressPos + Vector3.forward * scrollView.CellHeight;
            Vector3 offBottomPos = pastButtonPressPos - Vector3.up * scrollView.CellHeight;
            Vector3 offTopPos = pastButtonPressPos + Vector3.up * scrollView.CellHeight;
            Vector3 offRightPos = pastButtonPressPos + Vector3.right * scrollView.CellHeight;
            Vector3 offLeftPos = pastButtonPressPos - Vector3.right * scrollView.CellHeight;

            // Moving hand from outside top boundary should not trigger drag engagement
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(offTopPos);
            yield return hand.MoveTo(offBottomPos);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand from outside bottom boundary should not trigger drag engagement
            yield return hand.MoveTo(offTopPos);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand from outside right boundary should not trigger drag engagement
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(offRightPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(offTopPos);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand from outside left boundary should not trigger drag engagement
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(offLeftPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(offTopPos);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand from outside back boundary should not trigger drag engagement
            yield return hand.MoveTo(offBackPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(offTopPos);

            Assert.IsFalse(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsFalse(scrollView.IsEngaged, "Scroll view is engaged.");

            // Moving hand from outside front boundary should trigger drag engagement
            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(offFrontPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(offTopPos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is being dragged.");
            Assert.IsTrue(scrollView.IsEngaged, "Scroll view is engaged.");
        }

        /// <summary>
        /// Tests if updating the collection after scaling the scroll object does not alter clipping box local scale.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollViewCanBeScaled()
        {
            // Setting up a vertical 1x1 scroll view with one single pressable button item
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 1);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity,
                                                                         ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                         ScrollingObjectCollection.VelocityType.FalloffPerItem);
            scrollView.AddContent(objectCollection.gameObject);

            GameObject ClippingObject = scrollView.ClippingObject;

            // Clipping box dimensions should match the unique scroll item dimensions
            Assert.IsNotNull(ClippingObject);
            Assert.AreEqual(ClippingObject.transform.localScale.x, scrollView.CellWidth, 0.001, "Clipping box width did not scale as expected");
            Assert.AreEqual(ClippingObject.transform.localScale.y, scrollView.CellHeight, 0.001, "Clipping box height did not scale as expected");
            Assert.AreEqual(ClippingObject.transform.localScale.z, scrollView.CellWidth / 2, 0.001, "Clipping box depth did not scale as expected");

            // Doubling the scroll object scale should not alter the clipping box local scale
            float newScrollScale = 2.0f;
            scrollView.transform.localScale *= newScrollScale;

            yield return null;
            scrollView.UpdateContent();
            yield return null;

            Assert.AreEqual(ClippingObject.transform.localScale.x, scrollView.CellWidth, 0.001, "Clipping box width did not scale as expected");
            Assert.AreEqual(ClippingObject.transform.localScale.y, scrollView.CellHeight, 0.001, "Clipping box height did not scale as expected");
            Assert.AreEqual(ClippingObject.transform.localScale.z, scrollView.CellWidth / 2, 0.001, "Clipping box depth did not scale as expected");
        }

        /// <summary>
        /// Tests if scroll behaves as expected if scroll object is rotated.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollViewCanbeRotated()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.up * -1.0f,
                                                                                Quaternion.LookRotation(-Vector3.up),
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.up * -1.0f,
                                                                         Quaternion.LookRotation(-Vector3.up),
                                                                         ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                         ScrollingObjectCollection.VelocityType.FalloffPerItem);
            scrollView.AddContent(objectCollection.gameObject);

            // Setting up camera to look down
            MixedRealityPlayspace.Position = Vector3.zero;
            MixedRealityPlayspace.Rotation = Quaternion.LookRotation(-Vector3.up);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonPressPos = button1Component.transform.position - new Vector3(0, button1Component.StartPushDistance - offset, 0);
            Vector3 pastButtonPressPos = button1Component.transform.position - new Vector3(0, button1Component.PressDistance + offset, 0);
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.forward * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset);

            // Moving hand along z axis should still engage an up-down scroll view rotated 90 degrees around x
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonPressPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedPos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");

            yield return hand.Show(initialPos);

            Assert.AreEqual(0.032f, scrollView.ScrollContainerPosition.y, 0.0005, "Scroll container should be on second tier");
        }

        /// <summary>
        /// Tests if scroll can be moved by page, by tier or to make specific element to be presented in the first visible tier.
        /// </summary>
        [UnityTest]
        public IEnumerator CanBeScrolledByTierOrIndexOrPage()
        {
            // Setting up a horizontal 2x2 scroll view with nine pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 9);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.RowThenColumn,
                                                                                LayoutAnchor.UpperLeft,
                                                                                2,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(2,
                                                                         2,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity,
                                                                         ScrollingObjectCollection.ScrollDirectionType.LeftAndRight,
                                                                         ScrollingObjectCollection.VelocityType.FalloffPerItem);
            scrollView.AddContent(objectCollection.gameObject);

            // Initial scroll state
            Assert.AreEqual(0, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving to second tier
            scrollView.MoveByTiers(1, false);
            yield return null;

            Assert.AreEqual(2, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(6, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving to fourth tier
            scrollView.MoveByTiers(2, false);
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Scroll container should not move beyond its min position
            scrollView.MoveByTiers(1, false);
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving back to first tier
            scrollView.MoveByTiers(-4, false);
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving one page to third tier
            scrollView.MoveByPages(1, false);
            yield return null;

            Assert.AreEqual(4, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(8, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving half page to fourth tier as scroll container hits its min position
            scrollView.MoveByPages(1, false);
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving one page and a half back to first tier
            scrollView.MoveByPages(-2, false);
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Second item is already in first visible tier
            scrollView.MoveToIndex(1, false);
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving to second tier making fourth element to be on first visible tier
            scrollView.MoveToIndex(3, false);
            yield return null;

            Assert.AreEqual(2, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(6, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving to fourth tier as scroll container hits its min position
            scrollView.MoveToIndex(8, false); // should move to 6 as max
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");

            // Moving to back to first tier. Negative argument should not cause any errors
            scrollView.MoveToIndex(-1, false);
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleCellIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenCellIndex, "First hidden item is different from the expected");
        }

        /// <summary>
        /// Tests if far interaction with GGV pointer can engage the scroll drag.
        /// </summary>
        [UnityTest]
        public IEnumerator GGVScroll()
        {
            PlayModeTestUtilities.SetControllerSimulationMode(ControllerSimulationMode.HandGestures);

            // Setting up a horizontal 1x1 scroll view with two pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            float scale = 10f;
            scrollView.transform.localScale *= scale;

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = new Vector3(0.13f, -0.17f, 0.5f); // Far pointer focus is on button       
            Vector3 scrollEngagedPos = initialPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight * scale + offset);

            // Interaction with child button should behave normally if scroll drag not yet engaged
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(scrollEngagedPos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll drag was not triggered.");
        }

        /// <summary>
        /// Tests if it is possible to ensure that children click only happens on touch up by changing children configuration.
        /// </summary>
        [UnityTest]
        public IEnumerator ContentClickHappensOnTouchUp()
        {
            // Setting up a horizontal 1x1 scroll view with two pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();
            button1Component.ReleaseOnTouchEnd = false;

            bool button1PressCompleted = false;
            button1Component.ButtonReleased.AddListener(() =>
            {
                button1PressCompleted = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonTouchPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 pastButtonReleasePos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance - button1Component.ReleaseDistanceDelta - offset);
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset);

            // Button click is not completed without passing release plane or if scroll view is engaged in a drag
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedPos);
            yield return hand.MoveTo(pastButtonReleasePos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll drag begin was not triggered.");
            Assert.IsFalse(button1PressCompleted, "Button1 press release was triggered.");

            // Button click is only completed if passing release plane or if scroll view is not engaged in a drag
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(pastButtonReleasePos);

            Assert.IsFalse(scrollView.IsDragging, "Scroll drag was triggered.");
            Assert.IsTrue(button1PressCompleted, "Button1 press release was not triggered.");
        }

        /// <summary>
        /// Tests correct clipping logic for visible, partially visible and hidden content with default performance settings (DisableClippedGameobject set to true).
        /// Ensures best performance of the clipping primitive component by disabling gameobjects that are hidden.
        /// </summary>
        [UnityTest]
        public IEnumerator ClipOnlyVisibleContent()
        {
            // Setting up a vertical 1x1 scroll view with three sphere primitive items
            float sphereItemScale = 0.032f;
            var contentItems = InstantiatePrimitiveItems(PrimitiveType.Sphere, 3, sphereItemScale);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                sphereItemScale,
                                                                                sphereItemScale);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         sphereItemScale,
                                                                         Vector3.forward,
                                                                         Quaternion.identity,
                                                                         ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                         ScrollingObjectCollection.VelocityType.FalloffPerFrame);
            scrollView.AddContent(objectCollection.gameObject);

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preTouchPos = contentItems[0].transform.position + new Vector3(0, 0, sphereItemScale / 2 - offset);
            Vector3 pastTouchPos = preTouchPos + new Vector3(0, 0, offset);
            Vector3 scrollEngagedHalfPageUpPos = pastTouchPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight / 2 + offset);

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);

            List<Renderer> clippedRenderers = scrollView.ClipBox.GetRenderersCopy().ToList();

            var renderer0 = contentItems[0].GetComponent<Renderer>();
            var renderer1 = contentItems[1].GetComponent<Renderer>();
            var renderer2 = contentItems[2].GetComponent<Renderer>();

            var collider0 = contentItems[0].GetComponent<Collider>();
            var collider1 = contentItems[1].GetComponent<Collider>();
            var collider2 = contentItems[2].GetComponent<Collider>();

            // Completely visible objects should be active and have renderers clipped. Colliders should be enabled for interaction
            Assert.IsTrue(clippedRenderers.Contains(renderer0), "Renderer 0 is not being clipped");
            Assert.IsTrue(contentItems[0].activeSelf, "Sphere 0 is not active");
            Assert.IsTrue(collider0.enabled, "Collider 0 is disabled");

            // Barely visible objects should be active and have renderers clipped. Colliders should be disabled for interaction
            Assert.IsTrue(clippedRenderers.Contains(renderer1), "Renderer 1 is not being clipped");
            Assert.IsTrue(contentItems[1].activeSelf, "Sphere 1 is not active");
            Assert.IsFalse(collider1.enabled, "Collider 1 is enabled");

            // Hidden objects should be inactive and have renderers clipped. Collider state not important if scroll is not drag engaged
            Assert.IsTrue(clippedRenderers.Contains(renderer2), "Renderer 2 is not being clipped");
            Assert.IsFalse(contentItems[2].activeSelf, "Sphere 2 is active");

            // Scrolling half item up
            yield return hand.MoveTo(preTouchPos);
            yield return hand.MoveTo(pastTouchPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);
            yield return hand.MoveTo(initialPos);

            clippedRenderers = scrollView.ClipBox.GetRenderersCopy().ToList();

            // Partially visible objects should be active and have renderers clipped. Colliders should be disabled for interaction
            Assert.IsTrue(clippedRenderers.Contains(renderer0), "Renderer 0 is not being clipped");
            Assert.IsTrue(contentItems[0].activeSelf, "Sphere 0 is not active");
            Assert.IsFalse(collider0.enabled, "Collider 0 is enabled");

            Assert.IsTrue(clippedRenderers.Contains(renderer1), "Renderer 1 is not being clipped");
            Assert.IsTrue(contentItems[1].activeSelf, "Sphere 1 is not active");
            Assert.IsFalse(collider1.enabled, "Collider 1 is enabled");

            // Hidden objects should be inactive and have renderers clipped. Collider state not important if scroll is not drag engaged
            Assert.IsTrue(clippedRenderers.Contains(renderer2), "Renderer 2 is not being clipped");
            Assert.IsFalse(contentItems[2].activeSelf, "Sphere 2 is active");

            // Removing content from scroll content should also remove its renderers from the scroll clipping box
            scrollView.RemoveItem(contentItems[0]);
            clippedRenderers = scrollView.ClipBox.GetRenderersCopy().ToList();

            // Object is still visible but renderer should not be clipped
            Assert.IsFalse(clippedRenderers.Contains(renderer0), "Renderer 0 is being clipped");
        }

        /// <summary>
        /// Tests correct clipping logic for visible, partially visible and hidden content with DisableClippedRenderer set to true.
        /// Ensures best performance of the clipping primitive component by disabling renderers that are hidden, while not affecting the logic of other scripts attached to those objects.
        /// </summary>
        [UnityTest]
        public IEnumerator ClipOnlyVisibleRenderers()
        {
            // Setting up a vertical 1x1 scroll view with three sphere primitive items
            float sphereItemScale = 0.032f;
            var contentItems = InstantiatePrimitiveItems(PrimitiveType.Sphere, 3, sphereItemScale);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                sphereItemScale,
                                                                                sphereItemScale);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         sphereItemScale,
                                                                         Vector3.forward,
                                                                         Quaternion.identity,
                                                                         ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                         ScrollingObjectCollection.VelocityType.FalloffPerFrame);
            scrollView.DisableClippedGameObjects = false;
            scrollView.DisableClippedRenderers = true;

            scrollView.AddContent(objectCollection.gameObject);

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preTouchPos = contentItems[0].transform.position + new Vector3(0, 0, sphereItemScale / 2 - offset);
            Vector3 pastTouchPos = preTouchPos + new Vector3(0, 0, offset);
            Vector3 scrollEngagedHalfPageUpPos = pastTouchPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight / 2 + offset);

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);

            List<Renderer> clippedRenderers = scrollView.ClipBox.GetRenderersCopy().ToList();

            var renderer0 = contentItems[0].GetComponent<Renderer>();
            var renderer1 = contentItems[1].GetComponent<Renderer>();
            var renderer2 = contentItems[2].GetComponent<Renderer>();

            var collider0 = contentItems[0].GetComponent<Collider>();
            var collider1 = contentItems[1].GetComponent<Collider>();
            var collider2 = contentItems[2].GetComponent<Collider>();

            // Completely visible objects should have renderers enabled and have renderers clipped. Colliders should be enabled for interaction
            Assert.IsTrue(clippedRenderers.Contains(renderer0), "Renderer 0 is not being clipped");
            Assert.IsTrue(contentItems[0].activeSelf, "Sphere 0 is not active");
            Assert.IsTrue(renderer0.enabled, "Renderer 0 is disabled");
            Assert.IsTrue(collider0.enabled, "Collider 0 is disabled");

            // Barely visible content should still have renderers enabled and have renderers clipped. Colliders should be disabled for interaction
            Assert.IsTrue(clippedRenderers.Contains(renderer1), "Renderer 1 is not being clipped");
            Assert.IsTrue(contentItems[1].activeSelf, "Sphere 1 is not active");
            Assert.IsTrue(renderer1.enabled, "Renderer 1 is disabled");
            Assert.IsFalse(collider1.enabled, "Collider 1 is enabled");

            // Hidden content should have renderers disabled and have renderers clipped. Collider state not important if scroll is not drag engaged
            Assert.IsTrue(clippedRenderers.Contains(renderer2), "Renderer 2 is not being clipped");
            Assert.IsTrue(contentItems[2].activeSelf, "Sphere 2 is not active");
            Assert.IsFalse(renderer2.enabled, "Renderer 2 is enabled");
            Assert.IsFalse(collider2.enabled, "Collider 2 is enabled");

            // Scrolling half item up
            yield return hand.MoveTo(preTouchPos);
            yield return hand.MoveTo(pastTouchPos);
            yield return hand.MoveTo(scrollEngagedHalfPageUpPos);
            yield return hand.MoveTo(initialPos);

            clippedRenderers = scrollView.ClipBox.GetRenderersCopy().ToList();

            // Partially visible objects should have renderers enabled  and have renderers clipped. Colliders should be disabled for interaction
            Assert.IsTrue(clippedRenderers.Contains(renderer0), "Renderer 0 is not being clipped");
            Assert.IsTrue(contentItems[0].activeSelf, "Sphere 0 is not active");
            Assert.IsTrue(renderer0.enabled, "Renderer 0 is disabled");
            Assert.IsFalse(collider0.enabled, "Collider 0 is enabled");

            Assert.IsTrue(clippedRenderers.Contains(renderer1), "Renderer 1 is not being clipped");
            Assert.IsTrue(contentItems[0].activeSelf, "Sphere 1 is not active");
            Assert.IsTrue(renderer1.enabled, "Renderer 1 is disabled");
            Assert.IsFalse(collider1.enabled, "Collider 1 is enabled");

            // Hidden objects should have renderers disabled and have renderers clipped. Collider state not important if scroll is not drag engaged
            Assert.IsTrue(clippedRenderers.Contains(renderer2), "Renderer 2 is not being clipped");
            Assert.IsTrue(contentItems[0].activeSelf, "Sphere 2 is not active");
            Assert.IsFalse(renderer2.enabled, "Renderer 2 is enabled");
            Assert.IsFalse(collider2.enabled, "Collider 2 is enabled");

            // Removing content from scroll content should also remove its renderers from the scroll clipping box
            scrollView.RemoveItem(contentItems[0]);
            clippedRenderers = scrollView.ClipBox.GetRenderersCopy().ToList();

            // Object is still visible but renderer should not be clipped
            Assert.IsFalse(clippedRenderers.Contains(renderer0), "Renderer 0 is being clipped");
        }

        /// <summary>
        /// Tests if component can scroll content made of objects layouted manually without a BaseObjectCollection.
        /// </summary>
        [UnityTest]
        public IEnumerator CanScrollNonCollectionContent()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);

            var cellWidth = contentItems[0].GetComponent<NearInteractionTouchable>().Bounds.x;
            var cellHeight = contentItems[0].GetComponent<NearInteractionTouchable>().Bounds.y;

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         cellWidth,
                                                                         cellHeight,
                                                                         cellWidth,
                                                                         Vector3.forward,
                                                                         Quaternion.identity,
                                                                         ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                         ScrollingObjectCollection.VelocityType.FalloffPerItem);
            scrollView.AddContent(contentItems[0]);
            scrollView.AddContent(contentItems[1]);

            contentItems[0].transform.localPosition = new Vector3(cellWidth * 0.5f, cellHeight * -0.5f, 0f);
            contentItems[1].transform.localPosition = new Vector3(cellWidth * 0.5f, cellHeight * -1.5f, 0f);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset);

            // Moving hand along y axis should engage an up-down scroll drag
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonPressPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedPos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");

            yield return hand.Show(initialPos);

            Assert.AreEqual(0.032f, scrollView.ScrollContainerPosition.y, 0.0005, "Scroll container should be on second tier");
        }

        /// <summary>
        /// Tests if content made of objects of different sizes can be scrolled.
        /// </summary>
        [UnityTest]
        public IEnumerator CanScrollDifferentSizesContent()
        {
            // Setting up a vertical 1x1 scroll view with four pressable buttons items with two different sizes
            // Buttons are layouted in two grid object collection
            var contentItems1 = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 2);
            var contentItems2 = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2_32x96_PrefabGuid), 2);

            GridObjectCollection objectCollection1 = InstantiateObjectCollection(contentItems1,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            GridObjectCollection objectCollection2 = InstantiateObjectCollection(contentItems2,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                objectCollection1.transform.position + Vector3.right * objectCollection1.CellWidth,
                                                                                Quaternion.identity,
                                                                                0.096f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         1,
                                                                         objectCollection2.CellWidth + objectCollection1.CellWidth,
                                                                         objectCollection1.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection1.gameObject);
            scrollView.AddContent(objectCollection2.gameObject);
            PressableButton button1Component = contentItems1[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaScrollThreshold + scrollView.CellHeight + offset);

            // Moving hand along y axis should engage an up-down scroll drag
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonPressPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedPos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");

            yield return hand.Show(initialPos);

            Assert.AreEqual(0.032f, scrollView.ScrollContainerPosition.y, 0.0005, "Scroll container should be on second tier");
        }

        /// <summary>
        /// Tests if scroll amount corresponds to a smooth copy of hand movement delta.
        /// Overdamping is applied when scroll position is out of min and max bounds.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollAmountHasCorrectDamp()
        {
            // Setting up a vertical 1x2 scroll view with three pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 3);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         2,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.001f;
            float handTouchDelta = 0.05f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonTouchPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedUpPos = pastButtonPressPos + Vector3.up * handTouchDelta;
            Vector3 scrollEngagedDownPos = pastButtonPressPos - Vector3.up * handTouchDelta;

            // Scrolling out of min bounds by moving hand five centimeters down
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedDownPos);

            // Scroll amount should follow hand movement with applied overdamp of roughly 90 percent
            Assert.AreEqual((handTouchDelta - scrollView.HandDeltaScrollThreshold) * 0.1f, scrollView.ScrollContainerPosition.y, 0.005, "Out of bounds scroll drag amount was not overdamped");

            // Scrolling within min max bounds by moving hand five centimeters up
            scrollView.Reset();
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedUpPos);

            // Scroll amount should roughly follow hand movement with 1:1 ratio
            Assert.AreEqual(handTouchDelta - scrollView.HandDeltaScrollThreshold, scrollView.ScrollContainerPosition.y, 0.005, "Scroll drag amount was not 1:1");

            // Scaling scroll object and scrolling within min max bounds by moving hand five centimeters up
            var newScale = 2.0f;
            scrollView.transform.localScale *= newScale;
            scrollView.Reset();
            yield return null;

            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedUpPos);

            // Scroll amount should roughly follow hand movement with 1:1 ratio
            Assert.AreEqual((handTouchDelta - scrollView.HandDeltaScrollThreshold) / newScale, scrollView.ScrollContainerPosition.y, 0.005, "Scroll drag amount was not 1:1");
        }

        /// <summary>
        /// Tests that no errors are raised after the scrolling object collection is removed from the scene
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollViewCleanup()
        {
            // Setting up a vertical 1x2 scroll view with three pressable buttons items
            var contentItems = InstantiatePrefabItems(AssetDatabase.GUIDToAssetPath(PressableHololens2PrefabGuid), 3);

            GridObjectCollection objectCollection = InstantiateObjectCollection(contentItems,
                                                                                LayoutOrder.ColumnThenRow,
                                                                                LayoutAnchor.UpperLeft,
                                                                                1,
                                                                                Vector3.forward,
                                                                                Quaternion.identity,
                                                                                0.032f,
                                                                                0.032f);

            ScrollingObjectCollection scrollView = InstantiateScrollView(1,
                                                                         2,
                                                                         objectCollection.CellWidth,
                                                                         objectCollection.CellHeight,
                                                                         0.016f,
                                                                         Vector3.forward,
                                                                         Quaternion.identity);
            scrollView.AddContent(objectCollection.gameObject);

            PressableButton button1Component = contentItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Object.Destroy(scrollView.gameObject);

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
        }

        #endregion Tests

        #region Utilities

        private GameObject[] InstantiatePrefabItems(string path, int numberOfItems)
        {
            var items = new GameObject[numberOfItems];

            for (int i = 0; i < numberOfItems; i++)
            {
                items[i] = InstantiatePrefab(path);
            }

            return items;
        }

        private GameObject InstantiatePrefab(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

            return Object.Instantiate(prefab) as GameObject;
        }

        private GameObject[] InstantiatePrimitiveItems(PrimitiveType primitiveType, int numberOfItems, float scale)
        {
            var items = new GameObject[numberOfItems];

            for (int i = 0; i < numberOfItems; i++)
            {
                items[i] = GameObject.CreatePrimitive(primitiveType);
                items[i].transform.localScale *= scale;

                // Primitives used in the scroll test class should use mrtk standard material in order to be masked correctly
                items[i].GetComponent<Renderer>().sharedMaterial = mrtkMaterial;
            }

            return items;
        }

        private GridObjectCollection InstantiateObjectCollection(GameObject[] items,
                                                                 LayoutOrder layoutOrder,
                                                                 LayoutAnchor layoutAnchor,
                                                                 int itemsPerTier,
                                                                 Vector3 position,
                                                                 Quaternion rotation,
                                                                 float cellWidth,
                                                                 float cellHight)
        {
            Debug.Assert(items != null && items.Length > 0, "Number of items in object collection should not be null or empty");

            GameObject collectionGameObject = new GameObject();

            foreach (var item in items)
            {
                item.transform.parent = collectionGameObject.transform;
            }

            var objectCollection = collectionGameObject.AddComponent<GridObjectCollection>();
            objectCollection.Layout = layoutOrder;
            objectCollection.Anchor = layoutAnchor;
            objectCollection.SortType = CollationOrder.ChildOrder;
            objectCollection.CellWidth = cellWidth;
            objectCollection.CellHeight = cellHight;

            switch (layoutOrder)
            {
                case LayoutOrder.ColumnThenRow:
                    objectCollection.Columns = itemsPerTier;
                    break;
                case LayoutOrder.RowThenColumn:
                    objectCollection.Rows = itemsPerTier;
                    break;
                default:
                    break;
            }

            objectCollection.UpdateCollection();

            collectionGameObject.transform.position = position;
            collectionGameObject.transform.rotation = rotation;

            return objectCollection;
        }

        private ScrollingObjectCollection InstantiateScrollView(int cellsPerTier,
                                                                int tiersPerPage,
                                                                float cellWidth,
                                                                float cellHeight,
                                                                float cellDepth,
                                                                Vector3 position,
                                                                Quaternion rotation,
                                                                ScrollingObjectCollection.ScrollDirectionType scrollDirection = ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                ScrollingObjectCollection.VelocityType velocityType = ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                ScrollingObjectCollection.EditMode maskEditMode = ScrollingObjectCollection.EditMode.Auto)
        {
            GameObject scrollObject = new GameObject();

            var scrollView = scrollObject.AddComponent<ScrollingObjectCollection>();

            scrollView.CellsPerTier = cellsPerTier;
            scrollView.TiersPerPage = tiersPerPage;
            scrollView.CellWidth = cellWidth;
            scrollView.CellHeight = cellHeight;
            scrollView.CellDepth = cellDepth;
            scrollView.ScrollDirection = scrollDirection;
            scrollView.TypeOfVelocity = velocityType;
            scrollView.MaskEditMode = maskEditMode;

            scrollView.UpdateContent();

            scrollObject.transform.position = position;
            scrollObject.transform.rotation = rotation;

            return scrollView;
        }

        #endregion Utilities
    }
}