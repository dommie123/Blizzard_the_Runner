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
    public bool gameStarted;
    // Start is called before the first frame update
    void Start()
    {
        gameStarted = SceneManager.GetActiveScene().name == "Main (Retry)";
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameStarted();
        CheckPlayerDied();
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
}
