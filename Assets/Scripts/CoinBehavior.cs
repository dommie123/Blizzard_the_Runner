using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [SerializeField] private float destroyInSeconds;

    private void Awake() 
    {
        Destroy(this, destroyInSeconds);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            // TODO special effects here
            Destroy(this.gameObject, 0);
        }
    }
}
