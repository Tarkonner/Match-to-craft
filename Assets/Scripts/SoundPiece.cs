using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundPiece : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] float minPitch = .9f;
    [SerializeField] float maxPitch = 1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayAudioWithRandomPitch(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }


    public void PlayAudioWithRandomPitch(AudioClip[] clips)
    {
        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }
}
