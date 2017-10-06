using UnityEngine;
using NUnit.Framework;

namespace HoloToolkit.Unity.Tests
{
    public class WorldAnchorManagerTests
    {
        [SetUp]
        public void ClearScene()
        {
            TestUtils.ClearScene();
        }

        [Test]
        public void TestGenerateAnchorNameFromGameObject()
        {
            const string expected = "AnchorName";
            var gameObject = new GameObject(expected);
            var result = WorldAnchorManager.GenerateAnchorName(gameObject);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestGenerateAnchorNameFromParameter()
        {
            const string expected = "AnchorName";
            var gameObject = new GameObject();
            var result = WorldAnchorManager.GenerateAnchorName(gameObject, expected);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}