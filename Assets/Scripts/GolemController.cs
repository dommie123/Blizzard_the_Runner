using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    public static GolemController instance;

    [SerializeField] private bool isMoving;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float speed;
    [SerializeField] private float extraSpeedMod;
    [SerializeField] private bool isInCutscene;

    private float extraSpeed;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D collider;
    private PlayerController pController;


    public GameObject player;
    public float minPlayerDistance;

    private void Awake()
    {
        instance = this;
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        pController = player.gameObject.GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isInCutscene && pController.PlayerHasStartedGame() && !isMoving)
        {
            PlayOpeningSequence();
            return;
        }

        if (pController.IsDead())
        {
            return;
        }

        if (isMoving)
        {
            if (WillBeStopped())
            {
                body.velocity = new Vector2((speed + extraSpeed) / 3, body.velocity.y);
            }
            else
            {
                body.velocity = new Vector2((speed + extraSpeed), body.velocity.y);
            }
        }

        if (WillBeStopped())
        {
            Jump();
        }


        if (transform.position.y <= -10)
        {
            transform.position = new Vector3(transform.position.x, 15f, transform.position.z);
        }

        if ((player.transform.position.x - transform.position.x) > minPlayerDistance)
        {
            extraSpeed = extraSpeedMod;
        }
        else
        {
            extraSpeed = 0;
        }

        if ((player.transform.position.x - transform.position.x) > 25)
        {
            transform.position = new Vector3(player.transform.position.x - 20f, transform.position.y, transform.position.z);
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
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            isMoving = false;
        }
    }

    private bool WillBeStopped()
    {
        float distanceToObstacle = collider.bounds.extents.x;
        //RaycastHit2D hitGround = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.right, distanceToObstacle + 3f, groundMask);
        RaycastHit2D hitWall = Physics2D.CapsuleCast(collider.bounds.center, collider.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.right, distanceToObstacle + 3f, wallMask);

        bool result = hitWall.collider != null; //|| hitGround.collider != null;
        return result;
    }

    private bool WillHitPlayer()
    {
        // float distance = Vector2.Distance(this.transform.position, pController.transform.position);
        bool nextToPlayer = this.transform.position.x >= pController.transform.position.x - 1f;

        return nextToPlayer;
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
    }


}
