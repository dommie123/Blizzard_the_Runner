using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureBehavior : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        body.velocity = -transform.right * speed;
    }
}
