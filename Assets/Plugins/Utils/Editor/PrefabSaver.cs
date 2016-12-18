using System;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor {
  [InitializeOnLoad]
  public static class PrefabSaver {
    private const string kPrefabSaverPref = "PrefabSaverEnabled";
    private static bool s_Waiting;
    private static DateTime s_LastTime;

    static PrefabSaver() {
      Setup();
    }

    [MenuItem("Tools/Toggle PrefabSaver", priority = 641)]
    private static void Toggle() {
      Setup(true);
    }

    private static void Setup(bool toggle = false) {
      bool enabled = EditorPrefs.GetBool(kPrefabSaverPref, false);
      if (toggle) {
        enabled = !enabled;
        Debug.Log("PrefabSaver " + (enabled ? "enabled." : "disabled."));
      }
      EditorPrefs.SetBool(kPrefabSaverPref, enabled);
      PrefabUtility.prefabInstanceUpdated -= PrefabInstanceUpdated;
      EditorApplication.update -= Update;
      if (!enabled) {
        return;
      }
      PrefabUtility.prefabInstanceUpdated += PrefabInstanceUpdated;
      EditorApplication.update += Update;
    }

    private static void PrefabInstanceUpdated(GameObject instance) {
      s_Waiting = true;
      s_LastTime = DateTime.Now;
    }

    private static void Update() {
      if (!s_Waiting || (DateTime.Now - s_LastTime < TimeSpan.FromSeconds(0.25))) {
        return;
      }
      AssetDatabase.SaveAssets();
      Debug.Log("Prefab change detected. Project saved. (To turn off: Tools Menu > BDJ's Extensions> Toggle PrefabSaver)");
      s_Waiting = false;
    }
  }
}