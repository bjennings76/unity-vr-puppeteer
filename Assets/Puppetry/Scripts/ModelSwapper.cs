using UnityEngine;
using Utils;

public class ModelSwapper : MonoBehaviour {
	[SerializeField, HideInInspector] private SkinnedMeshRenderer[] m_Models;
	[SerializeField, HideInInspector] private bool m_Init;

	private int m_Index;

	public SkinnedMeshRenderer[] Models {  get { return m_Models; } }

	public int Index {
		get { return m_Index; }
		set {
			if (Models.Length == 0) return;
			m_Index = MathUtils.Mod(value, Models.Length);
			m_Models.ForEach((model, index) => model.gameObject.SetActive(Index == index));
		}
	}

	private void Awake() {
		m_Models = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		Index = m_Models.IndexOf(model => model.gameObject.activeSelf);
	}
}