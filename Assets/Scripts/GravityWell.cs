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
        myCollider = GetComponent<SphereCollider>();
        normalizedGain = gain/0.01f;
        topJoint = GameObject.Find("Top").transform;
        gravitySpawnerScript = GameObject.Find("GravityWellSpawner").GetComponent<GravityWellSpawner>();

        timeSpawned = Time.time;
        ballRenderer = GetComponent<Renderer>();
        originalColor = new Color(108/255f, 104/255f, 159/255f);
        gravityWellPosition.Add(transform.position.x);
        gravityWellPosition.Add(transform.position.y);
        gravityWellPosition.Add(transform.position.z);
    }

    // Update is called once per frame
    void Update() {
        xDiff = topJoint.position.x - transform.position.x;
        yDiff = topJoint.position.y - transform.position.y;
        zDiff = topJoint.position.z - transform.position.z;

        xSign =  (xDiff >= 0) ? 1 : -1;
        ySign = (yDiff >= 0) ? 1 : -1;
        zSign = (zDiff >= 0) ? 1 : -1;

        displacementFromTopJoint = new Vector3(xDiff, yDiff, zDiff);

        /*Mathf.Clamp(Mathf.Abs(xDiff), distanceMinimum, myCollider.radius)*xSign,
        Mathf.Clamp(Mathf.Abs(yDiff), distanceMinimum, myCollider.radius) * ySign,
        Mathf.Clamp(Mathf.Abs(zDiff), distanceMinimum, myCollider.radius) * zSign*/
        if (snap)
        {
            MatlabServer.instance.xFTop = -normalizedGain * displacementFromTopJoint.z;
            MatlabServer.instance.yFTop = normalizedGain * displacementFromTopJoint.x;
            MatlabServer.instance.zFTop = -normalizedGain * displacementFromTopJoint.y;

        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player") { 
            snap = true;
        }
        if (highlightedBall == true)
        {
            highlightedBall = false;
            wasHighlightedBall = true;
            ballRenderer.material.color = originalColor;
            gravitySpawnerScript.HighlightRandomBall();
            if (!GameControl.instance.gameStart) // If game has not started, start game (for initial point)
            {
                GameControl.instance.gameCountdown = true;
            }
            else // Increase score after game started
            {
                gravitySpawnerScript.IncreaseScore();
                timeEntered = GameControl.instance.timeElapsed; // Time user entered the point
                timeDuration = timeEntered - gravitySpawnerScript.timeLastBallLeft;
                timeDurationList.Add(timeDuration);
            }
        }
        else
        {
            if (GameControl.instance.gameStart)
            {
                gravitySpawnerScript.DecreaseScore();       
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pointSnapperScript = other.GetComponent<PointSnapper>();
            snap = false;
            MatlabServer.instance.xFTop = 0;
            MatlabServer.instance.yFTop = 0;
            MatlabServer.instance.zFTop = 0;
            gravitySpawnerScript.timeLastBallLeft = GameControl.instance.timeElapsed; // Time user exited the point

            if (GameControl.instance.gameStart && wasHighlightedBall)
            {
                wasHighlightedBall = false;
                pointSnapperListLength = pointSnapperScript.listLength;

                timeDurationList.Add(gravitySpawnerScript.score);
                timeDurationList.Add(gravitySpawnerScript.hits);
                timeDurationList.Add(gravitySpawnerScript.misses);
                table.Add(timeDurationList); // Contains timeduration, score, hits, misses
                table.Add(gravityWellPosition); // contains x,y,z of current gravity well

                // Next rows add: time, x, y, z trajectories
                table.Add(pointSnapperScript.time.GetRange(pointSnapperScript.time.Count - pointSnapperListLength, pointSnapperListLength));
                table.Add(pointSnapperScript.xDir.GetRange(pointSnapperScript.xDir.Count - pointSnapperListLength, pointSnapperListLength));
                table.Add(pointSnapperScript.yDir.GetRange(pointSnapperScript.yDir.Count - pointSnapperListLength, pointSnapperListLength));
                table.Add(pointSnapperScript.zDir.GetRange(pointSnapperScript.zDir.Count - pointSnapperListLength, pointSnapperListLength));
                pointSnapperScript.listLength = 0;
                SaveToExcel.instance.Save(table, 6, "Snapping" + GameControl.instance.spawnNumber + "_CL" + GameControl.instance.cognitiveLoading + "_Sc" + gravitySpawnerScript.hits + "_");
            }
        }
    }
}
