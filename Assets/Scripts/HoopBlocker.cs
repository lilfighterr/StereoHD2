using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopBlocker : MonoBehaviour {

    public bool inTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
            inTrigger = true;
    }
    void OnTriggerExit(Collider other)
    {

            inTrigger = false;

    }
}
