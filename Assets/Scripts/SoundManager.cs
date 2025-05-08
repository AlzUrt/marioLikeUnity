using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Instance unique pour le pattern Singleton
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        [Range(0f, 1f)]
        public float spatialBlend = 0f;  // 0 = 2D, 1 = 3D
        public bool loop = false;

        [HideInInspector]
        public AudioSource source;
    }

    // Catégories de sons pour une meilleure organisation
    [Header("Sons du joueur")]
    public Sound[] playerSounds;

    [Header("Sons d'interactions")]
    public Sound[] interactionSounds;

    [Header("Sons d'ennemis")]
    public Sound[] enemySounds;

    [Header("Sons d'environnement")]
    public Sound[] environmentSounds;

    [Header("Sons d'interface")]
    public Sound[] uiSounds;

    [Header("Musique")]
    public Sound[] music;

    // Dictionnaire pour accéder facilement aux sons par leur nom
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSounds(playerSounds);
        InitializeSounds(interactionSounds);
        InitializeSounds(enemySounds);
        InitializeSounds(environmentSounds);
        InitializeSounds(uiSounds);
        InitializeSounds(music);
    }

    private void InitializeSounds(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            // Créer un AudioSource pour chaque son
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.loop = s.loop;

            // Ajouter au dictionnaire pour un accès facile
            soundDictionary[s.name] = s;
        }
    }

    // Jouer un son par son nom
    public void Play(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning("Son non trouvé: " + name);
        }
    }

    // Arrêter un son par son nom
    public void Stop(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Stop();
        }
        else
        {
            Debug.LogWarning("Son non trouvé: " + name);
        }
    }

    // Jouer un son à une position spécifique dans le monde (pour les sons 3D)
    public void PlayAtPosition(string name, Vector3 position)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
        }
        else
        {
            Debug.LogWarning("Son non trouvé: " + name);
        }
    }

    // Jouer un son avec une variation de hauteur aléatoire
    public void PlayWithRandomPitch(string name, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            float originalPitch = sound.pitch;
            sound.source.pitch = Random.Range(minPitch, maxPitch);
            sound.source.Play();
            sound.pitch = originalPitch;
        }
        else
        {
            Debug.LogWarning("Son non trouvé: " + name);
        }
    }

    // Ajuster le volume d'un son
    public void SetVolume(string name, float volume)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.volume = Mathf.Clamp01(volume);
            sound.source.volume = sound.volume;
        }
        else
        {
            Debug.LogWarning("Son non trouvé: " + name);
        }
    }

    // Ajuster le volume global pour une catégorie de sons
    public void SetCategoryVolume(Sound[] category, float volume)
    {
        foreach (Sound s in category)
        {
            s.volume = Mathf.Clamp01(volume);
            s.source.volume = s.volume;
        }
    }

    // Arrêter tous les sons
    public void StopAllSounds()
    {
        foreach (Sound s in soundDictionary.Values)
        {
            s.source.Stop();
        }
    }

    // Lancer la musique de fond
    public void PlayMusic(string name)
    {
        // Arrêter toute musique en cours
        foreach (Sound s in music)
        {
            s.source.Stop();
        }

        // Jouer la nouvelle musique
        Play(name);
    }
}