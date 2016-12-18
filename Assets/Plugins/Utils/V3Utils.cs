using UnityEngine;

namespace Utils {
  public static class V3Utils {
    public static readonly Vector3[] CardinalDirections = {Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right};

    public static Vector3 Reflect(Vector3 v, Vector3 n, float factor = 1.0f) {
      float dot = Vector3.Dot(v, n);
      if (dot >= 0.0f) {
        return v;
      }
      return v - (factor + 1.0f)*dot*n;
    }

    public static float DistanceToPlane(Vector3 planePoint, Vector3 planeNormal, Vector3 p) {
      return Vector3.Dot(planeNormal, p - planePoint);
    }

    public static float DistanceToPlane(Transform t, Vector3 p) {
      if (t == null) {
        return float.MaxValue;
      }
      return DistanceToPlane(t.position, t.up, p);
    }

    public static float DistanceToPlane(GameObject go, Vector3 p) {
      if (go == null) {
        return float.MaxValue;
      }
      return DistanceToPlane(go.transform, p);
    }

    public static float DistanceOfPointToLineSegment(Vector3 p, Vector3 v, Vector3 w) {
      Vector3 vw = w - v;
      float l2 = vw.magnitude;
      if (l2 <= Mathf.Epsilon) {
        return Vector3.Distance(p, v);
      }
      Vector3 nvw = vw/l2;
      float t = Vector3.Dot(p - v, nvw)/l2;
      if (t <= 0.0f) {
        return Vector3.Distance(p, v);
      }
      if (t >= 1.0f) {
        return Vector3.Distance(p, w);
      }
      Vector3 projection = v + t*vw;
      return Vector3.Distance(p, projection);
    }

    public static Vector3 ProjectPointOnLineSegment(Vector3 p, Vector3 v, Vector3 w, out float t) {
      Vector3 vw = w - v;
      float l = vw.magnitude;
      if (l <= Mathf.Epsilon) {
        t = 0.0f;
        return v;
      }
      Vector3 nvw = vw/l;
      t = Mathf.Clamp01(Vector3.Dot(p - v, nvw)/l);
      if (t <= 0.0f) {
        return v;
      }
      if (t >= 1.0f) {
        return w;
      }
      Vector3 projection = v + t*vw;
      return projection;
    }

    public static Vector3 ProjectPointOntoPlane(GameObject go, Vector3 p) {
      if (go == null) {
        Debug.Log("null surface");
        return p;
      }
      return p - go.transform.up*Vector3.Dot(p, go.transform.up) + go.transform.up*Vector3.Dot(go.transform.position, go.transform.up);
    }

    public static Vector3 ConstrainedVector(Vector3 original, Vector3 constraint, Vector3 constraintDirection) {
      float x = ConstrainedFloat(original.x, constraint.x, constraintDirection.x);
      float y = ConstrainedFloat(original.y, constraint.y, constraintDirection.y);
      float z = ConstrainedFloat(original.z, constraint.z, constraintDirection.z);
      return new Vector3(x, y, z);
    }

    private static float ConstrainedFloat(float original, float constraint, float constraintDirection) {
      if (constraintDirection == 0f) {
        return original;
      }
      if (constraintDirection > 0f) {
        return constraint > original ? constraint : original;
      } // constraintDirection < 0f
      return constraint < original ? constraint : original;
    }

    public static Vector3 SmoothedVector(Vector3 input, Vector3 oldSmoothed, float timeConstantFactor = 0.1f) {
      return oldSmoothed*timeConstantFactor + input*(1f - timeConstantFactor);
    }

    public static Vector3 ComponentReciprocal(Vector3 v) {
      return new Vector3(1f/v.x, 1f/v.y, 1f/v.z);
    }

    // Identical to Unity's Lerp, but with no initial clamp of t.
    public static Vector3 UnclampedLerp(Vector3 from, Vector3 to, float t) {
      return new Vector3(@from.x + (to.x - @from.x)*t, @from.y + (to.y - @from.y)*t, @from.z + (to.z - @from.z)*t);
    }

