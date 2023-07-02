using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Check whether the object that entered the trigger was a Player.
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        Destroy(this.gameObject, 0);
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
                PlayerController.instance.Coins += 10;
                CoinManager.instance.UpdateCoins();
                break;
        }
    }
}
