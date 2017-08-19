using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class CameraCacheTests
    {
        [SetUp]
        public void SetUpTests()
        {
            EditorUtils.ClearScene();
        }

        [Test]
        public void GetNullCameraFromCache()
        {
            Assert.That(CameraCache.Main, Is.Null);
        }

        [Test]
        public void GetNullCameraFromCacheAfterDelete()
        {

            var mainCamera = CreateMainCamera();
            var unused = CameraCache.Main;
            Object.DestroyImmediate(mainCamera.gameObject);
            Assert.That(CameraCache.Main, Is.Null);
        }

        [Test]
        public void GetMainCameraFromCache()
        {
            var mainCamera = CreateMainCamera();
            Assert.That(CameraCache.Main, Is.EqualTo(mainCamera));
        }

        [Test]
        public void GetMainFromCacheWithMultiple()
        {
            var mainCamera = CreateMainCamera();
            var unused = CameraCache.Main;
            CreateMainCamera();
            Assert.That(CameraCache.Main, Is.EqualTo(mainCamera));
        }

        [Test]
        public void GetMainFromCacheAfterDelete()
        {
            var mainCamera = CreateMainCamera();
            var unused = CameraCache.Main;
            var secondMainCamera = CreateMainCamera();
            Object.DestroyImmediate(mainCamera.gameObject);
            Assert.That(CameraCache.Main, Is.EqualTo(secondMainCamera));
        }

        [Test]
        public void ManualMainCameraRefresh()
        {
            CreateMainCamera();
            var unused = CameraCache.Main;
            var secondMainCamera = CreateMainCamera();
            CameraCache.Refresh(secondMainCamera);
            Assert.That(CameraCache.Main, Is.EqualTo(secondMainCamera));
        }

        [Test]
        public void ManualMainCameraRefreshGetFirstAfterDelete()
        {
            var mainCamera = CreateMainCamera();
            var unused = CameraCache.Main;
            var secondMainCamera = CreateMainCamera();
            CameraCache.Refresh(secondMainCamera);
            Object.DestroyImmediate(secondMainCamera.gameObject);
            Assert.That(CameraCache.Main, Is.EqualTo(mainCamera));
        }

        private static Camera CreateCamera()
        {
            return new GameObject().AddComponent<Camera>();
        }

        private static Camera CreateMainCamera()
        {
            var camera = CreateCamera();
            camera.gameObject.tag = "MainCamera";
            return camera;
        }
    }
}
