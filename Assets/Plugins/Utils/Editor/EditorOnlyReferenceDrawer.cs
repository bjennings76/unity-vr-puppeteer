using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Editor {
  [CustomPropertyDrawer(typeof(EditorOnlyObjectReference), true)]
  public class EditorOnlyReferenceDrawer : PropertyDrawer {
    private Texture m_CachedIcon;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      // Using BeginProperty / EndProperty on the parent property means that
      // prefab override logic works on the entire property.
      EditorGUI.BeginProperty(position, label, property);

      // use some special tricks to get the underlying object
      EditorOnlyObjectReference referenceObject = fieldInfo.GetValue(property.serializedObject.targetObject) as EditorOnlyObjectReference;

      Object loadedObject = null;
      Type referencedObjectType = typeof(Object);

      if (referenceObject != null) {
        loadedObject = referenceObject.Get();
        referencedObjectType = referenceObject.GetReferencedType();
      }

      // add in our link broken icon to show this is some sort of magical property
      if (m_CachedIcon == null) {
        m_CachedIcon = AssetDatabaseUtils.LoadEditorIconRelativeToScript("EditorOnlyReferenceIcon.png", typeof(EditorOnlyReferenceDrawer), "");
      }
      GUIContent propertyLableWithIcon = new GUIContent(label.text, m_CachedIcon);

      // use a typed object field to show the objects actual value
      Object newObject = EditorGUI.ObjectField(position, propertyLableWithIcon, loadedObject, referencedObjectType, false);

      if (newObject != loadedObject) {
        if (referenceObject != null) {
          referenceObject.Set(newObject);
        }
        EditorUtility.SetDirty(property.serializedObject.targetObject);
      }

      EditorGUI.EndProperty();
    }
  }
}