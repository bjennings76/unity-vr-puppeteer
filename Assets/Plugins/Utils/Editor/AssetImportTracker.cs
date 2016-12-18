using System;
using System.Linq;
using UnityEditor;

namespace Utils.Editor {
  public class AssetImportTracker : AssetPostprocessor {
    public static event Action<string> SceneSaved;
    public static event Action<string> AssetImported;
    public static event Action<string[]> AssetsImported;

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
      if (SceneSaved != null) {
        XDebug.AssertDelegate(SceneSaved);
        importedAssets.Where(p => p.EndsWith(".unity")).Distinct().ForEach(p => SceneSaved(p));
      }

      if (AssetImported != null) {
        XDebug.AssertDelegate(AssetImported);
        importedAssets.Distinct().ForEach(p => AssetImported(p));
      }

      if (AssetsImported != null) {
        XDebug.AssertDelegate(AssetsImported);
        AssetsImported(importedAssets);
      }
    }
  }
}