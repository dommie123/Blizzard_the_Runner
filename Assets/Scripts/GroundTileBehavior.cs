using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileBehavior : MonoBehaviour
{
    [SerializeField] private float destroyInSeconds;
    [SerializeField] private bool dontDestroy;

    private void Awake()
    {
        if (!dontDestroy) {
            Destroy(this.gameObject, destroyInSeconds);
        }
    }
}
