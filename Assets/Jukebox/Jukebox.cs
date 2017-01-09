using System;
using System.Collections;
using System.IO;
using System.Linq;
using NAudio.Wave;
using UnityEngine;
using Utils;

[RequireComponent(typeof(AudioSource), typeof(FeedReader))]
public class Jukebox : MonoBehaviour {
  //private AudioSource m_AudioSource;
  private FeedReader m_FeedReader;

  private IWavePlayer m_WaveOutDevice;
  private WaveStream m_MainOutputStream;
  private WaveChannel32 m_VolumeStream;

  private void Awake() {
    //m_AudioSource = GetComponent<AudioSource>();
    m_FeedReader = GetComponent<FeedReader>();
    m_FeedReader.OnLoaded += OnLoaded;
  }

  private void OnDisable() { UnloadAudio(); }

  private void OnLoaded() {
    var first = m_FeedReader.Entries.LastOrDefault();
    if (first == null) return;
    LoadAudio(first.Url);
    //var www = new WWW(first.Url);
    //m_AudioSource.clip = www.GetAudioClip(true, true);
    //m_AudioSource.Play();
  }

  private void LoadAudio(string url) {
    var filePath = GetCachedFilePath(url);
    if (!File.Exists(filePath)) StartCoroutine(DownloadAndPlay(url, filePath));
    else LoadCachedFile(filePath);
  }

  private static string GetCachedFilePath(string url) {
    var fileName = FileUtils.CleanFileName(url.ReplaceRegex(@"https?://", ""));
    return Path.Combine(Application.persistentDataPath, fileName);
  }

  private IEnumerator DownloadAndPlay(string url, string filePath) {
    Debug.Log(string.Format("Downloading {0} to {1}", url, filePath));
    var www = new WWW(url);
    yield return www;

    if (!www.error.IsNullOrEmpty()) Debug.LogError("Error! Cannot open file: " + url + ": " + www.error, this);
    else {
      var directory = Path.GetDirectoryName(filePath);
      if (directory == null) throw new ArgumentException("filePath");
      if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
      File.WriteAllBytes(filePath, www.bytes);
      Debug.Log("Downloaded " + url + " to " + filePath, this);
      LoadAudioData(www.bytes, filePath);
    }
  }

  private void LoadCachedFile(string filePath) {
    Debug.Log("Loading from cache " + filePath, this);
    LoadAudioData(File.ReadAllBytes(filePath), filePath);
  }

  private void LoadAudioData(byte[] audioData, string source) {
    Debug.Log("Playing " + source);
    if (!LoadAudioFromData(audioData)) {
      Debug.LogError("Cannot open mp3 file!", this);
      return;
    }

    m_WaveOutDevice.Play();
    Resources.UnloadUnusedAssets();
  }

  private bool LoadAudioFromData(byte[] data) {
    try {
      var tmpStr = new MemoryStream(data);
      m_MainOutputStream = new Mp3FileReader(tmpStr);
      m_VolumeStream = new WaveChannel32(m_MainOutputStream);
      m_WaveOutDevice = new WaveOut();
      m_WaveOutDevice.Init(m_VolumeStream);
      return true;
    }
    catch (Exception ex) {
      Debug.LogWarning("Error! " + ex.Message);
    }

    return false;
  }

  private void UnloadAudio() {
    if (m_WaveOutDevice != null) m_WaveOutDevice.Stop();
    if (m_MainOutputStream != null) {
      // this one really closes the file and ACM conversion
      m_VolumeStream.Close();
      m_VolumeStream = null;

      // this one does the metering stream
      m_MainOutputStream.Close();
      m_MainOutputStream = null;
    }
    if (m_WaveOutDevice != null) {
      m_WaveOutDevice.Dispose();
      m_WaveOutDevice = null;
    }
  }
}