using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public static GameControl instance; // Allows easy access from other scripts. They just have to do GameControl.instance.something

    public GameObject effectorTop;
    public GameObject effectorBottom;
    public GameObject Ball;
    public TextMesh countdownText;
    public float endEffectorLength;
    public SceneName sceneIndex;
    public bool gameOver = false;
    public bool gameStart = false;
    public bool gameCountdown = false;
    public float duration = 60;

    private Transform topTransform;
    private Transform bottomTransform;
    private float timeBefore = 0f;
    private float timeDiff;
    private Vector3 ballPosition;
    private float KeyInputDelayTimer;
    private float timeLeft = 4.5f; //For 3s countdown
    private float viewedTime;
    private float timeElapsed = 0;
    private float timer;

    // Use this for initialization
    void Awake()
    { //Always called before start() functions
      //Makes sure that there is only one instance of GameControl (singleton)
        if (instance == null) //If no game control found
        {
            instance = this; //Then this is the instance of the game control
            sceneIndex = (SceneName)SceneManager.GetActiveScene().buildIndex;
        }
        else if (instance != this) //If the game object finds that instance is already on another game object, then this destroys itself as it's not needed
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        topTransform = effectorTop.GetComponent<Transform>();
        bottomTransform = effectorBottom.GetComponent<Transform>();
        ballPosition = Ball.GetComponent<Transform>().position;
        timer = duration;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            LoadScene("Main");
        }
        if (Input.GetKey(KeyCode.Alpha2) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            LoadScene("Hole");
        }
        if (Input.GetKey(KeyCode.Alpha3) && KeyInputDelayTimer + 0.1f < Time.time)
        {
            KeyInputDelayTimer = Time.time;
            LoadScene("Snapping");
        }

        if (!gameOver)
        {
            if (gameCountdown) //During countdown
            {
                timeLeft -= Time.deltaTime;
                viewedTime = timeLeft - 1;
                if (viewedTime < -1) // Remove GO! after a second
                {
                    gameCountdown = false;
                    //countdownText.gameObject.SetActive(false);
                    timer -= Time.deltaTime;
                }
                else if (viewedTime > 0.5)
                {
                    countdownText.text = viewedTime.ToString("F0");

                }
                else // start game, but keep GO! on.
                {
                    countdownText.text = "GO!";
                    gameStart = true;
                    timer -= Time.deltaTime;
                }
            }
            else //When gamecountdown ends
            {
                if (gameStart)
                {
                    timer -= Time.deltaTime;
                    countdownText.text = timer.ToString("F0");
                    gameOver = TimerCheckGameEnd(timer);
                }
            }
        }
        else
        {
            gameStart = false;
        }
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log("Load Scene: " + sceneName);
        MatlabServer.instance.StopThread();
        SceneManager.LoadScene(sceneName);
    }

    private bool TimerCheckGameEnd(float timeLeft)
    {
        if (timeLeft < 0)
        {
            countdownText.text = "GameOver";
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Restart()
    {
        MatlabServer.instance.StopThread();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Reload current scene to restart
    }
}
