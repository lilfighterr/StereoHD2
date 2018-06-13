using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour {

    public GameObject effectorTop;
    public GameObject effectorBottom;
    public GameObject Ball;
    public float endEffectorLength;

    private Transform topTransform;
    private Transform bottomTransform;
    private float timeBefore = 0f;
    private float timeDiff;
    private Vector3 ballPosition;

	// Use this for initialization
	void Start () {
        topTransform = effectorTop.GetComponent<Transform>();
        bottomTransform = effectorBottom.GetComponent<Transform>();
        ballPosition = Ball.GetComponent<Transform>().position;
	}


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update realTime: " + Time.realtimeSinceStartup);
        endEffectorLength = Vector3.Distance(topTransform.position, bottomTransform.position);
        if (Input.GetKey(KeyCode.Q))
        {
            Debug.Log(endEffectorLength);
        }
        if (Input.GetKey(KeyCode.R))
        {
            Ball.GetComponent<Transform>().position = ballPosition;
            Ball.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        }
    }
}
