using UnityEditor;
using UnityEngine;

namespace Utils.Editor {
  [CustomPropertyDrawer(typeof(RandomIntegerRange)), CustomPropertyDrawer(typeof(RandomFloatRange))]
  public class RandomRangePropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      int oldIndextLevel = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;
      position.xMin += oldIndextLevel*15.0f;
      Rect labelRect = position;
      labelRect.height = EditorGUIUtility.singleLineHeight;
      EditorGUI.LabelField(labelRect, label);
      Rect dataRect = labelRect;
      dataRect.yMin = labelRect.yMax;
      dataRect.height = EditorGUIUtility.singleLineHeight;
      Rect minRect = dataRect;
      minRect.width = (dataRect.width - 10)*0.5f;
      Rect maxRect = dataRect;
      maxRect.xMin = maxRect.xMax - minRect.width;

      float oldLabelWidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 30;


      //GUI.Box(minRect,GUIContent.none);
      //GUI.Box(maxRect, GUIContent.none);
      EditorGUI.PropertyField(minRect, property.FindPropertyRelative("m_Min"));
      EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("m_Max"));

      EditorGUIUtility.labelWidth = oldLabelWidth;
      EditorGUI.indentLevel = oldIndextLevel;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUIUtility.singleLineHeight*2;
    }
  }
}