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
    private ParticleSystem trailParticles;
    private Color initTrailParticlesColor;

    private void Awake()
    {
        player = GetComponent<PlayerController>();

        lightTrail = transform.Find("Light Trail").gameObject;
        lightTrail.SetActive(false);
        lightTrailRenderer = lightTrail.GetComponent<TrailRenderer>();

        trailParticles =  transform.Find("Light Trail").GetComponent<ParticleSystem>();
        initTrailParticlesColor = trailParticles.startColor;

        currentMaterial = speedTrailMaterial;   // Speed trail is default trail.
        lightTrailRenderer.material = currentMaterial;
    }

    private void FixedUpdate()
    {
        if (player.IsDead())
        {
            lightTrail.SetActive(false);
            trailIsActive = false;
            trailParticles.Stop();
            return;
        }
        
        int activePowerupIndex = player.GetActivePowerupIndex();

        if (activePowerupIndex >= 0 && !trailIsActive)
        {
            SetCurrentTrailMaterial(activePowerupIndex);

            lightTrail.SetActive(true);
            trailIsActive = true;

            lightTrailRenderer.emitting = true;
            trailParticles.Play();

        }
        else if (activePowerupIndex < 0 && trailIsActive)
        {
            trailParticles.Stop();
            lightTrailRenderer.emitting = false;

            lightTrail.SetActive(false);
            trailIsActive = false;
        }

        UpdateParticles();
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
        }

        lightTrailRenderer.material = currentMaterial;
    }

    private void UpdateParticles()
    {
        int activePowerupIndex = player.GetActivePowerupIndex();
        switch (activePowerupIndex)
        {
            case 0:
                trailParticles.startColor = Color.red;
                break;
            case 1: 
                trailParticles.startColor = Color.cyan;
                break;
            default: 
                trailParticles.startColor = Color.white;
                break;
        }
    }
}
