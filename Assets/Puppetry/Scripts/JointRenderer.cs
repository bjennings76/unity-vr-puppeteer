using UnityEngine;
using Utils;

[ExecuteInEditMode, RequireComponent(typeof(LineRenderer))]
public class JointRenderer : MonoBehaviour {
	[SerializeField] private float m_Width = 0.005f;
	[SerializeField] private Material m_Material;

	private Joint m_Joint;
	private LineRenderer m_Line;

	private void Start() {
		m_Line = this.GetOrAddComponent<LineRenderer>();

		if (!m_Line.sharedMaterial) {
			m_Line.sharedMaterial = m_Material;
			m_Line.widthMultiplier = m_Width;
		}

		m_Joint = GetComponent<Joint>();
	}

	private void Update() {
		if (!m_Joint) return;
		var sourceAnchor = transform.TransformPoint(m_Joint.anchor);
		var targetAnchor = m_Joint.connectedBody.transform.TransformPoint(m_Joint.connectedAnchor);
		m_Line.SetPositions(new[] {sourceAnchor, targetAnchor});
	}
}