using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class EditorUtilsTests
    {
        [Test]
        public void ClearOne()
        {
            GameObject.CreatePrimitive(PrimitiveType.Cube);
            EditorUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }

        [Test]
        public void ClearMany()
        {
            for (var i = 0; i < 10; i++)
            {
                GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            EditorUtils.ClearScene();
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
            EditorUtils.ClearScene();
            Assert.That(Object.FindObjectsOfType<GameObject>(), Is.Empty);
        }
    }
}
