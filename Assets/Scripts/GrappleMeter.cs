using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleMeter : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GrappleScript grappleScript;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        image.fillAmount = grappleScript.grappleCooldown/2;
    }
}
