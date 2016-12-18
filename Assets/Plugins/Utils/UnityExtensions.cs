using System.Collections.Generic;
using UnityEngine;

namespace Utils {
  public static class UnityExtensions {
    /// <summary>
    ///   Destroys all the children of the game object.
    /// </summary>
    /// <param name="obj">The game object containng the children to destroy.</param>
    public static void DestroyAllChildren(this MonoBehaviour obj) {
      if (obj) {
        obj.transform.DestroyAllChildren();
      }
    }

    /// <summary>
    ///   Destroys all the children of a transform.
    /// </summary>
    /// <param name="transform">The transform containing the children to destroy.</param>
    public static void DestroyAllChildren(this Transform transform) {
      if (!transform) {
        return;
      }
      for (int i = transform.childCount - 1; i >= 0; i--) {
        Transform t = transform.GetChild(i);
        if (!t) {
          continue;
        }
        UnityUtils.Destroy(t.gameObject);
      }
    }

    /// <summary>
    ///   If found, returns an existing component of the given type. If not found, adds and returns a new component of the
    ///   given type.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="component">The component of the game object to search.</param>
    /// <returns>The existing or new component of given type.</returns>
    public static T GetOrAddComponent<T>(this Component component) where T : Component {
      return component.gameObject.GetOrAddComponent<T>();
    }

    /// <summary>
    ///   Returns a component of given type. If not found, adds the component and then returns it.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="go">The game object to search.</param>
    /// <returns>The existing or new component of given type.</returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
      T result = go.GetComponent<T>();
      return result ? result : go.AddComponent<T>();
    }

    /// <summary>
    ///   Find the named child gameobject within the direct children of a gameobject, or create it if it doesn't exist.
    /// </summary>
    /// <param name="go">The gameobject to search for children.</param>
    /// <param name="name">name of the child gameobject</param>
    /// <returns>the named child gameobject.</returns>
    public static GameObject GetOrAddChildGameObject(this GameObject go, string name) {
      Transform childTransform = go.transform.Find(name);
      if (childTransform != null) {
        return childTransform.gameObject;
      }
      GameObject child = new GameObject(name);
      childTransform = child.transform;
      childTransform.parent = go.transform;
      childTransform.localPosition = Vector3.zero;
      childTransform.localRotation = Quaternion.identity;
      childTransform.localScale = Vector3.one;
      return child;
    }


    /// <summary>
    ///   Returns a list of children for the given transform.
    /// </summary>
    /// <param name="t">The transform to search for children.</param>
    /// <returns>A list of children of the given transform.</returns>
    public static List<Transform> GetChildren(this Transform t) {
      List<Transform> children = new List<Transform>();
      for (int i = 0; i < t.childCount; i++) {
        children.Add(t.GetChild(i));
      }
      return children;
    }

    public static void ResetTransform(this Transform t) {
      if (t == null) {
        return;
      }

      t.localScale = Vector3.one;
      t.localRotation = Quaternion.identity;
      t.localPosition = Vector3.zero;
    }
  }
}