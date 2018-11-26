// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class TestUtilsTests
    {
        [Test]
        public void ClearOne()
        {
            new GameObject();
            TestUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }

        [Test]
        public void ClearReferencedDisabled()
        {
            var gameObject = new GameObject();
            gameObject.SetActive(false);
            TestUtils.ClearScene();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.That(gameObject, Is.UnityNull);
        }

        [Test]
        public void ClearUnreferencedDisabled()
        {
            var unreferencedGameObject = new GameObject();
            unreferencedGameObject.SetActive(false);
            TestUtils.ClearScene();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.That(unreferencedGameObject, Is.UnityNull);
        }

        [Test]
        public void ClearMany()
        {
            for (var i = 0; i < 10; i++)
            {
                new GameObject();
            }
            TestUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }

        [Test]
        public void ClearHierarchy()
        {
            var empty = new GameObject();
            var parent = Object.Instantiate(empty);
            for (var i = 0; i < 10; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);
            }
            TestUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }

        [Test]
        public void CreateMainCamera()
        {
            TestUtils.ClearScene();
            var mainCamera = TestUtils.CreateMainCamera();
            Assert.That(mainCamera, Is.EqualTo(Camera.main));
        }

        [Test]
        public void CallAwakeTest()
        {
            var gameObject = new GameObject();
            var reflectionTest = gameObject.AddComponent<ReflectionTestBehaviour>();
            gameObject.CallAwake();
            Assert.That(reflectionTest.AwakeCalled, Is.True);
        }

        [Test]
        public void CallStartTest()
        {
            var gameObject = new GameObject();
            var reflectionTest = gameObject.AddComponent<ReflectionTestBehaviour>();
            gameObject.CallStart();
            Assert.That(reflectionTest.StartCalled, Is.True);
        }

        [Test]
        public void CallUpdateTest()
        {
            var gameObject = new GameObject();
            var reflectionTest = gameObject.AddComponent<ReflectionTestBehaviour>();
            gameObject.CallUpdate();
            gameObject.CallUpdate();
            Assert.That(reflectionTest.UpdateCallCount, Is.EqualTo(2));
        }

        [Test]
        public void CallAwakeUpdateChainTest()
        {
            var gameObject = new GameObject();
            var reflectionTest = gameObject.AddComponent<ReflectionTestBehaviour>();
            gameObject.CallAwake().CallStart();
            Assert.That(reflectionTest.AwakeCalled, Is.True);
            Assert.That(reflectionTest.StartCalled, Is.True);
        }

        [Test]
        public void CallGenericPrivateMethodTest()
        {
            var gameObject = new GameObject();
            var reflectionTest = gameObject.AddComponent<ReflectionTestBehaviour>();
            gameObject.CallAllMonoBehaviours("GenericPrivateMethod");
            Assert.That(reflectionTest.GenericPrivateMethodCalled, Is.True);
        }

        [Test]
        public void CallGenericPublicMethodTest()
        {
            var gameObject = new GameObject();
            var reflectionTest = gameObject.AddComponent<ReflectionTestBehaviour>();
            gameObject.CallAllMonoBehaviours("GenericPublicMethod");
            Assert.That(reflectionTest.GenericPublicMethodCalled, Is.True);
        }

        [Test]
        public void CallGenericMultipleComponentsTest()
        {
            var gameObject = new GameObject();
            var reflectionTest1 = gameObject.AddComponent<ReflectionTestBehaviour>();
            var reflectionTest2 = gameObject.AddComponent<ReflectionTestBehaviour>();
            gameObject.CallAllMonoBehaviours("GenericPrivateMethod");
            Assert.That(reflectionTest1.GenericPrivateMethodCalled, Is.True);
            Assert.That(reflectionTest2.GenericPrivateMethodCalled, Is.True);
        }
    }
}
