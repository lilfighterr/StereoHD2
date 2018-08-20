using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSnapper : MonoBehaviour {


    [HideInInspector] public List<double> xDir = new List<double>();
    [HideInInspector] public List<double> yDir = new List<double>();
    [HideInInspector] public List<double> zDir = new List<double>();
    [HideInInspector] public List<double> time = new List<double>();
    [HideInInspector] public int listLength = 0;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        time.Add(GameControl.instance.timeElapsed);
        xDir.Add(transform.position.x);
        yDir.Add(transform.position.y);
        zDir.Add(transform.position.z);
        listLength++;
    }
}
