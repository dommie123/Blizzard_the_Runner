using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverBehavior : MonoBehaviour
{
    public TMP_Text finalScoreText;
    public Button btnRetry;
    public Button btnMainMenu;
    public GameBehavior game;

    private AudioSource menuSFX;

    // Start is called before the first frame update
    void Awake()
    {
        menuSFX = GameObject.Find("Menu SFX").GetComponent<AudioSource>();

        // Give buttons event listeners and add methods to each.
        btnRetry.onClick.AddListener(Retry);
        btnMainMenu.onClick.AddListener(GoToMainMenu);

        // Update final score
        int finalScore = ScoreManager.instance.GetScore();
        finalScoreText.text = $"Score: {finalScore}";
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
