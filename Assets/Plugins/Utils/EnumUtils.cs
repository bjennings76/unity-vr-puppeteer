using System;
using UnityEngine;

namespace Utils {
  public static class EnumUtils {
    public static T PrevEnum<T>(T value) where T : struct {
      int index;
      T[] values;
      EnumLookup(value, out index, out values);
      return index == -1 ? default(T) : values[MathUtils.Mod(index - 1, values.Length)];
    }

    public static T NextEnum<T>(T value) where T : struct {
      int index;
      T[] values;
      EnumLookup(value, out index, out values);
      return index == -1 ? default(T) : values[MathUtils.Mod(index + 1, values.Length)];
    }

    // Helper function for PrevEnum() and NextEnum().
    private static void EnumLookup<T>(T value, out int index, out T[] values) where T : struct {
      Debug.AssertFormat(Enum.IsDefined(typeof(T), value), "{0} is not defined in {1}", value, typeof(T));
      values = (T[]) Enum.GetValues(typeof(T));
      for (int i = 0; i < values.Length; i++) {
        if (values[i].Equals(value)) {
          index = i;
          return;
        }
      }
      index = -1;
    }

    /// <summary>
    ///   Returns an enum value for the given string.
    /// </summary>
    /// <typeparam name="T">The type of enum to parse for.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="ignoreCase">Whether or not case should be ignored.</param>
    /// <returns>The enum value matching the string or the first value of the enum.</returns>
    public static T Parse<T>(string value, bool ignoreCase) where T : struct {
      T result;
      TryParse(value, out result, ignoreCase);
      return result;
    }

    /// <summary>
    ///   Returns an enum value for a given string or the given default value.
    /// </summary>
    /// <typeparam name="T">The type of enum to parse for.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="defaultValue">The default value to return if not found.</param>
    /// <param name="ignoreCase">Whether or not case should be ignored.</param>
    /// <returns>The enum value matching the string or the given default value.</returns>
    public static T Parse<T>(string value, T defaultValue, bool ignoreCase) where T : struct {
      T result;
      return TryParse(value, out result, ignoreCase) ? result : defaultValue;
    }

    public static bool TryParse<T>(string value, out T result, bool ignoreCase) where T : struct {
      for (int i = 0; i < Enum.GetNames(typeof(T)).Length; i++) {
        string enumName = Enum.GetNames(typeof(T))[i];

        if (!enumName.Equals(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) {
          continue;
        }

        result = (T) Enum.GetValues(typeof(T)).GetValue(i);
        return true;
      }

      result = (T) (object) 0;
      return false;
    }
  }
}