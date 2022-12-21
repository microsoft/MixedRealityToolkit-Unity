// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Dynamic collection of objects positioned in a horizontal or vertical stack with an auto sized back plate. 
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Object Bar")]
    public class ObjectBar : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("List of game objects contained in the Object Bar.  Each game object must contain a collider as the bounds are used" +
            " for position calculations.")]
        private List<GameObject> objectBarObjects = new List<GameObject>();

        /// <summary>
        /// List of game objects contained in the Object Bar.  Each game object must contain a collider as the bounds are used
        /// for position calculations.
        /// </summary>
        public List<GameObject> ObjectBarObjects
        {
            get => objectBarObjects;
            set
            {
                objectBarObjects = value;
                OnObjectBarUpdated.Invoke();
            }
        }

        [SerializeField]
        [Tooltip("The flow direction of the object bar objects.")]
        private ObjectBarFlowDirection objectBarFlowDirection = ObjectBarFlowDirection.Horizontal;

        /// <summary>
        /// The flow direction of the object bar objects.
        /// </summary>
        public ObjectBarFlowDirection ObjectBarFlowDirection
        {
            get => objectBarFlowDirection;
            set
            {
                objectBarFlowDirection = value;
                OnObjectBarUpdated.Invoke();
            }
        }

        [SerializeField]
        [Tooltip("The space between each individual object bar object along the x axis for a ObjectBarFlowDirection.Horizontial and along the y" +
            " axis for ObjectBarFlowDirection.Vertical.")]
        private Vector2 spacingBetween;

        /// <summary>
        /// The space between each individual object bar object along the x axis for a ObjectBarFlowDirection.Horizontial and along the y
        /// axis for ObjectBarFlowDirection.Vertical.
        /// </summary>
        public Vector2 SpacingBetween
        {
            get => spacingBetween;
            set
            {
                spacingBetween = value;
                OnObjectBarUpdated.Invoke();
            }
        }

        [Space(10)]
        [SerializeField]
        [Tooltip("The game object that represents the back plate for this object bar. This transform will be modified according " +
            " to the properties on this component.  For example, if the objects in the Object Bar Object's list increases, the " +
            " scale of this back plate will increase.")]
        private GameObject backPlateObject;

        /// <summary>
        /// The game object that represents the back plate for this object bar. This transform will be modified according 
        /// to the properties on this component.  For example, if the objects in the Object Bar Object's list increases, the 
        /// scale of this back plate will increase.
        /// </summary>
        public GameObject BackPlateObject
        {
            get => backPlateObject;
            set
            {
                backPlateObject = value;
                OnObjectBarUpdated.Invoke();
            }
        }

        [SerializeField]
        [Tooltip("The space between the object bar objects and the edge of the back plate.")]
        private Vector2 borderSpacing;

        /// <summary>
        /// The space between the object bar objects and the edge of the back plate.
        /// </summary>
        public Vector2 BorderSpacing
        {
            get => borderSpacing;
            set
            {
                borderSpacing = value;
                OnObjectBarUpdated.Invoke();
            }
        }

        [SerializeField]
        [Tooltip("The Z axis offset for the Back Pate Object.")]
        private float backPlateZOffset;

        /// <summary>
        /// The Z axis offset for the Back Pate Object.
        /// </summary>
        public float BackPlateZOffset
        {
            get => backPlateZOffset;
            set
            {
                backPlateZOffset = value;
                OnObjectBarUpdated.Invoke();
            }
        }

        [SerializeField]
        [Tooltip("Invoked when any property is modified or the size of the Object Bar Objects list has been changed.")]
        private UnityEvent onObjectBarUpdated;

        /// <summary>
        /// Invoked when any property is modified or the size of the Object Bar Objects list has been changed.
        /// </summary>
        public UnityEvent OnObjectBarUpdated
        {
            get => onObjectBarUpdated;
        }

        private Vector3 startCalculationPosition;
        private List<Collider> colliders = new List<Collider>();
        private int currentListCount;

        protected void Start()
        {
            // Update the Object Bar when properties are changed via script only during runtime
            if (Application.isPlaying)
            {
                OnObjectBarUpdated.AddListener(() => UpdateObjectBar());
            }
        }

        protected void OnDestroy()
        {
            if (Application.isPlaying)
            {
                OnObjectBarUpdated.RemoveAllListeners();
            }
        }

        protected void Update()
        {
            // Update the Object Bar while in edit mode for syncing changes when properties are modified via inspector.
            // Primarily used for updating the size of the back plate to match the amount of elements in the list.
            if (!Application.isPlaying)
            {
                UpdateObjectBar();
            }
            else
            {
                // While not in edit mode, update the Object Bar during runtime when the ObjectBarObject list count changes
                CheckObjectBarListCount();
            }
        }

        /// <summary>
        /// Update the position of the objects in the object bar and update the size and position of the back plate object.
        /// </summary>
        public void UpdateObjectBar()
        {
            bool allCollidersPresent = SyncColliders();

            startCalculationPosition = Vector3.zero;

            if (allCollidersPresent)
            {
                CalculateObjectPositions();

                CalculateBackPlateSize();
            }
        }

        private void CalculateObjectPositions()
        {
            if (ObjectBarObjects.Count != 0 && ObjectBarObjects[0] != null)
            {
                ObjectBarObjects[0].transform.localPosition = startCalculationPosition;

                // Local (relative to us) bounds of the previous object.
                Vector3 previousBounds = LocalBoundsFromCollider(colliders[0]);

                for (int i = 1; i < ObjectBarObjects.Count; i++)
                {
                    Vector3 newPosition;
                    // Local (relative to us) bounds of the current object.
                    Vector3 newBounds = LocalBoundsFromCollider(colliders[i]);
                    Vector3 previousObjectPosition = ObjectBarObjects[i - 1].transform.localPosition;

                    if (ObjectBarFlowDirection == ObjectBarFlowDirection.Horizontal)
                    {
                        float averageXDistance = (previousBounds.x + newBounds.x) * 0.5f;
                        newPosition = previousObjectPosition + (averageXDistance * Vector3.right) + (Vector3.right * SpacingBetween.x);
                    }
                    else
                    {
                        float averageYDistance = (previousBounds.y + newBounds.y) * 0.5f;
                        newPosition = previousObjectPosition + (averageYDistance * Vector3.up) + Vector3.up * SpacingBetween.y;
                    }

                    ObjectBarObjects[i].transform.localPosition = newPosition;
                    previousBounds = newBounds;
                }
            }
        }

        private void CalculateBackPlateSize()
        {
            if (BackPlateObject != null && ObjectBarObjects.Count != 0 && ObjectBarObjects[0] != null)
            {
                Vector3 newScale;
                Vector3 newPosition;
                Vector2 largestColliderSizes = GetLargestColliderSizes();
                Vector3 currentScale = BackPlateObject.transform.localScale;
                float distanceFirstToLast = Vector3.Distance(ObjectBarObjects[0].transform.localPosition, ObjectBarObjects[ObjectBarObjects.Count - 1].transform.localPosition);

                if (ObjectBarFlowDirection == ObjectBarFlowDirection.Horizontal)
                {
                    distanceFirstToLast += largestColliderSizes.x;
                    newScale = new Vector3(distanceFirstToLast + BorderSpacing.x, largestColliderSizes.y + BorderSpacing.y, currentScale.z);
                    newPosition = Vector3.right * (distanceFirstToLast * 0.5f - largestColliderSizes.x * 0.5f) + Vector3.back * BackPlateZOffset;
                }
                else // ObjectBarFlowDirection.Vertical
                {
                    distanceFirstToLast += largestColliderSizes.y;
                    newScale = new Vector3(largestColliderSizes.x + BorderSpacing.x, distanceFirstToLast + BorderSpacing.y, currentScale.z);
                    newPosition = Vector3.up * (distanceFirstToLast * 0.5f - largestColliderSizes.y * 0.5f) + Vector3.back * BackPlateZOffset;
                }

                BackPlateObject.transform.localScale = newScale;
                BackPlateObject.transform.localPosition = newPosition;
            }
        }

        private void CheckObjectBarListCount()
        {
            if (currentListCount != ObjectBarObjects.Count)
            {
                OnObjectBarUpdated.Invoke();
            }

            currentListCount = ObjectBarObjects.Count;
        }

        private bool SyncColliders()
        {
            colliders.Clear();

            for (int i = 0; i < ObjectBarObjects.Count; i++)
            {
                if (ObjectBarObjects[i] != null)
                {
                    if (ObjectBarObjects[i].TryGetComponent(out Collider collider))
                    {
                        colliders.Add(collider);
                    }
                    else
                    {
                        Debug.LogError($"Object at index {i} in the ObjectBarObjects list does not have a collider attached. The collider bounds size is utilized for position calculations.");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private Vector2 GetLargestColliderSizes()
        {
            float largestXSize = 0;
            float largestYSize = 0;

            for (int i = 0; i < colliders.Count; i++)
            {
                Vector3 bounds = LocalBoundsFromCollider(colliders[i]);
                if (bounds.x > largestXSize)
                {
                    largestXSize = bounds.x;
                }

                if (bounds.y > largestYSize)
                {
                    largestYSize = bounds.y;
                }
            }

            return new Vector2(largestXSize, largestYSize);
        }

        private Vector3 LocalBoundsFromCollider(Collider c)
        {
            Vector3 globalBounds = c is BoxCollider boxCollider ? boxCollider.transform.TransformVector(boxCollider.size) : c.bounds.size;
            return transform.InverseTransformVector(globalBounds);
        }
    }
}
