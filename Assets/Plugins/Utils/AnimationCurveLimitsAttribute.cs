using UnityEngine;
using Utils;

public class AnimationCurveLimitsAttribute : PropertyAttribute {
  public float TimeMin = 0;
  public float TimeMax = 0;
  public float ValueMin = 0;
  public float ValueMax = 0;
  public string CurveColor = Color.green.ToHexStringRGBA();
}