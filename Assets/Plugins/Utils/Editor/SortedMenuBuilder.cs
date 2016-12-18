using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SortedMenuBuilder {
  public class Menu {
    public delegate void MenuAction(IndentedTextWriter indentWriter);

    private string m_Name;
    private readonly bool m_Collapsable;
    private SortedDictionary<string, Menu> m_Children;
    private MenuAction m_Action;

    public Menu(string name, IComparer<string> comparer = null, bool collapsable = false) {
      m_Name = name;
      m_Collapsable = collapsable;
      m_Children = new SortedDictionary<string, Menu>(comparer);
    }


    public string Name {
      get {
        return m_Name;
      }
    }

    public IEnumerable<Menu> Children {
      get {
        return m_Children.Values;
      }
    }

    public MenuAction Action {
      get {
        return m_Action;
      }
    }

    public Menu Add(string menuPath, MenuAction action, bool collapsable = false) {
      Menu childMenu = null;
      string[] bits = menuPath.Split(new[] {'/'}, 2);
      if (bits.Length > 0) {
        string childMenuName = bits[0];

        if (!m_Children.TryGetValue(childMenuName, out childMenu)) {
          childMenu = new Menu(childMenuName, m_Children.Comparer, collapsable);
          m_Children.Add(childMenuName, childMenu);
        }
        if (bits.Length > 1) {
          childMenu = childMenu.Add(bits[1], action, collapsable);
        } else {
          childMenu.m_Action = action;
        }
      }
      return childMenu;
    }

    public void CollapseSingleMenuItems(string connector) {
      //XDebug.Log(typeof (SortedMenuBuilder), "m_Name={0}", m_Name);
      foreach (Menu child in m_Children.Values) {
        child.CollapseSingleMenuItems(connector);
      }

      if (m_Collapsable && (m_Children != null) && (m_Children.Count == 1) && (m_Action == null)) {
        Menu child = m_Children.First().Value;
        //XDebug.Log(typeof(SortedMenuBuilder), "collapsing child ={0}", child.m_Name);
        m_Name = string.Format("{0}{1}{2}", Name, connector, child.Name);
        m_Action = child.Action;
        m_Children = child.m_Children;
      }
    }
  }

  [Flags]
  public enum ClassQualifiers {
    Static = 1,
    Partial = 2,
  }

  private HashSet<string> m_UsedNames;

  public void WriteScript(Menu menu, string namespaceName, string className, ClassQualifiers classQualifiers, string scriptPath, int menuPriority) {
    m_UsedNames = new HashSet<string>();

    MemoryStream memStream = BuildScript(menu, namespaceName, className, classQualifiers, menuPriority);

    string fullScriptPath = Path.Combine(Application.dataPath, scriptPath);

    EditorApplication.LockReloadAssemblies();
    try {
      try {
        using (FileStream fileStream = new FileStream(fullScriptPath, FileMode.Create, FileAccess.Write)) {
          memStream.WriteTo(fileStream);
          fileStream.Close();
        }
      } catch (Exception e) {
        Debug.LogError(e);
        throw;
      }
    } finally {
      EditorApplication.UnlockReloadAssemblies();
    }
    AssetDatabase.ImportAsset("Assets/Scripts/Util/Editor/ScenesMenu.cs");
  }

  private MemoryStream BuildScript(Menu scenesMenu, string namespaceName, string className, ClassQualifiers classQualifiers, int menuPriority) {
    MemoryStream memStream = new MemoryStream();
    StreamWriter streamWriter = new StreamWriter(memStream, Encoding.UTF8);
    IndentedTextWriter indentWriter = new IndentedTextWriter(streamWriter);
    streamWriter.NewLine = "\n";
    indentWriter.NewLine = "\n";


    indentWriter.WriteLine("using UnityEngine;");
    indentWriter.WriteLine("using UnityEditor;");
    indentWriter.WriteLine();
    bool isPartial = (classQualifiers & ClassQualifiers.Partial) != 0;
    bool isStatic = (classQualifiers & ClassQualifiers.Static) != 0;
    bool usingNameSpace = !string.IsNullOrEmpty(namespaceName);

    if (usingNameSpace) {
      indentWriter.WriteLine("namespace {0} {{", namespaceName);
      indentWriter.Indent++;
    }

    indentWriter.WriteLine("public {1}{2}class {0} {{", className, isStatic ? "static " : "", isPartial ? "partial " : "");

    indentWriter.Indent++;

    WritePrefix(indentWriter);

    int priority = menuPriority + 12; // start at higher priority so that we get a separator after the Rebuild options.


    WriteMenu(indentWriter, scenesMenu, "", "", priority);

    WritePostfix(indentWriter);

    indentWriter.Indent--;
    indentWriter.WriteLine("}");

    if (usingNameSpace) {
      indentWriter.Indent--;
      indentWriter.WriteLine("}");
    }


    indentWriter.Flush();
    streamWriter.Flush();
    memStream.Flush();

    return memStream;
  }

  protected virtual void WritePrefix(IndentedTextWriter indentWriter) {}

  private void WriteMenu(IndentedTextWriter indentWriter, Menu menu, string menuPath, string menuMethodName, int priority) {
    if (string.IsNullOrEmpty(menuPath)) {
      menuPath = menu.Name;
    } else {
      menuPath = menuPath + "/" + menu.Name;
    }

    menuMethodName += menu.Name;

    if (menu.Action != null) {
      indentWriter.WriteLine("[MenuItem(\"{0}\", priority={1})]", menuPath, priority);
      indentWriter.WriteLine("public static void {0}MenuItem() {{", GetCleanUniqueName(menuMethodName));
      indentWriter.Indent++;
      menu.Action(indentWriter);
      indentWriter.Indent--;
      indentWriter.WriteLine("}");
      priority += 1;
    }

    foreach (Menu child in menu.Children) {
      WriteMenu(indentWriter, child, menuPath, menuMethodName, priority);
      priority += 1;
    }
  }


  private object GetCleanUniqueName(string name) {
    StringBuilder cleanName = new StringBuilder(name.Length);
    for (int i = 0; i < name.Length; i++) {
      char c = name[i];
      if (i == 0) {
        if (char.IsLetter(c)) {
          cleanName.Append(c);
        }
      } else {
        if (char.IsLetterOrDigit(c)) {
          cleanName.Append(c);
        }
      }
    }
    string cleanPrefix = cleanName.ToString();
    string result = cleanPrefix;
    int suffix = 0;
    while (m_UsedNames.Contains(result)) {
      result = cleanPrefix + suffix.ToString();
      suffix += 1;
    }
    m_UsedNames.Add(result);
    return result;
  }

  protected virtual void WritePostfix(IndentedTextWriter indentWriter) {}
}