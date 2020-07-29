// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ScrollViewTests : BasePlayModeTests
    {
        #region Tests

        /// <summary>
        /// Tests if near interaction with a pressable button item is reset after the user engages in a scroll drag.
        /// User should be able to interact with other buttons right after scroll engage is finished.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsNearInteractionWithChildren()
        {
            // Setting up a vertical 1x2 scroll view with three pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                3,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                2,
                                                                                ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton button1Component = scrollItems[0].GetComponentInChildren<PressableButton>();
            PressableButton button2Component = scrollItems[1].GetComponentInChildren<PressableButton>();

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
            scrollView.ListMomentumBegin.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonTouchPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight + offset);

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

            yield return hand.Hide();
        }

        /// <summary>
        /// Tests if far interaction with a pressable button item is reset after the user engages in a scroll drag.
        /// User should be able to interact with other buttons right after scroll engage is finished.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsFarInteractionWithChildren()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                2,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            float scale = 10f;
            scrollView.transform.localScale *= scale;

            TestUtilities.PlayspaceToOriginLookingForward();

            Interactable interactable1 = scrollItems[0].GetComponent<Interactable>();
            Interactable interactable2 = scrollItems[1].GetComponent<Interactable>();

            Assert.IsNotNull(interactable1);
            Assert.IsNotNull(interactable2);

            bool scrollDragBegin = false;
            scrollView.ListMomentumBegin.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = new Vector3(0.13f, -0.17f, 0.5f); // Far pointer focus is on button       
            Vector3 scrollEngagedPos = initialPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight * scale + offset);

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

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return hand.Hide();
        }

        /// <summary>
        /// Tests if interaction with a pressable button child triggers an undesired jump or scroll drag.
        /// </summary>
        [UnityTest]
        public IEnumerator NoJumpsWhenInteractingWithChildren()
        {
            // Setting up a vertical 1x2 scroll view with three pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                3,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                2,
                                                                                ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            float scale = 10f;
            scrollView.transform.localScale *= scale;
 

            TestUtilities.PlayspaceToOriginLookingForward();

            bool scrollDragBegin = false;
            scrollView.ListMomentumBegin.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = new Vector3(0.13f, -0.17f, 0.5f); // Far pointer focus is on button       
            Vector3 scrollEngagedPos = initialPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight + offset);

            // Interaction with child button should behave normally if scroll drag not yet engaged
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForSeconds(1.0f); // Waiting for possible timed drag trigger

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.AreEqual(scrollView.ScrollContainerPosition.y, 0, "Scroll container has moved.");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return hand.Hide();
        }

        /// <summary>
        /// Tests scroll drag engage by interacting with the background empty space of a scroll view 
        /// </summary>
        [UnityTest]
        public IEnumerator InteractionWithBackgroundEmptySpace()
        {
            // Setting up a vertical 2x1 scroll view with three pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                3,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                2,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            TestUtilities.PlayspaceToOriginLookingForward();

            bool scrollDragBegin = false;
            scrollView.ListMomentumBegin.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 scrollTouchPos = scrollItems[1].transform.position + Vector3.forward * 0.015f; // Touching scroll second colum slot        
            Vector3 scrollEngagedUpPos = scrollTouchPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight + offset); // Scrolls up one row
            Vector3 scrollEngagedDownPos = scrollTouchPos - Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight + offset); // Scrolls down one row

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

            yield return hand.Hide();
        }

        /// <summary>
        /// Tests if adding or deleting children items while scroll is engaged in a drag work as expected. 
        /// </summary>
        [UnityTest]
        public IEnumerator ChildrenCanBeAddedAndDeleted()
        {
            // Setting up a vertical 1x1 scroll view with three pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                3,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.FalloffPerItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            // This button will be added later to the scroll collection
            GameObject button4 = InstantiatePrefab(TestButtonUtilities.PressableHoloLens2PrefabPath);

            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton button1Component = scrollItems[0].GetComponentInChildren<PressableButton>();
            PressableButton button3Component = scrollItems[2].GetComponentInChildren<PressableButton>();
            PressableButton button4Component = button4.GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);
            Assert.IsNotNull(button3Component);
            Assert.IsNotNull(button4Component);

            bool scrollDragBegin = false;
            scrollView.ListMomentumBegin.AddListener(() =>
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
            Vector3 scrollEngagedHalfPageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight / 2 + offset);
            Vector3 scrollEngagedOnePageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight + offset);
            Vector3 scrollEngagedTwoPageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight * 2 + offset);

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

            // Removing scroll item while scroll is engaged
            scrollView.RemoveItem(scrollItems[1]);
            GameObject.Destroy(scrollItems[1]);

            // Scrolling to second row
            scrollDragBegin = false;
            button1TouchBegin = false;
            button3TouchBegin = false;
            button4TouchBegin = false;

            yield return hand.MoveTo(initialPos);
            yield return hand.MoveTo(preButtonTouchPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedOnePageUpPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was not triggered."); // both coliders disabled need to go deeper
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
            scrollView.AddItem(button4);

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

            hand.Hide();
        }

        /// <summary>
        /// Tests if scroll engage triggered by a near interaction is reset if pointer crosses outside boundaries threshold.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsWhenOutOfBoundaryThreshold()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                2,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton button1Component = scrollItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.002f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonTouchPos = button1Component.transform.position + new Vector3(0, 0, button1Component.StartPushDistance - offset);
            Vector3 pastButtonPressPos = button1Component.transform.position + new Vector3(0, 0, button1Component.PressDistance + offset);
            Vector3 scrollEngagedHalfPageUpPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight / 2 + offset);
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

            yield return hand.Hide();
        }

        /// <summary>
        /// Tests if scroll engage is only triggered by a near interaction if pointer comes from the front plane.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageOnlyFromFrontInteraction()
        {
            // Setting up a vertical 1x1 scroll view with two pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                2,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.FalloffPerItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton button1Component = scrollItems[0].GetComponentInChildren<PressableButton>();

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

            yield return hand.Hide();
        }

        /// <summary>
        /// Tests if updating the collection after scaling the scroll object does not alter clipping box local scale.
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollViewCanBeScaled()
        {
            // Setting up a vertical 1x1 scroll view with one single pressable button item
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                1,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.FalloffPerItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);

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
            scrollView.UpdateCollection();
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
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems, 
                                                                                2, 
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown, 
                                                                                1, 
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.FalloffPerItem,
                                                                                Vector3.up * -1.0f,
                                                                                Quaternion.LookRotation(-Vector3.up));
            // Setting up camera to look down
            MixedRealityPlayspace.Position = Vector3.zero;
            MixedRealityPlayspace.Rotation = Quaternion.LookRotation(-Vector3.up);

            PressableButton button1Component = scrollItems[0].GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(button1Component);

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = Vector3.zero;
            Vector3 preButtonPressPos = button1Component.transform.position - new Vector3(0, button1Component.StartPushDistance - offset, 0);
            Vector3 pastButtonPressPos = button1Component.transform.position - new Vector3(0, button1Component.PressDistance + 0.03f, 0);
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.forward * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight + offset);

            // Moving hand along z axis should still engage an up-down scroll view rotated 90 degrees around x
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.MoveTo(preButtonPressPos);
            yield return hand.MoveTo(pastButtonPressPos);
            yield return hand.MoveTo(scrollEngagedPos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll view is not being dragged.");

            yield return hand.Hide();
        }

        /// <summary>
        /// Tests if scroll can be moved by page, by tier or to make specific element to be presented in the first visible tier.
        /// </summary>
        [UnityTest]
        public IEnumerator CanBeScrolledByTierOrIndexOrPage()
        {
            // Setting up a horizontal 2x2 scroll view with nine pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems, 
                                                                                9, 
                                                                                ScrollingObjectCollection.ScrollDirectionType.LeftAndRight, 
                                                                                2, 
                                                                                2, 
                                                                                ScrollingObjectCollection.VelocityType.FalloffPerItem, 
                                                                                Vector3.forward, 
                                                                                Quaternion.identity);
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return null;

            // Initial scroll state
            Assert.AreEqual(0, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving to second tier
            scrollView.MoveByTiers(1, false);
            yield return null;

            Assert.AreEqual(2, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(6, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving to fourth tier
            scrollView.MoveByTiers(2, false);
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Scroll container should not move beyond its min position
            scrollView.MoveByTiers(1, false);
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving back to first tier
            scrollView.MoveByTiers(-4, false);
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving one page to third tier
            scrollView.MoveByPages(1, false);
            yield return null;

            Assert.AreEqual(4, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(8, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving half page to fourth tier as scroll container hits its min position
            scrollView.MoveByPages(1, false);
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving one page and a half back to first tier
            scrollView.MoveByPages(-2, false);
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Second item is already in first visible tier
            scrollView.MoveToIndex(1, false);
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving to second tier making fourth element to be on first visible tier
            scrollView.MoveToIndex(3, false);
            yield return null;

            Assert.AreEqual(2, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(6, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving to fourth tier as scroll container hits its min position
            scrollView.MoveToIndex(8, false); // should move to 6 as max
            yield return null;

            Assert.AreEqual(6, scrollView.FirstVisibleItemIndex,  "First visible item is different from the expected");
            Assert.AreEqual(10, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");

            // Moving to back to first tier. Negative argument should not cause any errors
            scrollView.MoveToIndex(-1, false); 
            yield return null;

            Assert.AreEqual(0, scrollView.FirstVisibleItemIndex, "First visible item is different from the expected");
            Assert.AreEqual(4, scrollView.FirstHiddenItemIndex, "First hidden item is different from the expected");
        }

        /// <summary>
        /// Tests if far interaction with GGV pointer can engage the scroll drag.
        /// </summary>
        [UnityTest]
        public IEnumerator GGVScroll()
        {
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            // Setting up a horizontal 1x1 scroll view with two pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                2,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            float scale = 10f;
            scrollView.transform.localScale *= scale;

            TestUtilities.PlayspaceToOriginLookingForward();

            // Hand positions
            float offset = 0.001f;
            Vector3 initialPos = new Vector3(0.13f, -0.17f, 0.5f); // Far pointer focus is on button       
            Vector3 scrollEngagedPos = initialPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight * scale + offset);

            // Interaction with child button should behave normally if scroll drag not yet engaged
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(scrollEngagedPos);

            Assert.IsTrue(scrollView.IsDragging, "Scroll drag was not triggered.");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return hand.Hide();

            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Articulated);
        }

        /// <summary>
        /// Tests if it is possible to ensure that children click only happens on touch up by changing children configuration.
        /// </summary>
        [UnityTest]
        public IEnumerator ContentClickHappensOnTouchUp()
        {
            // Setting up a horizontal 1x1 scroll view with two pressable buttons items
            ScrollingObjectCollection scrollView = InstantiateScrollAndChildren(out GameObject[] scrollItems,
                                                                                2,
                                                                                ScrollingObjectCollection.ScrollDirectionType.UpAndDown,
                                                                                1,
                                                                                1,
                                                                                ScrollingObjectCollection.VelocityType.NoVelocitySnapToItem,
                                                                                Vector3.forward,
                                                                                Quaternion.identity);
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton button1Component = scrollItems[0].GetComponentInChildren<PressableButton>();
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
            Vector3 scrollEngagedPos = pastButtonPressPos + Vector3.up * (scrollView.HandDeltaMagThreshold + scrollView.CellHeight + offset);

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

            yield return hand.Hide();
        }

        #endregion Tests

        #region Utilities

        private GameObject InstantiatePrefab(string path)
        {
            Object buttonPrefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject button = Object.Instantiate(buttonPrefab) as GameObject;

            return button;
        }

        private ScrollingObjectCollection InstantiateScrollAndChildren(out GameObject[] scrollItems, 
                                                                       int numberOfItems, 
                                                                       ScrollingObjectCollection.ScrollDirectionType scrollDirection, 
                                                                       int itemsPerTier, 
                                                                       int viewableArea,
                                                                       ScrollingObjectCollection.VelocityType velocityType,
                                                                       Vector3 position,
                                                                       Quaternion rotation)
        {
            Debug.Assert(numberOfItems > 0);

            GameObject scrollObject = new GameObject();

            scrollItems = new GameObject[numberOfItems];
            for (int i = 0; i < numberOfItems; i++)
            {
                scrollItems[i] = InstantiatePrefab(TestButtonUtilities.PressableHoloLens2PrefabPath);
                scrollItems[i].transform.parent = scrollObject.transform;
            }

            var scrollView = scrollObject.AddComponent<ScrollingObjectCollection>();
            scrollView.ScrollDirection = scrollDirection;
            scrollView.CellWidth = scrollItems[0].GetComponent<NearInteractionTouchable>().Bounds.x;
            scrollView.CellHeight = scrollItems[0].GetComponent<NearInteractionTouchable>().Bounds.y;
            scrollView.TypeOfVelocity = velocityType;
            scrollView.ItemsPerTier = itemsPerTier;
            scrollView.ViewableArea = viewableArea;
            scrollView.UpdateCollection();

            scrollObject.transform.position = position;
            scrollObject.transform.rotation = rotation;

            return scrollView;
        }

        #endregion Utilities
    }
}