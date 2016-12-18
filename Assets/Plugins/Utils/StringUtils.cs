using System.Text.RegularExpressions;

namespace Utils {
  public static class StringUtils {
    private static Regex camelCaseRegex = new Regex(@"([\p{Ll}\p{Lo}\p{Nd}\p{Nl}])([\p{Lt}\p{Lu}])");

    public static string CamelCaseSplitName(string name) {
      return camelCaseRegex.Replace(name, (Match match) => {
                                      string[] groups = new string[match.Groups.Count - 1];

                                      for (int i = 0; i < match.Groups.Count - 1; i++) {
                                        groups[i] = match.Groups[i + 1].Value;
                                      }

                                      return string.Join(" ", groups);
                                    });
    }

    // TEMP: For prototyping text only.
    // Very simple-minded; doesn't account for newlines already in the text. Only works correctly with Monospace fonts.
    public static string MonospaceWordWrap(string original, int softWrapWidth) {
      if (original.Length <= softWrapWidth) {
        return original;
      }

      char[] originalChars = original.ToCharArray();
      char[] newChars = new char[originalChars.Length];

      int lineWidth = 0;
      for (int i = 0; i < originalChars.Length; i++) {
        char currentChar = originalChars[i];
        if ((lineWidth >= softWrapWidth) && (currentChar == ' ')) {
          newChars[i] = '\n';
          lineWidth = 0;
        } else {
          newChars[i] = currentChar;
          lineWidth += 1;
        }
      }

      return new string(newChars);
    }


    private static readonly string[] Units = {"B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

    public static string GetHumanReadableSize(long sizeInBytes) {
      int unitIndex = 0;
      while (sizeInBytes >= 1024) {
        sizeInBytes /= 1024;
        ++unitIndex;
      }

      string unit = Units[unitIndex];
      return string.Format("{0:0.#} {1}", sizeInBytes, unit);
    }
  }
}