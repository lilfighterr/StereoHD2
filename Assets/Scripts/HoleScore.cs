using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScore : MonoBehaviour
{
    public TextMesh scoreText;
    public BallDrop ballDropScript;

    private int score = 0;
    private Vector3 holePosition;
    private List<int> trials;
    private List<double> xHole, yHole;
    private void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMesh>();
        ballDropScript = GameObject.Find("Ball").GetComponent<BallDrop>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            score += 1;
            scoreText.text = " Score\n" + score.ToString();

            trials.Add(ballDropScript.trials); // Collect # of trials per hole. Hole # is index of trials
            holePosition = GetComponentInParent<Transform>().position; // Get hole position
            xHole.Add(holePosition.x);
            yHole.Add(holePosition.z);
        }
    }
}
