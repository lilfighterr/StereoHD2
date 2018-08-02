using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrop : MonoBehaviour {
    // This script will deactivate the grab script, therefore dropping the ball

    private GrabScript grabScript;
    private Rigidbody rigidBody;
    private float KeyInputDelayTimer;

    // Use this for initialization
    void Start () {
        grabScript = GetComponent<GrabScript>();
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            grabScript.enabled = !grabScript.enabled;
            rigidBody.velocity = Vector3.zero;
            rigidBody.useGravity = !rigidBody.useGravity;
        }
    }
}
