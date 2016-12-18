using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor {
  public static class AssetDatabaseUtils {
    /// <summary>
    ///   Creates all directories in path, unless they already exist.
    /// </summary>
    /// <param name="path">Forward-slash separted path.</param>
    /// <remarks>Uses the AssetDatabase system so a .meta file is immediately created with each folder.</remarks>
    public static void CreateDirectory(string path) {
      if (path.Contains("\\")) {
        XDebug.LogError("Expected path to be separated by Unity-style foward slashes ('/'), but it contains a backslash ('\\'): {0}", path);
        return;
      }
      string pathSoFar = "";
      foreach (string folder in path.Split('/')) {
        string joinedPath = JoinPaths(pathSoFar, folder);
        if (!AssetDatabase.IsValidFolder(joinedPath)) {
          AssetDatabase.CreateFolder(pathSoFar, folder);
        }
        pathSoFar = joinedPath;
      }
    }

    private static string JoinPaths(string parent, string child) {
      if (string.IsNullOrEmpty(parent)) {
        return child;
      } else if (string.IsNullOrEmpty(child)) {
        return parent;
      } else {
        return parent + "/" + child;
      }
    }

    private static Dictionary<Type, string> s_EditorIconPathCache;

    // Attempts to load an icon from a folder relative to the .cs file that contains editorType
    // Super useful for writing editor scripts that are agnostic to their location within the assets folder
    // This assumes that the editorType that is passed in lives in a .cs file whose filename is the same as the short typename
    public static Texture LoadEditorIconRelativeToScript(string iconFileName, Type editorType, string iconsFolderName = "Icons") {
      Texture icon = null;
      if (s_EditorIconPathCache == null) {
        s_EditorIconPathCache = new Dictionary<Type, string>();
      }

      string iconDir = null;
      if (!s_EditorIconPathCache.TryGetValue(editorType, out iconDir)) {
        string typeName = editorType.Name;

        //Debug.LogFormat("Attempting asset search for \"{0}\"",typeName);

        string[] assets = AssetDatabase.FindAssets(string.Format("{0} t:MonoScript", typeName));
        if (assets.Length > 0) {
          string path = AssetDatabase.GUIDToAssetPath(assets[0]);
          if (!string.IsNullOrEmpty(path)) {
            string assetBasePath = Path.GetDirectoryName(path);
            iconDir = assetBasePath;
            s_EditorIconPathCache[editorType] = iconDir;
          }
        } else {
          Debug.LogErrorFormat("Failed to find script {0}, this will cause icon loading to fail for \"{1}\"", typeName, iconFileName);
        }
      }

      string iconPath = iconFileName;
      if (!string.IsNullOrEmpty(iconsFolderName)) {
        iconPath = iconsFolderName + Path.AltDirectorySeparatorChar + iconPath;
      }
      if (iconDir != null) {
        iconPath = iconDir + Path.AltDirectorySeparatorChar + iconPath;
        icon = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D)) as Texture;
      }
      if (icon == null) {
        Debug.LogErrorFormat("{0}, Failed to Load icon \"{1}\"", editorType.Name, iconPath);
        icon = Texture2D.whiteTexture;
      }

      return icon;
    }
  }
}