using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Utils {
  public class GOUtils {
    public static GameObject CollidableParentOrOriginal(GameObject go) {
      if (go == null) {
        return null;
      }
      if (go.transform.parent == null) {
        return go;
      }
      GameObject best = go;

      Transform tmp = go.transform.parent;
      while (tmp != null) {
        if (tmp.gameObject.GetComponent<Collider>() != null) {
          best = tmp.gameObject;
        }
        tmp = tmp.transform.parent;
      }
      return best;
    }

    public static GameObject RenderableParentOrOriginal(GameObject go) {
      if (go == null) {
        return null;
      }
      if (go.transform.parent == null) {
        return go;
      }
      GameObject best = go;

      Transform tmp = go.transform.parent;
      while (tmp != null) {
        if (tmp.gameObject.GetComponent<Renderer>() != null) {
          best = tmp.gameObject;
        }
        tmp = tmp.transform.parent;
      }
      return best;
    }

    public static bool CombinedRendererAABB(GameObject go, ref Bounds combined) {
      if (go == null) {
        return false;
      }
      Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
      if ((renderers == null) || (renderers.Length == 0)) {
        return false;
      }
      bool first = true;
      foreach (Renderer r in renderers) {
        if (first) {
          combined = r.bounds;
          first = false;
        } else {
          MathUtils.Combine(combined, r.bounds, ref combined);
        }
      }
      return true;
    }

    public static GameObject[] FindGameObjectsWithLayer(int layer) {
      GameObject[] goArray = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
      List<GameObject> goList = new List<GameObject>();
      for (int i = 0; i < goArray.Length; i++) {
        if (goArray[i].layer == layer) {
          goList.Add(goArray[i]);
        }
      }
      if (goList.Count == 0) {
        return null;
      }
      return goList.ToArray();
    }

    public static float GetRadius(Collider col) {
      if (col == null) {
        return 0.0f;
      }
      SphereCollider sc = col as SphereCollider;
      if (sc != null) {
        return sc.radius;
      }
      return MathUtils.GetRadius(col.bounds);
    }

    public static GameObject FindTopChildWithTag(GameObject parent, string tag, bool checkParent = true) {
      if (parent == null) {
        return null;
      }

      if (checkParent) {
        if (parent.tag == tag) {
          return parent;
        }
      }

      foreach (Transform t in parent.transform) {
        if (t.tag == tag) {
          return t.gameObject;
        }
      }

      foreach (Transform t in parent.transform) {
        GameObject result = FindTopChildWithTag(t.gameObject, tag);
        if (result != null) {
          return result;
        }
      }

      return null;
    }

    public static GameObject FindNodeWithLayerMask(GameObject go, LayerMask mask) {
      if (go == null) {
        return null;
      }

      if ((mask & (1 << go.layer)) != 0) {
        return go;
      }
      Transform t = go.transform;
      int childCount = t.childCount;
      for (int i = 0; i < childCount; i++) {
        Transform child = t.GetChild(i);
        GameObject result = FindNodeWithLayerMask(child.gameObject, mask);
        if (result != null) {
          return result;
        }
      }
      return null;
    }

    public static GameObject[] FindNodesWithLayerMask(GameObject go, LayerMask mask) {
      List<GameObject> result = new List<GameObject>();
      FindNodesWithLayerMask(go, mask, result);
      return result.ToArray();
    }

    private static void FindNodesWithLayerMask(GameObject go, LayerMask mask, List<GameObject> result) {
      if (go != null) {
        if ((mask & (1 << go.layer)) != 0) {
          result.Add(go);
        }
        foreach (Transform t in go.transform) {
          FindNodesWithLayerMask(t.gameObject, mask, result);
        }
      }
    }


    public static void FindNodesWithSubStringInName(GameObject go, string subString, ref List<GameObject> found) {
      if (go == null) {
        return;
      }

      if (go.name.Contains(subString)) {
        found.Add(go);
      }

      foreach (Transform t in go.transform) {
        FindNodesWithSubStringInName(t.gameObject, subString, ref found);
      }
    }

    public static GameObject FindNodeWithName(GameObject go, string name) {
      if (go == null) {
        return null;
      }

      if (go.name == name) {
        return go;
      }

      foreach (Transform t in go.transform) {
        GameObject res = FindNodeWithName(t.gameObject, name);
        if (res != null) {
          return res;
        }
      }
      return null;
    }

    public static bool IsChildOfOrSame(Transform parent, Transform obj) {
      if ((parent == null) || (obj == null)) {
        return false;
      }

      if (parent == obj) {
        return true;
      }

      int numChildren = parent.childCount;

      for (int i = 0; i < numChildren; i++) {
        Transform child = parent.GetChild(i);

        if (IsChildOfOrSame(child, obj)) {
          return true;
        }
      }
      return false;
    }

    public static bool IsRelatedTo(GameObject a, GameObject b) {
      if ((a == null) || (b == null)) {
        return false;
      }

      if (a.transform.root != null) {
        return IsChildOfOrSame(a.transform.root, b.transform);
      }
      return IsChildOfOrSame(a.transform, b.transform);
    }

    public static string FullPathName(GameObject go) {
      if (go != null) {
        return FullPathName(go.transform);
      } else {
        return "null";
      }
    }

    public static string FullPathName(Transform t) {
      if (t != null) {
        StringBuilder builder = new StringBuilder(t.name);
        t = t.parent;
        while (t != null) {
          builder.Insert(0, Path.DirectorySeparatorChar);
          builder.Insert(0, t.name);
          t = t.parent;
        }
        return builder.ToString();
      } else {
        return "null";
      }
    }
  }
}