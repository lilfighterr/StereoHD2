using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrabScript : MonoBehaviour {

    public Vector3 acceleration;
    public Vector3 velocity;
    public Vector3 speed;
    public float xForce, yForce;
    public float collisionStatus;
    public RoomAliveToolkit.RATUserViewCamera userCam;

    private Vector3 mousePosScreen, currentPos, previousPos;
    private Vector3 previousVel;
    private Rigidbody carRb2d;
    private Rigidbody characterRb2d;
    private Vector3 robotPos;
    private float T11 = 1, T12 = 0, T13 = 1, T21 = 0, T22 = 1, T23 = 1, T31 = 1, T32 = 1, T33 = 1;
    private float a = 0, b = 0, c = 0, d = 0;
    private bool forceFeedback;
    private Transform characterTransform;
    private Vector3 startTop, startBottom, robotTop, robotBottom;
    private Vector3 difference;
    private bool initializedBias = false;
    private void Start()
    {
        Calibrate();
        characterRb2d = GetComponent<Rigidbody>();
        characterTransform = GetComponent<Transform>();
        robotPos = Vector3.zero;
        previousPos = transform.position;
        previousVel = characterRb2d.velocity;

        startTop = startBottom = characterTransform.position;

    }

    public void Calibrate() //Initialize
    {
        try
        {
            T11 = PlayerPrefs.GetFloat("T11");
            T12 = PlayerPrefs.GetFloat("T12");
            T13 = PlayerPrefs.GetFloat("T13");

            T21 = PlayerPrefs.GetFloat("T21");
            T22 = PlayerPrefs.GetFloat("T22");
            T23 = PlayerPrefs.GetFloat("T23");

            T31 = PlayerPrefs.GetFloat("T31");
            T32 = PlayerPrefs.GetFloat("T32");
            T33 = PlayerPrefs.GetFloat("T33");
            /*
            a = PlayerPrefs.GetFloat("a");
            b = PlayerPrefs.GetFloat("b");
            c = PlayerPrefs.GetFloat("c");
            d = PlayerPrefs.GetFloat("d");*/
        }
        catch
        {
            Debug.Log("Please Calibrate");
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
		if (Input.GetKey(KeyCode.UpArrow)) //If using mouse to move 
        {
            characterRb2d.velocity = new Vector3(1,1,1);  
        }
        else if (MatlabServer.instance.serverRunning == true) //If connected to rehab robot
        {
            if (name == "Top")
            {
                robotTop = new Vector3(-MatlabServer.instance.recvBuffer[1], MatlabServer.instance.recvBuffer[2], MatlabServer.instance.recvBuffer[0]);
                if (!initializedBias)
                {
                    initializedBias = true;
                    difference = startTop - robotTop;
                    Debug.Log("TopDiff: " + difference);
                }
                characterTransform.position = robotTop + difference;
            }
            else
            {
                robotBottom = new Vector3(-MatlabServer.instance.recvBuffer[4], MatlabServer.instance.recvBuffer[5], MatlabServer.instance.recvBuffer[3]);
                if (!initializedBias)
                {
                    initializedBias = true;
                    difference = startBottom - robotBottom;
                    Debug.Log("BotDiff: " + difference);
                }
                characterTransform.position = robotBottom + difference;
            }
            
            //characterRb2d.velocity = (CalibratedMovement() - characterTransform.position) * 30;
        }
        else
        {
            //Do Nothing
        }
        velocity = (transform.position - previousPos) / Time.deltaTime;
        acceleration = (characterRb2d.velocity - previousVel) / Time.deltaTime;
        speed = new Vector3(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y), 0);

        previousPos = transform.position;
        previousVel = characterRb2d.velocity;
    }


    /*public Vector3 CalibratedMovement()
    {
        // Ps = T*Pr
        float xS = MatlabServer.instance.xMove * T11 + MatlabServer.instance.yMove * T12;
        float yS = MatlabServer.instance.xMove * T21 + MatlabServer.instance.yMove * T22;
        float lambda = MatlabServer.instance.xMove * T31 + MatlabServer.instance.yMove * T32 + T33;

        
        float xS = MatlabServer.instance.xMove * a - MatlabServer.instance.yMove * b + c;
        float yS = MatlabServer.instance.xMove * b + MatlabServer.instance.yMove * a + d;
        
        return new Vector3(xS/lambda, yS/lambda, 0f);
    }*/
}
