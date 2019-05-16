//TODO anborod
//using Assets.Demo.Utilities;
//using Microsoft.Azure.SpatialAnchors;
//using Microsoft.Azure.SpatialAnchors.Unity;
//using Microsoft.Azure.SpatialAnchors.Unity.Samples;
//using Microsoft.MixedReality.Toolkit;
//using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
//using System;
//using System.Collections.Concurrent;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEngine;
//using Object = UnityEngine.Object;

//namespace Assets.Demo
//{
//    public class AzureSpatialAnchorsCoordinateService : MonoBehaviour, IDependencyHolder<ISpatialCoordinateService>
//    {
//        private ISpatialCoordinateService dependency;
//        private readonly ConcurrentQueue<Action> updateThreadQueue = new ConcurrentQueue<Action>();

//        [SerializeField]
//        private GameObject anchorPrefab = null;

//        [SerializeField]
//        [PasswordField]
//        private string spatialAnchorsAccountId = null;

//        [SerializeField]
//        [PasswordField]
//        private string spatialAnchorsAccountKey = null;

//        public ISpatialCoordinateService Dependency
//        {
//            get
//            {
//                if (dependency == null)
//                {
//                    AzureSpatialAnchorsDemoWrapper wrapper = gameObject.EnsureComponent<AzureSpatialAnchorsDemoWrapper>();
//                    wrapper.SpatialAnchorsAccountId = spatialAnchorsAccountId;
//                    wrapper.SpatialAnchorsAccountKey = spatialAnchorsAccountKey;

//                    dependency = new SpatialCoordinateService(anchorPrefab, TimeSpan.FromHours(24), updateThreadQueue.Enqueue);
//                }

//                return dependency;
//            }
//        }

//        private void Update()
//        {
//            while (updateThreadQueue.TryDequeue(out Action action))
//            {
//                try
//                {
//                    action();
//                }
//                catch (Exception ex)
//                {
//                    Debug.LogException(ex);
//                }
//            }
//        }

//        public class SpatialCoordinateService : SpatialCoordinateServiceUnityBase<string>
//        {
//            public class SpatialCoordinate : SpatialCoordinateUnityBase<string>
//            {
//#if UNITY_ANDROID || UNITY_IOSS
//                private readonly bool isAndroidOriOS = true;
//#else
//                private readonly bool isAndroidOriOS = false;
//#endif

//                private readonly CloudSpatialAnchor cloudSpatialAnchor;
//                private readonly GameObject associatedGameObject;

//                private int frameOfLastUpdate;

//                public SpatialCoordinate(CloudSpatialAnchor cloudSpatialAnchor, GameObject associatedGameObject)
//                    : base(cloudSpatialAnchor.Identifier)
//                {
//                    this.cloudSpatialAnchor = cloudSpatialAnchor;
//                    this.associatedGameObject = associatedGameObject;

//                    frameOfLastUpdate = Time.frameCount;
//                }

//                private void UpdateTransform()
//                {
//                    if (isAndroidOriOS && Time.frameCount != frameOfLastUpdate)
//                    {
//                        Pose pose = cloudSpatialAnchor.GetAnchorPose();
//                        associatedGameObject.transform.position = pose.position;
//                        associatedGameObject.transform.rotation = pose.rotation;
//                    }

//                    frameOfLastUpdate = Time.frameCount;
//                }

//                protected override Vector3 CoordinateToWorldSpace(Vector3 vector)
//                {
//                    UpdateTransform();

//                    return associatedGameObject.transform.TransformPoint(vector);
//                }

//                protected override Quaternion CoordinateToWorldSpace(Quaternion quaternion)
//                {
//                    UpdateTransform();

//                    return quaternion * associatedGameObject.transform.rotation;
//                }

//                protected override Vector3 WorldToCoordinateSpace(Vector3 vector)
//                {
//                    UpdateTransform();

//                    return associatedGameObject.transform.InverseTransformPoint(vector);
//                }

//                protected override Quaternion WorldToCoordinateSpace(Quaternion quaternion)
//                {
//                    UpdateTransform();

//                    return quaternion * Quaternion.Inverse(associatedGameObject.transform.rotation);
//                }
//            }

//            private readonly bool isEditor;
//            private readonly GameObject anchorPrefab;
//            private readonly TimeSpan anchorDefaultExpiration;
//            private readonly Action<Action> queueOnUpdateThread;

//            public SpatialCoordinateService(GameObject anchorPrefab, TimeSpan anchorDefaultExpiration, Action<Action> queueOnUpdateThread)
//            {
//#if UNITY_EDITOR
//                isEditor = true;
//#else
//                isEditor = false;
//#endif
//                this.anchorPrefab = anchorPrefab;
//                this.anchorDefaultExpiration = anchorDefaultExpiration;
//                this.queueOnUpdateThread = queueOnUpdateThread;
//            }

