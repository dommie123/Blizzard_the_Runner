using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehavior : MonoBehaviour
{
    [SerializeField] private float destroyInSeconds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake() 
    {
        Destroy(this.gameObject, destroyInSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
