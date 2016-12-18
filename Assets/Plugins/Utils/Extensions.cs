using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Color32Extensions {
  public static class Extensions {
    public static int ToInt(this Color32 c) {
      return (c.r << 0) | (c.g << 8) | (c.b << 16) | ((255 - c.a) << 24);
    }

    public static Color32 ToColor32(this int i) {
      Color32 c = new Color32();
      c.r = (byte) ((i >> 0) & 0xff);
      c.g = (byte) ((i >> 8) & 0xff);
      c.b = (byte) ((i >> 16) & 0xff);
      c.a = (byte) (255 - ((i >> 24) & 0xff));
      return c;
    }
  }
}

namespace Utils {
  public static class Extensions {
    public static bool Approximately(this Bounds a, Bounds b) {
      return a.min.Approximately(b.min) && a.max.Approximately(b.max);
    }

    public static bool Approximately(this Vector3 a, Vector3 b) {
      return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
    }

    public static bool Approximately(this Vector3 a, Vector3 b, float epsilon) {
      return (Mathf.Abs(a.x - b.x) < epsilon) && (Mathf.Abs(a.y - b.y) < epsilon) && (Mathf.Abs(a.z - b.z) < epsilon);
    }

    public static bool Approximately(this Quaternion a, Quaternion b) {
      return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z) && Mathf.Approximately(a.w, b.w);
    }

    public static bool Approximately(this Quaternion a, Quaternion b, float epsilon) {
      return (Mathf.Abs(a.x - b.x) < epsilon) && (Mathf.Abs(a.y - b.y) < epsilon) && (Mathf.Abs(a.z - b.z) < epsilon) && (Mathf.Abs(a.w - b.w) < epsilon);
    }

    public static bool Approximately(this float a, float b) {
      return Mathf.Approximately(a, b);
    }

    public static bool Approximately(this float a, float b, float epsilon) {
      return Mathf.Abs(a - b) < epsilon;
    }

    public static bool Contains(this Bounds b, Bounds other) {
      return b.Contains(other.min) && b.Contains(other.max);
    }

    public static string ToHexStringRGBA(this Color color) {
      Color32 color32 = color;
      return color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2") + color32.a.ToString("X2");
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) {
      if (items == null) {
        return;
      }

      foreach (T obj in items) {
        action(obj);
      }
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action) {
      if (items == null) {
        return;
      }

      int i = 0;
      foreach (T obj in items) {
        action(obj, i);
        i++;
      }
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> list, params T[] except) {
      return Enumerable.Except(list, except);
    }

    public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
      if (items == null) {
        throw new ArgumentNullException("items");
      }

      if (predicate == null) {
        throw new ArgumentNullException("predicate");
      }

