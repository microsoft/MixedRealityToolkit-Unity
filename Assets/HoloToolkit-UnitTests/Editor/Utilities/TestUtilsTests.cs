using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class TestUtilsTests
    {
        [Test]
        public void ClearOne()
        {
            GameObject.CreatePrimitive(PrimitiveType.Cube);
            TestUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }

        [Test]
        public void ClearMany()
        {
            for (var i = 0; i < 10; i++)
            {
                GameObject.CreatePrimitive(PrimitiveType.Cube);
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
    }
}
