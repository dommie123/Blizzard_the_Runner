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
    private ParticleSystem.MinMaxGradient initTrailParticlesColor;

    private void Awake()
    {
        player = GetComponent<PlayerController>();

        lightTrail = transform.Find("Light Trail").gameObject;
        lightTrail.SetActive(false);
        lightTrailRenderer = lightTrail.GetComponent<TrailRenderer>();

        trailParticles = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule tpMain = trailParticles.main;

        initTrailParticlesColor = tpMain.startColor;

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

            lightTrailRenderer.emitting = true;
            // trailParticles.Play();

        }
        else if (activePowerupIndex < 0 && trailIsActive)
        {
            // trailParticles.Stop();
            lightTrailRenderer.emitting = false;

            lightTrail.SetActive(false);
            trailIsActive = false;
        }

        // UpdateParticles();
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
        ParticleSystem.MainModule tpMain = trailParticles.main;
                
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.1f);
        curve.AddKey(0.75f, 1.0f);

        ParticleSystem.ForceOverLifetimeModule fol = trailParticles.forceOverLifetime;        
        fol.enabled = true;
        fol.y = new ParticleSystem.MinMaxCurve(-player.GetBody().velocity.y, curve);

        switch (activePowerupIndex)
        {
            case 0:
                tpMain.startColor = new ParticleSystem.MinMaxGradient(Color.red);
                break;
            case 1: 
                tpMain.startColor = new ParticleSystem.MinMaxGradient(Color.cyan);
                break;
            default: 
                tpMain.startColor = initTrailParticlesColor;
                break;
        }
    }
}
