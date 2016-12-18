using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utils {
  /// <summary>
  ///   EditorOnlyObjectReference
  ///   This class is used to provide links to objects throughout that project that
  ///   are serialised like normally references, but that won't be pulled in for
  ///   builds.
  ///   This works by storing object references as guid and file id, and then expanding those
  ///   at Editor time to locate the real object reference.
  ///   There is some template magic happening at the bottom of this file which is to make
  ///   the default object base class be locked to particular types, but this is somewhat
  ///   thwarted by Unity not serialising with generics, which can be got around by making an
  ///   explicit inherited version. Somewhat clunky, but means that users of this class
  ///   can make a simple declaration to get a typed reference eg
  ///   [Serializable] public class MyEditorOnlyReference : EditorOnlyReference<MyCoolObject> {}
  /// </summary>
  [Serializable]
  public class EditorOnlyObjectReference {
    [SerializeField] private int m_FileId;
    [SerializeField] private string m_GUID;

    [NonSerialized] private Object m_CachedObject;

    public virtual Type GetReferencedType() {
      return typeof(Object);
    }

    public Object Get() {
#if UNITY_EDITOR
      if (m_CachedObject == null) {
        if (m_FileId != 0) {
          m_CachedObject = GetObjectFromGUIDAndFileID(m_GUID, m_FileId);
          XDebug.Assert(m_CachedObject != null, string.Format("EditorOnlyObjectReference not found for filedid {0} and guid {1}", m_FileId, m_GUID));
        }
      }
      return m_CachedObject;
#else
		return null;
#endif
    }


    public void Set(Object newObject) {
#if UNITY_EDITOR
      if (newObject != m_CachedObject) {
        if (newObject == null) {
          m_FileId = 0;
          m_GUID = "";
          m_CachedObject = null;
        } else {
          m_FileId = Unsupported.GetLocalIdentifierInFile(newObject.GetInstanceID());
          XDebug.Assert(m_FileId > 0, "EditorOnlyObjectReference: object not in file. Can only hold saved asset references in EditorOnlyObjectReference");
          if (m_FileId > 0) {
            m_CachedObject = newObject;
            m_GUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newObject));
          }
        }
      }
#else
#endif
    }

#if UNITY_EDITOR

    private static Object GetObjectFromGUIDAndFileID(string guid, int fileid) {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      Object[] loadedObjects = AssetDatabase.LoadAllAssetsAtPath(path);
      for (int i = 0; i < loadedObjects.Length; ++i) {
        int localFileID = Unsupported.GetLocalIdentifierInFile(loadedObjects[i].GetInstanceID());
        if (localFileID == fileid) {
          return loadedObjects[i];
        }
      }
      return null;
    }


    public static Object GetObjectFromSerializedProperty(SerializedProperty serializedProperty) {
      SerializedProperty fileId = serializedProperty.FindPropertyRelative("m_FileId");
      SerializedProperty guid = serializedProperty.FindPropertyRelative("m_GUID");
      if ((fileId != null) && (guid != null)) {
        return GetObjectFromGUIDAndFileID(guid.stringValue, fileId.intValue);
      }
      return null;
    }


    public static void SetSerializedPropertyWithObject(SerializedProperty serializedProperty, Object referencedObject) {
      SerializedProperty fileId = serializedProperty.FindPropertyRelative("m_FileId");
      SerializedProperty guid = serializedProperty.FindPropertyRelative("m_GUID");
      if ((fileId != null) && (guid != null)) {
        if (referencedObject == null) {
          fileId.intValue = 0;
          guid.stringValue = "";
        } else {
          int localFileId = Unsupported.GetLocalIdentifierInFile(referencedObject.GetInstanceID());
          fileId.intValue = localFileId;
          if (localFileId > 0) {
            guid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(referencedObject));
            ;
          }
        }
      }
    }

#endif
  }

  // templated version here that can be used to lock the type to specific values
  [Serializable]
  public class EditorOnlyReference<T> : EditorOnlyObjectReference where T : Object {
    public override Type GetReferencedType() {
      return typeof(T);
    }

    public static implicit operator T(EditorOnlyReference<T> a) {
      return a.Get() as T;
    }

    public static implicit operator EditorOnlyReference<T>(T a) {
      EditorOnlyReference<T> objReference = new EditorOnlyReference<T>();
      objReference.Set(a);
      return objReference;
    }
  }


  // Explicit common reference types
  // You must declare your type like this so that unitys serialiser won't skip this class
  // Can optionally add implicit operator to convert between stored and referenced type

  [Serializable]
  public class EditorOnlyGameObjectReference : EditorOnlyReference<GameObject> {
    public static implicit operator EditorOnlyGameObjectReference(GameObject a) {
      EditorOnlyGameObjectReference objReference = new EditorOnlyGameObjectReference();
      objReference.Set(a);
      return objReference;
    }
  }

  [Serializable]
  public class EditorOnlyMaterialReference : EditorOnlyReference<Material> {
    public static implicit operator EditorOnlyMaterialReference(Material a) {
      EditorOnlyMaterialReference objReference = new EditorOnlyMaterialReference();
      objReference.Set(a);
      return objReference;
    }
  }

  [Serializable]
  public class EditorOnlyTextureReference : EditorOnlyReference<Texture> {
    public static implicit operator EditorOnlyTextureReference(Texture a) {
      EditorOnlyTextureReference objReference = new EditorOnlyTextureReference();
      objReference.Set(a);
      return objReference;
    }
  }

  [Serializable]
  public class EditorOnlyMeshReference : EditorOnlyReference<Mesh> {
    public static implicit operator EditorOnlyMeshReference(Mesh a) {
      EditorOnlyMeshReference objReference = new EditorOnlyMeshReference();
      objReference.Set(a);
      return objReference;
    }
  }

  [Serializable]
  public class EditorOnlyMonoBehaviourReference : EditorOnlyReference<MonoBehaviour> {
    public static implicit operator EditorOnlyMonoBehaviourReference(MonoBehaviour a) {
      EditorOnlyMonoBehaviourReference objReference = new EditorOnlyMonoBehaviourReference();
      objReference.Set(a);
      return objReference;
    }
  }
}