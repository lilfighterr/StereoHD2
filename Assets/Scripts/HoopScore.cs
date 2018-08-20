using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopScore : MonoBehaviour {

    public TextMesh scoreText;
    public GameObject scoreBlocker;

    private int score=0;
    private HoopBlocker blockerScript;
    private BallPusher ballScript;
    private List<List<double>> table = new List<List<double>>();
    private List<double> xDir = new List<double>();
    private List<double> yDir = new List<double>();
    private List<double> zDir = new List<double>();
    private double hoopHandleSeparation = 0.0372;

    // Use this for initialization
    void Start () {
        blockerScript = scoreBlocker.GetComponent<HoopBlocker>();
	}

    private void Update()
    {
        //if (GameControl.instance.gameStart)
        //{
            
            xDir.Add(transform.position.x);
            yDir.Add(transform.position.y);
            zDir.Add(transform.position.z);
        //}
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

                ballScript = other.GetComponent<BallPusher>();
                table.Add(ballScript.time);
                table.Add(xDir.GetRange(xDir.Count - ballScript.listLength, ballScript.listLength));
                table.Add(yDir.GetRange(yDir.Count - ballScript.listLength, ballScript.listLength));
                table.Add(zDir.GetRange(zDir.Count - ballScript.listLength, ballScript.listLength));
                table.Add(ballScript.xDir);
                table.Add(ballScript.yDir);
                table.Add(ballScript.zDir);
                SaveToExcel.instance.Save(table, 7, "Ball" + GameControl.instance.spawnNumber + "_AV" + GameControl.instance.ARVR + "_CL" + GameControl.instance.cognitiveLoading + "_Sc" + score);
                table.Clear();
            }
        }
    }
}
