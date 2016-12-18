using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Utils {
  public static class StringExtensions {
    /// <summary>
    ///   Returns true if the <paramref name="text" /> string contans the <paramref name="search" /> string using the supplied
    ///   <paramref name="comparisonType" />.
    /// </summary>
    /// <param name="text">The string to test for the check string.</param>
    /// <param name="search">The check string.</param>
    /// <param name="comparisonType">The comparison method to use which allows for IgnoreCase methods.</param>
    /// <returns>True if the search string is found in the given text.</returns>
    public static bool Contains(this string text, string search, StringComparison comparisonType) {
      if (search.IsNullOrEmpty()) {
        return true;
      }
      if (text.IsNullOrEmpty()) {
        return false;
      }
      return text.IndexOf(search, comparisonType) >= 0;
    }

    /// <summary>
    ///   Returns true if the strings is null or an empty string.
    /// </summary>
    /// <param name="source">The string to test.</param>
    /// <returns>True if the string is null or empty, otherwise false.</returns>
    [ContractAnnotation("source:null => true")]
    public static bool IsNullOrEmpty(this string source) {
      return string.IsNullOrEmpty(source);
    }

    public static string EmptyAsNull(this string source) {
      return (source == null) || source.Equals("") ? null : source;
    }

    public static string[] Split(this string str, string sep) {
      return str.Split(new[] {sep}, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string AggregateString<T>(this IEnumerable<T> ie, string sep = ",", Func<T, string> xfrm = null, string prepend = "", string append = "") {
      if (xfrm == null) {
        xfrm = v => v.ToString();
      }
      return ie.Aggregate(new StringBuilder(prepend), (c, n) => c.Length > prepend.Length ? c.Append(sep).Append(xfrm(n)) : c.Append(xfrm(n))).Append(append).ToString();
    }

    public static string Start(this string str, int c) {
      return (str != null) && (str.Length >= c) ? str.Substring(0, c) : null;
    }

    public static string End(this string str, int c) {
      return (str != null) && (str.Length >= c) ? str.Substring(str.Length - c, c) : null;
    }

    public static char Head(this string str) {
      return !string.IsNullOrEmpty(str) ? str[0] : (char) 0;
    }

    public static string Tail(this string str) {
      return !string.IsNullOrEmpty(str) ? str.Substring(1) : null;
    }

    public static char Last(this string str) {
      return !string.IsNullOrEmpty(str) ? str[str.Length - 1] : (char) 0;
    }

    public static string Init(this string str) {
      return !string.IsNullOrEmpty(str) ? str.Substring(0, str.Length - 1) : null;
    }

    public static string Slurp(this string str, string start) {
      return !str.StartsWith(start) ? str : str.Substring(start.Length);
    }

    public static string Slurp(this string str, string start, out bool found) {
      if (!str.StartsWith(start)) {
        found = false;
        return str;
      }
      found = true;
      return str.Substring(start.Length);
    }

    public static string Chomp(this string str, string start) {
      return !str.EndsWith(start) ? str : str.Substring(0, str.Length - start.Length);
    }

    public static string Chomp(this string str, string start, out bool found) {
      if (!str.EndsWith(start)) {
        found = false;
        return str;
      }
      found = true;
      return str.Substring(0, str.Length - start.Length);
    }

    public static string Interleave(this string str, string spacer) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.Length; i++) {
        if (i != 0) {
          sb.Append(spacer);
        }
        sb.Append(str[i]);
      }
      return sb.ToString();
    }

    public static string Sample(this string str, int chars, string ellipsis = "...", char pad = (char) 0) {
      int ml = chars - ellipsis.Length;
      int slen = Math.Min(str.Length, ml);
      string l = str.Substring(0, slen);
      if (pad > (char) 0) {
        l = l.PadRight(ml, pad);
      }
      return l + (str.Length < ml ? "" : ellipsis);
    }

    public static string Repeat(this char chatToRepeat, int repeat) {
      return repeat == 0 ? "" : new string(chatToRepeat, repeat);
    }

    public static string Repeat(this string stringToRepeat, int repeat) {
      if (repeat <= 0) {
        return "";
      }
      StringBuilder builder = new StringBuilder(repeat*stringToRepeat.Length);
      for (int i = 0; i < repeat; i++) {
        builder.Append(stringToRepeat);
      }
      return builder.ToString();
    }

    /// <summary>
    ///   A convenience extension version of Regex.Replace.
    /// </summary>
    /// <param name="input">The string to search for a match.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="replacement">The replacement string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
    /// <returns>
    ///   A new string that is identical to the input string, except that the replacement string takes the place of each
    ///   matched string.
    /// </returns>
    public static string ReplaceRegex(this string input, string pattern, string replacement, RegexOptions options = RegexOptions.None) {
      return Regex.Replace(input, pattern, replacement, options);
    }

    /// <summary>
    ///   Replaces a string's old value with a new value using the string comparison type.
    /// </summary>
    /// <param name="originalString">The string to run the search/replace on.</param>
    /// <param name="oldValue">The old value to find.</param>
    /// <param name="newValue">The new value to replace.</param>
    /// <param name="comparisonType">The type of comparison to use.</param>
    /// <returns></returns>
    public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType) {
      int startIndex = 0;
      while (true) {
        startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
        if (startIndex == -1) {
          break;
        }
        originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);
        startIndex += newValue.Length;
      }

      return originalString;
    }

    public static string UntilFirst(this string haystack, string needle, string els = null) {
      int index = haystack.IndexOf(needle, StringComparison.Ordinal);
      if (index == -1) {
        return els;
      }
      return haystack.Substring(0, index);
    }

    public static string UntilFirst(this string haystack, char needle, string els = null) {
      int index = haystack.IndexOf(needle);
      if (index == -1) {
        return els;
      }
      return haystack.Substring(0, index);
    }

    public static string UntilLast(this string haystack, string needle, string els = null) {
      int index = haystack.LastIndexOf(needle, StringComparison.Ordinal);
      if (index == -1) {
        return els;
      }
      return haystack.Substring(0, index);
    }

    public static string UntilLast(this string haystack, char needle, bool nullfail) {
      return haystack.UntilLast(needle, nullfail ? null : haystack);
    }

    public static string UntilLast(this string haystack, char needle, string els = null) {
      int index = haystack.LastIndexOf(needle);
      if (index == -1) {
        return els;
      }
      return haystack.Substring(0, index);
    }

    public static string AfterLast(this string haystack, string needle, string els = null) {
      int index = haystack.LastIndexOf(needle, StringComparison.Ordinal);
      if (index == -1) {
        return els;
      }
      int needleEnd = index + needle.Length;
      return haystack.Substring(needleEnd, haystack.Length - needleEnd);
    }

    public static string AfterLast(this string haystack, char needle, string els = null) {
      int index = haystack.LastIndexOf(needle);
      if (index == -1) {
        return els;
      }
      int needleEnd = index + 1;
      return haystack.Substring(needleEnd, haystack.Length - needleEnd);
    }

    public static string AfterFirst(this string haystack, string needle, string els = null) {
      int index = haystack.IndexOf(needle, StringComparison.Ordinal);
      if (index == -1) {
        return els;
      }
      int needleEnd = index + needle.Length;
      return haystack.Substring(needleEnd, haystack.Length - needleEnd);
    }

    public static string AfterFirst(this string haystack, char needle, string els = null) {
      int index = haystack.IndexOf(needle);
      if (index == -1) {
        return els;
      }
      int needleEnd = index + 1;
      return haystack.Substring(needleEnd, haystack.Length - needleEnd);
    }

    /// <summary>
    ///   Parses out a string (e.g. file name or camel case ID) into a readable name.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <param name="capitalize">If true, will capitalize the first character of words.</param>
    /// <param name="removeNumbers">If true, will remove numbers.</param>
    /// <returns>The converted human-readable string.</returns>
    public static string ToSpacedName(this string text, bool capitalize = true, bool removeNumbers = true, bool removeSingleLetters = true) {
      text = text.ReplaceRegex(@"[A-Z][a-z]", " $0").ReplaceRegex(@"([0-9])([A-Za-z])|([A-Za-z])([0-9])", "$1$3 $2$4");

      if (removeNumbers) {
        string[] testArray = text.Split(new[] {' ', '\t', '.', '_', '#', '$', '%', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < testArray.Length; i++) {
          if ((testArray[i].Length <= 1) && removeSingleLetters) {
            continue;
          }
          if (sb.Length != 0) {
            sb.Append(" ");
          }
          sb.Append(testArray[i]);
        }
        text = sb.ToString();
      } else if (removeSingleLetters) {
        string[] testArray = text.Split(new[] {' ', '\t', '.', '_', '#', '$', '%'}, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < testArray.Length; i++) {
          if ((testArray[i].Length == 1) && !char.IsDigit(testArray[i][0])) {
            continue;
          }
          if (sb.Length != 0) {
            sb.Append(" ");
          }
          sb.Append(testArray[i]);
        }
        text = sb.ToString();
      }

      return capitalize ? text.ToCapitalized().Trim() : text.Trim();
    }

    /// <summary>
    ///   Capitalizes the first letter of each word in the supplied string.
    /// </summary>
    /// <param name="text">The text to capitalize.</param>
    /// <returns>The text with capitalized first characters.</returns>
    public static string ToCapitalized(this string text, bool invariant = false) {
      return invariant ? CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text) : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
    }

    /// <summary>
    ///   Capitalizes the first letter of each word except words normally lower case in titles.
    /// </summary>
    /// <param name="text">The text to change to title case.</param>
    /// <returns>The text converted to title case.</returns>
    public static string ToTitleCase(this string text) {
      // Cycle through all words
      foreach (Match match in Regex.Matches(text, @"\w+", RegexOptions.IgnoreCase)) {
        // Set replacement words to lowercase
        string prefix = text.Substring(0, match.Index);
        string change = Regex.IsMatch(match.Value, @"\b(a|an|the|at|by|for|in|of|on|to|up|and|as|but|it|or|nor|with)\b", RegexOptions.IgnoreCase) ? match.Value.ToLower() : match.Value.ToCapitalized();
        string postFix = text.Substring(match.Index + match.Length);

        text = prefix + change + postFix;
      }

      return text;
    }

    public static string ToUnderscored(this string text) {
      return Regex.Replace(text, @"(?:(?<!\b)([A-Z])|\s+)", m => "_" + (m.Captures.Count > 0 ? m.Captures[1].ToString().ToLower() : ""), RegexOptions.Singleline).ToLower();
    }

    public static string FromCamel(this string text) {
      return Regex.Replace(text, "(?<=[a-z])([A-Z0-9])", " $1").ToCapitalized(true);
    }

    public static string ToCamel(this string text, bool firstLower = false) {
      text = text.Trim();
      if (text.Length == 0) {
        return "";
      }
      if (text.Length == 1) {
        return text.ToLowerInvariant();
      }
      string camel = Regex.Replace(text, "[\\W_]+", " ", RegexOptions.Singleline);
      camel = Regex.Replace(camel, "(.)([A-Z][a-z])", "$1 $2");
      camel = camel.ToCapitalized(true).Replace(" ", "");
      if (camel.Length == 0) {
        return "";
      }
      if (camel.Length == 1) {
        return camel.ToLowerInvariant();
      }
      if (firstLower) {
        return camel.Substring(0, 1).ToLowerInvariant() + camel.Substring(1);
      }
      return camel;
    }

    public static string ToStringSafe(this object src) {
      return src == null ? "" : src.ToString();
    }


    public static bool MatchMultipartString(string input, string match) {
      if (input == null) {
        return false;
      }
      string[] pieces = match.Split(' ');
      for (int i = 0; i < pieces.Length; i++) {
        if (input.IndexOf(pieces[i], StringComparison.OrdinalIgnoreCase) == -1) {
          return false;
        }
      }
      return true;
    }
  }
}