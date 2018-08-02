using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopCollision : MonoBehaviour {

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 contactNormal = collision.contacts[0].normal.normalized;
        //float distanceFromContact = Vector3.Distance(contactPoint, bottomJoint.position);

        Vector3 contactForce = contactNormal * collision.rigidbody.mass;

        MatlabServer.instance.xFBot = contactForce.z;
        MatlabServer.instance.yFBot = -contactForce.x;
        MatlabServer.instance.zFBot = contactForce.y;
    }

    private void OnCollisionExit(Collision collision)
    {
        MatlabServer.instance.xFBot = 0;
        MatlabServer.instance.yFBot = 0;
        MatlabServer.instance.zFBot = 0;
    }
    }

