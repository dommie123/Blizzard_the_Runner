using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileBehavior : MonoBehaviour
{
    [SerializeField] private float destroyInSeconds;

    private void Awake()
    {
        Destroy(this.gameObject, destroyInSeconds);
    }
}
