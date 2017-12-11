using HoloToolkit.Unity;
using UnityEngine;
using HoloToolkit.Unity.UX;

namespace HoloToolkit.Unity.Controllers
{
    [RequireComponent(typeof(Parabola))]
    [RequireComponent(typeof(DistorterGravity))]
    public class ParabolicPointer : NavigationPointer
    {        
        public override void OnPreRaycast() {

            CalculateParabola();

            base.OnPreRaycast();
        }

        private void CalculateParabola()
        {
            //// Make sure our parabola only rotates on y/x axis
            //// NOTE: Parabola's custom line transform field should be set to a transform OTHER than its gameObject's transform
            //Vector3 eulerAngles = transform.eulerAngles;
            //eulerAngles.z = 0f;
            //eulerAngles.x = 0f;
            ////parabolaMain.LineTransform.eulerAngles = eulerAngles;

            //// Get the rotation of the actual pointer
            //// If we're pointing up (x < 0) then extend the length of the parabola
            //float adjustedParabolaDistance = parabolaDistance;
            //float adjustedParabolaHeight = parabolaHeight;
            //float adjustedParabolaDropDist = parabolaDropDist;

            //eulerAngles = transform.eulerAngles;
            //float arc = Mathf.DeltaAngle(0f, eulerAngles.x) / 360;
            //adjustedParabolaDistance += (parabolaRotationArc.Evaluate(arc) * parabolaRotationArcMultiplier);
            //adjustedParabolaHeight += (parabolaHeightArc.Evaluate(arc) * parabolaHeightArcMultiplier);
            //adjustedParabolaDropDist += (parabolaDropDistArc.Evaluate(arc) * parabolaDropDistArcMultiplier);

            //// Set up our parabola
            //// First point is always origin
            //parabolaMain.FirstPoint = PointerOrigin;
            //Vector3 parabolaTarget = PointerOrigin + (parabolaMain.LineTransform.forward * adjustedParabolaDistance) + (Vector3.down * adjustedParabolaDropDist);
            //// Use the x rotation as the distance

            //// Set the distance of the parabola based on how far back it's tipped
            //parabolaMain.LastPoint = parabolaTarget;
            //parabolaMain.Height = adjustedParabolaHeight;
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
        [Range(0.1f, 10f)]
        private float parabolaDistance = 1f;
        [SerializeField]
        [Range(0.1f, 10f)]
        private float parabolaHeight = 1f;
        [SerializeField]
        [Range(0.1f, 10f)]
        private float parabolaDropDist = 1f;
        [SerializeField]
        private AnimationCurve parabolaRotationArc;
        [SerializeField]
        private AnimationCurve parabolaHeightArc;
        [SerializeField]
        private AnimationCurve parabolaDropDistArc;
        [SerializeField]
        [Range(1f, 36f)]
        private float parabolaRotationArcMultiplier = 10f;
        [SerializeField]
        [Range(1f, 36f)]
        private float parabolaHeightArcMultiplier = 10f;
        [SerializeField]
        [Range(1f, 36f)]
        private float parabolaDropDistArcMultiplier = 10f;
        [SerializeField]
        [DropDownComponent(true,true)]
        private Parabola parabolaMain;

        protected override void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                CalculateParabola();
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
