using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileUtils
{
  public static string CleanFileName(string fileName) { return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "_")).TrimEnd('_'); }

  public static void CreateFile(string fullPath, string text)
  {
    var directory = Path.GetDirectoryName(fullPath);
    if (directory == null) throw new ArgumentException("fullPath");
    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
    File.WriteAllText(fullPath, text);
    Debug.Log("Created " + fullPath);
  }
}