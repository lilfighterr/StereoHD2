using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellSpawner : MonoBehaviour {

    public GameObject gravityWellPrefab;
    public GameObject spawnBoxArea;
    public TextMesh scoreText;
    public int initialSpawnCount = 20;

    private Vector3 boxSize;
    private Vector3 boxCenter;
    private float xSpawn, ySpawn, zSpawn;
    private float KeyInputDelayTimer;
    private float colliderRadius;
    private float colliderScale;
    private LayerMask layerMask = (1 << 9); //Layer mask on layer 9 
    private Vector3 initialStart;
    private GameObject[] spawnedBalls;
    private Renderer rend;
    private Color originalColor;
    private List<int> randomizeList = new List<int>();
    private int score = 0;
    private int hits = 0;
    private int misses = 0;


    // Use this for initialization
    void Start () {
        // Initialize array of balls
        spawnedBalls = new GameObject[initialSpawnCount];

        // Instantiate initial point to reach
        initialStart = new Vector3(-0.0523f, -0.0542f, 0.8347f); //Initial point
        spawnedBalls[0] =  Instantiate(gravityWellPrefab, initialStart, Quaternion.identity); //First spawn
        rend = spawnedBalls[0].GetComponent<Renderer>();
        originalColor = rend.material.color;
        rend.material.color = Color.yellow; // Make it color yellow
        spawnedBalls[0].GetComponent<GravityWell>().highlightedBall = true; // Set its status to highlighted

        // Set boundaries for area of spawn
        boxSize = spawnBoxArea.GetComponent<Collider>().bounds.size;
        boxCenter = spawnBoxArea.transform.position;
        colliderRadius = gravityWellPrefab.GetComponent<SphereCollider>().radius;
        colliderScale = gravityWellPrefab.GetComponent<SphereCollider>().transform.lossyScale.x;

        // Spawn # of balls cased on spawn count
        for (int i = 1; i < initialSpawnCount; i++)
        {
            SpawnGravityWell(i);
            randomizeList.Add(i);
        }

        ListShuffle.Shuffle<int>(randomizeList); //Shuffle List
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            HighlightRandomBall();
            //SpawnGravityWell();
        }
    }

    public void SpawnGravityWell(int index) // Spawn gravity well
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
        spawnedBalls[index] =Instantiate(gravityWellPrefab, pos, Quaternion.identity);
    }

    public Vector3 MoveGravityWell()
    {
        xSpawn = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
        ySpawn = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
        zSpawn = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
        Vector3 pos = new Vector3(xSpawn, ySpawn, zSpawn);
        return pos;
    }

    // Highlighting a random ball
    public void HighlightRandomBall()
    {
        if (randomizeList.Count != 0)
        {
            spawnedBalls[randomizeList[0]].GetComponent<Renderer>().material.color = Color.yellow; // Make it color yellow
            spawnedBalls[randomizeList[0]].GetComponent<GravityWell>().highlightedBall = true; // Set its status to highlighted
            randomizeList.RemoveAt(0);
        }
    }

    public void IncreaseScore()
    {
        hits++;
        score = hits - misses;
        scoreText.text = " Score\n" + score.ToString();
    }

    public void DecreaseScore()
    {
        misses++;
        score = hits - misses;
        scoreText.text = " Score\n" + score.ToString();
    }
}
