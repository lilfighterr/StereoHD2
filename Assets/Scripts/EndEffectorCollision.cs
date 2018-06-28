using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEffectorCollision : MonoBehaviour {

    public Transform topJoint, bottomJoint;
    
    private float endEffectorLength;
    private float lengthFromTop, lengthFromBottom;
    private Vector3 contactForce;
    public Vector3 forceTop, forceBot;
    private int contactCount;

	// Use this for initialization
	void Start () {
        forceTop = Vector3.zero;
        forceBot = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.collider.gameObject.name);

        Vector3 contactNormal = collision.contacts[0].normal.normalized;
        Debug.DrawRay(collision.contacts[0].point, contactNormal, Color.red, 2, true);

        endEffectorLength = Vector3.Distance(topJoint.position, bottomJoint.position);
        lengthFromTop = Vector3.Distance(topJoint.position, collision.contacts[0].point);
        lengthFromBottom = Vector3.Distance(bottomJoint.position, collision.contacts[0].point);
        contactForce = contactNormal * collision.rigidbody.mass;

        if (lengthFromTop <= endEffectorLength && lengthFromBottom <= endEffectorLength) // Collision in middle
        {
            forceTop = contactForce * (lengthFromBottom / endEffectorLength);
            forceBot = contactForce * (lengthFromTop / endEffectorLength);
        }
        else if (lengthFromTop < endEffectorLength && lengthFromBottom > endEffectorLength) // collision top
        {
            //Debug.Log("Top!");
            forceTop = contactForce * (1 + lengthFromTop / (lengthFromBottom - lengthFromTop));
            forceBot = (contactForce * lengthFromTop) / (lengthFromBottom - lengthFromTop); 
        }
        else if (lengthFromTop > endEffectorLength && lengthFromBottom < endEffectorLength) // Collision bottom
        {
            //Debug.Log("Bottom!");
            forceTop = (contactForce * lengthFromBottom) / (lengthFromTop - lengthFromBottom);
            forceBot = contactForce * (1 + lengthFromBottom / (lengthFromTop - lengthFromBottom)); 
        }
        MatlabServer.instance.xFTop = forceTop.z;
        MatlabServer.instance.yFTop = -forceTop.x;
        MatlabServer.instance.zFTop = forceTop.y;
        MatlabServer.instance.xFBot = forceBot.z;
        MatlabServer.instance.yFBot = -forceBot.x;
        MatlabServer.instance.zFBot = forceBot.y;
    }

    private void OnCollisionExit(Collision collision)
    {
        forceTop = Vector3.zero;
        forceBot = Vector3.zero;
        MatlabServer.instance.xFTop = forceTop.z;
        MatlabServer.instance.yFTop = -forceTop.x;
        MatlabServer.instance.zFTop = forceTop.y;
        MatlabServer.instance.xFBot = forceBot.z;
        MatlabServer.instance.yFBot = -forceBot.x;
        MatlabServer.instance.zFBot = forceBot.y;
    }
}
