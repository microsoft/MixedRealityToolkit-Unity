using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.UX
{
    public interface INavigationHotSpot
    {
        Vector3 Position { get; }
        Vector3 Normal { get; }
        bool IsActive { get; }
        bool OverrideTargetOrientation { get; }
        float TargetOrientation { get; }
    }

    [UseWith(typeof(InteractibleHighlight))]
    public class NavigationHotSpot : FocusTarget, INavigationHotSpot
    {
        [SerializeField]
        private bool overrideOrientation = false;
        private InteractibleHighlight highlight;

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        public Vector3 Normal
        {
            get
            {
                return transform.up;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActiveAndEnabled;
            }
        }

        public bool OverrideTargetOrientation
        {
            get
            {
                return overrideOrientation;
            }
        }

        public float TargetOrientation
        {
            get
            {
                return transform.eulerAngles.y;
            }
        }

        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            if (HasFocus)
            {
                highlight.Highlight = true;
            }
        }

        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            highlight.Highlight = false;
        }

        public void OnEnable()
        {
            highlight = gameObject.EnsureComponent<InteractibleHighlight>();
            highlight.enabled = true;
        }

        public void OnDisable()
        {
            highlight = gameObject.EnsureComponent<InteractibleHighlight>();
            highlight.enabled = false;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = IsActive ? Color.green : Color.red;
            Gizmos.DrawLine(Position + (Vector3.up * 0.1f), Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f));
            Gizmos.DrawSphere(Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f), 0.01f);
        }
    }
}