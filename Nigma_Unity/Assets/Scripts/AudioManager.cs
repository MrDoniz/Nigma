using UnityEngine;

namespace Nigma.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource sfxSource;
        public AudioSource bgmSource;

        [Header("Audio Clips")]
        public AudioClip woodClacClip;
        public AudioClip paperRustleClip;
        public AudioClip stampThumpClip;
        public AudioClip uiClickClip;
        public AudioClip bgmJazzLoop;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            PlayBGM();
        }

        private void SetupAudioSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.volume = 0.5f;
            }
        }

        public void PlayWoodClac(float volume = 1f)
        {
            if (woodClacClip != null) 
            {
                sfxSource.pitch = Random.Range(0.9f, 1.1f);
                sfxSource.PlayOneShot(woodClacClip, volume);
            }
            else Debug.Log("[AudioManager] (Missing Clip) 🪵 CLAC (Madera)");
        }

        public void PlayPaperRustle(float volume = 1f)
        {
            if (paperRustleClip != null) 
            {
                sfxSource.pitch = Random.Range(0.95f, 1.05f);
                sfxSource.PlayOneShot(paperRustleClip, volume);
            }
            else Debug.Log("[AudioManager] (Missing Clip) 📄 SHHH (Papel)");
        }

        public void PlayPickup()
        {
            // Reusing paper rustle for pickup, slightly higher pitch
            if (paperRustleClip != null)
            {
                sfxSource.pitch = Random.Range(1.1f, 1.2f);
                sfxSource.PlayOneShot(paperRustleClip, 0.8f);
            }
            else Debug.Log("[AudioManager] (Missing Clip) 👆 PICKUP");
        }

        public void PlayUIClick()
        {
            if (uiClickClip != null)
            {
                sfxSource.pitch = Random.Range(0.95f, 1.05f);
                sfxSource.PlayOneShot(uiClickClip, 0.6f);
            }
            else if (stampThumpClip != null)
            {
                // Fallback a stamp si no hay UI click
                sfxSource.pitch = Random.Range(1.3f, 1.5f);
                sfxSource.PlayOneShot(stampThumpClip, 0.4f);
            }
        }

        public void PlayStampThump()
        {
            if (stampThumpClip != null) sfxSource.PlayOneShot(stampThumpClip);
            else Debug.Log("[AudioManager] (Missing Clip) 🛑 THUMP (Sello)");
        }

        public void PlayBGM()
        {
            if (bgmJazzLoop != null && !bgmSource.isPlaying)
            {
                bgmSource.clip = bgmJazzLoop;
                bgmSource.Play();
            }
            else if (bgmJazzLoop == null)
            {
                Debug.Log("[AudioManager] (Missing Clip) 🎷 BGM Jazz Loop");
            }
        }
    }
}
