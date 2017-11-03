using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.UX
{
    [ExecuteInEditMode]
    public class LineObjectFollower : MonoBehaviour
    {
        public Transform Object;

        [Header("Follow Settings")]
        [Range(0f, 1f)]
        public float NormalizedDistance = 0f;

        private void Update() {
            Vector3 linePoint = source.GetPoint(NormalizedDistance);
            Object.position = linePoint;
        }

        [SerializeField]
        private LineBase source;
    }
}