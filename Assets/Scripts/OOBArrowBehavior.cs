using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OOBArrowBehavior : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private Image oobArrow;
    // private TMP_Text oobDistText;   // How far off-screen is the player?
    private void Awake()
    {
        oobArrow = GameObject.Find("OOB Arrow").GetComponent<Image>();
        // oobDistText = GameObject.Find("OOB Distance Text").GetComponent<TMP_Text>();   
    }

    private void FixedUpdate()
    {
        Vector3 objectViewportPosition = Camera.main.WorldToViewportPoint(player.transform.position);
        float playerOffScreenHeight = player.transform.position.y - 14;

        if (playerOffScreenHeight > 1f)
        {
            // oobDistText.text = $"{Mathf.Round(playerOffScreenHeight)}M";

            Color arrowColor = new Color(255, 255, 255, (0.1f * playerOffScreenHeight));
            oobArrow.color = arrowColor;
        }
        else
        {
            // oobDistText.text = "";
            oobArrow.color = new Color(0, 0, 0, 0);
        }
    }
}
