using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    public GameObject effectorTop;
    public GameObject effectorBottom;
    public float endEffectorLength;

    private Transform topTransform;
    private Transform bottomTransform;

	// Use this for initialization
	void Start () {
        topTransform = effectorTop.GetComponent<Transform>();
        bottomTransform = effectorBottom.GetComponent<Transform>();
	}

    // Update is called once per frame
    void Update()
    {
        endEffectorLength = Vector3.Distance(topTransform.position, bottomTransform.position);
        if (Input.GetKey(KeyCode.Q))
        {
            Debug.Log(endEffectorLength);
        }
    }
}
