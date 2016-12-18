﻿using UnityEngine;

namespace Utils {
  public static class TransformExtensions {
    public static Quaternion TransformRotation(this Transform transform, Quaternion rotation) {
      //float angle;
      //Vector3 axis;
      //rotation.ToAngleAxis(out angle, out axis);
      //axis = transform.TransformDirection(axis);
      //return Quaternion.AngleAxis(angle, axis);

      Quaternion world = transform.rotation*rotation;
      NormalizeQuaternion(ref world);
      return world;
    }

    public static Quaternion InverseTransformRotation(this Transform transform, Quaternion rotation) {
      //float angle;
      //Vector3 axis;
      //rotation.ToAngleAxis(out angle, out axis);
      //axis = transform.InverseTransformDirection(axis);
      //return Quaternion.AngleAxis(angle, axis);
      Quaternion local = Quaternion.Inverse(transform.rotation)*rotation;
      NormalizeQuaternion(ref local);
      return local;
    }

    private static void NormalizeQuaternion(ref Quaternion q) {
      float sum = 0;
      for (int i = 0; i < 4; ++i) {
        sum += q[i]*q[i];
      }
      float magnitudeInverse = 1/Mathf.Sqrt(sum);
      for (int i = 0; i < 4; ++i) {
        q[i] *= magnitudeInverse;
      }
    }
  }
}