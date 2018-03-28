using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.ARCapture
{
    public class PersistentTouch
    {
        public Touch touchData;
        public float lifetime;
        public PersistentTouch(Touch t)
        {
            touchData = t;
            lifetime = 0.0f;
        }
    }

    /// <summary>
    /// Input source supporting basic touchscreen input.
    /// </summary>
    public class TouchScreenInput : Singleton<TouchScreenInput>
    {
        const float kContactEpsilon = 2.0f/60.0f;
        const float kMaxTapContactTime = 0.5f;

        private List<PersistentTouch> m_ActiveTouches;

        public delegate void OnTapDelegate(Touch touch, GameObject tappedObject);
        public OnTapDelegate OnTap;
        public delegate void OnHoldStartDelegate(Touch touch, GameObject tappedObject);
        public OnHoldStartDelegate OnHoldStart;
        public delegate void OnHoldCancelledDelegate(Touch touch, GameObject tappedObject);
        public OnHoldCancelledDelegate OnHoldCancelled;

        public delegate void OnHoldEndDelegate(Touch touch, GameObject tappedObject);
        public OnHoldEndDelegate OnHoldEnd;

        void Start()
        {
            m_ActiveTouches = new List<PersistentTouch>();
        }

        protected virtual void Update()
        {
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        UpdateTouch(touch);
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        RemoveTouch(touch);
                        break;

                    default:
                        break;
                }
            }
        }

        public bool UpdateTouch(Touch t)
        {
            PersistentTouch knownTouch = (m_ActiveTouches.Find(item => item.touchData.fingerId == t.fingerId));
            if (knownTouch != null)
            {
                knownTouch.lifetime += Time.deltaTime;
                return true;
            }
            else
            {
                m_ActiveTouches.Add(new PersistentTouch(t));
                OnHoldStartedEvent(t.fingerId, t);
                return false;
            }
        }

        public void RemoveTouch(Touch t)
        {
            PersistentTouch knownTouch = m_ActiveTouches.Find(item => item.touchData.fingerId == t.fingerId);
            if (knownTouch != null)
            {
                if (t.phase == TouchPhase.Ended)
                {
                    if (knownTouch.lifetime < kContactEpsilon)
                    {
                        OnHoldCanceledEvent(t.fingerId, t);
                    }
                    else if (knownTouch.lifetime < kMaxTapContactTime)
                    {
                        OnTappedEvent(t.fingerId, t.tapCount, t);
                    }
                    else
                    {
                        OnHoldCompletedEvent(t.fingerId, t);
                    }
                }
                else
                {
                    OnHoldCanceledEvent(t.fingerId, t);
                }
                m_ActiveTouches.Remove(knownTouch);
            }
        }

        public Touch? GetTouch(int id)
        {
            PersistentTouch knownTouch = (m_ActiveTouches.Find(item => item.touchData.fingerId == id));
            if (knownTouch != null)
                return knownTouch.touchData;
            else
                return null;
        }

        protected void OnTappedEvent(int id, int tapCount, Touch touch)
        {
            OnHoldCanceledEvent(id, touch);

            if(OnTap != null)
                OnTap(touch, RaycastTouch(touch));
        }

        protected void OnHoldStartedEvent(int id, Touch touch)
        {
            if(OnHoldStart != null)
                OnHoldStart(touch, RaycastTouch(touch));
        }

        protected void OnHoldCanceledEvent(int id, Touch touch)
        {
            if(OnHoldCancelled != null)
                OnHoldCancelled(touch, RaycastTouch(touch));
        }

        protected void OnHoldCompletedEvent(int id, Touch touch)
        {
            if(OnHoldEnd != null)
                OnHoldEnd(touch, RaycastTouch(touch));
        }

        GameObject RaycastTouch(Touch touch)
        {
            var screenWPos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit hit;
            return Physics.Raycast(screenWPos,Camera.main.transform.forward, out hit) ? hit.collider.gameObject : null;
        }

        protected void OnManipulationStartedEvent(Vector3 cumulativeDelta) {}

        protected void OnManipulationUpdatedEvent(Vector3 cumulativeDelta) {}

        protected void OnManipulationCompletedEvent(Vector3 cumulativeDelta) {}

        protected void OnManipulationCanceledEvent(Vector3 cumulativeDelta) {}
    }
}
