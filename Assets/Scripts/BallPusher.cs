using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPusher : MonoBehaviour {

    public float pushForceMin;
    public float pushForceMax;

    [HideInInspector]
    public List<double> xDir, yDir, zDir, time;
    public int listLength = 0;

    private float KeyInputDelayTimer;
    private Rigidbody ballBody;
    private Vector3 ballPosition;
    private float timeSpawned;

    // Use this for initialization
    void Start () {
        timeSpawned = Time.time;
        ballBody = GetComponent<Rigidbody>();
        ballPosition = GetComponent<Transform>().position;
        ballBody.AddForce(0, 0, -Random.Range(pushForceMin, pushForceMax), ForceMode.Force);
    }
	
	// Update is called once per frame
	void Update () {
        time.Add(Time.time);
        xDir.Add(transform.position.x);
        yDir.Add(transform.position.y);
        zDir.Add(transform.position.z);
        listLength++;

        if (Input.GetKey(KeyCode.Q) && KeyInputDelayTimer + 0.1f < Time.time) // Push forward
        {
            KeyInputDelayTimer = Time.time;
            ballBody.AddForce(0, 0, -Random.Range(pushForceMin, pushForceMax), ForceMode.Force);
        }

        if (Input.GetKey(KeyCode.R) && KeyInputDelayTimer + 0.1f < Time.time) // Reset position
        {
            GetComponent<Transform>().position = ballPosition;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        if (Time.time - timeSpawned > 6) // Destroy after 6s
        {
            Destroy(gameObject);
        }
    }

}
