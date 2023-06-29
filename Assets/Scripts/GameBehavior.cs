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

    [SerializeField] private float cutsceneTimer;
    [SerializeField] private float golemTimer;
    [SerializeField] private float playerTimer;

    [SerializeField] private GolemController golem;
    [SerializeField] private GameObject invisibleBox;

    private bool gameOverSequenceStarted;
    private float cutsceneTime;
    private float golemTime;
    private float playerTime;
    private Rigidbody2D playerBody;

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = MainSceneIsActive();
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);

        gameOverSequenceStarted = false;
        cutsceneTime = 0f;
        golemTime = 0f;
        playerTime = 0f;

        playerBody = (player != null) ? player.GetComponent<Rigidbody2D>() : null;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameStarted();

        if (gameStarted)
        {
            UpdateTimers();
        }

        CheckPlayerDied();
        CheckGamePaused();

        if (gameOverSequenceStarted)
        {
            GameOverSequence();
        }
    }

    public void StartGame()
    {
        gameStarted = true;

        if (invisibleBox)
        {
            Destroy(invisibleBox, 0.5f);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void CheckGameStarted() 
    {
        if (!gameOverSequenceStarted)
        {
            Time.timeScale = 1;
        }

        if (gameStarted)
        {   
            player.PlayerStartedGame();  
            player.SetAutoRun(MainSceneIsActive());
            golem.SetMoving(MainSceneIsActive());
            hud.SetActive(true);
            titleScreen.SetActive(false);
        }
        else
        {
            // Time.timeScale = 0;
            player.SetAutoRun(false);
            golem.SetMoving(false);
            hud.SetActive(false);
            titleScreen.SetActive(true);
        }
    }

    public void SkipCutscene()
    {
        SceneManager.LoadScene("Main");
    }

    private void CheckPlayerDied() 
    {
        if (player.IsDead())
        {
            gameOverScreen.SetActive(true);
            hud.SetActive(false);
            gameOverSequenceStarted = true;
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
        else if (gameStarted && !gameOverSequenceStarted)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }

    private void UpdateTimers()
    {
        cutsceneTime += Time.deltaTime;
        golemTime += Time.deltaTime;
        playerTime += Time.deltaTime;

        if (cutsceneTime >= cutsceneTimer && !MainSceneIsActive())
        {
            SceneManager.LoadScene("Main");
        }

        if (golemTime >= golemTimer && !MainSceneIsActive()) 
        {
            golem.SetMoving(true);
        }

        if (playerTime >= playerTimer && !MainSceneIsActive())
        {
            player.SetAutoRun(true);
        }
    }

    private bool MainSceneIsActive()
    {
        return SceneManager.GetActiveScene().name == "Main";
    }

    private void GameOverSequence()
    {  
        // Slow time down gradually until it eventually stops.
        float slowTimeInterval;

        if (Time.timeScale > 0.5)
        {
            slowTimeInterval = 0.001f;
        }
        else if (Time.timeScale <= 0.5 && Time.timeScale > 0.25)
        {
            slowTimeInterval = 0.0005f;
        }
        else if (Time.timeScale <= 0.25 && Time.timeScale > 0.125)
        {
            slowTimeInterval = 0.00025f;
        }
        else
        {
            slowTimeInterval = 0.000125f;
        }

        float newTimeScale = Time.timeScale - slowTimeInterval;
        Time.timeScale = (newTimeScale >= 0) ? newTimeScale : 0; 
        Debug.Log($"New Time Scale: {Time.timeScale}");
    }
}
