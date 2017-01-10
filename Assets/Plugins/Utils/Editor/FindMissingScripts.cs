using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinding.Serialization.JsonFx;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.Editor {
  public static class FindMissingScripts {
    private static int s_GoCount;
    private static int s_ComponentsCount;
    private static int s_MissingCount;

    private static void FindMissing(string manifestName, Action<Dictionary<string, List<string>>, Dictionary<string, List<string>>> search) {
      s_GoCount = 0;
      s_ComponentsCount = 0;
      s_MissingCount = 0;
      var start = DateTime.Now;
      var manifest = new Dictionary<string, List<string>>();
      var oldManifest = LoadManifest(manifestName);
      search(manifest, oldManifest);
      SaveManifest(manifestName, manifest);
      Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing in {3:N2} seconds", s_GoCount, s_ComponentsCount, s_MissingCount, (DateTime.Now - start).TotalSeconds));
    }

    [MenuItem("Tools/Find Missing Scripts in Scene")]
    public static void FindMissingInScene() {
      for (var i = 0; i < SceneManager.sceneCount; i++) {
        var scene = SceneManager.GetSceneAt(i);
        Debug.Log("Searching " + scene.path);
        var manifestName = GetSceneManifestName(scene);
        FindMissing(manifestName, (manifest, oldManifest) => {
                      var tested = new List<GameObject>();
                      foreach (var go in scene.GetRootGameObjects()) {
                        if (tested.Contains(go)) {
                          continue;
                        }

                        var shortPath = string.Format("root[{0}]/{1}", go.transform.GetSiblingIndex(), go.name);
                        FindInGO(go, shortPath, manifest, oldManifest, true, -1);
                        tested.Add(go);
                      }
                    });
      }
    }

    [MenuItem("Tools/Find Missing Scripts in All Prefabs")]
    public static void FindMissingInAssets() {
      Debug.Log("Searching all prefabs...");

      FindMissing("Prefab", (manifest, oldManifest) => {
                    var paths = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
                    for (var i = 0; i < paths.Length; i++) {
                      var path = paths[i];
                      if (EditorUtility.DisplayCancelableProgressBar("Find Missing Scripts", "Searching all prefabs... (" + s_MissingCount + " components missing)", i*1f/paths.Length)) { return; }
                      var shortPath = GetPathFromAssetsDirectory(path).Replace('\\', '/');
                      var go = AssetDatabase.LoadAssetAtPath<GameObject>(shortPath);

                      if (!go) {
                        continue;
                      }

                      FindInGO(go, shortPath, manifest, oldManifest, true);
                    }

                    EditorUtility.ClearProgressBar();
                  });
    }

    private static string GetSceneManifestName(Scene scene) {
      var pieces = scene.path.Replace(".unity", "").Replace("Assets/", "").Split('/');
      return pieces.AggregateString("-");
    }

    private static Dictionary<string, List<string>> LoadManifest(string manifestName) {
      if (manifestName.IsNullOrEmpty() || !Directory.Exists("Manifests")) { return new Dictionary<string, List<string>>(); }

      var path = Path.Combine("Manifests", manifestName + " Manifest.txt");

      if (!File.Exists(path)) { return new Dictionary<string, List<string>>(); }

      var text = File.ReadAllText(path);
      var result = Deserialize(text);
      var dictionary = result as Dictionary<string, object>;

      if (dictionary == null) {
        Debug.LogError("Couldn't deserialize " + result.GetType().Name);
        return null;
      }

      var manifest = new Dictionary<string, List<string>>();

      foreach (var kvp in dictionary) { manifest[kvp.Key] = ((string[]) kvp.Value).ToList(); }
      return manifest;
    }

    private static void SaveManifest(string manifestName, Dictionary<string, List<string>> prefabs) {
      if (manifestName.IsNullOrEmpty()) { return; }
      if (!Directory.Exists("Manifests")) { Directory.CreateDirectory("Manifests"); }
      File.WriteAllText(Path.Combine("Manifests", manifestName + " Manifest.txt"), Serialize(prefabs));
    }

    private static string GetPathFromAssetsDirectory(string fullPath) {
      if (fullPath.IsNullOrEmpty()) { return ""; } //throw new Exception("Cannot get path from assets directory when the path is null."); }
      fullPath = fullPath.Replace('\\', '/');
      var assetsPos = fullPath.IndexOf("Assets", StringComparison.OrdinalIgnoreCase);
      return assetsPos < 1 ? fullPath : fullPath.Substring(assetsPos);
    }

    private static void FindInGO(GameObject go, string path, Dictionary<string, List<string>> directory, Dictionary<string, List<string>> oldDirectory, bool fromFile = false, int level = 1, GameObject root = null) {
      if (directory.ContainsKey(path)) {
        Debug.LogError("Path already exists in component directory: " + path, go);
        return;
      }

      s_GoCount++;
      var components = go.GetComponents<Component>();

      var componentList = new List<string>();
      List<string> oldComponentList;
      oldDirectory.TryGetValue(path, out oldComponentList);

      var componentsMatch = oldComponentList != null;

      for (var i = 0; i < components.Length; i++) {
        s_ComponentsCount++;
        var component = components[i];


        if (component) {
          var componentType = component.GetType().FullName;
          componentList.Add(component.GetType().FullName);
          componentsMatch = componentsMatch && i < oldComponentList.Count && componentType == oldComponentList[i];
          continue;
        }

        componentsMatch = componentsMatch && i < oldComponentList.Count;

        string previousComponent = null;
        if (componentsMatch && !oldComponentList[i].Contains("*** MISSING ***")) {
          var match = Regex.Match(oldComponentList[i], @"^\*\*\* (WAS|MISSING) (?<name>\S+) \*\*\*");
          if (match.Success) { previousComponent = match.Groups["name"].Value; }
          else { previousComponent = oldComponentList[i]; }
        }

        var result = previousComponent == null ? "*** MISSING ***" : "*** MISSING " + previousComponent + " ***";

        componentList.Add(result);
        s_MissingCount++;
        var warning = string.Format("Missing {0}script: {1}@{2}", previousComponent == null ? "" : previousComponent + " ", path, i);
        Debug.LogWarning(warning, level > 2 && fromFile ? root : go);
      }

      if (componentList.Count > 1) { directory.Add(path, componentList); }

      for (var i = 0; i < go.transform.childCount; i++) {
        var childT = go.transform.GetChild(i);
        FindInGO(childT.gameObject, string.Format("{0}[{1}]/{2}", path, i, childT.name), directory, oldDirectory, fromFile, level > 0 ? level + 1 : level, root ?? go);
      }
    }

    private static string Serialize(object data) {
      var jsonOutput = new StringBuilder();
      var jsonWriterSettings = new JsonWriterSettings {PrettyPrint = true};
      var jsonWriter = new JsonWriter(jsonOutput, jsonWriterSettings);
      jsonWriter.Write(data);
      return jsonOutput.ToString();
    }

    private static object Deserialize(string jsonText) { return JsonReader.Deserialize(jsonText); }
  }
}