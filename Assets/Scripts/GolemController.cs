using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    public static GolemController instance;
    [SerializeField] private bool isMoving;
    [SerializeField] private float speed;
    [SerializeField] private float extraSpeedMod;
    private float extraSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    private Rigidbody2D body;
    private CapsuleCollider2D collider;
    public GameObject player;
    public float minPlayerDistance;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        instance = this;
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        //isMoving = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.instance.IsDead())
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
        // float distance = Vector2.Distance(this.transform.position, PlayerController.instance.transform.position);
        bool nextToPlayer = this.transform.position.x >= PlayerController.instance.transform.position.x - 1f;

        return nextToPlayer;
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
    }
}
