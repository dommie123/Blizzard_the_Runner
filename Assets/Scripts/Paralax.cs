using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float parallaxEffect;
    
    private float length, startPos;

    private void Awake()
    {
        startPos = transform.position.x;
        length = spriteRenderer.bounds.size.x;
    }
    private void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;
    }
}
