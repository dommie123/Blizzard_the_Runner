using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OOBArrowBehavior : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private Image oobArrow;
    
    private void Awake()
    {
        oobArrow = GameObject.Find("OOB Arrow").GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        Vector3 objectViewportPosition = Camera.main.WorldToViewportPoint(player.transform.position);
        float playerOffScreenHeight = player.transform.position.y - 14;

        if (playerOffScreenHeight > 1f)
        {
            Color arrowColor = new Color(255, 255, 255, (0.1f * playerOffScreenHeight));
            oobArrow.color = arrowColor;
        }
        else
        {
            oobArrow.color = new Color(0, 0, 0, 0);
        }
    }
}
