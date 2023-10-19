using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{    
    public static ScoreManager instance;
    [SerializeField] private TMP_Text scoreText;

    private int score;

    private void Awake()
    {
        instance = this;

        score = 0;
        scoreText.text = $"Score: {score.ToString()}";
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        scoreText.text = $"Score: {score.ToString()}";
    }

    public int GetScore()
    {
        return score;
    }
}
