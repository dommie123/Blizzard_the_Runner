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
    [SerializeField] private float shakeCameraAfterSeconds;
    [SerializeField] private float cameraShakeTimer1;
    [SerializeField] private float shakeCameraAgainAfterSeconds;
    [SerializeField] private float cameraShakeTimer2;
    [SerializeField] private float golemFootStepInterval;
    [SerializeField] private float maxGolemDistanceFromPlayer;

    [SerializeField] private GolemController golem;
    [SerializeField] private GameObject invisibleBox;
    [SerializeField] private CameraShakeSystem cameraShake;
    [SerializeField] private BGMBehavior bgm;

    private bool gameOverSequenceStarted;
    private bool cameraShakeStarted;
    private bool firstCameraShakeTriggered;
    private bool cameraShakeStartedAgain;
    private bool secondCameraShakeTriggered;

    private float cutsceneTime;
    private float golemTime;
    private float playerTime;
    private float cameraShakeTime;
    private float golemFootstepTime;

    private Rigidbody2D playerBody;
    private AudioSource footstepSFX;
    private AudioSource landingSFX;
    private AudioSource golemRisingSFX;
    private AudioSource golemRoarSFX;
    private AudioSource menuSFX;

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = MainSceneIsActive();
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);

        gameOverSequenceStarted = false;
        cameraShakeStarted = false;
        firstCameraShakeTriggered = false;
        cameraShakeStartedAgain = false;
        secondCameraShakeTriggered = false;

        cutsceneTime = 0f;
        golemTime = 0f;
        playerTime = 0f;
        cameraShakeTime = 0f;
        golemFootstepTime = 0f;

        playerBody = (player != null) ? player.GetComponent<Rigidbody2D>() : null;
        footstepSFX = GameObject.Find("Footsteps").GetComponent<AudioSource>();
        landingSFX = GameObject.Find("Landing").GetComponent<AudioSource>();
        menuSFX = GameObject.Find("Menu SFX").GetComponent<AudioSource>();
        golemRisingSFX = GameObject.Find("Rising").GetComponent<AudioSource>();
        golemRoarSFX = GameObject.Find("Roar").GetComponent<AudioSource>();

        bgm = GameObject.Find("BGM").GetComponent<BGMBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameStarted();

        if (gameStarted)
        {
            UpdateTimers();
        }

        if (MainSceneIsActive())
        {
            UpdateGolemFootsteps();
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
        menuSFX.Play();
        bgm.StopPlaying();
        gameStarted = true;

        if (invisibleBox)
        {
            Destroy(invisibleBox, 0.5f);
        }
    }

    public void QuitGame()
    {
        menuSFX.Play();
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
            player.SetAutoRun(false);
            golem.SetMoving(false);
            hud.SetActive(false);
            titleScreen.SetActive(true);
        }
    }

    public void SkipCutscene()
    {
        menuSFX.Play();
        SceneManager.LoadScene("Main");
    }

    private void CheckPlayerDied() 
    {
        if (player.IsDead())
        {
            gameOverScreen.SetActive(true);
            hud.SetActive(false);
            gameOverSequenceStarted = true;
            bgm.StopPlaying();
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
        cameraShakeTime += Time.deltaTime;

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

        ManageCameraShakeSystem();
    }

    private void ManageCameraShakeSystem()
    {
        if (MainSceneIsActive())
        {
            return;
        }

        if (cameraShakeTime >= shakeCameraAfterSeconds && !cameraShakeStarted)
        {
            cameraShake.StartShaking();
            cameraShakeStarted = true;
            cameraShakeTime = 0f;
            golemRisingSFX.Play();
        }

        if (cameraShakeTime >= cameraShakeTimer1 && cameraShakeStarted && !firstCameraShakeTriggered)
        {
            cameraShake.StopShaking();
            firstCameraShakeTriggered = true;
            cameraShakeTime = 0f;
            cameraShake.ChangeShakeMode("rumble");
        }

        if (cameraShakeTime >= shakeCameraAgainAfterSeconds && cameraShakeStarted && firstCameraShakeTriggered && !cameraShakeStartedAgain)
        {
            cameraShake.StartShaking();
            cameraShakeStartedAgain = true;
            cameraShakeTime = 0f;
            golemRoarSFX.Play();
        }

        if (cameraShakeTime >= cameraShakeTimer2 && cameraShakeStarted && firstCameraShakeTriggered && cameraShakeStartedAgain && !secondCameraShakeTriggered)
        {
            cameraShake.StopShaking();
            secondCameraShakeTriggered = true;
            cameraShakeTime = 0f;
        }
    }

    private void UpdateGolemFootsteps()
    {
        if (golem.DistanceToPlayer() > maxGolemDistanceFromPlayer)
        {
            cameraShake.StopShaking();
            return;
        }

        float perceivedDistanceValue = 1 / (golem.DistanceToPlayer() / 2);
        cameraShake.SetAmplitude(perceivedDistanceValue);
        footstepSFX.volume = perceivedDistanceValue;
        golemFootstepTime += Time.deltaTime;

        if (golemFootstepTime >= golemFootStepInterval)
        {
            golemFootstepTime = 0f;
            cameraShake.StartShaking();
            footstepSFX.Play();
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
    }
}
