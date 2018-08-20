using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellSpawner : MonoBehaviour {

    public GameObject gravityWellPrefab;
    public GameObject spawnBoxArea;
    public TextMesh scoreText;
    public int initialSpawnCount = 20;
    public double timeLastBallLeft = 0;
    public int score = 0;
    public int hits = 0;
    public int misses = 0;

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
    private List<double> doublesRandomizedList = new List<double>();
    private List<double> xSpawnList = new List<double>();
    private List<double> ySpawnList = new List<double>();
    private List<double> zSpawnList = new List<double>();
    private List<List<double>> table = new List<List<double>>();

    private string[][] loadedData;
    private Vector3 pos;
    private int spawnData = 0;
    private int pointIndex = 0;

    // Use this for initialization
    void Start () {
        // Change scene number on Game Control
        GameControl.instance.sceneNumber = 2;

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

        if (GameControl.instance.useData)
        {
            LoadData(); // Use data from CSV
        }

        // Spawn # of balls cased on spawn count
        for (int i = 1; i < initialSpawnCount; i++)
        {
            SpawnGravityWell(i);
            if (GameControl.instance.useData)
            {
                randomizeList.Add(int.Parse(loadedData[3][i-1]));
            }
            else
            {
                randomizeList.Add(i);
            }
        }

        if (!GameControl.instance.useData) ListShuffle.Shuffle<int>(randomizeList); //Shuffle List
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            HighlightRandomBall();
            //SpawnGravityWell();
        }
        if (Input.GetKey(KeyCode.G) && KeyInputDelayTimer + 0.1f < Time.time) // Press G to generate spawn points
        {
            KeyInputDelayTimer = Time.time;
            GeneratePositionSets();
        }
    }

    public void SpawnGravityWell(int index) // Spawn gravity well
    {
        if (GameControl.instance.useData)
        {
            pos = new Vector3(float.Parse(loadedData[0][pointIndex]), float.Parse(loadedData[1][pointIndex]), float.Parse(loadedData[2][pointIndex]));
            pointIndex++;
        }
        else
        {
            // Randomize location within box
            xSpawn = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
            ySpawn = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
            zSpawn = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
            pos = new Vector3(xSpawn, ySpawn, zSpawn);

            // Check if spawn point will collide with other points
            while (Physics.CheckSphere(pos, colliderRadius * colliderScale, layerMask)) // Will only apply to Layer specified in layer mask
            {
                // Randomize again if it will collide
                xSpawn = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
                ySpawn = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
                zSpawn = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
                pos = new Vector3(xSpawn, ySpawn, zSpawn);
            }

            //Save point locations
            xSpawnList.Add(xSpawn);
            ySpawnList.Add(ySpawn);
            zSpawnList.Add(zSpawn);
        }
       

        

        // Spawn point
        spawnedBalls[index] =Instantiate(gravityWellPrefab, pos, Quaternion.identity);
    }

    public Vector3 MoveGravityWell()
    {
        xSpawn = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
        ySpawn = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
        zSpawn = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
        pos = new Vector3(xSpawn, ySpawn, zSpawn);
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

    private void GeneratePositionSets() // Generate a CSV file of spawn locations. First row is X, 2nd is Y, 3rd is Z. Each column corresponds to a point in space
    {
        table.Add(xSpawnList);
        table.Add(ySpawnList);
        table.Add(zSpawnList);
        randomizeList.ForEach(i => doublesRandomizedList.Add(i));
        table.Add(doublesRandomizedList);
        SaveToExcel.instance.Save(table, 4, "Gravity_Spawn");
        table.Clear();
        xSpawnList.Clear();
        ySpawnList.Clear();
        zSpawnList.Clear();
        Debug.Log("Generated positions!");
    }

    private void LoadData()
    {
        loadedData = SaveToExcel.instance.Load("SpawnGeneration/Gravity_Spawn_" + GameControl.instance.spawnNumber + ".csv");
        Debug.Log("Loaded Data");
    }
}
