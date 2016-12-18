using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public static class EditorExtensions {
  public static string NameOrNull(this SerializedProperty o) {
    return o != null ? o.name : "null";
  }

  public static IEnumerable<GameObject> SceneRoots() {
    // JOE_UNITY542_HACKS
    // Swapping this for multiscene aware placeholder, but it needs a rethink
    return SceneManager.GetActiveScene().GetRootGameObjects();

    //HierarchyProperty prop = new HierarchyProperty(HierarchyType.GameObjects);
    //var expanded = new int[0];
    //while (prop.Next(expanded)) {
    //	yield return prop.pptrValue as GameObject;
    //}
  }

  public static IEnumerable<GameObject> AllSceneGameObjects() {
    foreach (GameObject go in SceneRoots()) {
      yield return go;
      foreach (GameObject child in AllGameObjectsRecursive(go)) {
        yield return child;
      }
    }
  }

  private static IEnumerable<GameObject> AllGameObjectsRecursive(GameObject go) {
    foreach (Transform t in go.transform) {
      yield return t.gameObject;
      foreach (GameObject child in AllGameObjectsRecursive(t.gameObject)) {
        yield return child;
      }
    }
  }

  /// <summary>
  ///   Resets the transform's local position, rotation, and scale without moving the its children relative to the global
  ///   space.
  /// </summary>
  /// <param name="transform">Transform to zero out.</param>
  public static void Zero(Transform transform) {
    Undo.RecordObject(transform, "Zero Selected");
    IEnumerable<Transform> children = OrphanChildren(transform);
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;
    transform.localScale = Vector3.one;
    ReadoptChildren(children, transform);
  }

  public static void ZeroToCenter(Transform transform) {
    Undo.RecordObject(transform, "Zero Selected");
    Vector3 center = UnityUtils.GetCenter(transform);
    IEnumerable<Transform> children = OrphanChildren(transform);
    transform.position = center;
    transform.localRotation = Quaternion.identity;
    transform.localScale = Vector3.one;
    ReadoptChildren(children, transform);
  }

  private static IEnumerable<Transform> OrphanChildren(Transform transform) {
    Transform[] children = new Transform[transform.childCount];

    for (int i = transform.childCount - 1; i >= 0; i--) {
      Transform child = transform.GetChild(i);
      Undo.RecordObject(child, "Zero Selected");
      children[i] = child;
      Undo.SetTransformParent(child, null, "Zero Selected");
    }

    return children;
  }

  private static void ReadoptChildren(IEnumerable<Transform> children, Transform transform) {
    foreach (Transform child in children) {
      Undo.SetTransformParent(child, transform, "Zero Selected");
    }
  }
}