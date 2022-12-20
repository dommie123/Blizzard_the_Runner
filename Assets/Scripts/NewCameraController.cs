using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector2 offset;
    private Vector3 initialPosition;

    public float offsetX;
    public float offsetY;

    public bool lockX;
    public bool lockY;

    // Start is called before the first frame update
    private void Awake()
    {
        // offset = new Vector2(offsetX, offsetY);
        initialPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        offset = new Vector2(offsetX, offsetY);
        
        if (!lockX && lockY)
        {
            transform.position = new Vector3(player.transform.position.x + offset.x, initialPosition.y + offset.y, -10);
        }

        else if (!lockY && lockX)
        {
            transform.position = new Vector3(initialPosition.x + offset.x, player.transform.position.y + offset.y, -10);
        }

        else if (!lockX && !lockY)
        {
            transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, -10);
        }
    }
}
