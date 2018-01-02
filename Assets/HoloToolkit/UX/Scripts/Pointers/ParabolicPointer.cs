using HoloToolkit.Unity;
using UnityEngine;
using HoloToolkit.Unity.UX;

namespace HoloToolkit.Unity.Controllers
{
    [RequireComponent(typeof(ParabolaPhysical))]
    [RequireComponent(typeof(DistorterGravity))]
    public class ParabolicPointer : NavigationPointer
    {        
        public override void OnPreRaycast() {

            UpdateParabola();

            base.OnPreRaycast();
        }

        private void UpdateParabola()
        {
            // Make sure our parabola only rotates on y/x axis
            // NOTE: Parabola's custom line transform field should be set to a transform OTHER than its gameObject's transform
            parabolaMain.Direction = transform.forward + Vector3.up;
            parabolaMain.LineTransform.eulerAngles = Vector3.zero;
        }

        public override void OnSelectPressed()
        {
            base.OnSelectPressed();

            for (int i = 0; i < disableWhileActive.Length; i++)
            {
                disableWhileActive[i].InteractionEnabled = false;
            }

            // Initiate teleportation
            PointerTeleportManager.Instance.InitiateTeleport(this);
        }

        public override void OnSelectReleased()
        {
            base.OnSelectReleased();

            for (int i = 0; i < disableWhileActive.Length; i++)
            {
                disableWhileActive[i].InteractionEnabled = true;
            }

            // Finish teleportation
            PointerTeleportManager.Instance.TryToTeleport();
        }

        [Header("Pointer Control")]
        [SerializeField]
        [Tooltip("Pointers that you want to disable while teleporting")]
        private ControllerPointerBase[] disableWhileActive;

        [Header("Parabola settings")]
        [SerializeField]
        private AnimationCurve parabolaVelocityCurve = AnimationCurve.Linear(-1f, 1f, 1f, 1f);
        [SerializeField]
        [Range(1f, 36f)]
        private float parabolaVelocityMultiplier = 10f;
        [SerializeField]
        [DropDownComponent(true,true)]
        private ParabolaPhysical parabolaMain;

        protected override void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                UpdateParabola();
            }
        }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ParabolicPointer))]
        public new class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}
