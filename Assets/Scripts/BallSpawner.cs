using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {

    public GameObject BallPrefab;
    public GameObject Box;
    public Vector3 center;
    public Vector3 size;
    public float TimePerSpawn;
    
    private Transform boxSize;
    private Transform ballSize;
    private float nextSpawnTime = 0.0f;

    private float KeyInputDelayTimer;

    // Use this for initialization
    void Start () {
        boxSize = Box.GetComponent<Transform>();
        ballSize = BallPrefab.GetComponent<Transform>();
        center = new Vector3(boxSize.position.x, boxSize.position.y + boxSize.lossyScale.y/2 + ballSize.lossyScale.y/2, boxSize.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.KeypadEnter) && KeyInputDelayTimer + 0.1f < Time.time) // Press enter to start
        {
            KeyInputDelayTimer = Time.time;
            if (!GameControl.instance.gameStart) // If game hasn't started
            {
                GameControl.instance.gameCountdown = true; // Start counting down
            }
        }
        if (GameControl.instance.gameStart && Time.time > nextSpawnTime) // Start spawning balls once it starts
        {
            nextSpawnTime = Time.time + TimePerSpawn;
            SpawnBall();
        }
	}

    public void SpawnBall() // Spawn ball in a random X spot in the middle Z on top of the box
    {
        Vector3 pos = new Vector3(Random.Range(center.x - boxSize.lossyScale.x/2 + ballSize.lossyScale.x/2, center.x + boxSize.lossyScale.x / 2 - ballSize.lossyScale.x / 2), center.y, center.z);
        Instantiate(BallPrefab, pos, Quaternion.identity);
    }
}
