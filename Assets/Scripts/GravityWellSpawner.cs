using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellSpawner : MonoBehaviour {

    public GameObject gravityWellPrefab;
    public GameObject spawnBoxArea;

    private Vector3 boxSize;
    private Vector3 boxCenter;
    private float xSpawn, ySpawn, zSpawn;
    private float KeyInputDelayTimer;
    private float colliderRadius;
    private float colliderScale;
    private LayerMask layerMask = (1 << 9); //Layer mask on layer 9 

    // Use this for initialization
    void Start () {
        boxSize = spawnBoxArea.GetComponent<Collider>().bounds.size;
        boxCenter = spawnBoxArea.transform.position;
        colliderRadius = gravityWellPrefab.GetComponent<SphereCollider>().radius;
        colliderScale = gravityWellPrefab.GetComponent<SphereCollider>().transform.lossyScale.x;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            SpawnGravityWell();
        }
    }

    public void SpawnGravityWell() // Spawn gravity well
    {
        // Randomize location within box
        xSpawn = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
        ySpawn = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
        zSpawn = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
        Vector3 pos = new Vector3(xSpawn, ySpawn, zSpawn);

        // Check if spawn point will collide with other points
        while (Physics.CheckSphere(pos, colliderRadius * colliderScale, layerMask)) // Will only apply to Layer specified in layer mask
        {
            // Randomize again if it will collide
            xSpawn = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
            ySpawn = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
            zSpawn = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
            pos = new Vector3(xSpawn, ySpawn, zSpawn);
        }

        // Spawn point
        Instantiate(gravityWellPrefab, pos, Quaternion.identity);
    }

    public Vector3 MoveGravityWell()
    {
        xSpawn = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
        ySpawn = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
        zSpawn = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
        Vector3 pos = new Vector3(xSpawn, ySpawn, zSpawn);
        return pos;
    }
}
