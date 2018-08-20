using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScore : MonoBehaviour
{
    public TextMesh scoreText;
    public BallDrop ballDropScript;
    [HideInInspector]public bool triggered = false;

    private int score = 0;
    private Vector3 holePosition;
    private List<double> trials = new List<double>();
    private List<double> time = new List<double>();
    private List<double> durationPerHole = new List<double>();
    private List<double> xHole = new List<double>();
    private List<double> yHole = new List<double>();
    private List<double> xDir = new List<double>();
    private List<double> yDir = new List<double>();
    private List<double> zDir = new List<double>();
    private List<List<double>> table = new List<List<double>>();
    private double holeHeight = -0.3238;
    private bool saved = false;
    private double previousTime = 0;
   

    private void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMesh>();
        ballDropScript = GameObject.Find("Ball").GetComponent<BallDrop>();
    }

    private void Update()
    {
        if (GameControl.instance.gameOver)
        {
            if (!saved)
            {
                saved = true;
                table.Add(time);
                table.Add(durationPerHole);
                table.Add(trials);
                table.Add(xHole);
                table.Add(yHole);
                table.Add(xDir);
                table.Add(yDir);
                table.Add(zDir);

                SaveToExcel.instance.Save(table, 8, "Hole" + GameControl.instance.spawnNumber + "_AV" + GameControl.instance.ARVR + "_CL" + GameControl.instance.cognitiveLoading);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            score += 1;
            triggered = true;
            scoreText.text = " Score\n" + score.ToString();
            time.Add(GameControl.instance.timeElapsed);
            durationPerHole.Add(GameControl.instance.timeElapsed-previousTime);
            previousTime = GameControl.instance.timeElapsed;
            trials.Add(ballDropScript.trials); // Collect # of trials per hole. Hole # is index of trials
            ballDropScript.trials = 0;
            holePosition = GetComponentInParent<Transform>().position; // Get hole position
            xHole.Add(holePosition.x);
            yHole.Add(holePosition.z);
            xDir.Add(ballDropScript.dropPosition.x);
            yDir.Add(ballDropScript.dropPosition.y - holeHeight);
            zDir.Add(ballDropScript.dropPosition.z);
        }
    }
}
