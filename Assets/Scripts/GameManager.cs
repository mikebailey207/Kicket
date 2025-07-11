using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    AudioSource firstSound;

    private int lastOverShown = -1;

    public int runsScored = 0; // Example score variable
    public int ballsPlayed = 0;

    public int currentOver => ballsPlayed / 6;
    public bool isSwingOver => currentOver % 2 == 0; // true on 0, 2, 4...

    public bool batsmanOut = false;

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
        scoreText.text = "Score: " + runsScored.ToString("0") + " NOT OUT";
        firstSound.Play();
    }

    public void NextBall()
    {
        ballsPlayed++;
        ballsPlayedText.text = "Ball: " + (ballsPlayed + 1);
    }

    public void AddRuns(int runs)
    {
        runsScored += runs;
        
        scoreText.text = "Score: " + runsScored.ToString("0") + " NOT OUT";
      
        Debug.Log("Total runs: {runsScored}, Balls played: {ballsPlayed}");
    }

    public void ShowNewOverText()
    {
        string bowlerType = isSwingOver ? "Fast-medium" : "Spin";
        string bowlerName = isSwingOver ? "Biggs 'Bigs' Bigs" : "Swan 'The Swan' Swann"; // or randomise later
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

    private IEnumerator ClearOverText()
    {
        yield return new WaitForSeconds(4.5f);
        overText.text = "";
    }

    public void ResetGame()
    {
        runsScored = 0;
        ballsPlayed = 0;
        ballsPlayedText.text = "Ball: " + (ballsPlayed + 1);
        scoreText.text = "Score: 0 NOT OUT";
    
    }
}