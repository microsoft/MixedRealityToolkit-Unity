using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class TestUtilsTests
    {
        [Test]
        public void ClearOne()
        {
            TestUtils.CreateGameObject();
            TestUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }

        [Test]
        public void ClearReferencedDisabled()
        {
            var gameObject = TestUtils.CreateGameObject();
            gameObject.SetActive(false);
            TestUtils.ClearScene();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.That(gameObject == null, Is.True);
        }

        [Test]
        public void ClearUnreferencedDisabled()
        {
            var unreferencedGameObject = new GameObject();
            unreferencedGameObject.SetActive(false);
            TestUtils.ClearScene();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.That(unreferencedGameObject == null, Is.True);
        }

        [Test]
        public void ClearMany()
        {
            for (var i = 0; i < 10; i++)
            {
                TestUtils.CreateGameObject();
            }
            TestUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }

        [Test]
        public void ClearHierarchy()
        {
            var empty = TestUtils.CreateGameObject();
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
    }
}