    public static Vector3 SnapToCardinalDirection(Vector3 baseDirection) {
      float max = Mathf.NegativeInfinity;
      float maxSign = 0.0f;
      int maxIndex = -1;
      for (int i = 0; i < 3; i++) {
        if ((i == 0) || (Mathf.Abs(baseDirection[i]) > max)) {
          max = baseDirection[i];
          maxSign = Mathf.Sign(max);
          maxIndex = i;
        }
      }
      Vector3 result = Vector3.zero;
      result[maxIndex] = 1.0f*maxSign;
      return result;
    }

    public static float RadiusOfTangentConeAtSphereCenter(Vector3 sphereCenter, float sphereRadius, Vector3 point, float pointRadius) {
      float sizeA = sphereRadius + pointRadius;
      Vector3 vecB = point - sphereCenter;

      //XDebug.DrawLine(typeof(MathUtils), point, sphereCenter, Colors.LightGreen, 0.1f);

      float vecBSqrM = vecB.sqrMagnitude;

      // overlaps with sphere, push it out until it doesn't.
      float sizeB = 0f;
      if (vecBSqrM < sizeA*sizeA) {
        float multiplier = (sizeA + 1e-3f)/Mathf.Sqrt(vecBSqrM);
        vecB *= multiplier;
        sizeB = vecB.magnitude;
      } else {
        sizeB = Mathf.Sqrt(vecBSqrM);
      }

      // trig identities make "cos(pi/2 - asin(a/b)) * a" into "a(a/b)"
      float tangentPlaneDistFromCenter = sizeA*(sizeA/sizeB);

      //XDebug.DrawWireCube(typeof(MathUtils), tangentPlanePoint, Vector3.one * 0.1f, Colors.WhiteSmoke, 0.1f);

      float coneRadiusAtTangent = Mathf.Sqrt(sizeA*sizeA - tangentPlaneDistFromCenter*tangentPlaneDistFromCenter);

      return coneRadiusAtTangent*(sizeB/Mathf.Max(sizeB - tangentPlaneDistFromCenter, 0.00001f));
    }

    public static Vector3 TangentOfPointToSphere(Vector3 sphereCenter, float sphereRadius, Vector3 point, float pointRadius, Vector3 tangentPlaneNormal) {
      //XDebug.DrawWireSphere(typeof(MathUtils), sphereCenter, sphereRadius + pointRadius, Colors.BlueViolet, 0.1f);

      float sizeA = sphereRadius + pointRadius;
      Vector3 vecB = point - sphereCenter;

      //XDebug.DrawLine(typeof(MathUtils), point, sphereCenter, Colors.LightGreen, 0.1f);

      float vecBSqrM = vecB.sqrMagnitude;

      // overlaps with sphere, push it out until it doesn't.
      float sizeB = 0f;
      if (vecBSqrM < sizeA*sizeA) {
        float multiplier = (sizeA + 1e-3f)/Mathf.Sqrt(vecBSqrM);
        vecB *= multiplier;
        sizeB = vecB.magnitude;
      } else {
        sizeB = Mathf.Sqrt(vecBSqrM);
      }

      // trig identities make "cos(pi/2 - asin(a/b)) * a" into "a(a/b)"
      float tangentPlaneDistFromCenter = sizeA*(sizeA/sizeB);

      Vector3 tangentPlanePoint = vecB*(tangentPlaneDistFromCenter/sizeB) + sphereCenter;

      //XDebug.DrawWireCube(typeof(MathUtils), tangentPlanePoint, Vector3.one * 0.1f, Colors.WhiteSmoke, 0.1f);

      float tangentConeRadius = Mathf.Sqrt(sizeA*sizeA - tangentPlaneDistFromCenter*tangentPlaneDistFromCenter);
      Vector3 preferredTangentDir = Vector3.Cross(tangentPlaneNormal, vecB).normalized;

      Vector3 preferredTangent = tangentConeRadius*preferredTangentDir;

      Vector3 tangentPoint = preferredTangent + tangentPlanePoint;

      //XDebug.DrawLine(typeof(MathUtils), point, tangentPoint, Colors.IndianRed, 0.1f);

      return tangentPoint;
    }

    public static bool Approximately(Vector3 a, Vector3 b) {
      return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
    }

    // return the max of v.x, v.y or v.z
    public static float MaxComponent(this Vector3 v) {
      return Mathf.Max(Mathf.Max(v.x, v.y), v.z);
    }
  }
}