using UnityEditor;
using UnityEngine;

namespace Utils.Editor {
  public static class EditorSelectionTracker {
    private static Object s_LastSelection;
    private static int s_LastSelectionCount;

    public static event System.Action<Object> SelectionChanged;

    static EditorSelectionTracker() {
      EditorApplication.update -= Update;
      EditorApplication.update += Update;
    }

    private static void Update() {
      if ((s_LastSelection == Selection.activeObject) && (s_LastSelectionCount == Selection.gameObjects.Length)) {
        return;
      }

      XDebug.AssertDelegate(SelectionChanged);

      s_LastSelection = Selection.activeObject;
      s_LastSelectionCount = Selection.gameObjects.Length;

      if (SelectionChanged != null) {
        SelectionChanged(Selection.activeObject);
      }
    }
  }
}