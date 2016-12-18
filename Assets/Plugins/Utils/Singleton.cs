using UnityEngine;

namespace Utils {
  public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T s_Instance;

    public static bool IsQuitting { get; private set; }

    static Singleton() {
      IsQuitting = false;
    }


    public virtual void OnApplicationQuit() {
      Debug.Log(string.Format("{0}.OnApplicationQuit()", GetType().Name));
      IsQuitting = true;
    }

    // Returns the instance of this singleton
    protected static T Instance {
      get {
        if (s_Instance == null) {
          if (IsQuitting) {
            // OK, let's check if the instance is REALLY null, or if Unity is just lying to us. 
            // If it is really null then this singleton has never been accessed except during ApplicationQuit (whjich is an error)
            // if it is not really null, then chances are it has just been destyroyed as a part of quitting
            // so we'll ignore it and just return the exisiting destroyed object
            Debug.AssertFormat(!ReferenceEquals(s_Instance, null), "An attempt was made to create the singleton {0} while the game is quitting.", typeof(T).Name);
          } else {
            // We're not quitting, so just create (or recreate) the singleton if it is unity null
            // this happens when we do a LoadScene (such as when running integration tests)
            s_Instance = (T) FindObjectOfType(typeof(T));
            if (s_Instance == null) {
              GameObject container = new GameObject();
              container.name = typeof(T) + "Container";
              s_Instance = (T) container.AddComponent(typeof(T));
            }
          }
        }
        return s_Instance;
      }
    }

    public static T InstantiateFromPrefab(GameObject p) {
      GameObject go = Instantiate(p) as GameObject;
      if (go != null) {
        s_Instance = go.GetComponent<T>();
        Debug.AssertFormat(s_Instance, "Component of type {0} not found when instantiating singleton prefab {1}", typeof(T), p.name);
      }
      return s_Instance;
    }

    public static bool Exists() {
      return s_Instance != null;
    }
  }
}