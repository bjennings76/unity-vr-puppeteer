using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ScaleCycler : MonoBehaviour {
	[SerializeField] private Renderer m_AttractorPrefab;
	[SerializeField, Range(1, 20)] private int m_Count = 5;
	[SerializeField, Range(0.1f, 10)] private float m_Duration = 3;
	[SerializeField] private AnimationCurve m_Opacity;
	[SerializeField] private AnimationCurve m_Size;
	[SerializeField] private AnimationCurve m_Distance;

	private readonly List<Renderer> m_Pieces = new List<Renderer>();
	private int m_LastCount;

	private void Start() {
		StartCycle();
	}

	private void StartCycle() {
		m_LastCount = m_Count;
		m_Pieces.ForEach(r => UnityUtils.Destroy(r.gameObject));
		m_Pieces.Clear();

		for (var i = 0; i < m_Count; i++) m_Pieces.Add(Instantiate(m_AttractorPrefab, transform));
	}

	private void LateUpdate() {
		CheckCount();
		UpdateCycle();
	}

	private void CheckCount() {
		if (m_LastCount != m_Count) StartCycle();
	}

	private void UpdateCycle() {
		m_Pieces.ForEach((p, i) => {
			var offset = (float) i / m_Count;
			var time = Time.time / m_Duration + offset;
			p.transform.localScale = m_Size.Evaluate(time) * Vector3.one;
			p.material.color = p.material.color.SetAlpha(m_Opacity.Evaluate(time));
			p.transform.localPosition = new Vector3(0, 0, m_Distance.Evaluate(time));
		});
	}
}