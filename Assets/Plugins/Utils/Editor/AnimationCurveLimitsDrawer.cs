using UnityEditor;
using UnityEngine;

namespace Utils.Editor {
  [CustomPropertyDrawer(typeof(AnimationCurveLimitsAttribute))]
  public class AnimationCurveLimitsDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      Color curveColor = Color.green;
      Rect limit = new Rect(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity);
      AnimationCurveLimitsAttribute limitAttribute = attribute as AnimationCurveLimitsAttribute;
      if (limitAttribute != null) {
#if UNITY_5_1_3 //TODO @TJM Take this out once we the linux CI is updated, OR we move to Unity 5.3 (In the later case replace with UNITY_5_3_OR_NEWER)
				if (!Color.TryParseHexString(limitAttribute.CurveColor, out curveColor)) {
					curveColor = Color.green;
				}
#endif
        limit.xMin = limitAttribute.TimeMin;
        limit.xMax = limitAttribute.TimeMax;
        limit.yMin = limitAttribute.ValueMin;
        limit.yMax = limitAttribute.ValueMax;
      }

      position = EditorGUI.PrefixLabel(position, label);
      EditorGUI.CurveField(position, property, curveColor, limit);
    }
  }
}