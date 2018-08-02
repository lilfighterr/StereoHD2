using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScore : MonoBehaviour
{
    public TextMesh scoreText;

    private int score;

    private void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMesh>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            score += 1;
            scoreText.text = " Score\n" + score.ToString();
        }
    }
}
