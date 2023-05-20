using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    [SerializeField] private LayerMask wall;

    private PlayerController player;

    void Start() 
    {
        player = transform.parent.GetComponent<PlayerController>();
    }

    private void FixedUpdate() 
    {
        if (Physics2D.Raycast(transform.position, transform.right, 0.75f, wall))
        {
            Debug.Log("Hit a wall");
            player.KillPlayer();
        }    
    }

    // private void OnTriggerEnter2D(Collider2D other) 
    // {   
    //     string targetLayerName = LayerMask.LayerToName(other.gameObject.layer);
    //     string sourceLayerName = LayerMask.LayerToName(wall);

    //     if (other.gameObject.tag == sourceLayerName || targetLayerName == sourceLayerName) 
    //     {
    //         Debug.Log("Hit a wall");
    //         player.KillPlayer();
    //     }
    // }
}
