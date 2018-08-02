using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector3 velocity;

    public Attractor[] attractors;

    private void Update()
    {
        foreach (Attractor attractor in attractors)
            velocity += attractor.GetLocalEffect(transform.position);

        transform.position += (velocity * Time.deltaTime);
    }
}