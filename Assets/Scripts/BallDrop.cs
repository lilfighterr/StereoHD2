using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrop : MonoBehaviour {
    // This script will deactivate the grab script, therefore dropping the ball
    public int trials = 0;
    public Vector3 dropPosition = new Vector3();

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
            KeyInputDelayTimer = Time.time;
            if (!GameControl.instance.gameStart) //First drop starts the timer
            {
                GameControl.instance.gameStart = true;
                GameControl.instance.gameStartDouble = 1;
                MatlabServer.instance.parameters = GameControl.instance.sceneNumber * 1000 + GameControl.instance.spawnNumber * 100 + GameControl.instance.ARVR * 10 + GameControl.instance.cognitiveLoading;
            }
            if (!GameControl.instance.gameOver) //Cant drop/pickup if game over
            {
                dropPosition = transform.position;
                grabScript.enabled = !grabScript.enabled;
                rigidBody.velocity = Vector3.zero;
                rigidBody.useGravity = !rigidBody.useGravity;
                isDropped = !isDropped;
                if (isDropped) trials += 1;
            }
        }
    }
}
