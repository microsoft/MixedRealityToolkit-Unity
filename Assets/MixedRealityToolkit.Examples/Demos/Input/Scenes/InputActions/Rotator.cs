using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Silly script that applies a rotation about the up axis on demand.
public class Rotator : MonoBehaviour
{
    public float angle = 45f;

    public void Rotate()
    {
        transform.Rotate(0, angle, 0);
    }
}
