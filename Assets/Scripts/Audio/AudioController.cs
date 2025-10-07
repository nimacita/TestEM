using UnityEngine;
using Utilities.Audio;
using Utilities.EventManager;

public class AudioController : MonoBehaviour, IInitializable
{

    [Header("Music Settings")]
    [SerializeField] private AudioSource musicSource;

    [Header("Sound Settings")]
    [SerializeField] private SoundObject[] soundObjects;

    public void Initialized()
    {
        PlayMusic();
    }

    #region Subscribes

    private void OnEnable()
    {
        EventManager.Subscribe(eEventType.onPlaySound, OnSoundPlay);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(eEventType.onPlaySound, OnSoundPlay);
    }

    #endregion

    private void OnSoundPlay(object arg0)
    {
        eSoundType soundType = (eSoundType)arg0;
        PlaySelectSound(soundType);
    }

    private void PlaySelectSound(eSoundType soundType)
    {
        foreach (SoundObject sound in soundObjects)
        {
            if(sound.soundType == soundType)
            {
                AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];
                sound.audioSource.clip = clip;
                sound.audioSource.Play();
                return;
            }
        }
    }

    private void PlayMusic()
    {
        musicSource.Play();
    }
}
