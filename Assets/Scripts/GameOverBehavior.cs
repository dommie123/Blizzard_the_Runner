using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverBehavior : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreAndCoinsText;
    [SerializeField] private TMP_Text totalScoreText;

    private AudioSource menuSFX;

    private void Awake()
    {
        menuSFX = GameObject.Find("Menu SFX").GetComponent<AudioSource>();

        // Update final score
        int distCovered = ScoreManager.instance.GetScore();
        int coinsCollected = CoinManager.instance.GetCoins();
        int finalScore = distCovered + coinsCollected;

        scoreAndCoinsText.text = $"Distance Covered: {distCovered} m\nCoins Collected: {coinsCollected}";
        totalScoreText.text = $"Total Score: {finalScore}";
    }

    public void Retry()
    {
        menuSFX.Play();
        SceneManager.LoadSceneAsync("Main");
    }

    public void GoToMainMenu()
    {
        menuSFX.Play();
        SceneManager.LoadSceneAsync("Title");
    }
}
