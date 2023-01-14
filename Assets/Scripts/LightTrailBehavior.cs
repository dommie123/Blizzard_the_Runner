using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrailBehavior : MonoBehaviour
{
    [SerializeField] private Material speedTrailMaterial;
    [SerializeField] private Material jumpTrailMaterial;
    [SerializeField] private Material coinTrailMaterial;

    private bool trailIsActive;
    private GameObject lightTrail;
    private TrailRenderer lightTrailRenderer;
    private PlayerController player;
    private Material currentMaterial;

    private void Awake()
    {
        player = GetComponent<PlayerController>();

        lightTrail = transform.Find("Light Trail").gameObject;
        lightTrail.SetActive(false);
        lightTrailRenderer = lightTrail.GetComponent<TrailRenderer>();

        currentMaterial = speedTrailMaterial;   // Speed trail is default trail.
        lightTrailRenderer.material = currentMaterial;
    }

    private void FixedUpdate()
    {
        int activePowerupIndex = player.GetActivePowerupIndex();

        if (activePowerupIndex >= 0 && !trailIsActive)
        {
            SetCurrentTrailMaterial(activePowerupIndex);

            lightTrail.SetActive(true);
            trailIsActive = true;
        }
        else if (activePowerupIndex < 0 && trailIsActive)
        {
            lightTrail.SetActive(false);
            trailIsActive = false;
        }
    }

    private void SetCurrentTrailMaterial(int powerupIndex)
    {
        switch (powerupIndex)
        {
            case 0: 
                currentMaterial = speedTrailMaterial;
                break;
            case 1: 
                currentMaterial = jumpTrailMaterial;
                break;
            case 2: 
                currentMaterial = coinTrailMaterial;
                break;
            default:    
                currentMaterial = null;
                Debug.LogWarning($"Warning: Invalid powerup index of {powerupIndex}. Please fix the issue!");
                break;
        }

        lightTrailRenderer.material = currentMaterial;
    }
}
