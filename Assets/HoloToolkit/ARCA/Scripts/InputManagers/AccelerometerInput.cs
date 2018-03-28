using UnityEngine;

namespace HoloToolkit.ARCapture
{
    public class AccelerometerInput : Singleton<AccelerometerInput>
    {
        public float MinAccelerationThreshold;

        public delegate void OnAccelerationDelegate(Vector3 acceleration);
        public OnAccelerationDelegate OnAcceleration;

        void Update()
        {
            if(Input.acceleration.magnitude > MinAccelerationThreshold)
            {
                if(OnAcceleration != null)
                OnAcceleration(Input.acceleration);
            }
        }
    }
}
