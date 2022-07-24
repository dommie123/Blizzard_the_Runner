using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    public PlayerController player;
    public GameObject gameOverScreen;
    public GameObject hud;
    public GameObject titleScreen;
    public GameObject pauseMenu;
    public bool gameStarted;
    public bool gamePaused;
    // Start is called before the first frame update
    void Start()
    {
        gameStarted = SceneManager.GetActiveScene().name == "Main (Retry)";
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameStarted();
        CheckPlayerDied();
        CheckGamePaused();
    }

    private void CheckGameStarted() 
    {
        if (gameStarted)
        {
            Time.timeScale = 1;
            hud.SetActive(true);
            titleScreen.SetActive(false);
            GolemController.instance.SetMoving(true);
        }
        else
        {
            Time.timeScale = 0;
            hud.SetActive(false);
            titleScreen.SetActive(true);
            GolemController.instance.SetMoving(false);
        }
    }

    private void CheckPlayerDied() 
    {
        if (player.IsDead())
        {
            gameOverScreen.SetActive(true);
            hud.SetActive(false);
        }
    }

    private void CheckGamePaused()
    {
        gamePaused = player.playerPausedGame;

        if (gamePaused && gameStarted)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }    
        else if (gameStarted)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }
}
