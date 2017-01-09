using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using Utils;

[RequireComponent(typeof(AudioSource))]
public class FeedReader : MonoBehaviour
{
  [SerializeField] private string m_FeedUrl = "http://geeklyinc.com/feed/";
  [SerializeField] private TextAsset m_TestFeed;
  [SerializeField] private string m_AssetType = "audio";

  private string FileName { get { return FileUtils.CleanFileName(m_FeedUrl.ReplaceRegex(@"https?://", "")) + ".xml"; } }

  private string LocalCachePath { get { return Path.Combine(Application.persistentDataPath, @"feed\" + FileName); } }

  public List<FeedEntry> Entries { get; private set; }

  public event Action OnLoaded;

  public void Start()
  {
    if (m_TestFeed) Load(m_TestFeed.text);
    else if (File.Exists(LocalCachePath)) LoadFeedFromCache();
    else StartCoroutine(DownloadFeed());
  }

  private void LoadFeedFromCache()
  {
    Debug.Log("Loading feed from " + LocalCachePath, this);
    Load(File.ReadAllText(LocalCachePath));
  }

  private IEnumerator DownloadFeed()
  {
    Debug.Log("Downloading " + m_FeedUrl, this);
    var w = new WWW(m_FeedUrl);
    yield return w;
    if (w.error != null) Debug.Log("Error Loading Feed: " + w.error);
    else
    {
      FileUtils.CreateFile(LocalCachePath, w.text);
      Load(w.text);
    }
  }

  private void Load(string xmlText)
  {
    Debug.Log("Loading feed XML...");
    var doc = new XmlDocument();
    doc.LoadXml(xmlText);
    var items = doc.SelectNodes(string.Format("//item[contains(enclosure/@type,'{0}')]", m_AssetType));
    if (items == null) return;
    Entries = items.Cast<XmlNode>().Select(n => new FeedEntry(n)).OrderByDescending(f => f.Date).ToList();
    Debug.Log("XML feed loaded.");
    if (OnLoaded != null) OnLoaded();
  }

  public class FeedEntry
  {
    public string Title { get; set; }
    public string Url { get; set; }
    public DateTime Date { get; set; }

    public FeedEntry(XmlNode item)
    {
      if (item == null) return;
      var titleNode = item.SelectSingleNode("title");
      var urlNode = item.SelectSingleNode("enclosure/@url");
      var dateNode = item.SelectSingleNode("pubDate");
      if (titleNode == null || urlNode == null) return;
      Title = titleNode.InnerText;
      Url = urlNode.Value;
      DateTime date;
      if (dateNode != null && DateTime.TryParse(dateNode.InnerText, out date)) Date = date;
    }

    public override string ToString() { return string.Format("{0}  [{1}]", Title, Date); }
  }
}