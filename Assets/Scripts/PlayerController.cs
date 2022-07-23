using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float Speed {get; set;}
    public float JumpHeight {get; set;}
    public int Coins {get; set;}
    private Rigidbody2D body;
    [SerializeField] private float initialSpeed;
    [SerializeField] private float initialJumpHeight;
    private float powerupDuration;      // How long the boost is when the player collects a powerup
    private float powerdownDuration;    // How long the slowdown is when the player hits an obstacle
    private bool jumped;
    private bool isDead;
    private bool isHit;
    private bool pwrupTimerIsSet;
    private bool hitTimerIsSet;
    private int distanceTravelled;
    private int hitCounter;
    private Collider2D collider;

    void Awake()
    {
        Speed = initialSpeed;
        JumpHeight = initialJumpHeight;
        Coins = 0;

        instance = this;
        pwrupTimerIsSet = false;
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        jumped = false;
        isDead = false;        
        distanceTravelled = 0;
        hitCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        body.velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, body.velocity.y);

        if (Input.GetKey(KeyCode.Space) && !jumped)
        {
            body.velocity = new Vector2(body.velocity.x, JumpHeight);
            jumped = true;
        }

        UpdateScore();
        UpdatePowerupTimer();
        UpdateHitTimer();
    }

    public bool IsGrounded()
    {
        return !jumped;
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Ground")
        {
            jumped = false;
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
            // Debug.Log($"Coins: {Coins}");
        }
        if (other.gameObject.tag == "Powerup")
        {
            // TODO set a timer. When it expires, remove the effect (unless powerup gives coins)
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

    private void BuffPlayer()
    {
        Speed += 2;
        JumpHeight += 2;
    }

    private void NerfPlayer() 
    {
        Speed -= 2;
        JumpHeight -= 2;
        hitCounter++;
    }

    private void ResetStats()
    {
        Speed = initialSpeed;
        JumpHeight = initialJumpHeight;
    }
}
