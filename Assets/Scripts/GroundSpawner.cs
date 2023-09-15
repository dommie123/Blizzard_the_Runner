using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    private GameObject groundPiece;
    private Collider2D collider;
    public List<GameObject> groundPieces;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private int tilesToSpawn;
    [SerializeField] private float destroyInSeconds;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        int rng = Random.Range(0, groundPieces.Count);
        groundPiece = groundPieces[rng];
    }



    private void OnTriggerEnter2D(Collider2D other) 
    {
        for (int i = 0; i < tilesToSpawn; i++)
        {
            if (other.gameObject.tag == "Player") 
            {
                SpawnGround();
                collider.enabled = false;
            } 
        }
    }

    private void SpawnGround()
    {   
        // Spawn position is only ahead of the tile, not in the air or any deeper or shallower than before.
        Vector3 spawnPos = new Vector3(
            this.transform.parent.transform.position.x + spawnOffset.x, 
            this.transform.parent.transform.position.y, 
            this.transform.parent.transform.position.z
        );
        // Debug.Log(groundPiece);
        GameObject temp = Instantiate(groundPiece, spawnPos, Quaternion.identity);

    }
}
