using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI scoreText;

    public int runsScored = 0; // Example score variable
    public int ballsPlayed = 0;

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
    }

    public void AddRuns(int runs)
    {
        runsScored += runs;
        ballsPlayed++;
        scoreText.text = "Score: " + runsScored.ToString("0") + " NOT OUT";
        Debug.Log("Total runs: {runsScored}, Balls played: {ballsPlayed}");
    }

    public void ResetGame()
    {
        scoreText.text = "Score: 0 NOT OUT";
        runsScored = 0;
        ballsPlayed = 0;
    }
}