using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private SpriteRenderer sprite;
    private Collider2D collider;
    private void Awake() 
    {
        collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player" && IsNonFatalObstacle())
        {
            hitParticles.Play();
            sprite.enabled = false;
            collider.enabled = false;
            Destroy(this.gameObject, 0.5f);  
        }
    }

    private bool IsNonFatalObstacle()
    {
        return hitParticles != null && sprite != null;
    }
}
