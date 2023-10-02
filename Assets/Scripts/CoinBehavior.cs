using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [SerializeField] private float destroyInSeconds;
    [SerializeField] private ParticleSystem collectParticles;
    private SpriteRenderer sprite;

    private void Awake() 
    {
        sprite = GetComponent<SpriteRenderer>();
        Destroy(this, destroyInSeconds);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            sprite.enabled = false;
            collectParticles.Play();
            Destroy(this.gameObject, 0.5f);
        }
    }
}
