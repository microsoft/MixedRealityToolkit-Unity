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
        /// Tests if interaction with a pressable button is reset after scroll drag is engaged
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsNearInteractionwithChildren()
        {
            // Setting up scroll view object with two pressable button children
            GameObject scrollObject = new GameObject();

            GameObject button1 = InstantiatePrefab(TestButtonUtilities.PressableHoloLens2PrefabPath);
            button1.transform.parent = scrollObject.transform;

            GameObject button2 = InstantiatePrefab(TestButtonUtilities.PressableHoloLens2PrefabPath);
            button2.transform.parent = scrollObject.transform;

            GameObject button3 = InstantiatePrefab(TestButtonUtilities.PressableHoloLens2PrefabPath);
            button3.transform.parent = scrollObject.transform;

            var scrollView = scrollObject.AddComponent<ScrollingObjectCollection>();
            scrollView.ScrollDirection = ScrollingObjectCollection.ScrollDirectionType.UpAndDown;
            scrollView.CellWidth = button1.GetComponent<NearInteractionTouchable>().Bounds.x;
            scrollView.CellHeight = button1.GetComponent<NearInteractionTouchable>().Bounds.y;
            scrollView.Tiers = 1;
            scrollView.ViewableArea = 2;
            scrollView.HandDeltaMagThreshold = scrollView.CellHeight * 0.2f;
            scrollView.UpdateCollection();

            scrollObject.transform.position = Vector3.forward;
            TestUtilities.PlayspaceToOriginLookingForward();

            PressableButton buttonComponent = button1.GetComponentInChildren<PressableButton>();

            Assert.IsNotNull(buttonComponent);

            bool buttonTouchBegin = false;
            buttonComponent.TouchBegin.AddListener(() =>
            {
                buttonTouchBegin = true;
            });

            bool buttonTouchEnd = false;
            buttonComponent.TouchEnd.AddListener(() =>
            {
                buttonTouchEnd = true;
            });

            bool buttonPressBegin = false;
            buttonComponent.ButtonPressed.AddListener(() =>
            {
                buttonPressBegin = true;
            });

            bool buttonPressCompleted = false;
            buttonComponent.ButtonReleased.AddListener(() =>
            {
                buttonPressCompleted = true;
            });

            bool scrollDragBegin = false;
            scrollView.ListMomentumBegin.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 buttonPos = buttonComponent.transform.position;
            Vector3 startHandPos = buttonPos + new Vector3(0, 0, buttonComponent.StartPushDistance - offset); // Just before touch
            Vector3 onPressPos = buttonPos + new Vector3(0, 0, buttonComponent.PressDistance + offset); // Past press plane         
            Vector3 onScrollEngagedPos = onPressPos + new Vector3(0, -(scrollView.HandDeltaMagThreshold + offset), 0); // Engaging in scroll drag

            // If scroll drag not yet engaged then child button interaction should behave normally
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(startHandPos);
            yield return hand.MoveTo(onPressPos);
            yield return hand.MoveTo(startHandPos);

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.IsTrue(buttonTouchBegin, "Button touch begin did not trigger.");
            Assert.IsTrue(buttonPressBegin, "Button press begin did not trigger.");
            Assert.IsTrue(buttonPressCompleted, "Button press release did not trigger.");

            yield return hand.Hide();

            Assert.IsTrue(buttonTouchEnd, "Button touch end did not trigger.");

            // Reset values
            buttonTouchBegin = false;
            buttonTouchEnd = false;
            buttonPressBegin = false;
            buttonPressCompleted = false;
            scrollDragBegin = false;

            // Scroll drag engage should halt interaction with any child button            
            yield return hand.Show(startHandPos);
            yield return hand.MoveTo(onPressPos);
            
            Assert.IsTrue(buttonTouchBegin, "Button touch begin did not trigger.");
            Assert.IsTrue(buttonPressBegin, "Button press begin did not trigger.");

            yield return hand.MoveTo(onScrollEngagedPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin dit not trigger.");
            Assert.IsTrue(buttonTouchEnd, "Button touch end did not trigger.");
            Assert.IsFalse(buttonPressCompleted, "Button press release did not trigger.");

            yield return hand.Hide();
        }

        /// <summary>
        /// Tests if interaction triggered by far pointer on interactable object is reset after scroll is engaged
        /// </summary>
        [UnityTest]
        public IEnumerator ScrollEngageResetsFarInteractionwithChildren()
        {          
            // Setting up scroll view object with two pressable button children
            GameObject scrollObject = new GameObject();
            float scale = 10f;

            GameObject button1 = InstantiatePrefab(TestButtonUtilities.PressableHoloLens2PrefabPath);
            button1.transform.localScale *= scale;
            button1.transform.parent = scrollObject.transform;

            GameObject button2 = InstantiatePrefab(TestButtonUtilities.PressableHoloLens2PrefabPath);
            button2.transform.localScale *= scale;
            button2.transform.parent = scrollObject.transform;

            var scrollView = scrollObject.AddComponent<ScrollingObjectCollection>();
            scrollView.ScrollDirection = ScrollingObjectCollection.ScrollDirectionType.UpAndDown;
            scrollView.CellWidth = button1.GetComponent<NearInteractionTouchable>().Bounds.x * scale;
            scrollView.CellHeight = button1.GetComponent<NearInteractionTouchable>().Bounds.y * scale;
            scrollView.Tiers = 1;
            scrollView.ViewableArea = 1;
            scrollView.HandDeltaMagThreshold = scrollView.CellHeight * 0.2f;
            scrollView.UpdateCollection();

            scrollObject.transform.position = Vector3.forward;
            TestUtilities.PlayspaceToOriginLookingForward();

            Interactable interactableComponent1 = button1.GetComponent<Interactable>();

            Assert.IsNotNull(interactableComponent1);

            bool scrollDragBegin = false;
            scrollView.ListMomentumBegin.AddListener(() =>
            {
                scrollDragBegin = true;
            });

            // Hand positions
            float offset = 0.001f;
            Vector3 buttonPos = interactableComponent1.transform.position;
            Vector3 startHandPos = new Vector3(0.13f, -0.17f, 0.5f); // Far pointer focus is on button       
            Vector3 onScrollEngagedPos = startHandPos + new Vector3(0, -(scrollView.HandDeltaMagThreshold + offset), 0); // Engaging in scroll drag

            // If scroll drag not yet engaged child button interaction should behave normally
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(startHandPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.IsTrue(interactableComponent1.HasFocus, "Interactable does not have far pointer focus.");
            Assert.IsTrue(interactableComponent1.HasPress, "Interactable did not get press from far interaction.");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);

            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered");
            Assert.IsTrue(interactableComponent1.HasFocus, "Interactable does not have far pointer focus.");
            Assert.IsFalse(interactableComponent1.HasPress, "Interactable still have press from far interaction.");

            // Scroll drag engage should halt interaction with any child button 
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            
            Assert.IsFalse(scrollDragBegin, "Scroll drag begin was triggered.");
            Assert.IsTrue(interactableComponent1.HasFocus, "Interactable does not have far pointer focus.");
            Assert.IsTrue(interactableComponent1.HasPress, "Interactable did not get press from far interaction.");

            yield return hand.MoveTo(onScrollEngagedPos);

            Assert.IsTrue(scrollDragBegin, "Scroll drag begin was not triggered");
            Assert.IsFalse(interactableComponent1.HasFocus, "Interactable still have far pointer focus.");
            Assert.IsFalse(interactableComponent1.HasPress, "Interactable still have press from far interaction.");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
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

        #endregion Utilities
    }
}