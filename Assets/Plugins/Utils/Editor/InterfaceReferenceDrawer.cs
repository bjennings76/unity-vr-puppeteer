using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Editor {
  [CustomPropertyDrawer(typeof(InterfaceReference), true)]
  public class InterfaceReferenceDrawer : PropertyDrawer {
    private const string kObjectReferencePropertyName = "m_ObjectReference";

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      PropertyField(position, property, fieldInfo, label);
    }

    // Draw the property inside the given rect
    public static void PropertyField(Rect position, SerializedProperty property, FieldInfo fieldInfo, GUIContent label) {
      // Using BeginProperty / EndProperty on the parent property means that
      // prefab override logic works on the entire property.
      //EditorGUI.BeginProperty(position, label, property);

      SerializedProperty prop = property.FindPropertyRelative(kObjectReferencePropertyName);


      EditorGUI.BeginChangeCheck();
      EditorGUI.PropertyField(position, prop, label);
      if (EditorGUI.EndChangeCheck()) {
        ValidateInterfaceReference(property, fieldInfo);
      }

      //EditorGUI.EndProperty();
    }

    private static void ValidateInterfaceReference(SerializedProperty property, FieldInfo fieldInfo) {
      SerializedProperty objectReferenceProp = property.FindPropertyRelative(kObjectReferencePropertyName);
      if (objectReferenceProp != null) {
        Object obj = objectReferenceProp.objectReferenceValue;
        if (obj != null) {
          Type interfaceType = GetInterfaceType(fieldInfo.FieldType);

          if (interfaceType != null) {
            //XDebug.Log("interfaceType = {0}", interfaceType.Name);
            if (interfaceType.IsInstanceOfType(obj)) {
              // hey we've got something that directly implements our interface! we're done!
            } else {
              Object newValue = null;
              GameObject go = obj as GameObject;
              if (go != null) {
                // if the object reference dragged in was a gameobject then search its components
                Component[] components = go.GetComponents(typeof(Component));
                for (int i = 0; i < components.Length; i++) {
                  Component component = components[i];
                  if (interfaceType.IsInstanceOfType(component)) {
                    newValue = component;
                    break;
                  }
                }
              }
              objectReferenceProp.objectReferenceValue = newValue;
            }
          }
        } else {
          //the object reference is null, which is a valid value for our interface so we're done.
        }
      }
    }

    public static Type GetInterfaceType(Type fieldType) {
      Type referenceType = typeof(InterfaceReference<>);
      Type interfaceType = null;
      while (fieldType != null) {
        //XDebug.Log("fieldType = {0}", fieldType.Name);

        if (fieldType.IsGenericType) {
          if (fieldType.GetGenericTypeDefinition() == referenceType) {
            //XDebug.Log("Found reference Type");
            Type[] arguments = fieldType.GetGenericArguments();
            if (arguments.Length == 1) {
              interfaceType = arguments[0];
            }

            break;
          }
        }
        fieldType = fieldType.BaseType;
      }

      return interfaceType;
    }

    public static void SetReferenceValue(SerializedProperty property, Object value) {
      SerializedProperty objectReferenceProp = property.FindPropertyRelative(kObjectReferencePropertyName);
      if (objectReferenceProp != null) {
        objectReferenceProp.objectReferenceValue = value;
      }
    }

    public static Object GetReferenceValue(SerializedProperty property) {
      Object result = null;
      SerializedProperty objectReferenceProp = property.FindPropertyRelative(kObjectReferencePropertyName);
      if (objectReferenceProp != null) {
        result = objectReferenceProp.objectReferenceValue;
      }
      return result;
    }
  }
}