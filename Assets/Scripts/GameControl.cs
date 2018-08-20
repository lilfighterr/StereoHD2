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
    public GameObject blinker;
    public float endEffectorLength;
    //public SceneName sceneIndex;
    public bool gameOver = false;
    public bool gameStart = false;
    public bool gameCountdown = false;
    public float duration = 60;
    public float timeElapsed = 0;
    public float blinkTimerOn = 2f;
    public float blinkTimerOff = 2f;
    public bool useData = false;
    public double gameStartDouble = 0;
    public double spawnNumber;
    public double cognitiveLoading;
    public double sceneNumber;
    public double ARVR;

    private Transform topTransform;
    private Transform bottomTransform;
    private float timeBefore = 0f;
    private float timeDiff;
    private Vector3 ballPosition;
    private float KeyInputDelayTimer;
    private float timeLeft = 4.5f; //For 3s countdown
    private float viewedTime;
    private float timer;
    private float blinkerTime;
    private bool paramsInit = false;

    // Use this for initialization
    void Awake()
    { //Always called before start() functions
      //Makes sure that there is only one instance of GameControl (singleton)
        if (instance == null) //If no game control found
        {
            instance = this; //Then this is the instance of the game control
            //sceneIndex = (SceneName)SceneManager.GetActiveScene().buildIndex;
            if (useData)
            {
                spawnNumber = ReadSpawnNumber();
                cognitiveLoading = ReadCognitiveLoading();
                ARVR = ReadARVR();
            }
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
            LoadScene("Catcher");
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
                    gameStartDouble = 1;
                    if (!paramsInit)
                    {
                        paramsInit = true;
                        MatlabServer.instance.parameters = sceneNumber * 1000 + spawnNumber * 100 + ARVR * 10 + cognitiveLoading;
                    }

                    timer -= Time.deltaTime;
                }
            }
            else //When gamecountdown ends
            {
                if (gameStart)
                {
                    timeElapsed += Time.deltaTime;
                    timer -= Time.deltaTime;
                    countdownText.text = timer.ToString("F0");

                    blinkerTime -= Time.deltaTime; //For blinker
                    if (blinkerTime < 0 && blinker.activeSelf)
                    {
                        blinker.SetActive(false);
                        blinkerTime = blinkTimerOff;
                    }
                    else if (blinkerTime < 0 && !blinker.activeSelf)
                    {
                        blinker.SetActive(true);
                        blinkerTime = blinkTimerOn;
                    }

                    gameOver = TimerCheckGameEnd(timer);
                }
            }
        }
        else
        {
            gameStart = false;
            gameStartDouble = 0;
        }
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log("Load Scene: " + sceneName);
        //MatlabServer.instance.StopThread();
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

    private double ReadSpawnNumber()
    {
        string text = System.IO.File.ReadAllText("SpawnNumber.txt");

        return double.Parse(text);
    }

    private double ReadCognitiveLoading()
    {
        string text;
        try
        {
            text = System.IO.File.ReadAllText("CognitiveLoading.txt");
            Debug.Log(text);
        }
        catch
        {
            text = "-1";
        }
        return double.Parse(text);
    }

    private double ReadARVR()
    {
        string text;
        try
        {
            text = System.IO.File.ReadAllText("ARVR.txt");
            Debug.Log(text);
        }
        catch
        {
            text = "-1";
        }
        return double.Parse(text);
    }
}
