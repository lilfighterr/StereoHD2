using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public float range;
    public float mass; // not really a mass, but whatever

    public Vector3 GetLocalEffect(Vector3 position)
    {
        Vector3 delta = position - transform.position;
        if (delta.sqrMagnitude > range * range)
            return Vector3.zero;

        float percentage = (range - delta.magnitude) / range;

        return -delta.normalized * percentage * percentage * mass;
    }
}