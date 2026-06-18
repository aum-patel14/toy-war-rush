// TOY WAR RUSH - AudioManager.cs
// Pooled SFX and crossfade music.

using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private int sfxPoolSize = 20;
    [SerializeField] private int maxSimultaneousSfx = 8;
    [SerializeField] private AudioClip[] sfxClips;

    private readonly Queue<AudioSource> _sfxPool = new();
    private readonly Dictionary<string, AudioClip> _clipLookup = new();
    private AudioSource _musicSource;
    private int _activeSfxCount;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;

        for (int i = 0; i < sfxPoolSize; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _sfxPool.Enqueue(source);
        }

        foreach (var clip in sfxClips)
        {
            if (clip != null)
                _clipLookup[clip.name] = clip;
        }

        // Also load from Resources/Audio/
        var resources = Resources.LoadAll<AudioClip>("Audio");
        foreach (var clip in resources)
            _clipLookup[clip.name] = clip;
    }

    public void PlaySFX(string name)
    {
        if (!_clipLookup.TryGetValue(name, out var clip) || _activeSfxCount >= maxSimultaneousSfx)
            return;

        if (_sfxPool.Count == 0) return;

        var source = _sfxPool.Dequeue();
        source.clip = clip;
        source.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        source.Play();
        _activeSfxCount++;
        StartCoroutine(ReturnSourceWhenDone(source, clip.length));
    }

    private System.Collections.IEnumerator ReturnSourceWhenDone(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        _sfxPool.Enqueue(source);
        _activeSfxCount--;
    }

    public void PlayMusic(AudioClip clip, float crossfade = 0.5f)
    {
        if (clip == null) return;
        _musicSource.clip = clip;
        _musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        _musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        _musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume) => PlayerPrefs.SetFloat("SFXVolume", volume);
}
