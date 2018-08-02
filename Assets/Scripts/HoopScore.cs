using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopScore : MonoBehaviour {

    public TextMesh scoreText;
    public GameObject scoreBlocker;

    private int score=0;
    private HoopBlocker blockerScript;

	// Use this for initialization
	void Start () {
        blockerScript = scoreBlocker.GetComponent<HoopBlocker>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (blockerScript.inTrigger)
            {
                other.gameObject.tag = "Untagged";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (blockerScript.inTrigger)
            {
                score += 1;
                scoreText.text = " Score\n" + score.ToString();
            }
        }
    }
}
