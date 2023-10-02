using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBehavior : MonoBehaviour
{
    [SerializeField] private float destroyInSeconds;

    private void Awake() 
    {
        Destroy(this.gameObject, destroyInSeconds);
    }
    
    protected abstract void OnCollectItem();

    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Check whether the object that entered the trigger was a Player.
        if (other.gameObject.tag == "Player")
        {
            OnCollectItem();
        }
    }
}
