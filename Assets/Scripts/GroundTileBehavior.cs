using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileBehavior : MonoBehaviour
{
    [SerializeField] private float destroyInSeconds;
    // Start is called before the first frame update

    private void Start() 
    {
        
    }

    private void Update() 
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Fatal")
        {
            Destroy(this.gameObject, destroyInSeconds);
        }
    }
}
