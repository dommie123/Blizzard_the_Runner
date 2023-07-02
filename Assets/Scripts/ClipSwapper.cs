using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipSwapper : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    private AudioSource audio;

    // Start is called before the first frame update
    void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = clips[0];
    }

    public void SwitchToClip(int index)
    {
        audio.Stop();
        audio.clip = clips[index];
    }
}
