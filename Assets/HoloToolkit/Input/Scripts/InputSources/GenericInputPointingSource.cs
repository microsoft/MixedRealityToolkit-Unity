using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class GenericInputPointingSource : GenericInputSource, IPointingSource
    {
        public GenericInputPointingSource(uint sourceId, string name) : base(sourceId, name) { }

        public Cursor Cursor { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public bool OwnAllInput { get; set; }

        public RayStep[] Rays { get { return rays; } }

        public PointerResult Result { get; set; }

        public float? ExtentOverride { get; set; }

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public bool InteractionEnabled { get { return true; } }

        public bool FocusLocked { get; set; }

        public bool TryGetPointerPosition(out Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPointingRay(out Ray pointingRay)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPointerRotation(out Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        public bool InputIsFromSource(InputEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        private RayStep[] rays = new RayStep[1] { new RayStep(Vector3.zero, Vector3.forward) };

        public virtual void OnPreRaycast()
        {
            Ray pointingRay;
            if (TryGetPointingRay(out pointingRay))
            {
                rays[0].CopyRay(pointingRay, FocusManager.Instance.GetPointingExtent(this));
            }

            if (RayStabilizer != null)
            {
                RayStabilizer.UpdateStability(rays[0].origin, rays[0].direction);
                rays[0].CopyRay(RayStabilizer.StableRay, FocusManager.Instance.GetPointingExtent(this));
            }
        }

        public virtual void OnPostRaycast() { }

        public bool OwnsInput(BaseEventData eventData)
        {
            return (OwnAllInput || InputIsFromSource(eventData));
        }

        public bool InputIsFromSource(BaseEventData eventData)
        {
            var inputData = (eventData as IInputSourceInfoProvider);

            return (inputData != null)
                && (inputData.InputSource == this)
                && (inputData.SourceId == SourceId);
        }
    }
}