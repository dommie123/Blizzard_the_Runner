using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private GameObject obj;
    [SerializeField] private int numToSpawn;
    [SerializeField] private float chanceToSpawn;
    [SerializeField] private float distanceBetweenObjects;

    private void Awake() 
    {
        if (!obj)
        {
            return;
        }

        for (int i = 0; i < numToSpawn; i++)
        {
            float chosenNumber = Random.Range(0.0f, 1.0f);
            if (chosenNumber <= chanceToSpawn)
            {
                SpawnObject();

                // Prevent multiple objects from spawning on top of each other.
                spawnOffset.x += distanceBetweenObjects;
                //spawnOffset.y += distanceBetweenObjects; // spawns them in diagonal lines, quick and dirty way to test verticallity
            }
            else 
            {
                break;
            }
        }
    }

    private void SpawnObject()
    {   
        Vector3 spawnPos = this.transform.position + spawnOffset;

        Instantiate(obj, spawnPos, Quaternion.identity);
    }
}
