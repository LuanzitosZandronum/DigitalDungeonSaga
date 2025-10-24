using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [System.Serializable]
    public struct Sound     {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;
        [Range(0f, 1f)]
        public float pitchVariation;
    }

    [Header("Sounds")]
    public Sound[] sounds;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            musicSource.loop = true;
            musicSource.volume = 0.5f;
            sfxSource.volume = 1.0f;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayMusicByName(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);

        if (s.clip == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        if (musicSource.clip != s.clip)
        {
            musicSource.clip = s.clip;
            musicSource.volume = s.volume;
            musicSource.pitch = s.pitch;
            musicSource.Play();
        }
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void PlaySFXByName(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s.clip == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        float minPitch = s.pitch * (1f - s.pitchVariation);
        float maxPitch = s.pitch * (1f + s.pitchVariation);
        float randomPitch = Random.Range(minPitch, maxPitch);

        sfxSource.volume = s.volume;
        sfxSource.pitch = randomPitch;
        sfxSource.PlayOneShot(s.clip);
    }
}