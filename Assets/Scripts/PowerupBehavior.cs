using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehavior : ObjectBehavior
{
    // Component Variables
    [SerializeField] private ParticleSystem collectParticles;
    private SpriteRenderer sprite;

    // Other Variables
    [SerializeField] private Sprite openChestSprite;

    private void Start() 
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    protected override void OnCollectItem()
    {        
        sprite.sprite = openChestSprite;

        collectParticles.Play();
        Destroy(this.gameObject, 0.5f);
        int effectIndex = Random.Range(0, 3);
        PlayerController.instance.SetActivePowerupIndex(effectIndex);

        switch (effectIndex) {
            case 0:
                PlayerController.instance.Speed += 5;
                break;
            case 1:
                PlayerController.instance.JumpHeight *= 2;
                break;
            case 2:
                PlayerController.instance.ActivateCoinTimer();
                break;
        }
    }
}
