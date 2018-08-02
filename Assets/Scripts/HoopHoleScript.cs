using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopHoleScript : MonoBehaviour {

    private float KeyInputDelayTimer;
    private float timeSpawned;

    // Use this for initialization
    void Start () {
        timeSpawned = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.KeypadEnter) && KeyInputDelayTimer + 0.1f < Time.time && Time.time - timeSpawned > 0.1)
        {
            KeyInputDelayTimer = Time.time;
            Destroy(gameObject);
        }
    }
}
