using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleMeter : MonoBehaviour
{
    [SerializeField] private Image image;
    public GrappleScript grappleScript;


    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        image.fillAmount = grappleScript.grappleCooldown/2;
    }
}
