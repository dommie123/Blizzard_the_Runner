using System.Collections;
using System.Collections.Generic;
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
    public GrappleScript grappleScript;
    [SerializeField] private float initialSpeed;
    [SerializeField] private float initialJumpHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float debuffJump;
    [SerializeField] private float debuffSpeed;
    private float powerupDuration;      // How long the boost is when the player collects a powerup
    private float powerdownDuration;    // How long the slowdown is when the player hits an obstacle
    private float wallJumpCooldown;
    private bool jumped;
    private bool isDead;
    private bool isHit;
    private bool pwrupTimerIsSet;
    private bool hitTimerIsSet;
    private bool isTouchingWall;
    private int distanceTravelled;
    private int hitCounter;
    private CapsuleCollider2D collider;

    void Awake()
    {
        Speed = initialSpeed;
        JumpHeight = initialJumpHeight;
        Coins = 0;

        instance = this;
        pwrupTimerIsSet = false;
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        anim = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        sprite = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        grappleScript = GetComponent<GrappleScript>();
        jumped = false;
        wallJumpCooldown = 0.2f;
        isDead = false; 
        isTouchingWall = false;       
        distanceTravelled = 0;
        hitCounter = 0;
        playerPausedGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        //Debug.Log(IsGrounded());

        UpdateSpriteDirection();
        UpdatePlayerInputs();
        UpdateScore();
        UpdatePowerupTimer();
        UpdateHitTimer();
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

    private bool IsTouchingWall()
    {
        float distanceToWall = collider.bounds.extents.x;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(collider.bounds.center, collider.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.right, /*distanceToWall*/ 0.1f, wallMask);
        return raycastHit.collider != null;
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
            Coins++;
            CoinManager.instance.UpdateCoins();
            grappleScript.grappleCooldown -= .5f;
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
            Debug.Log("Powerup has expired!");
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
            Debug.Log("Player stats restored!");
            for (int i = 0; i < hitCounter; i++)
            {
                BuffPlayer();
            }
            hitTimerIsSet = false;
            hitCounter = 0;
        }
    }

    private void UpdateScore()
    {
        if (distanceTravelled < gameObject.transform.position.x)
        {
            distanceTravelled = (int) gameObject.transform.position.x;
        }

        ScoreManager.instance.SetScore(distanceTravelled);
    }

    private void UpdatePlayerInputs() 
    {
        float direction = Input.GetAxis("Horizontal");

        //UpdateWallJumpPhysics(direction);

        if (wallJumpCooldown >= 0.2f)
        {
            body.velocity = new Vector2(direction * Speed, body.velocity.y);
            anim.SetFloat("Speed", Mathf.Abs(body.velocity.x));
        }

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

 /*
    private void UpdateWallJumpPhysics(float direction)
    {
        if (wallJumpCooldown <= 0.2f)
        {
            wallJumpCooldown += Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) && IsTouchingWall())
        {
            body.velocity = new Vector2(-Mathf.Sign(direction) * (JumpHeight / 2), JumpHeight);
            wallJumpCooldown = 0f;
            anim.SetBool("Is Touching Wall", false);
        }
        else if (IsTouchingWall())
        {
            body.velocity = new Vector2(0, -body.gravityScale);
            anim.SetBool("Is Touching Wall", true);
        }
    }
 */
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
    }

    private void PauseGame() 
    {
        playerPausedGame = !playerPausedGame;
    }

    private void UpdateSpriteDirection()
    {
        if (IsTouchingWall() && Input.GetAxis("Horizontal") < 0)
            sprite.flipX = false;
        else if (IsTouchingWall() ^ Input.GetAxis("Horizontal") < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;
    }
}