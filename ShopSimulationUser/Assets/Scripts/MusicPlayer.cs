using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    [SerializeField]
    private AudioClip musicClip;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = musicClip;
        audio.Play();
    }
}
