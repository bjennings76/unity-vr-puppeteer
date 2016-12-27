using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(FeedReader))]
public class Jukebox : MonoBehaviour {
  private AudioSource m_AudioSource;
  private FeedReader m_FeedReader;

  private void Awake() {
    m_AudioSource = GetComponent<AudioSource>();
    m_FeedReader = GetComponent<FeedReader>();
    m_FeedReader.OnLoaded += OnLoaded;
  }

  private void OnLoaded() {
    var first = m_FeedReader.Entries.FirstOrDefault();
    if (first == null) return;
    var www = new WWW(first.Url);
    m_AudioSource.clip = www.GetAudioClip(true, true);
    m_AudioSource.Play();
  }
}