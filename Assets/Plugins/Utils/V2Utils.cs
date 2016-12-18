using UnityEngine;

namespace Utils {
  public static class V2Utils {
    public static float DistanceOfPointToLineSegment(Vector2 p, Vector2 v, Vector2 w) {
      Vector2 vw = w - v;
      float l2 = vw.magnitude;
      if (l2 <= Mathf.Epsilon) {
        return Vector2.Distance(p, v);
      }
      Vector2 nvw = vw/l2;
      float t = Vector2.Dot(p - v, nvw)/l2;
      if (t <= 0.0f) {
        return Vector2.Distance(p, v);
      }
      if (t >= 1.0f) {
        return Vector2.Distance(p, w);
      }
      Vector2 projection = v + t*vw;
      return Vector2.Distance(p, projection);
    }

    public static Vector2 ProjectPointOnLineSegment(Vector2 p, Vector2 v, Vector2 w, out float t) {
      Vector2 vw = w - v;
      float l = vw.magnitude;
      if (l <= Mathf.Epsilon) {
        t = 0.0f;
        return v;
      }
      Vector2 nvw = vw/l;
      t = Mathf.Clamp01(Vector2.Dot(p - v, nvw)/l);
      if (t <= 0.0f) {
        return v;
      }
      if (t >= 1.0f) {
        return w;
      }
      Vector2 projection = v + t*vw;
      return projection;
    }
  }
}