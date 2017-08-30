using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class ComponentExtensionsTests
    {
        [SetUp]
        [TearDown]
        public void ClearScene()
        {
            TestUtils.ClearScene();
        }

        [Test]
        public void EnsureComponentNotNull()
        {
            var gameObject = TestUtils.CreateGameObject();
            gameObject.EnsureComponent<BoxCollider>();
            Assert.That(gameObject.GetComponent<BoxCollider>() == null, Is.False);
        }

        [Test]
        public void EnsureOnlyOneComponentWithExisting()
        {
            var gameObject = TestUtils.CreateGameObject();
            gameObject.AddComponent<BoxCollider>();
            gameObject.EnsureComponent<BoxCollider>();
            Assert.That(gameObject.GetComponents<BoxCollider>().Length, Is.EqualTo(1));
        }

        [Test]
        public void EnsureReturnsSameComponentWithExisting()
        {
            var gameObject = TestUtils.CreateGameObject();
            var existingComponent = gameObject.AddComponent<BoxCollider>();
            var ensuredComponent = gameObject.EnsureComponent<BoxCollider>();
            Assert.That(ensuredComponent, Is.EqualTo(existingComponent));
        }

        [Test]
        public void EnsureComponentOnComponent()
        {
            var gameObject = TestUtils.CreateGameObject();
            var existingComponent = gameObject.AddComponent<BoxCollider>();
            var ensuredComponent = existingComponent.EnsureComponent<SphereCollider>();
            Assert.That(ensuredComponent == null, Is.False);
        }
    }
}
