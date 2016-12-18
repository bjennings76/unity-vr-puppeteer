using System;
using System.Linq;
using JetBrains.Annotations;

namespace Utils.Editor {
  public class EditorSavingTracker : UnityEditor.AssetModificationProcessor {
    public static event Action<string> SceneSaving;

    [UsedImplicitly]
    public static string[] OnWillSaveAssets(string[] paths) {
      if (SceneSaving != null) {
        XDebug.AssertDelegate(SceneSaving);
        paths.Where(path => path.EndsWith(".unity")).ForEach(p => SceneSaving(p));
      }

      return paths;
    }
  }
}