using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    private AudioSource audioSource;
    [SerializeField] AudioClip[] songs;
    private int currentSong = -1;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentSong = Random.Range(0, songs.Length);
        audioSource.clip = songs[currentSong];
        audioSource.Play();

        StartCoroutine(PlayNextSong(audioSource.clip.length));
    }

    private IEnumerator PlayNextSong(float songTime)
    {
        yield return new WaitForSeconds(songTime);
        int newSong = Random.Range(0, songs.Length);

        while(newSong == currentSong)
            newSong = Random.Range(0, songs.Length);

        audioSource.clip = songs[newSong];
        audioSource.Play();
        currentSong = newSong;

        StartCoroutine(PlayNextSong(audioSource.clip.length));
    }
}
