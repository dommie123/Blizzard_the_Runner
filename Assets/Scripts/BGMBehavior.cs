using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMBehavior : MonoBehaviour
{
    [SerializeField] private double goalTime;
    [SerializeField] private AudioSource[] _audioSources;

    [SerializeField] private AudioClip currentOpening;
    [SerializeField] private AudioClip currentLoop;

    private int audioToggle;
    private double musicDuration;

    private AudioClip currentClip;

    private void Awake() 
    {
        currentClip = currentOpening;
        audioToggle = 0;
    }

    private void Update()
    {
        if (AudioSettings.dspTime > goalTime - 0.5)
        {
            PlayScheduledClip();
        }
    }

    private void PlayScheduledClip()
    {
        goalTime = AudioSettings.dspTime + 0.5;

        _audioSources[audioToggle].clip = currentClip;
        _audioSources[audioToggle].PlayScheduled(goalTime);

        musicDuration = (double)currentClip.samples / currentClip.frequency;
        goalTime += musicDuration;

        audioToggle = 1 - audioToggle;

        if (currentClip == currentOpening)
        {
            currentClip = currentLoop;
        }
    }

    public void StopPlaying()
    {
        foreach (AudioSource source in _audioSources)
        {
            source.Stop();
        }
    }

    public void Pause()
    {
        foreach (AudioSource source in _audioSources)
        {
            source.Pause();
        }
    }

    public void Resume()
    {
        foreach (AudioSource source in _audioSources)
        {
            source.UnPause(); 
        }
    }
}
