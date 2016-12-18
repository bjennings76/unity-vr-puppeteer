using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

public class ProgressStepAttribute : Attribute {
  private string m_Description;
  private float m_Weight;

  public ProgressStepAttribute(string description, float weight = 1.0f) {
    m_Description = description;
    m_Weight = weight;
  }

  public string Description {
    get {
      return m_Description;
    }
  }

  public float Weight {
    get {
      return m_Weight;
    }
  }
}


public abstract class ProgressBar {
  public interface ITask {
    string Title { get; }
    string Description { get; }
    float Start { get; }
    float Length { get; }
    void DisplayProgressBar(string title, string info, float progress);
    void Done();
  }

  protected class SubTask : ITask {
    private ProgressBar m_ProgressBar;
    private string m_Description;
    private float m_Start;
    private float m_Length;


    public SubTask(ProgressBar progressBar, string description, float start, float length) {
      m_ProgressBar = progressBar;
      m_Description = description;
      m_Start = start;
      m_Length = length;
    }

    public string Title {
      get {
        return m_ProgressBar.Parent.Title;
      }
    }

    public string Description {
      get {
        return m_Description;
      }
    }


    public float Start {
      get {
        return m_Start;
      }
    }

    public float Length {
      get {
        return m_Length;
      }
    }

    public void DisplayProgressBar(string title, string info, float progress) {
      m_ProgressBar.Parent.DisplayProgressBar(title, info, progress);
    }


    public void Done() {
      DisplayProgressBar(Title, Description, m_Start + m_Length);
    }
  }

  protected class RootTask : ITask {
    private string m_Title;
    private bool m_Cancelable;
    private double m_LastUpdateTime;
    private const double k_MinPeriod = 0.1;

    public RootTask(string title, bool cancelable) {
      m_Title = title;
      m_Cancelable = cancelable;
      m_LastUpdateTime = float.NegativeInfinity;
    }

    public string Title {
      get {
        return m_Title;
      }
    }

    public string Description {
      get {
        return "";
      }
    }

    public float Start {
      get {
        return 0.0f;
      }
    }

    public float Length {
      get {
        return 1.0f;
      }
    }

    public void DisplayProgressBar(string title, string info, float progress) {
      if (EditorApplication.timeSinceStartup >= m_LastUpdateTime + k_MinPeriod) {
        m_LastUpdateTime = EditorApplication.timeSinceStartup;
        if (m_Cancelable) {
          if (EditorUtility.DisplayCancelableProgressBar(title, info, progress)) {
            throw new UserCancelledException();
          }
        } else {
          EditorUtility.DisplayProgressBar(title, info, progress);
        }
      }
    }

    public void Done() {
      EditorUtility.ClearProgressBar();
    }
  }

  protected class SilentRootTask : ITask {
    public SilentRootTask() {}

    public string Title {
      get {
        return "";
      }
    }

    public string Description {
      get {
        return "";
      }
    }

    public float Start {
      get {
        return 0.0f;
      }
    }

    public float Length {
      get {
        return 1.0f;
      }
    }

    public void DisplayProgressBar(string title, string info, float progress) {
      // Do Nothing in Silent mode
    }

    public void Done() {
      // Do Nothing in Silent mode
    }
  }

  public class UserCancelledException : Exception {}

  private ITask m_Parent;

  protected ProgressBar(string description, bool cancelable, bool silent) {
    if (silent) {
      m_Parent = new SilentRootTask();
    } else {
      m_Parent = new RootTask(description, cancelable);
    }
  }

  protected ProgressBar(ITask parent) {
    m_Parent = parent;
  }

  protected ITask Parent {
    get {
      return m_Parent;
    }
  }


  protected ITask StartTask(string description, float start, float length) {
    float globalStart = Parent.Start + start*Parent.Length;
    float globalLength = length*Parent.Length;
    string globalDescription = description;
    //if (!string.IsNullOrEmpty(Parent.Description)) {
    //    globalDescription = Parent.Description + "\n\n" +description;
    //}
    Parent.DisplayProgressBar(m_Parent.Title, globalDescription, globalStart);
    return new SubTask(this, globalDescription, globalStart, globalLength);
  }


  public void Done() {
    Parent.Done();
  }
}

public class ProgressBarCounted : ProgressBar {
  private int m_NumSteps;

  public ProgressBarCounted(string title, int numSteps, bool cancelable = false, bool silent = false) : base(title, cancelable, silent) {
    Init(numSteps);
  }

  public ProgressBarCounted(ITask parent, int numSteps) : base(parent) {
    Init(numSteps);
  }

  private void Init(int numSteps) {
    m_NumSteps = numSteps;
  }

  public ITask StartStep(int step, string description) {
    return StartTask(description, (float) step/(float) m_NumSteps, 1.0f/(float) m_NumSteps);
  }
}

public class ProgressBarEnum<T> : ProgressBar where T : IConvertible {
  private class StepData {
    private float m_StartProgress;
    private float m_Length;
    private string m_Description;

    public StepData(string description, float startWeight, float weight) {
      m_Description = description;
      m_StartProgress = startWeight;
      m_Length = weight;
    }

    public string Description {
      get {
        return m_Description;
      }
    }

    public float StartProgress {
      get {
        return m_StartProgress;
      }
    }

    public float Length {
      get {
        return m_Length;
      }
    }

    public void Normalize(float totalWeight) {
      m_StartProgress = m_StartProgress/totalWeight;
      m_Length = m_Length/totalWeight;
    }
  }

  //private string m_Title;
  private Dictionary<int, StepData> m_StepMap;

  public ProgressBarEnum(string title, bool cancelable = false, bool silent = false) : base(title, cancelable, silent) {
    Init();
  }

  public ProgressBarEnum(ITask parent) : base(parent) {
    Init();
  }

  private void Init() {
    Type stepType = typeof(T);
    if (!stepType.IsEnum) {
      throw new ArgumentException("T must be an enum type");
    }

    m_StepMap = new Dictionary<int, StepData>();
    string[] names = Enum.GetNames(stepType);
    Array values = Enum.GetValues(stepType);

    float totalWeight = 0.0f;

    for (int i = 0; i < names.Length; i++) {
      int value = (int) values.GetValue(i);
      string name = names[i];
      string description = name;
      float weight = 1.0f;

      MemberInfo[] members = stepType.GetMember(names[i]);
      object[] attributes = members[0].GetCustomAttributes(typeof(ProgressStepAttribute), false);
      if ((attributes != null) && (attributes.Length > 0)) {
        ProgressStepAttribute progressStep = attributes[0] as ProgressStepAttribute;
        if (progressStep != null) {
          description = progressStep.Description;
          weight = progressStep.Weight;
        }
      }

      m_StepMap[value] = new StepData(description, totalWeight, weight);
      totalWeight += weight;
    }

    foreach (StepData stepData in m_StepMap.Values) {
      stepData.Normalize(totalWeight);
    }
  }

  public ITask StartStep(T step) {
    StepData stepData = null;
    int value = step.ToInt32(null);
    m_StepMap.TryGetValue(value, out stepData);
    return StartTask(stepData.Description, stepData.StartProgress, stepData.Length);
  }
}