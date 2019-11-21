using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool musicEnabled = true;
    public bool fxEnabled = true;

    [Range(0, 1)]
    public float musicVolume = 1.0f;

    [Range(0, 1)]
    public float fxVolume = 1.0f;

    public AudioClip clearRowSound;
    public AudioClip moveSound;
    public AudioClip dropSound;
    public AudioClip gameOverSound;     
    public AudioClip errorSound;

    public AudioSource musicSource;

    public AudioClip[] musicClips;

    private AudioClip randomMusicClip;

    private void Start()
    {
        randomMusicClip = GetRandomClip(musicClips);
        PlayBackgroundMusic(randomMusicClip);
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        return randomClip;
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (!musicEnabled || !musicClip || !musicSource)
        {
            return;
        }

        musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        UpdateMusic();
    }

    public void ToggleFx()
    {
        fxEnabled = !fxEnabled;
    }

    private void UpdateMusic()
    {
        if (musicSource.isPlaying != musicEnabled)
        {
            if (musicEnabled)
            {
                randomMusicClip = GetRandomClip(musicClips);
                PlayBackgroundMusic(randomMusicClip);
            }
            else
            {
                musicSource.Stop();
            }
        }
    }
}