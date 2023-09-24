using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GolemState {
    INTRO,
    RUNNING,
    JUMPING,
    IDLE
}

public class GolemController : MonoBehaviour
{
    public static GolemController instance;

    [SerializeField] private bool isMoving;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float speed;
    [SerializeField] private float extraSpeedMod;
    [SerializeField] private float extraJumpSpeed;
    [SerializeField] private float footstepInterval;
    [SerializeField] private bool isInCutscene;
    [SerializeField] private GolemState state;

    private float extraSpeed;
    private float footstepTimer;
    // private float currentHeight;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D collider;
    private PlayerController pController;
    private GameObject groundAheadDetector;
    private GameObject groundDetector;
    private CameraShakeSystem cameraShake;
    private AudioSource sfx;

    public GameObject player;
    public float minPlayerDistance;

    // Animation Variables
    [SerializeField] private float jumpPrepTime;
    [SerializeField] private float runAfterSeconds;
    private float jumpPrepTimer;
    private float runSecondsPassed;
    private bool openingSequenceStarted;

    private void Awake()
    {
        instance = this;
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        pController = player.gameObject.GetComponent<PlayerController>();
        groundAheadDetector = GameObject.Find("Ground Ahead Detector");
        groundDetector = GameObject.Find("Floor Detector");
        cameraShake = GameObject.Find("Camera Shake System").GetComponent<CameraShakeSystem>();
        sfx = GetComponent<AudioSource>();
        footstepTimer = 0f;
        // currentHeight = transform.position.y;

        jumpPrepTimer = 0f;
        runSecondsPassed = 0f;
        openingSequenceStarted = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if (state = GolemState.INTRO && pController.PlayerHasStartedGame() && !isMoving)
        // {
        //     PlayOpeningSequence();
        //     return;
        // }

        // if (pController.IsDead())
        // {
        //     return;
        // }

        // if (isMoving)
        // {
        //     if (WillBeStopped())
        //     {
        //         body.velocity = new Vector2((speed + extraSpeed) / 3, body.velocity.y);
        //     }
        //     else
        //     {
        //         body.velocity = new Vector2((speed + extraSpeed), body.velocity.y);
        //     }
        // }

        // if (WillBeStopped())
        // {
        //     Jump();
        // }


        // if ((player.transform.position.x - transform.position.x) > minPlayerDistance)
        // {
        //     extraSpeed = extraSpeedMod;
        // }
        // else
        // {
        //     extraSpeed = 0;
        // }

        // if ((player.transform.position.x - transform.position.x) > 25)
        // {
        //     transform.position = new Vector3(player.transform.position.x - 20f, transform.position.y, transform.position.z);
        // }
        RaycastHit2D groundAheadHit, groundHit;
        footstepTimer += Time.deltaTime;

        // If golem is unable to recover somehow, put it back above ground.
        if (transform.position.y <= -10)
        {
            transform.position = new Vector3(transform.position.x, 15f, transform.position.z);
        }

        switch(state) 
        {
            case GolemState.INTRO:
                anim.SetBool("Is In Intro", true);
                if (pController.PlayerHasStartedGame()) 
                { 
                    if (!openingSequenceStarted) { 
                        Debug.Log("Opening sequence started");
                        PlayOpeningSequence(); 
                    }

                    runSecondsPassed += Time.deltaTime;

                    if (runSecondsPassed >= runAfterSeconds) {
                        state = GolemState.RUNNING;
                    }
                }
                else
                {
                    runSecondsPassed = 0f;
                }

                break;

            case GolemState.RUNNING:
                groundAheadHit = Physics2D.Raycast(groundAheadDetector.transform.position, Vector2.down, 50f, groundMask);
                groundHit = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, 0.1f, groundMask);

                anim.SetBool("Is In Intro", false);
                anim.SetBool("Is Grounded", groundHit);
                anim.SetBool("Is Running", true);

                if (isMoving)
                {
                    if (!groundAheadHit || WillBeStopped())
                    {
                        body.velocity = new Vector2((speed + extraSpeed) / 3, body.velocity.y);
                    }
                    else
                    {
                        body.velocity = new Vector2((speed + extraSpeed), body.velocity.y);

                        if (footstepTimer >= footstepInterval && groundHit)
                        {
                            TakeStep();
                            footstepTimer = 0f;
                        }
                    }
                }

                if (!groundAheadHit || WillBeStopped())
                {
                    state = GolemState.JUMPING;
                }

                extraSpeed = ((player.transform.position.x - transform.position.x) > minPlayerDistance) ? extraSpeedMod : 0;

                if ((player.transform.position.x - transform.position.x) > 25)
                {
                    transform.position = new Vector3(player.transform.position.x - 20f, transform.position.y, transform.position.z);
                }

                break;

            case GolemState.JUMPING:
                // Set ground detection variables
                groundAheadHit = Physics2D.Raycast(groundAheadDetector.transform.position, Vector2.down, 50f, groundMask);
                groundHit = Physics2D.Raycast(groundDetector.transform.position, Vector2.down, 0.1f, groundMask);

                // Set animation variables
                anim.SetBool("Is In Intro", false);
                anim.SetBool("Is Grounded", groundHit);
                anim.SetBool("Is Running", false);

                if (jumpPrepTimer == 0f) {
                    anim.SetTrigger("Jump");
                }

                // Begin Jump
                jumpPrepTimer += Time.deltaTime;

                // If time has elapsed, jump.
                if (jumpPrepTimer >= jumpPrepTime) {
                    float jumpSeconds = jumpPrepTimer - jumpPrepTime;
                    float xVelocity = (player.transform.position.x - transform.position.x) > minPlayerDistance
                        ? (speed + extraSpeedMod)
                        : speed;

                    float yVelocity = (groundHit) 
                        ? Mathf.Abs(jumpHeight * Mathf.Log10(jumpSeconds)) 
                        : body.velocity.y;
                    
                    body.velocity = new Vector2((xVelocity * extraJumpSpeed), yVelocity);

                    // If the golem lands after leaving the ground, switch back to Running state
                    if (groundHit && groundAheadHit) {
                        jumpPrepTimer = 0f;
                        state = GolemState.RUNNING;
                    } 
                    // If the golem cannot land safely on ground after a certain time, do a tiny midair hop (to prevent falling through stage)
                    if (!groundHit && !groundAheadHit && body.velocity.y <= -3f) {
                        body.velocity = new Vector2(xVelocity, jumpHeight / 2);
                    }
                } else {
                    body.velocity = new Vector2(speed / 3, body.velocity.y);
                }

                break;

            case GolemState.IDLE:
            default: 
                break;
        }
    }

    public void SetMoving(bool isMoving) 
    {
        this.isMoving = isMoving;
        anim.SetBool("Is Moving", isMoving);
    }

    public float DistanceToPlayer()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    private void PlayOpeningSequence()
    {
        anim.SetTrigger("Start Intro Sequence");
        openingSequenceStarted = true;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            state = GolemState.IDLE;
        }
    }

    private bool WillBeStopped()
    {
        float distanceToObstacle = collider.bounds.extents.x;
        RaycastHit2D hitWall = Physics2D.CapsuleCast(collider.bounds.center, collider.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.right, distanceToObstacle + 3f, wallMask);

        bool result = hitWall.collider != null; 
        return result;
    }

    private bool WillHitPlayer()
    {
        bool nextToPlayer = this.transform.position.x >= pController.transform.position.x - 1f;

        return nextToPlayer;
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
    }

    private void TakeStep()
    {
        float xDistanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        float effectAmplitude = (15 - xDistanceToPlayer < 0) ? 0f : (15 - xDistanceToPlayer) / 4;

        cameraShake.SetAmplitude(effectAmplitude);
        sfx.volume = effectAmplitude * 3;

        cameraShake.StartShaking();
        sfx.Play();
    }
}