//            protected async override Task RunTrackingAsync(CancellationToken cancellationToken)
//            {
//                if (isEditor)
//                {
//                    await Task.Delay(-1, cancellationToken).IgnoreCancellation();
//                }
//                else
//                {
//                    await WaitForSessionValidAsync(cancellationToken);
//                    AzureSpatialAnchorsDemoWrapper.Instance.OnAnchorLocated += OnAnchorDiscovered;

//                    AzureSpatialAnchorsDemoWrapper.Instance.EnableProcessing = true;
//                    await Task.Delay(-1, cancellationToken).IgnoreCancellation();
//                    AzureSpatialAnchorsDemoWrapper.Instance.EnableProcessing = false;

//                    AzureSpatialAnchorsDemoWrapper.Instance.OnAnchorLocated -= OnAnchorDiscovered;
//                }
//            }

//            private async Task WaitForSessionValidAsync(CancellationToken cancellationToken)
//            {
//                while (!AzureSpatialAnchorsDemoWrapper.Instance.SessionValid())
//                {
//                    await Task.Delay(100, cancellationToken);
//                }
//            }

//            private void OnAnchorDiscovered(object sender, AnchorLocatedEventArgs args)
//            {
//                bool isAndroidOriOS = false;
//#if UNITY_ANDROID || UNITY_IOSS
//                isAndroidOriOS = true;
//#endif

//                queueOnUpdateThread(() =>
//                {
//                    Pose anchorPose = isAndroidOriOS ? args.Anchor.GetAnchorPose() : Pose.identity;

//                    if (args.Status != LocateAnchorStatus.Located)
//                    {
//                        return;
//                    }

//                    GameObject spawnedObject = Object.Instantiate(anchorPrefab, anchorPose.position, anchorPose.rotation);
//                    spawnedObject.AddARAnchor();

//#if WINDOWS_UWP || UNITY_WSA
//                    if (args.Anchor != null)
//                    {
//                        spawnedObject.GetComponent<UnityEngine.XR.WSA.WorldAnchor>().SetNativeSpatialAnchorPtr(args.Anchor.LocalAnchor);
//                    }
//#endif

//                    OnNewCoordinate(args.Identifier, new SpatialCoordinate(args.Anchor, spawnedObject));
//                });
//            }

//            protected async override Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken)
//            {
//                ThrowIfDisposed();

//                if (isEditor)
//                {
//                    return null;
//                }

//                await WaitForSessionValidAsync(cancellationToken);

//                while (!AzureSpatialAnchorsDemoWrapper.Instance.EnoughDataToCreate)
//                {
//                    await Task.Delay(100, cancellationToken);
//                }

//                GameObject spawnedObject = Object.Instantiate(anchorPrefab, worldPosition, worldRotation);
//                spawnedObject.AddARAnchor();

//                //TODO anborod: let anchor above localize
//                await Task.Delay(100, cancellationToken);
//                try
//                {
//                    IntPtr nativeAnchorPoitner = spawnedObject.GetNativeAnchorPointer();

//                    if (nativeAnchorPoitner == IntPtr.Zero)
//                    {
//                        throw new InvalidOperationException("Failed to get the local XR Anchor pointer");
//                    }

//                    CloudSpatialAnchor cloudSpatialAnchor = await AzureSpatialAnchorsDemoWrapper.Instance.StoreAnchorInCloud(new CloudSpatialAnchor()
//                    {
//                        LocalAnchor = nativeAnchorPoitner,
//                        Expiration = DateTimeOffset.Now.Add(anchorDefaultExpiration)
//                    });

//                    if (cloudSpatialAnchor == null)
//                    {
//                        Object.Destroy(spawnedObject);
//                        return null;
//                    }

//                    return new SpatialCoordinate(cloudSpatialAnchor, spawnedObject);
//                }
//                catch
//                {
//                    Object.Destroy(spawnedObject);
//                    throw;
//                }
//            }

//            public async override Task<ISpatialCoordinate> TryGetCoordinateByIdAsync(string id, CancellationToken cancellationToken)
//            {
//                ThrowIfDisposed();

//                if (isEditor)
//                {
//                    return null;
//                }

//                await WaitForSessionValidAsync(cancellationToken);

//                AzureSpatialAnchorsDemoWrapper.Instance.SetAnchorIdsToLocate(new string[] { id });
//                CloudSpatialAnchorWatcher watcher = null;

//                try
//                {
//                    watcher = AzureSpatialAnchorsDemoWrapper.Instance.CreateWatcher();

//                    ISpatialCoordinate coordinate;

//                    while (!knownCoordinates.TryGetValue(id, out coordinate))
//                    {
//                        await Task.Delay(1000, cancellationToken);
//                    }

//                    return coordinate;
//                }
//                finally
//                {
//                    watcher?.Stop();
//                    AzureSpatialAnchorsDemoWrapper.Instance.ResetAnchorIdsToLocate();
//                }
//            }
//        }
//    }
//}