using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{    
    public static ScoreManager instance;
    public TMP_Text scoreText;

    private int score;
    private void Awake() 
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.text = $"Score: {score.ToString()}";
    }

    // Update is called once per frame
    void Update()
    {
        
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
