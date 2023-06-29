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

    public bool canCombo = false;

    [SerializeField] private bool autoRun; //player will automaticallt run to the right and left key is ignored

    private int distanceTravelled;
    private int hitCounter;
    private int activePowerupIndex;        // Tells which powerup is currently active, -1 for none
    private int scorePenalty;              // Added when player goes off-screen

    public int combo = 0;
    public int comboCap = 10;

    private CapsuleCollider2D collider;

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
        jumped = false;
        isDead = false; 
        distanceTravelled = 0;
        hitCounter = 0;
        playerPausedGame = false;
        activePowerupIndex = -1;
        scorePenalty = 0;

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

        anim.SetBool("Is Grounded", IsGrounded());

        if (isDead)
        {
            anim.SetBool("Dead", true);
            return;
        }

        CheckForWall();
        UpdateSpriteDirection();
        UpdatePlayerInputs();
        UpdateScore();
        UpdatePowerupTimer();
        UpdateHitTimer();
        UpdateCombo();

        if (transform.position.y < -6f)
        {
            body.velocity = Vector2.zero;
            isDead = true;
        }

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

    public void KillPlayer()
    {
        isDead = true;
    }

    public void PlayerStartedGame()
    {
        playerStartedGame = true;
    }

    public bool PlayerHasStartedGame()
    {
        return playerStartedGame;
    }

    private void PlayOpeningSequence()
    {

        // Debug.Log(anim.runtimeAnimatorController.animationClips.Length);

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.5f, wallMask);

        if (!hit) 
        {
            return;
        }

        if (hit && body.velocity.x == 0 && body.velocity.y == 0)
        {
            Debug.Log("Hit a wall");
            isDead = true;
        }    
    }

    private void UpdateCombo()
    {
        if (grappleScript.isGrappling && !IsGrounded() && canCombo)
        {
            canCombo = false;

            combo++;

        }
        else if (IsGrounded())
        {
            canCombo = false;
            combo = 0;
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
            isDead = true;
            UpdateScore();
        } 
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Coin")
        {
            Coins += (1 * Mathf.Clamp(combo,1,comboCap));
            CoinManager.instance.UpdateCoins();
            grappleScript.grappleCooldown -= .25f;
        }

        if (other.gameObject.tag == "Powerup")
        {
            pwrupTimerIsSet = true;
            powerupDuration = 10.0f;
        }

        if (other.gameObject.tag == "Obstacle")
        {
            hitTimerIsSet = true;
            powerdownDuration = 0.5f;
            NerfPlayer();
            anim.SetBool("Hit Obstacle", true);
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

        if (powerupDuration <= 0)
        {
            ResetStats();
            pwrupTimerIsSet = false;
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
        playerPausedGame = !playerPausedGame;
    }

    private void UpdateSpriteDirection()
    {
        anim.SetFloat("Vertical Velocity", body.velocity.y);
    }
}
