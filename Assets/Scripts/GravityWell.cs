using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour {
    public Transform topJoint;
    public float gain;
    public Vector3 force;

    private Vector3 displacementFromTopJoint;
    private SphereCollider myCollider;
    private float distanceMinimum = 0.01f; //DO NOT SET TO 0 OR ELSE WILL GO TO INF
    private bool snap = false;
    private float KeyInputDelayTimer;
    private float normalizedGain;
    public float xDiff, yDiff, zDiff;
    private int xSign, ySign, zSign;

    // Use this for initialization
    void Start () {
        myCollider = GetComponent<SphereCollider>();
        normalizedGain = gain/0.01f;
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

        if (Input.GetKey(KeyCode.Space) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            Debug.Log(snap);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") { 
            snap = true;
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
