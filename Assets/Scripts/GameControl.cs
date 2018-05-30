using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    public GameObject effectorTop;
    public GameObject effectorBottom;
    public float endEffectorLength;

    private Transform topTransform;
    private Transform bottomTransform;
    private float timeBefore = 0f;
    private float timeDiff;

	// Use this for initialization
	void Start () {
        topTransform = effectorTop.GetComponent<Transform>();
        bottomTransform = effectorBottom.GetComponent<Transform>();
	}

    void FixedUpdate()
    {
        timeDiff = Time.realtimeSinceStartup - timeBefore;
        timeBefore = Time.realtimeSinceStartup;
        if (timeDiff > 0.02)
        {
            Debug.Log("GREATER at" + Time.realtimeSinceStartup + "seconds. Diff: " + timeDiff );
        }
        //Debug.Log("Fixed intervals: " + timeDiff);
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
    }
}
