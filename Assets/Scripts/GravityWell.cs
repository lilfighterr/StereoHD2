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


    // Use this for initialization
    void Start () {
        myCollider = GetComponent<SphereCollider>();
        normalizedGain = gain/0.01f;
        topJoint = GameObject.Find("Top").transform;
        gravitySpawnerScript = GameObject.Find("GravityWellSpawner").GetComponent<GravityWellSpawner>();

        timeSpawned = Time.time;
        ballRenderer = GetComponent<Renderer>();
        originalColor = new Color(108/255f, 104/255f, 159/255f);
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
            ballRenderer.material.color = originalColor;
            gravitySpawnerScript.HighlightRandomBall();
            if (!GameControl.instance.gameStart)
            {
                GameControl.instance.gameCountdown = true;
            }
            else
            {
                gravitySpawnerScript.IncreaseScore();
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
            snap = false;
            MatlabServer.instance.xFTop = 0;
            MatlabServer.instance.yFTop = 0;
            MatlabServer.instance.zFTop = 0;
        }
    }
}
