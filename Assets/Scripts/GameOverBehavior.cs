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
    public Button btnRetry;
    public Button btnMainMenu;
    public GameBehavior game;

    private AudioSource menuSFX;

    void Awake()
    {
        menuSFX = GameObject.Find("Menu SFX").GetComponent<AudioSource>();

        // Give buttons event listeners and add methods to each.
        btnRetry.onClick.AddListener(Retry);
        btnMainMenu.onClick.AddListener(GoToMainMenu);

        // Update final score
        int distCovered = ScoreManager.instance.GetScore();
        int coinsCollected = CoinManager.instance.GetCoins();
        int finalScore = distCovered + coinsCollected;

        scoreAndCoinsText.text = $"Distance Covered: {distCovered} m\nCoins Collected: {coinsCollected}";
        totalScoreText.text = $"Total Score: {finalScore}";
    }

    private void Retry()
    {
        menuSFX.Play();
        SceneManager.LoadSceneAsync("Main");
    }

    private void GoToMainMenu()
    {
        menuSFX.Play();
        SceneManager.LoadSceneAsync("Title");
    }
}