      int retVal = 0;
      foreach (T item in items) {
        if (predicate(item)) {
          return retVal;
        }
        retVal++;
      }
      return -1;
    }

    public static int IndexOf<T>(this IEnumerable<T> items, T item) {
      return items.IndexOf(i => EqualityComparer<T>.Default.Equals(item, i));
    }

    public static string NameOrNull(this Object o) {
      return o != null ? o.name : "null";
    }

    public static void IgnoreCollisionsWith(this Collider c, Collider other) {
      if ((c != null) && c.enabled && c.gameObject.activeInHierarchy && (other != null) && other.enabled && other.gameObject.activeInHierarchy) {
        Physics.IgnoreCollision(c, other);
      }
    }

    public static void EnableCollisionsWith(this Collider c, Collider other) {
      if ((c != null) && c.enabled && c.gameObject.activeInHierarchy && (other != null) && other.enabled && other.gameObject.activeInHierarchy) {
        Physics.IgnoreCollision(c, other, false);
      }
    }

    [Flags]
    public enum FullNameFlags {
      TypeName = 1 << 0,
      FullTypeName = (1 << 1) | TypeName,
      Name = 1 << 2,
      FullScenePath = (1 << 3) | Name,
      AssetPath = 1 << 4,
      SiblingIndex = 1 << 5,

      Default = FullTypeName | FullScenePath,
      UniqueName = Name | SiblingIndex,
      UnqiuePath = FullScenePath | SiblingIndex,
      FullSceneOrAssetPath = FullScenePath | AssetPath,
      All = FullSceneOrAssetPath | FullTypeName
    }

    public static string FullName(this Object o, FullNameFlags flags = FullNameFlags.Default, int maxLength = 0) {
      if (o == null) {
        return "null";
      }

      bool contentBeforeType = false;
      bool contentBeforeName = false;

      StringBuilder builder = new StringBuilder();


#if UNITY_EDITOR
      if (FlagSet(flags, FullNameFlags.AssetPath)) {
        string assetPath = AssetDatabase.GetAssetPath(o);
        if (!string.IsNullOrEmpty(assetPath)) {
          builder.Append(assetPath);
          contentBeforeType = true;
          contentBeforeName = true;
        } else {
          builder.Append(SceneManager.GetActiveScene());
          contentBeforeType = true;
          contentBeforeName = true;
        }
      }
#endif

      Transform t = GetTransform(o);

      if (FlagSet(flags, FullNameFlags.Name)) {
        if (contentBeforeName) {
          builder.Append(" ");
        }
        if (FlagSet(flags, FullNameFlags.FullScenePath)) {
          if (t != null) {
            BuildScenePath(t, builder, flags);
            contentBeforeType = true;
          } else {
            builder.Append(o.name);
            contentBeforeType = true;
          }
        } else {
          builder.Append(o.name);
          contentBeforeType = true;
        }
      }

      if (FlagSet(flags, FullNameFlags.SiblingIndex) && t) {
        builder.AppendFormat("[{0}]", t.GetSiblingIndex());
      }

      if (FlagSet(flags, FullNameFlags.TypeName)) {
        if (contentBeforeType) {
          builder.Append(":");
        }

        Type type = o.GetType();
        if (FlagSet(flags, FullNameFlags.FullTypeName)) {
          builder.Append(type.FullName);
        } else {
          builder.Append(type.Name);
        }
      }

      string fullName = builder.ToString();

      if ((maxLength > 0) && (fullName.Length > maxLength)) {
        const string kInsert = "...";
        int prefixLength = (maxLength - kInsert.Length)/3;
        int suffixLength = maxLength - (kInsert.Length + prefixLength);
        string prefix = fullName.Substring(0, prefixLength);
        string suffix = fullName.Substring(fullName.Length - suffixLength, suffixLength);
        return prefix + kInsert + suffix;
      } else {
        return fullName;
      }
    }

    public static Ray ForwardRay(this Transform t) {
      if (t == null) {
        throw new ArgumentNullException("t");
      }

      return new Ray(t.position, t.forward);
    }

    private static Transform GetTransform(Object o) {
      Component c = o as Component;
      if (c != null) {
        return c.transform;
      }

      GameObject go = o as GameObject;
      if (go != null) {
        return go.transform;
      }

      return null;
    }

    private static bool FlagSet(FullNameFlags flags, FullNameFlags mask) {
      return (flags & mask) != 0;
    }

    private static void BuildScenePath(Transform t, StringBuilder builder, FullNameFlags flags) {
      if (t.parent != null) {
        BuildScenePath(t.parent, builder, flags);
        if (FlagSet(flags, FullNameFlags.SiblingIndex)) {
          builder.AppendFormat("[{0}]", t.parent.GetSiblingIndex());
        }
        builder.Append("/");
      }
      builder.Append(t.name);
    }


    public static Vector3 WithX(this Vector3 v, float x) {
      return new Vector3(x, v.y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, float y) {
      return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z) {
      return new Vector3(v.x, v.y, z);
    }

    //Swizzle operations to contract Vector3 down to Vector2
    public static Vector2 XY(this Vector3 v) {
      return new Vector2(v.x, v.y);
    }

    public static Vector2 XZ(this Vector3 v) {
      return new Vector2(v.x, v.z);
    }

    public static Vector2 YZ(this Vector3 v) {
      return new Vector2(v.y, v.z);
    }

    // Deswizzle operations to extend Vector2 back to Vector3 
    // We can't give the x,y and z params a default value of 0.0f 
    // because even though the .Net compiler will cope Mono craps its pants :( 
    public static Vector3 XY(this Vector2 v, float z) {
      return new Vector3(v.x, v.y, z);
    }

    public static Vector3 XZ(this Vector2 v, float y) {
      return new Vector3(v.x, y, v.y);
    }

    public static Vector3 YZ(this Vector2 v, float x) {
      return new Vector3(x, v.x, v.y);
    }

    public static bool IsNaN(this float f) {
      return float.IsNaN(f);
    }

    public static bool IsNaN(this Vector3 v) {
      return v.x.IsNaN() || v.y.IsNaN() || v.z.IsNaN();
    }

    public static float DistanceTo(this Vector3 v, Vector3 other) {
      Vector3 difference = v - other;
      return difference.magnitude;
    }

    public static float SqrDistanceTo(this Vector3 v, Vector3 other) {
      Vector3 difference = v - other;
      return difference.sqrMagnitude;
    }
  }
}