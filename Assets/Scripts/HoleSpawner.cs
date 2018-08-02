using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSpawner : MonoBehaviour
{

    public GameObject HolePrefab;
    public GameObject Box;
    public Vector3 center;
    public Vector3 size;

    private Transform boxSize;
    private Transform holeSize;
    private float xSpawn, ySpawn, zSpawn;
    private bool start = false;
    private float KeyInputDelayTimer;

    // Use this for initialization
    void Start()
    {
        boxSize = Box.GetComponent<Transform>();
        holeSize = HolePrefab.GetComponent<Transform>();
        center = new Vector3(boxSize.position.x, boxSize.position.y, boxSize.position.z);
        SpawnHole();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKey(KeyCode.KeypadEnter) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            SpawnHole();
        }*/

    }

    public void SpawnHole()
    {
        xSpawn = Random.Range(center.x - boxSize.lossyScale.x / 2 + holeSize.lossyScale.x / 2, center.x + boxSize.lossyScale.x / 2 - holeSize.lossyScale.x / 2);
        ySpawn = center.y;
        zSpawn = Random.Range(center.z - boxSize.lossyScale.z / 2 + holeSize.lossyScale.y, center.z + boxSize.lossyScale.z / 2 - holeSize.lossyScale.y);
        Vector3 pos = new Vector3(xSpawn, ySpawn, zSpawn);
        Instantiate(HolePrefab, pos, Quaternion.AngleAxis(-90,Vector3.left));
    }
}
