using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class CameraCacheTests
    {
        [SetUp]
        [TearDown]
        public void ClearScene()
        {
            TestUtils.ClearScene();
        }

        [Test]
        public void GetNullCameraFromCache()
        {
            Assert.That(CameraCache.Main, Is.Null);
        }

        [Test]
        public void GetNullCameraFromCacheAfterDelete()
        {

            var mainCamera = TestUtils.CreateMainCamera();
            var unused = CameraCache.Main;
            Object.DestroyImmediate(mainCamera.gameObject);
            Assert.That(CameraCache.Main, Is.Null);
        }

        [Test]
        public void GetMainCameraFromCache()
        {
            var mainCamera = TestUtils.CreateMainCamera();
            Assert.That(CameraCache.Main, Is.EqualTo(mainCamera));
        }

        [Test]
        public void GetMainFromCacheWithMultiple()
        {
            var mainCamera = TestUtils.CreateMainCamera();
            var unused = CameraCache.Main;
            TestUtils.CreateMainCamera();
            Assert.That(CameraCache.Main, Is.EqualTo(mainCamera));
        }

        [Test]
        public void GetMainFromCacheAfterDelete()
        {
            var mainCamera = TestUtils.CreateMainCamera();
            var unused = CameraCache.Main;
            var secondMainCamera = TestUtils.CreateMainCamera();
            Object.DestroyImmediate(mainCamera.gameObject);
            Assert.That(CameraCache.Main, Is.EqualTo(secondMainCamera));
        }

        [Test]
        public void ManualMainCameraRefresh()
        {
            TestUtils.CreateMainCamera();
            var unused = CameraCache.Main;
            var secondMainCamera = TestUtils.CreateMainCamera();
            CameraCache.Refresh(secondMainCamera);
            Assert.That(CameraCache.Main, Is.EqualTo(secondMainCamera));
        }

        [Test]
        public void ManualMainCameraRefreshGetFirstAfterDelete()
        {
            var mainCamera = TestUtils.CreateMainCamera();
            var unused = CameraCache.Main;
            var secondMainCamera = TestUtils.CreateMainCamera();
            CameraCache.Refresh(secondMainCamera);
            Object.DestroyImmediate(secondMainCamera.gameObject);
            Assert.That(CameraCache.Main, Is.EqualTo(mainCamera));
        }
    }
}
