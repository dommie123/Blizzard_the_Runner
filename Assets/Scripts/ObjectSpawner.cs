using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    // public static ObjectSpawner instance;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private GameObject obj;
    [SerializeField] private int numToSpawn;
    [SerializeField] private bool hasFixedYPos;
    [SerializeField] private float chanceToSpawn;
    [SerializeField] private float maxYRange;
    [SerializeField] private float minYRange;
    [SerializeField] private float distanceBetweenObjects;

    private void Awake() 
    {
        // instance = this;
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
            }
        }
    }

    private void SpawnObject()
    {   
        Vector3 golemPosition = GolemController.instance.transform.position;
        Vector3 playerPosition = PlayerController.instance.transform.position;
        Vector3 groundPosition = this.transform.parent.transform.position;

        float spawnX = (playerPosition.x + golemPosition.x) / 2;
        float spawnY = hasFixedYPos ? groundPosition.y : Random.Range(groundPosition.y, groundPosition.y + 3);

        Vector3 spawnPos = new Vector3(
            spawnX + spawnOffset.x, 
            spawnY + spawnOffset.y, 
            groundPosition.z
        );

        Instantiate(obj, spawnPos, Quaternion.identity);
    }
}
