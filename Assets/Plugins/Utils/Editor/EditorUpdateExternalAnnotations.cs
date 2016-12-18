using System;
using System.Linq;
using UnityEditor;

namespace Utils.Editor {
  public class EditorUpdateExternalAnnotations : AssetPostprocessor {
    public static event Action<string> SceneSaved;

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
      if (SceneSaved == null) {
        return;
      }

      XDebug.AssertDelegate(SceneSaved);
      importedAssets.Where(p => p.EndsWith(".unity")).Distinct().ForEach(p => SceneSaved(p));
    }
  }
}