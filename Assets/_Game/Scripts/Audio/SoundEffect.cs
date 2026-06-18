// TOY WAR RUSH - SoundEffect.cs
// Component for world-space one-shot sounds.

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool playOnEnable;

    private AudioSource _source;

    private void Awake() => _source = GetComponent<AudioSource>();

    private void OnEnable()
    {
        if (playOnEnable && clip != null)
            Play();
    }

    public void Play()
    {
        _source.clip = clip;
        _source.Play();
    }
}
