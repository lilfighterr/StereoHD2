using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour {
    public Transform topJoint;
    public float gain;
    public Vector3 force;
    public float timeSpawned;
    public float xDiff, yDiff, zDiff;
    public int ballNumber = 0;
    public bool highlightedBall = false;

    private Vector3 displacementFromTopJoint;
    private SphereCollider myCollider;
    private GravityWellSpawner gravitySpawnerScript;
    private float distanceMinimum = 0.01f; //DO NOT SET TO 0 OR ELSE WILL GO TO INF
    private bool snap = false;
    private float KeyInputDelayTimer;
    private float normalizedGain;
    private int xSign, ySign, zSign;
    private Color originalColor;
    private Renderer ballRenderer;
    private List<List<double>> table = new List<List<double>>();
    private List<double> timeDurationList = new List<double>();
    private List<double> gravityWellPosition = new List<double>();
    private double timeEntered;
    private double timeLeft;
    private double timeDuration;
    private PointSnapper pointSnapperScript;
    private int pointSnapperListLength;
    private bool wasHighlightedBall = false;
    

    // Use this for initialization
    void Start () {
        myCollider = GetComponent<SphereCollider>(); // Get collider of this object
        normalizedGain = gain/0.01f; // 
        topJoint = GameObject.Find("Top").transform; // Get the top part of the end effector
        gravitySpawnerScript = GameObject.Find("GravityWellSpawner").GetComponent<GravityWellSpawner>(); // Get access to the spawner script

        timeSpawned = Time.time; // Time this object was spawned
        ballRenderer = GetComponent<Renderer>(); // Get the renderer to control color for later
        originalColor = new Color(108/255f, 104/255f, 159/255f); // Unhighlighted color
        gravityWellPosition.Add(transform.position.x); // Save the x y z position in a table
        gravityWellPosition.Add(transform.position.y);
        gravityWellPosition.Add(transform.position.z);
    }

    // Update is called once per frame
    void Update() {
        // Checks how close the end-effector is from it
        xDiff = topJoint.position.x - transform.position.x;
        yDiff = topJoint.position.y - transform.position.y;
        zDiff = topJoint.position.z - transform.position.z;
        displacementFromTopJoint = new Vector3(xDiff, yDiff, zDiff);

        if (snap) // If the end-effector hits the object, send forces to snap
        {
            MatlabServer.instance.xFTop = -normalizedGain * displacementFromTopJoint.z;
            MatlabServer.instance.yFTop = normalizedGain * displacementFromTopJoint.x;
            MatlabServer.instance.zFTop = -normalizedGain * displacementFromTopJoint.y;
        }
    }

    // When end-effector enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") { // If it was the end-effector
            snap = true; // Turn on snap
        }
        if (highlightedBall == true) // If this is the highlighted ball
        {
            highlightedBall = false; // Mark that it's not highlighted anymore
            wasHighlightedBall = true; // Mark that it was highlighted
            ballRenderer.material.color = originalColor; // Unhighlight
            if (!GameControl.instance.gameOver) gravitySpawnerScript.HighlightRandomBall(); // Stop highlighting if gameover
            if (!GameControl.instance.gameStart) // If game has not started, start game (for initial point)
            {
                GameControl.instance.gameCountdown = true;
                GameObject cylinder = GameObject.Find("CalibrationCylinder");
                cylinder.SetActive(false);
            }
            else // Increase score after game started
            {
                gravitySpawnerScript.IncreaseScore();
                timeEntered = GameControl.instance.timeElapsed; // Time user entered the point
                timeDuration = timeEntered - gravitySpawnerScript.timeLastBallLeft; 
                timeDurationList.Add(timeDuration); // How long it took to reach this point from the previous point
            }
        }
        else
        {
            if (GameControl.instance.gameStart)
            { // Decrease score if it was an unhighlighted ball
                gravitySpawnerScript.DecreaseScore();       
            }
        }
    }

    // When end-effector exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") // If it was the end-effector
        {
            pointSnapperScript = other.GetComponent<PointSnapper>();
            snap = false; // Turn off snap and reset forces to 0
            MatlabServer.instance.xFTop = 0;
            MatlabServer.instance.yFTop = 0;
            MatlabServer.instance.zFTop = 0;
            gravitySpawnerScript.timeLastBallLeft = GameControl.instance.timeElapsed; // Time user exited the point

            if (GameControl.instance.gameStart && wasHighlightedBall)
            { // If it exited a highlighted ball
                wasHighlightedBall = false; 
                pointSnapperListLength = pointSnapperScript.listLength;

                timeDurationList.Add(gravitySpawnerScript.score); // Add current net score
                timeDurationList.Add(gravitySpawnerScript.hits); // Add current # of highlighted balls hit 
                timeDurationList.Add(gravitySpawnerScript.misses); // Add current # of unhighlighted balls hit
                table.Add(timeDurationList); // Contains timeduration, score, hits, misses
                table.Add(gravityWellPosition); // contains x,y,z of current gravity well

                // Next rows add: time, x, y, z trajectories
                table.Add(pointSnapperScript.time.GetRange(pointSnapperScript.time.Count - pointSnapperListLength, pointSnapperListLength));
                table.Add(pointSnapperScript.xDir.GetRange(pointSnapperScript.xDir.Count - pointSnapperListLength, pointSnapperListLength));
                table.Add(pointSnapperScript.yDir.GetRange(pointSnapperScript.yDir.Count - pointSnapperListLength, pointSnapperListLength));
                table.Add(pointSnapperScript.zDir.GetRange(pointSnapperScript.zDir.Count - pointSnapperListLength, pointSnapperListLength));
                pointSnapperScript.listLength = 0;
                SaveToExcel.instance.Save(table, 6, "Snapping" + GameControl.instance.spawnNumber + "_AV" + GameControl.instance.ARVR + "_CL" + GameControl.instance.cognitiveLoading + "_Sc" + gravitySpawnerScript.hits);
                timeDurationList.Clear();
                table.Clear();
            }
        }
    }
}
