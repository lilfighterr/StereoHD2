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
    private int ballIndex = 0;
    private List<double> xSpawn = new List<double>();
    private List<double> ySpawn = new List<double>();
    private List<double> zSpawn = new List<double>();
    private List<List<double>> table = new List<List<double>>();
    private string[][] loadedData;
    private Vector3 pos;
    private int spawnData = 0;

    private float KeyInputDelayTimer;

    // Use this for initialization
    void Start () {
        // Change scene number on gamecontrol
        GameControl.instance.sceneNumber = 0;

        boxSize = Box.GetComponent<Transform>();
        ballSize = BallPrefab.GetComponent<Transform>();
        center = new Vector3(boxSize.position.x, boxSize.position.y + boxSize.lossyScale.y/2 + ballSize.lossyScale.y/2, boxSize.position.z);
        if (GameControl.instance.useData)
        {
            LoadData(); // Use data from CSV
        }
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
        if (Input.GetKey(KeyCode.G) && KeyInputDelayTimer + 0.1f < Time.time) // Press enter to start
        {
            KeyInputDelayTimer = Time.time;
            GeneratePositionSets();
        }
    }

    public void SpawnBall() // Spawn ball in a random X spot in the middle Z on top of the box
    {
        if (GameControl.instance.useData)
        {
            pos = new Vector3(float.Parse(loadedData[0][ballIndex]), float.Parse(loadedData[1][ballIndex]), float.Parse(loadedData[2][ballIndex]));
            ballIndex++;
        }
        else
        {
            pos = new Vector3(Random.Range(center.x - boxSize.lossyScale.x / 2 + ballSize.lossyScale.x / 2, center.x + boxSize.lossyScale.x / 2 - ballSize.lossyScale.x / 2), center.y, center.z); 
        }
        Instantiate(BallPrefab, pos, Quaternion.identity);
    }

    private void GeneratePositionSets() // Generate a CSV file of spawn locations. First row is X, 2nd is Y, 3rd is Z. Each column corresponds to a point in space
    {
        int numToGenerate = Mathf.RoundToInt(GameControl.instance.duration / TimePerSpawn) + 1;
        for (int i = 0; i < numToGenerate; i++)
        {
            Vector3 pos = new Vector3(Random.Range(center.x - boxSize.lossyScale.x / 2 + ballSize.lossyScale.x / 2, center.x + boxSize.lossyScale.x / 2 - ballSize.lossyScale.x / 2), center.y, center.z);
            xSpawn.Add(pos.x);
            ySpawn.Add(pos.y);
            zSpawn.Add(pos.z);
        }
        table.Add(xSpawn);
        table.Add(ySpawn);
        table.Add(zSpawn);
        SaveToExcel.instance.Save(table, 3, "Ball_Spawn");
        table.Clear();
        xSpawn.Clear();
        ySpawn.Clear();
        zSpawn.Clear();
        Debug.Log("Generated positions!");
    }

    private void LoadData()
    {
        loadedData = SaveToExcel.instance.Load("SpawnGeneration/Ball_Spawn_" + GameControl.instance.spawnNumber + ".csv");
        Debug.Log("Loaded Data");
    }
}
