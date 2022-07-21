using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    public static GolemController instance;
    [SerializeField] private bool isMoving;
    [SerializeField] private float speed;
    private Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        instance = this;
        body = GetComponent<Rigidbody2D>();
        //isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            body.velocity = new Vector2(speed, body.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player")
        {
            isMoving = false;
        }
    }
}
