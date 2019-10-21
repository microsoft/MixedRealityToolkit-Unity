using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToBounds : MonoBehaviour
{
    [SerializeField]
    Transform frontBounds = null;

    [SerializeField]
    Transform backBounds = null;

    [SerializeField]
    Transform leftBounds = null;

    [SerializeField]
    Transform rightBounds = null;

    [SerializeField]
    Transform bottomBounds = null;

    [SerializeField]
    Transform topBounds = null;

    Vector3 positionAtStart;

    // Start is called before the first frame update
    void Start()
    {
        positionAtStart = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > rightBounds.position.x || transform.position.x < leftBounds.position.x ||
            transform.position.y > topBounds.position.y || transform.position.y < bottomBounds.position.y ||
            transform.position.z > backBounds.position.z || transform.position.z < frontBounds.position.z)
        {
            transform.position = positionAtStart;
        }
    }
}
