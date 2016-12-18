using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utils.Editor {
  public partial class EditorUtils {
    [MenuItem("CONTEXT/Component/Move To Top", true)]
    public static bool VerifyComponentMoveToTopMenu(MenuCommand command) {
      Component component = command.context as Component;
      if (component != null) {
        Component[] components = component.gameObject.GetComponents<Component>();
        int index = components.IndexOf(component);

        // Transform is always index 0, and if we're index 1 we can't move any higher...
        return index > 1;
      }
      return false;
    }

    [MenuItem("CONTEXT/Component/Move To Top", false, 501)]
    public static void ComponentMoveToTopMenu(MenuCommand command) {
      Component component = command.context as Component;

      if (component != null) {
        Component[] components = component.gameObject.GetComponents<Component>();
        int index = components.IndexOf(component);

        XDebug.Assert(index >= 0, component, "component not found in components... huh??");

        int numSpacesToMove = index - 1; // -1 because we want Transform to still be at the top!

        if (numSpacesToMove > 0) {
          Undo.RegisterCompleteObjectUndo(component.gameObject, string.Format("Move {0} to top", component.GetType().Name));
          for (int i = 0; i < numSpacesToMove; i++) {
            ComponentUtility.MoveComponentUp(component);
          }
        }
      }
    }

    [MenuItem("CONTEXT/Component/Move To Bottom", true)]
    public static bool VerifyComponentMoveToBottomMenu(MenuCommand command) {
      Component component = command.context as Component;
      if (component != null) {
        Component[] components = component.gameObject.GetComponents<Component>();
        int index = components.IndexOf(component);

        // return true if we're anything other than the last component in the list
        // Also return false if index==0, as that will be the transform, which can't move.
        return (index < components.Length - 1) && (index > 0);
      }
      return false;
    }

    [MenuItem("CONTEXT/Component/Move To Bottom", false, 502)]
    public static void ComponentMoveToBottomMenu(MenuCommand command) {
      Component component = command.context as Component;

      if (component != null) {
        Component[] components = component.gameObject.GetComponents<Component>();
        int index = components.IndexOf(component);

        XDebug.Assert(index >= 0, component, "component not found in components... huh??");

        int numSpacesToMove = components.Length - 1 - index;

        if (numSpacesToMove > 0) {
          Undo.RegisterCompleteObjectUndo(component.gameObject, string.Format("Move {0} to bottom", component.GetType().Name));
          for (int i = 0; i < numSpacesToMove; i++) {
            ComponentUtility.MoveComponentDown(component);
          }
        }
      }
    }
  }
}