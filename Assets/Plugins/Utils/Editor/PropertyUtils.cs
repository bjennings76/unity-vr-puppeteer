using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Utils.Editor {
  public static class PropertyUtils {
    public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property) {
      Type scriptTypeFromProperty = GetScriptTypeFromProperty(property);
      if (scriptTypeFromProperty == null) {
        return null;
      }
      return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath);
    }

    private static Type GetScriptTypeFromProperty(SerializedProperty property) {
      SerializedProperty serializedProperty = property.serializedObject.FindProperty("m_Script");
      if (serializedProperty == null) {
        return null;
      }
      MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
      if (monoScript == null) {
        return null;
      }
      return monoScript.GetClass();
    }

    private static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path) {
      FieldInfo fieldInfo = null;
      Type type = host;

      string[] array = path.Split(new char[] {'.'});

      for (int i = 0; i < array.Length; i++) {
        string text = array[i];
        if ((i < array.Length - 1) && (text == "Array")) {
          if (array[i + 1].StartsWith("data[")) {
            if (IsArrayOrList(type)) {
              type = GetArrayOrListElementType(type);
            }
            i++;
          } else if (array[i + 1] == "size") {
            if (IsArrayOrList(type)) {
              type = GetArrayOrListElementType(type);
            }
            i++;
          }
        } else {
          FieldInfo fieldInfo2 = null;
          Type type2 = type;
          while ((fieldInfo2 == null) && (type2 != null)) {
            fieldInfo2 = type2.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            type2 = type2.BaseType;
          }
          if (fieldInfo2 == null) {
            return null;
          }
          fieldInfo = fieldInfo2;
          type = fieldInfo.FieldType;
        }
      }
      return fieldInfo;
    }

    private static bool IsArrayOrList(Type listType) {
      return listType.IsArray || (listType.IsGenericType && (listType.GetGenericTypeDefinition() == typeof(List<>)));
    }

    private static Type GetArrayOrListElementType(Type listType) {
      if (listType.IsArray) {
        return listType.GetElementType();
      }
      if (listType.IsGenericType && (listType.GetGenericTypeDefinition() == typeof(List<>))) {
        return listType.GetGenericArguments()[0];
      }
      return null;
    }
  }
}