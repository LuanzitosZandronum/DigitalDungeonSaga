using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();

    [System.Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;
        [Range(0f, 1f)]
        public float pitchVariation;

        [Tooltip("Tempo mínimo em segundos para que este som possa ser repetido.")]
        public float defaultCooldown;
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
            sfxSource.spatialBlend = 0; // Garantir que SFX seja 2D

            musicSource.loop = true;
            musicSource.volume = 0.5f;
            sfxSource.volume = 1.0f;

            foreach (var sound in sounds)
            {
                soundCooldowns.Add(sound.name, 0f);
            }
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

    // CORREÇÃO AQUI: Adicionar o parâmetro cooldownOverride com valor padrão de 0f
    public void PlaySFXByName(string name, float cooldownOverride = 0f)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s.clip == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        // 1. CALCULA O COOLDOWN: Escolhe o maior entre o Override e o Default
        float cooldown = Mathf.Max(cooldownOverride, s.defaultCooldown);

        // 2. VERIFICA O COOLDOWN
        if (soundCooldowns.ContainsKey(name))
        {
            float lastPlayTime = soundCooldowns[name];

            // Se o tempo atual for menor que (tempo da última jogada + cooldown), CANCELA.
            if (Time.time < lastPlayTime + cooldown)
            {
                return;
            }
        }

        // 3. ATUALIZA O COOLDOWN (SOMENTE se o som for Tocado)
        if (soundCooldowns.ContainsKey(name))
        {
            soundCooldowns[name] = Time.time;
        }
        else
        {
            soundCooldowns.Add(name, Time.time);
        }

        // 4. TOCA O SOM
        float minPitch = s.pitch * (1f - s.pitchVariation);
        float maxPitch = s.pitch * (1f + s.pitchVariation);
        float randomPitch = Random.Range(minPitch, maxPitch);

        sfxSource.volume = s.volume;
        sfxSource.pitch = randomPitch;
        sfxSource.PlayOneShot(s.clip);
    }
}