using UnityEngine;

namespace Utils {
  public static class MathUtils {
    public static float ClampAngle(float a, float min = -180.0f, float max = 180.0f) {
      while (max < min) {
        max += 360.0f;
      }
      while (a > max) {
        a -= 360.0f;
      }
      while (a < min) {
        a += 360.0f;
      }

      if (a > max) {
        if (a - (max + min)*0.5f < 180.0f) {
          return max;
        } else {
          return min;
        }
      } else {
        return a;
      }
    }

    public static Vector3 ClampAngle(Vector3 a, float min = -180.0f, float max = 180.0f) {
      a.x = ClampAngle(a.x, min, max);
      a.y = ClampAngle(a.y, min, max);
      a.z = ClampAngle(a.z, min, max);
      return a;
    }

    public static float GetRadius(Bounds bounds) {
      return Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z)*0.5f;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds) {
      Vector3 p = bounds.center;
      p.x += Random.Range(-1.0f, 1.0f)*bounds.extents.x;
      p.y += Random.Range(-1.0f, 1.0f)*bounds.extents.y;
      p.z += Random.Range(-1.0f, 1.0f)*bounds.extents.z;
      return p;
    }

    public static float GaussianRand(float mean, float stdDev) {
      // Box-Muller Transformation
      float u1 = Random.value; // uniform random
      float u2 = Random.value;
      double randStdNormal = Mathf.Sqrt(-2.0f*Mathf.Log(u1))*Mathf.Sin(2.0f*Mathf.PI*u2); //random normal(0,1)
      return mean + stdDev*(float) randStdNormal; //random normal(mean,stdDev^2)
    }

    public static Vector3 Min(Vector3 a, Vector3 b) {
      Vector3 c = a;
      if (b.x < c.x) {
        c.x = b.x;
      }
      if (b.y < c.y) {
        c.y = b.y;
      }
      if (b.z < c.z) {
        c.z = b.z;
      }
      return c;
    }

    public static Vector3 Max(Vector3 a, Vector3 b) {
      Vector3 c = a;
      if (b.x > c.x) {
        c.x = b.x;
      }
      if (b.y > c.y) {
        c.y = b.y;
      }
      if (b.z > c.z) {
        c.z = b.z;
      }
      return c;
    }

    public static Bounds Combine(Bounds a, Bounds b, ref Bounds combined) {
      Vector3 min = Min(a.min, b.min);
      Vector3 max = Max(a.max, b.max);
      combined.SetMinMax(min, max);
      return combined;
    }

    public static Bounds BoundsWithMinMax(Vector3 min, Vector3 max) {
      Bounds b = new Bounds();
      b.SetMinMax(min, max);
      return b;
    }

    public static bool Or(int offset, int count, params bool[] values) {
      for (int i = offset; i < offset + count; i++) {
        if (values[i] == true) {
          return true;
        }
      }

      return false;
    }

    public static bool Or(params bool[] values) {
      return Or(0, values.Length, values);
    }

    public static float Average(int offset, int count, params float[] values) {
      if (count == 0) {
        return 0.0f;
      }

      float total = 0.0f;
      for (int i = offset; i < offset + count; i++) {
        total += values[i];
      }

      return total/(float) count;
    }

    public static float Average(params float[] values) {
      return Average(0, values.Length, values);
    }

    public static float MaxAbs(int offset, int count, params float[] values) {
      if (count == 0) {
        return 0.0f;
      } else if (count == 1) {
        return values[offset];
      }

      float max = values[offset];
      {
        float maxAbs = Mathf.Abs(max);
        for (int i = offset + 1; i < offset + count; i++) {
          float candidateAbs = Mathf.Abs(values[i]);
          if (candidateAbs > maxAbs) {
            max = values[i];
            maxAbs = candidateAbs;
          }
        }
      }

      return max;
    }


    //specialization for two float to avoid array allocation in the most common case
    public static float MaxAbs(float a, float b) {
      float maxA = Mathf.Abs(a);
      float maxB = Mathf.Abs(b);
      if (maxB > maxA) {
        return b;
      }
      return a;
    }

    // general purpose 
    public static float MaxAbs(params float[] values) {
      return MaxAbs(0, values.Length, values);
    }

    public static float Max(int offset, int count, params float[] values) {
      if (count == 0) {
        return 0.0f;
      } else if (count == 1) {
        return values[offset];
      }

      float max = values[offset];
      for (int i = offset + 1; i < offset + count; i++) {
        max = Mathf.Max(max, values[i]);
      }

      return max;
    }

    /// <summary>
    ///   Euclidean division modulo. Gives a positive result for a negative dividend.
    /// </summary>
    /// <remarks>
    ///   As opposed to the % operator, which gives a negative result for a negative dividend.
    /// </remarks>
    public static int Mod(int a, int n) {
      return (a%n + n)%n;
    }
  }
}