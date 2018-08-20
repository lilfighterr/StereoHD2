using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSpawner : MonoBehaviour
{

    public GameObject HolePrefab;
    public GameObject Box;
    public Vector3 center;
    public Vector3 size;
    public HoleScore HoleScript;

    private Transform boxSize;
    private Transform holeSize;
    private float xSpawn, ySpawn, zSpawn;
    private bool start = false;
    private float KeyInputDelayTimer;
    private GameObject Hole;
    private Transform HoleTransform;
    
    private List<double> xSpawnList = new List<double>();
    private List<double> ySpawnList = new List<double>();
    private List<double> zSpawnList = new List<double>();
    private List<List<double>> table = new List<List<double>>();
    private string[][] loadedData;
    private Vector3 pos;
    private int spawnData = 0;
    private int holeIndex = 0;

    // Use this for initialization
    void Start()
    {
        // Change scene number on gamecontrol
        GameControl.instance.sceneNumber = 1;

        if (GameControl.instance.useData)
        {
            LoadData(); // Use data from CSV
        }
        boxSize = Box.GetComponent<Transform>();
        holeSize = HolePrefab.GetComponent<Transform>();
        center = new Vector3(boxSize.position.x, boxSize.position.y, boxSize.position.z);
        SpawnHole();
        Hole = GameObject.Find("HoopHole(Clone)");
        HoleTransform = Hole.GetComponent<Transform>();
        HoleScript = Hole.GetComponentInChildren<HoleScore>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.KeypadEnter) && KeyInputDelayTimer + 0.1f < Time.time) // Press enter to start
        {
            KeyInputDelayTimer = Time.time;
            HoleTransform.position = MoveHole();
        }
        if (HoleScript.triggered)
        {
            HoleScript.triggered = false;
            HoleTransform.position = MoveHole();
        }
        if (Input.GetKey(KeyCode.G) && KeyInputDelayTimer + 0.1f < Time.time) // Press G to generate spawn points
        {
            KeyInputDelayTimer = Time.time;
            GeneratePositionSets();
        }
        if (Input.GetKey(KeyCode.L) && KeyInputDelayTimer + 0.1f < Time.time) // Press L to load spawn points
        {
            KeyInputDelayTimer = Time.time;
            LoadData();
        }
    }

    public void SpawnHole() // Spawn Hole. Only used once
    {
        if (GameControl.instance.useData)
        {
            pos = new Vector3(float.Parse(loadedData[0][holeIndex]), float.Parse(loadedData[1][holeIndex]), float.Parse(loadedData[2][holeIndex]));
            holeIndex++;
        }
        else
        {
            xSpawn = Random.Range(center.x - boxSize.lossyScale.x / 2 + holeSize.lossyScale.x / 2, center.x + boxSize.lossyScale.x / 2 - holeSize.lossyScale.x / 2);
            ySpawn = center.y;
            zSpawn = Random.Range(center.z - boxSize.lossyScale.z / 2 + holeSize.lossyScale.y, center.z + boxSize.lossyScale.z / 2 - holeSize.lossyScale.y);
            pos = new Vector3(xSpawn, ySpawn, zSpawn);
        }
        Instantiate(HolePrefab, pos, Quaternion.AngleAxis(-90,Vector3.left));
    }

    public Vector3 MoveHole() // Move hole. Called every time "enter" is pressed
    {
        if (GameControl.instance.useData)
        {
            pos = new Vector3(float.Parse(loadedData[0][holeIndex]), float.Parse(loadedData[1][holeIndex]), float.Parse(loadedData[2][holeIndex]));
            holeIndex++;
        }
        else
        {
            xSpawn = Random.Range(center.x - boxSize.lossyScale.x / 2 + holeSize.lossyScale.x / 2, center.x + boxSize.lossyScale.x / 2 - holeSize.lossyScale.x / 2);
            ySpawn = center.y;
            zSpawn = Random.Range(center.z - boxSize.lossyScale.z / 2 + holeSize.lossyScale.y, center.z + boxSize.lossyScale.z / 2 - holeSize.lossyScale.y);
            pos = new Vector3(xSpawn, ySpawn, zSpawn);
        }
        return pos;
    }

    private void GeneratePositionSets() // Generate a CSV file of spawn locations. First row is X, 2nd is Y, 3rd is Z. Each column corresponds to a point in space
    {
        for (int i = 0; i < 70; i++)
        {
            xSpawn = Random.Range(center.x - boxSize.lossyScale.x / 2 + holeSize.lossyScale.x / 2, center.x + boxSize.lossyScale.x / 2 - holeSize.lossyScale.x / 2);
            ySpawn = center.y;
            zSpawn = Random.Range(center.z - boxSize.lossyScale.z / 2 + holeSize.lossyScale.y, center.z + boxSize.lossyScale.z / 2 - holeSize.lossyScale.y);

            xSpawnList.Add(xSpawn);
            ySpawnList.Add(ySpawn);
            zSpawnList.Add(zSpawn);
        }
        table.Add(xSpawnList);
        table.Add(ySpawnList);
        table.Add(zSpawnList);
        SaveToExcel.instance.Save(table, 3, "Hole_Spawn");
        table.Clear();
        xSpawnList.Clear();
        ySpawnList.Clear();
        zSpawnList.Clear();
        Debug.Log("Generated positions!");
    }

    private void LoadData()
    {
        loadedData = SaveToExcel.instance.Load("SpawnGeneration/Hole_Spawn_"+GameControl.instance.spawnNumber+".csv");
        Debug.Log("Loaded Data");
    }
}
