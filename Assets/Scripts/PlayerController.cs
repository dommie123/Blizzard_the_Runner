using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float Speed { get; set; }
    public float JumpHeight { get; set; }
    public int Coins { get; set; }

    public bool playerPausedGame;

    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer sprite;
    private GrappleScript grappleScript;

    private AudioSource coinSFX;
    private AudioSource powerupSFX;
    private AudioSource comboSFX;
    private AudioSource hitCactusSFX;
    private AudioSource hitVultureSFX;
    private AudioSource dieSFX;
    private AudioSource menuSFX;
    private AudioSource fellSFX;
    private ClipSwapper comboSwapper;
    private ParticleSystem runParticles;
    private ParticleSystem landingParticles;
    private ParticleSystem smokeParticles;

    [SerializeField] private float initialSpeed;
    [SerializeField] private float initialJumpHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float debuffJump;
    [SerializeField] private float debuffSpeed;

    private float powerupDuration;      // How long the boost is when the player collects a powerup
    private float powerdownDuration;    // How long the slowdown is when the player hits an obstacle
    private float direction = 0f;

    private bool jumped;
    private bool isDead;
    private bool isHit;
    private bool pwrupTimerIsSet;
    private bool hitTimerIsSet;
    private bool canLand;

    public bool canCombo = false;

    [SerializeField] private bool autoRun; //player will automaticallt run to the right and left key is ignored

    private int distanceTravelled;
    private int hitCounter;
    private int activePowerupIndex;        // Tells which powerup is currently active, -1 for none
    private int scorePenalty;              // Added when player goes off-screen
    private int hitFrames;                 // How many frames the player has been "hit" for
    private int maxHitFrames;              // How many frames the player is allowed to stay "hit" before they die (this gives them a chance to recover from getting stuck on a wall)

    public int combo = 0;
    public int comboCap = 10;

    private CapsuleCollider2D collider;

    private Vector3 lastPos;

    // Opening Cutscene Variables
    [SerializeField] private bool isInCutscene;
    [SerializeField] private bool hasHitCutsceneTrigger;
    [SerializeField] private float startAfterSeconds;
    [SerializeField] private float runAfterSeconds;
    [SerializeField] private float lookUpAfterSeconds;
    [SerializeField] private float lookUpForSeconds;

    private bool playerStartedGame;
    private float runTimer;        // How much time has passed since player hit the trigger
    private float transitionTimer;
    private bool coinTimerActive;
    private Vector3 coinPowerupPosition;
    private int lastCoinDistanceTravelled;

    void Awake()
    {
        Speed = initialSpeed;
        JumpHeight = initialJumpHeight;
        Coins = 0;

        instance = this;
        pwrupTimerIsSet = false;
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        grappleScript = GetComponent<GrappleScript>();

        coinSFX = GameObject.Find("Coin SFX").GetComponent<AudioSource>();
        powerupSFX = GameObject.Find("Powerup SFX").GetComponent<AudioSource>();
        hitCactusSFX = GameObject.Find("Cactus SFX").GetComponent<AudioSource>();
        hitVultureSFX = GameObject.Find("Vulture SFX").GetComponent<AudioSource>();
        comboSFX = GameObject.Find("Combo").GetComponent<AudioSource>();
        dieSFX = GameObject.Find("Die").GetComponent<AudioSource>();
        menuSFX = GameObject.Find("Menu SFX").GetComponent<AudioSource>();
        fellSFX = GameObject.Find("Fell Into Pit").GetComponent<AudioSource>();
        runParticles = GameObject.Find("Run Particles").GetComponent<ParticleSystem>();
        landingParticles = GameObject.Find("Landing Particles").GetComponent<ParticleSystem>();
        smokeParticles = GameObject.Find("Smoke Particles").GetComponent<ParticleSystem>();     // Activate these when the player dies.

        comboSwapper = GameObject.Find("Combo").GetComponent<ClipSwapper>();

        jumped = false;
        isDead = false; 
        distanceTravelled = 0;
        hitCounter = 0;
        playerPausedGame = false;
        activePowerupIndex = -1;
        scorePenalty = 0;
        lastPos = transform.position;
        hitFrames = 0;
        maxHitFrames = 4;
        coinTimerActive = false;
        coinPowerupPosition = Vector3.zero;
        lastCoinDistanceTravelled = 0;
        canLand = false;

        // Opening Cutscene Variables
        playerStartedGame = false;
        runTimer = 0f;
        transitionTimer = 0f;

        if (isInCutscene)
        {
            anim.SetLayerWeight(1, 1f);

        }
        else
        {
            anim.SetLayerWeight(1, 0f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isInCutscene)
        {
            PlayOpeningSequence();
            return;
        }

        UpdateParticles();

        anim.SetBool("Is Grounded", IsGrounded());

        if (isDead)
        {
            anim.SetBool("Dead", true);
            return;
        }

        UpdateOtherAnimVariables();
        UpdatePlayerInputs();
        UpdateScore();
        UpdatePowerupTimer();
        UpdateHitTimer();
        UpdateCombo();

        if (transform.position.y < -6f)
        {
            body.velocity = Vector2.zero;
            KillPlayer(true);
        }

    }

    private void FixedUpdate() 
    {
        CheckForWall();
    }

    public bool IsGrounded()
    {
        float distanceToGround = collider.bounds.extents.y;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(collider.bounds.center, collider.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.down, /*distanceToGround*/ 0.1f, groundMask);
        return raycastHit.collider != null;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetActivePowerupIndex()
    {
        return activePowerupIndex;
    }

    public void SetActivePowerupIndex(int powerupIndex)
    {
        activePowerupIndex = powerupIndex;
    }

    public void SetAutoRun(bool autoRun)
    {
        this.autoRun = autoRun;
    }

    public void KillPlayer(bool hasFallenIntoPit)
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        if (hasFallenIntoPit)
        {
            fellSFX.Play();
            smokeParticles.Stop();
        }
        else
        {
            sprite.color = Color.red;   // Change sprite color to red to reflect player death
            dieSFX.Play();
        }
        UpdateScore();
    }

    public void PlayerStartedGame()
    {
        playerStartedGame = true;
    }

    public bool PlayerHasStartedGame()
    {
        return playerStartedGame;
    }

    public void ActivateCoinTimer()
    {
        coinTimerActive = true;
    }

    private void PlayOpeningSequence()
    {
        if (playerStartedGame && !hasHitCutsceneTrigger && transitionTimer < startAfterSeconds)
        {
            transitionTimer += Time.deltaTime;
        }
        
        if (playerStartedGame && !hasHitCutsceneTrigger && transitionTimer >= startAfterSeconds)
        {
            anim.SetBool("Game Started", playerStartedGame);    // This bool will only be used for opening cutscene, as using it elsewhere seems redundant.
            transitionTimer = 0f;
            body.velocity = new Vector2(Speed / 2, body.velocity.y);
        }

        else if (playerStartedGame && hasHitCutsceneTrigger && runTimer < runAfterSeconds)
        {
            body.velocity = new Vector2(0, body.velocity.y);
            transitionTimer += Time.deltaTime;
            runTimer += Time.deltaTime;
        }

        else if (playerStartedGame && hasHitCutsceneTrigger && runTimer >= runAfterSeconds)
        {
            body.velocity = new Vector2(Speed, body.velocity.y);
            sprite.flipX = false;
        }

        if (transitionTimer >= lookUpAfterSeconds && lookUpAfterSeconds > 0f)
        {
            anim.SetTrigger("Cutscene Trigger 2");
            transitionTimer = 0f;
            lookUpAfterSeconds = -1f;
        }

        if (transitionTimer >= lookUpForSeconds && lookUpForSeconds > 0f)
        {
            anim.SetTrigger("Cutscene Trigger 3");
            transitionTimer = 0f;
            lookUpForSeconds = -1f;
        }

    }

    private void CheckForWall()
    {
        // RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.5f, wallMask);
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, transform.right, 1.0f, wallMask);
        // Debug.DrawLine(transform.position, transform.position + transform.right + new Vector3(1.5f, 0, 0), Color.red);
        Vector3 currentSpeed = Vector3.zero;

        if (lastPos != transform.position) 
        {
            currentSpeed = transform.position - lastPos;
            currentSpeed /= Time.deltaTime;
            lastPos = transform.position;
        }

        // Debug.Log($"{hit.transform}, {currentSpeed.magnitude}units/s");

        if (hit.transform && currentSpeed.magnitude < 0.5f)
        {
            hitFrames++;
        }
        else
        {
            hitFrames = 0;
        }

        if (hitFrames >= maxHitFrames)
        {
            KillPlayer(false);
        }    
    }

    private void UpdateCombo()
    {
        if (grappleScript.isGrappling && !IsGrounded() && canCombo)
        {
            canCombo = false;
            comboSwapper.SwitchToClip((combo >= comboCap - 1) ? comboCap - 1 : combo);

            comboSFX.Play();
            combo++;
        }
        else if (IsGrounded())
        {
            canCombo = false;
            combo = 0;
            comboSwapper.SwitchToClip(0);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Ground")
        {
            jumped = false;
            anim.SetBool("Jumped", false);

        } 
        if (other.gameObject.tag == "Fatal")
        {
            KillPlayer(false);
        } 
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Coin")
        {
            Coins += (1 * Mathf.Clamp(combo,1,comboCap));
            CoinManager.instance.UpdateCoins();
            grappleScript.grappleCooldown -= .25f;
            coinSFX.Play();
        }

        if (other.gameObject.tag == "Powerup")
        {
            pwrupTimerIsSet = true;
            powerupDuration = 10.0f;
            coinPowerupPosition = transform.position;
            powerupSFX.Play();
        }

        if (other.gameObject.tag == "Cactus")
        {
            HitObstacle();
            hitCactusSFX.Play();
        }

        if (other.gameObject.tag == "Vulture")
        {
            HitObstacle();
            hitVultureSFX.Play();
        }

        if (other.gameObject.tag == "Cutscene Trigger 1")
        {
            anim.SetTrigger("Cutscene Trigger 1");
            hasHitCutsceneTrigger = true;
        }
    }

    private void UpdatePowerupTimer()
    {
        if (!pwrupTimerIsSet)
        {
            return;
        }

        powerupDuration -= Time.deltaTime;

        if (coinTimerActive)
        {
            int coinDistanceTravelled = ((int) transform.position.x) - ((int) coinPowerupPosition.x);

            if (coinDistanceTravelled > lastCoinDistanceTravelled) {
                Coins += (coinDistanceTravelled - lastCoinDistanceTravelled) * (int) Mathf.Round(1 * Mathf.Clamp(combo,1,comboCap));
                CoinManager.instance.UpdateCoins();
                lastCoinDistanceTravelled = coinDistanceTravelled;
            }
        }

        if (powerupDuration <= 0)
        {
            ResetStats();
            pwrupTimerIsSet = false;
            coinTimerActive = false;
            lastCoinDistanceTravelled = 0;
        }
    }

    private void UpdateHitTimer()
    {
        if (!hitTimerIsSet)
        {
            return;
        }
 
        powerdownDuration -= Time.deltaTime;

        if (powerdownDuration <= 0)
        {
            for (int i = 0; i < hitCounter; i++)
            {
                BuffPlayer();
            }
            hitTimerIsSet = false;
            hitCounter = 0;
            anim.SetBool("Hit Obstacle", false);
        }
    }

    private void UpdateScore()
    {
        if (distanceTravelled < transform.position.x && mainCamera != null)
        {
            Vector3 objectViewportPosition = mainCamera.WorldToViewportPoint(transform.position);

            if (objectViewportPosition.y > 1f)
            {
                scorePenalty += (int) transform.position.x - distanceTravelled;
            }

            distanceTravelled = (int) transform.position.x;
        }

        ScoreManager.instance.SetScore(distanceTravelled - scorePenalty);
    }

    private void UpdatePlayerInputs() 
    {
        if (autoRun)
        {
            direction = 1f;
        }
        else if (!autoRun)
        {
            direction = Input.GetAxis("Horizontal");
        }

        body.velocity = new Vector2(direction * Speed, body.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(body.velocity.x));

        if (Input.GetKey(KeyCode.Space) && IsGrounded() && jumped == false)
        {
            body.velocity = new Vector2(body.velocity.x, JumpHeight);
            
            jumped = true;
            anim.SetBool("Jumped", true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    private void BuffPlayer()
    {
        Speed = initialSpeed;
        JumpHeight = initialJumpHeight;
    }

    private void NerfPlayer() 
    {
        Speed = debuffSpeed;
        JumpHeight = debuffJump;
        hitCounter++;
    }

    private void ResetStats()
    {
        Speed = initialSpeed;
        JumpHeight = initialJumpHeight;
        activePowerupIndex = -1;
    }

    private void PauseGame() 
    {
        menuSFX.Play();
        playerPausedGame = !playerPausedGame;
    }

    private void UpdateOtherAnimVariables()
    {
        anim.SetFloat("Vertical Velocity", body.velocity.y);
        anim.SetBool("Is Grappling", grappleScript.isGrappling);
    }

    private void HitObstacle()
    {
        hitTimerIsSet = true;
        powerdownDuration = 0.5f;
        NerfPlayer();
        anim.SetBool("Hit Obstacle", true);
    }

    private void UpdateParticles()
    {
        if (isDead && !smokeParticles.isPlaying)
        {
            smokeParticles.Play();
        }
        else if (isDead)
        {
            // Rotate particles behind player's current trajectory
            float smokeXRotation = Mathf.Clamp((body.velocity.y * 10), -90, 90);
            float smokeYRotation = Mathf.Clamp((body.velocity.x * 15), 0, 90);
            smokeParticles.transform.rotation = Quaternion.Euler(smokeXRotation, -smokeYRotation, smokeParticles.transform.position.z);

            // Match speed of particles with character's current velocity (sort of)
            var spMain = smokeParticles.main;
            spMain.startSpeed = Mathf.Clamp(body.velocity.magnitude, 3, 20);
            return;
        }


        if (IsGrounded() && !isDead && !runParticles.isPlaying)
        {
            runParticles.Play();
        }
        else if (runParticles.isPlaying && (!IsGrounded() || isDead))
        {
            runParticles.Stop();
        }

        var lpem = landingParticles.emission;
        lpem.rateOverTime = Mathf.Abs(body.velocity.y * 20);

        if (IsGrounded() && canLand)
        {
            landingParticles.Play();
            canLand = false;
        }
        else if (!IsGrounded() && !canLand)
        {
            canLand = true;
        }
    }
}
