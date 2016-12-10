using UnityEngine;

public class HighlightOnMouseOver : MonoBehaviour {
  private MeshRenderer m_Renderer;

  public bool Highlight;
  private bool m_LastHighlight;

  private void Start() {
    m_Renderer = GetComponent<MeshRenderer>();
  }

  private void Update() {
    if (m_LastHighlight == Highlight) {
      return;
    }

    m_LastHighlight = Highlight;
    m_Renderer.material.shader = Shader.Find(Highlight ? "Self-Illumin/Outlined Diffuse" : "Diffuse");
  }
}