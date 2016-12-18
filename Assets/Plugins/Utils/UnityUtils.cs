using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils {
  public static class UnityUtils {
    /// <summary>
    ///   Determines how to destroy an object based on whether or not the game is running and we are in editor mode.
    /// </summary>
    /// <param name="obj">The object to destroy.</param>
    /// <param name="life">An optional delay in seconds before the object is destroyed. (Only valid during runtime.)</param>
    public static void Destroy(Object obj, float life = 0) {
      if (!obj) {
        return;
      }
      if (Application.isPlaying) {
        Object.Destroy(obj, life);
      } else {
        Object.DestroyImmediate(obj);
      }
    }

    /// <summary>
    ///   Gets a list of child types reflected from the given type.
    /// </summary>
    /// <typeparam name="T">The type to gather child types from.</typeparam>
    /// <returns>The list of child types.</returns>
    public static List<Type> GetChildTypes<T>() {
      List<Type> derivedTypes = new List<Type>();

      // For each assembly in this app domain
      foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies()) {
        // For each type in the assembly
        foreach (Type type in assem.GetTypes()) {
          // If this is derived from T and instantiable
          if (typeof(T).IsAssignableFrom(type) && (type != typeof(T)) && !type.IsAbstract) {
            // Addit to the derived type list.
            derivedTypes.Add(type);
          }
        }
      }

      return derivedTypes;
    }

    /// <summary>
    ///   Gets the relative path between one path and another.
    /// </summary>
    /// <param name="startPath">Starting path.</param>
    /// <param name="targetPath">Target path.</param>
    /// <returns>The relative path between the two supplied paths.</returns>
    public static string GetRelativePath(string startPath, string targetPath) {
      if (targetPath.IsNullOrEmpty() || startPath.IsNullOrEmpty()) {
        return null;
      }

      if (Directory.Exists(startPath)) {
        return GetRelativePath(new DirectoryInfo(startPath), new FileInfo(targetPath));
      }

      return GetRelativePath(new FileInfo(startPath), new FileInfo(targetPath));
    }

    /// <summary>
    ///   Gets the relative path between two files.
    /// </summary>
    /// <param name="start">Starting file.</param>
    /// <param name="target">Target file.</param>
    /// <returns>The relative path between the two supplied files.</returns>
    public static string GetRelativePath(FileSystemInfo start, FileSystemInfo target) {
      Uri uri2;

      Uri uri1 = new Uri(target.FullName);

      if (start is FileInfo) {
        uri2 = new Uri(start.FullName);
      } else {
        string folderName = start.FullName;

        if (!folderName.EndsWith(Path.DirectorySeparatorChar.ToString())) {
          folderName += Path.DirectorySeparatorChar;
        }

        uri2 = new Uri(folderName);
      }

      Uri relativeUri = uri2.MakeRelativeUri(uri1);

      return Uri.UnescapeDataString(NormalizePath(relativeUri.ToString()));
    }

    private static string NormalizePath(string path) {
      return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }

    /// <summary>
    ///   Gets the game object regardless of what type of thing it is.
    /// </summary>
    /// <param name="thing">The thing to check for a game object.</param>
    /// <returns>The game object of the thing.</returns>
    public static GameObject GetGameObject(Object thing) {
      Transform t = thing as Transform;
      if (t) {
        return t.gameObject;
      }
      Component c = thing as Component;
      if (c) {
        return c.gameObject;
      }
      GameObject go = thing as GameObject;
      if (go) {
        return go;
      }
      return null;
    }

    public static string GetPath(GameObject root, Component target) {
      return GetPath(root.transform, target.transform);
    }

    private static string GetPath(Transform root, Transform target) {
      Transform t = target;
      string path = t.FullName(Extensions.FullNameFlags.UniqueName);

      while (t.parent && (t != root)) {
        t = t.parent;
        path = string.Format("{0}/{1}", t.FullName(Extensions.FullNameFlags.UniqueName), path);
      }

      if (root && (t != root)) {
        throw new Exception("Couldn't find " + target.name + " under " + root.name);
      }

      //XDebug.Log("Path for " + target + " = " + path);
      return path;
    }

    public static Dictionary<T, Transform> Orphan<T>(IEnumerable<T> things) where T : Object {
      Dictionary<T, Transform> orphans = new Dictionary<T, Transform>();
      foreach (T thing in things) {
        GameObject go = GetGameObject(thing);
        if (go.transform.childCount == 0) {
          continue;
        }

        Transform child = go.transform.GetChild(0);
        orphans[thing] = child;
        child.SetParent(null);
      }

      return orphans;
    }

    public static void Readopt<T>(Dictionary<T, Transform> orphans) where T : Object {
      List<KeyValuePair<T, Transform>> active = orphans.Where(kvp => kvp.Key && kvp.Value).ToList();

      foreach (KeyValuePair<T, Transform> kvp in active) {
        GameObject go = GetGameObject(kvp.Key);
        kvp.Value.SetParent(go.transform);
      }
    }

    public static Camera Camera {
      get {
        if (!s_Camera) {
          s_Camera = Camera.allCameras.FirstOrDefault(c => c.name == "MLCamera0") ?? Camera.main ?? Camera.current ?? Camera.allCameras.FirstOrDefault();
        }
        return s_Camera;
      }
    }
    private static Camera s_Camera;

    public static Vector3 GetCenter(params Transform[] components) {
      return GetBoundsOfChildren(components).center;
    }

    private static Bounds GetBoundsOfChildren(params Transform[] components) {
      bool foundBounds = false;
      Bounds bounds = new Bounds();

      components.ForEach(c => c.GetComponentsInChildren<Renderer>().ForEach(r => {
                                                                              if (!foundBounds) {
                                                                                foundBounds = true;
                                                                                bounds = r.bounds;
                                                                              } else {
                                                                                bounds.Encapsulate(r.bounds);
                                                                              }
                                                                            }));

      components.ForEach(c => c.GetComponentsInChildren<Collider>().ForEach(r => {
                                                                              if (!foundBounds) {
                                                                                foundBounds = true;
                                                                                bounds = r.bounds;
                                                                              } else {
                                                                                bounds.Encapsulate(r.bounds);
                                                                              }
                                                                            }));

      if (foundBounds) {
        return bounds;
      }

      components.ForEach(c => {
                           Bounds b = new Bounds(c.transform.position, Vector3.one);
                           if (!foundBounds) {
                             foundBounds = true;
                             bounds = b;
                           } else {
                             bounds.Encapsulate(b);
                           }
                         });

      return bounds;
    }

    public static float TwoPartLerp(float first, float second, float third, float t) {
      float duration;
      float subT;
      if (t < 0.5f) {
        subT = t*2f;
        duration = Mathf.Lerp(first, second, subT);
      } else if (t < 1f) {
        subT = (t - 0.5f)*2f;
        duration = Mathf.Lerp(second, third, subT);
      } else {
        duration = third;
      }
      return duration;
    }

    public enum BoundsType {
      All,
      Collider,
      Renderer
    }

    public static Bounds GetBounds(Transform transform, BoundsType type = BoundsType.All, Collider excluded = null) {
      Bounds bounds = new Bounds {size = Vector3.zero};
      if (!transform) {
        return bounds;
      }

      // Save initial rotation.
      Quaternion rotation = transform.localRotation;
      transform.localRotation = Quaternion.identity;
      bounds.center = transform.position;

      if ((type == BoundsType.All) || (type == BoundsType.Collider)) {
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders) {
          if (c != excluded) {
            bounds = CombineBounds(bounds, c.bounds);
          }
        }
      }

      if ((type == BoundsType.All) || (type == BoundsType.Renderer)) {
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
          bounds = CombineBounds(bounds, renderer.bounds);
        }
      }

      // Restore initial rotation.
      transform.localRotation = rotation;

      return bounds;
    }

    private static Bounds CombineBounds(Bounds bounds, Bounds subBounds) {
      bounds.max = new Vector3(Mathf.Max(subBounds.max.x, bounds.max.x), Mathf.Max(subBounds.max.y, bounds.max.y), Mathf.Max(subBounds.max.z, bounds.max.z));
      bounds.min = new Vector3(Mathf.Min(subBounds.min.x, bounds.min.x), Mathf.Min(subBounds.min.y, bounds.min.y), Mathf.Min(subBounds.min.z, bounds.min.z));
      return bounds;
    }

    public static void DelayAction(float duration, Action onComplete, object target = null) {
      if (onComplete == null) {
        Debug.LogError("No action found.");
        return;
      }

      Sequence sequence = DOTween.Sequence().SetDelay(duration).OnComplete(() => onComplete());
      if (target != null) {
        sequence.SetTarget(target);
      }
    }

    //  --- Based on http://stackoverflow.com/a/13727044

    public static Type GetFirstTypeByName(string typeName) {
      if (string.IsNullOrEmpty(typeName)) {
        return null;
      }

      string[] typeQualification = typeName.Split('.');
      string name = typeQualification[typeQualification.Length - 1];
      bool qualifiedName = typeQualification.Length > 1;

      foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
        Type[] assemblyTypes = a.GetTypes();
        for (int j = 0; j < assemblyTypes.Length; j++) {
          if (assemblyTypes[j].Name == name) {
            if (!qualifiedName || (assemblyTypes[j].FullName == typeName)) {
              return assemblyTypes[j];
            }
          }
        }
      }

      return null;
    }
  }
}