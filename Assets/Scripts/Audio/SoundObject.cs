using UnityEngine;


namespace Utilities.Audio
{
    [System.Serializable]
    public class SoundObject
    {
        public string soundName;
        public AudioSource audioSource;
        public AudioClip[] clips;
        public eSoundType soundType;
    }
}

