using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioBox : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] private AudioMixerGroup master;
    [SerializeField] private AudioMixerGroup music;
    [SerializeField] private AudioMixerGroup effects;
    private enum audioGruops {Master, Music, Effects }
    [SerializeField] private audioGruops outputChannel = audioGruops.Effects;

    [Header("Pitch")]
    [SerializeField] private bool randomPitch = false;
    [Range(-3, 3)] [SerializeField] private float minPitch = .9f;
    [Range(-3, 3)] [SerializeField] private float maxPitch = 1.1f;

    [Header("Volume")]
    [SerializeField] private bool randomVolume = false;
    [Range(0, 1)] [SerializeField] private float minVolume = .9f;
    [Range(0, 1)] [SerializeField] private float maxVolume = 1f;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        //Set channel to play
        switch (outputChannel)
        {
            case audioGruops.Master:
                _audioSource.outputAudioMixerGroup = master;
                break;

            case audioGruops.Music:
                _audioSource.outputAudioMixerGroup = music;
                break;

            case audioGruops.Effects:
                _audioSource.outputAudioMixerGroup = effects;
                break;
        }
    }

    public void PlaySound()
    {
        _audioSource.Stop();

        if(randomPitch)
            _audioSource.pitch = Random.Range(minPitch, maxPitch);

        if(randomVolume)
            _audioSource.volume = Random.Range(minVolume, maxVolume);

        _audioSource.Play();
    }
}
