using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrop : MonoBehaviour {
    // This script will deactivate the grab script, therefore dropping the ball
    public int trials = 0;

    private GrabScript grabScript;
    private Rigidbody rigidBody;
    private float KeyInputDelayTimer;
    private bool isDropped = false;

    // Use this for initialization
    void Start () {
        grabScript = GetComponent<GrabScript>();
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space) && KeyInputDelayTimer + 0.1f < Time.time) // Drop ball
        {
            if (!GameControl.instance.gameStart)
            {
                GameControl.instance.gameStart = true;
            }
            if (!GameControl.instance.gameOver)
            {
                KeyInputDelayTimer = Time.time;
                grabScript.enabled = !grabScript.enabled;
                rigidBody.velocity = Vector3.zero;
                rigidBody.useGravity = !rigidBody.useGravity;
                isDropped = !isDropped;
                if (isDropped) trials += 1;
            }
        }
    }
}
