using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    [SerializeField] private LayerMask wall;
    [SerializeField] private Rigidbody2D body;

    private PlayerController player;

    void Start() 
    {
        player = transform.parent.GetComponent<PlayerController>();
    }

    private void FixedUpdate() 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.75f, wall);

        if (!hit) 
        {
            return;
        }

        if (hit && body.velocity.x == 0 && body.velocity.y == 0)
        {
            player.KillPlayer(false);
        }    
    }
}
