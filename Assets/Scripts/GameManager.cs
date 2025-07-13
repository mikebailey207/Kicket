using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI ballsPlayedText;

    [SerializeField]
    private TextMeshProUGUI overText;

    [SerializeField]
    private TextMeshProUGUI runsThisBallText;

    [SerializeField]
    private TextMeshProUGUI targetText;

    [SerializeField]
    private TextMeshProUGUI fixedTargetText;

    [SerializeField]
    private TextMeshProUGUI pavilionText;

    [SerializeField]
    private TextMeshProUGUI outText;

    [SerializeField]
    private TextMeshProUGUI staticRoundText;

    [SerializeField] private AudioSource firstSound;
    [SerializeField] private AudioSource swannSound;
    [SerializeField] private AudioSource geoffSound;
    [SerializeField] private AudioSource harrySound;
    [SerializeField] private AudioSource valerieSound;
    [SerializeField] private AudioSource waveyDaveSound;


    [SerializeField]
    GameObject quarterFinalScreen;

    [SerializeField]
    GameObject semiFinalScreen;

    [SerializeField]
    GameObject finalScreen;

    [SerializeField]
    GameObject winScreen;

    [SerializeField]
    private AudioSource levelUpSound;

    [SerializeField] GameObject instructionsScreen;

    private int lastOverShown = -1;

    public int runsScored = 0; // Example score variable
    public int ballsPlayed = 0;

    public int currentOver => ballsPlayed / 6;
    public bool isSwingOver => currentOver % 2 == 0; // true on 0, 2, 4...

    public bool batsmanOut = false;
    public bool gameOver = false;
    private bool canLevelUp = true;
    private bool showingIntro = true;

    public int target = 25;

    public int ballsRemaining = 18;

    public int level = 1;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    private void Start()
    {
        fixedTargetText.text = "Target: " + target.ToString("0");
        scoreText.text = "Score: " + runsScored.ToString("0") + " NOT OUT";
        targetText.text = "Target: " + target.ToString("0") + " FROM " + ballsRemaining.ToString("0") + " BALLS";
        pavilionText.text = "CHAPS K.C.";
        staticRoundText.text = "TITHERWALLINGTON CHAPS. QUARTER FINAL.";

        Time.timeScale = 0;
    }

    private void Update()
    {
        if(canLevelUp && runsScored >= target)
        {
            canLevelUp = false;
            LevelUp();
        }
        if (ballsRemaining <= 0)
        {
            ResetGame();

            SceneManager.LoadScene(0);
        }
        if(Input.GetKeyDown(KeyCode.Space) && showingIntro)
        {
           
            Time.timeScale = 1;
            runsThisBallText.text = "";

            if (level == 1)
            {
                quarterFinalScreen.SetActive(false);
                instructionsScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else if (level == 2) semiFinalScreen.SetActive(false);
            else finalScreen.SetActive(false);
            ShowNewOverText();
            showingIntro = false;
        }

    }

    public void NextBall()
    {
        int runsNeeded = target - runsScored;
        targetText.text = "RUNS REQUIRED: " + runsNeeded.ToString("0") + " FROM " + ballsRemaining.ToString("0") + " BALLS";
        ballsPlayed++;
        ballsPlayedText.text = "Ball: " + (ballsPlayed + 1);
    }

    public void AddRuns(int runs)
    {
        runsScored += runs;
        ballsRemaining--; // move this up

        scoreText.text = "Score: " + runsScored.ToString("0") + " NOT OUT";

       

        int runsNeeded = target - runsScored;
        targetText.text = "RUNS REQUIRED: " + runsNeeded.ToString("0") + " FROM " + ballsRemaining.ToString("0") + " BALLS";

        Debug.Log("Total runs: {runsScored}, Balls played: {ballsPlayed}");
    }
    public void ShowNewOverText()
    {
        string bowlerName = "";
        string bowlerType = "";

        if (isSwingOver)
        {
            // Set bowler type by level
            switch (level)
            {
                case 1:
                    bowlerType = "Medium-fast";
                    break;
                case 2:
                    bowlerType = "Fast-medium";
                    break;
                case 3:
                    bowlerType = "Fast";
                    break;
                default:
                    bowlerType = "Fast-medium";
                    break;
            }
        }
        else
        {
            bowlerType = "Spin";
        }

        // Set bowler name and play sound
        if (level == 2)
        {
            if (isSwingOver)
            {
                bowlerName = "Geoff 'Babs' Wetherby";
                geoffSound.Play();
            }
            else
            {
                bowlerName = "Harry 'Half pint' Grundy";
                harrySound.Play();
            }
        }
        else if (level == 3)
        {
            if (isSwingOver)
            {
                bowlerName = "Valerie 'The Thomas' Charlston";
                valerieSound.Play();
            }
            else
            {
                bowlerName = "Wavey-Dave 'Colin' Costello";
                waveyDaveSound.Play();
            }
        }
        else // Level 1 or fallback
        {
            if (isSwingOver)
            {
                bowlerName = "Biggs 'Bigs' Bigs";
                firstSound.Play();
            }
            else
            {
                bowlerName = "Swan 'The Swan' Swann";
                swannSound.Play();
            }
        }



        overText.text = $"New Over\n{bowlerName}, {bowlerType}";
        StartCoroutine(ClearOverText());
    }

    public void TryShowNewOver()
    {
        if (currentOver != lastOverShown)
        {
            lastOverShown = currentOver;
            ShowNewOverText();
        }
    }

    public void ShowRunsThisBall(int runs)
    {
        if (!gameOver)
        {
            runsThisBallText.text = runs.ToString("0") + " runs";
            StartCoroutine(ClearRunsText());
        }
    }
    public void ShowOutText(string caughtOrBowled)
    {
        outText.text = caughtOrBowled;
        gameOver = true;
        StartCoroutine(ClearOutText());
    }
    private IEnumerator ClearOverText()
    {
        yield return new WaitForSeconds(4.5f);
        overText.text = "";
    }
    private IEnumerator ClearRunsText()
    {
        yield return new WaitForSeconds(3f);
        runsThisBallText.text = "";
    }
    private IEnumerator ClearOutText()
    {
        yield return new WaitForSeconds(3f);
        outText.text = "";
    }
    public void LevelUp()
    {
        if(level == 3)
        {
            winScreen.SetActive(true);
            Time.timeScale = 0;
        }
        runsScored = 0;
        ballsPlayed = 0;
        ballsPlayedText.text = "Ball: " + (ballsPlayed + 1);
        scoreText.text = "Score: 0 NOT OUT";
        levelUpSound.Play();
        if(level == 1)
        {
            pavilionText.text = "RAPSCALLIONS K.C.";
            staticRoundText.text = "FLETCHINGSWORTH RAPSCALLIONS. SEMI FINAL.";
            target = 36;
            ballsRemaining = 24;
            targetText.text = "RUNS REQUIRED: " + target.ToString("0") + " FROM " + ballsRemaining.ToString("0") + " BALLS";
            fixedTargetText.text = "Target: " + target.ToString("0");
            semiFinalScreen.SetActive(true);
            showingIntro = true;
            Time.timeScale = 0;
        }
        else if(level == 2)
        {
            pavilionText.text = "CHUFFS K.C.";
            staticRoundText.text = "MCVITIE CHUFFS. FINAL.";
            target = 50;
            ballsRemaining = 30;
            targetText.text = "RUNS REQUIRED: " + target.ToString("0") + " FROM " + ballsRemaining.ToString("0") + " BALLS";
            fixedTargetText.text = "Target: " + target.ToString("0");
            finalScreen.SetActive(true);
            showingIntro = true;
            Time.timeScale = 0;
        }
        level++;
        SceneManager.LoadScene(0);
        canLevelUp = true;
    }

    public void ResetGame()
    {
        gameOver = false;
        runsScored = 0;
        ballsPlayed = 0;
        if (level == 1)
        {
            ballsRemaining = 18;
            target = 18;
        }
        else if (level == 2)
        {
            ballsRemaining = 24;
            target = 36;
  
        }
        else
        {
            ballsRemaining = 30;
            target = 50;
           
        }
        ballsPlayedText.text = "Ball: " + (ballsPlayed + 1);
        scoreText.text = "Score: 0 NOT OUT";
        targetText.text = "RUNS REQUIRED: " + target.ToString("0") + " FROM " + ballsRemaining.ToString("0") + " BALLS";

    }
}