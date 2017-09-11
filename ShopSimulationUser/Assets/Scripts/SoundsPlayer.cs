using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour {

    AudioSource audio;

    [SerializeField]
    private AudioClip cartwheels;
    [SerializeField]
    private AudioClip ding;
    [SerializeField]
    private AudioClip pop;

    
    [Range(0.0f, 10.0f)]
    public float volume;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audio = GetComponent<AudioSource>();
    }

    public void CartwheelsSound()
    {
        audio.PlayOneShot(cartwheels, volume);
    }

    public void DingSound()
    {
        audio.PlayOneShot(ding, volume);
    }

    public void PopSound()
    {
        audio.PlayOneShot(pop, volume);
    }

    public void Stop()
    {
        audio.Stop();
    }
}
