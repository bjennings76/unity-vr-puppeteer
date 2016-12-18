using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

public class ModelSwapper : MonoBehaviour {
	private SkinnedMeshRenderer[] m_Models;
	private int m_ModelIndex;
	private bool m_Init;

	private void Start() { Init(); }

	private void Init() {
		if (m_Init) { return; }

		m_Init = true;
		m_Models = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		for (int i = 0; i < m_Models.Length; i++) {
			var model = m_Models[i];
			if (model.gameObject.activeSelf) {
				m_ModelIndex = i;
				break;
			}
		}
		UpdateModel();
	}

	[UsedImplicitly]
	public void NextModel() {
		Init();
		m_ModelIndex = GetNext(m_Models, m_ModelIndex);
		UpdateModel();
	}

	[UsedImplicitly]
	public void PreviousModel() {
		Init();
		m_ModelIndex = GetPrevious(m_Models, m_ModelIndex);
		UpdateModel();
	}

	private void UpdateModel() {
		m_Models.ForEach(m => m.gameObject.SetActive(false));
		m_Models[m_ModelIndex].gameObject.SetActive(true);
	}

	private static int GetNext<T>(IEnumerable<T> array, int index) {
		index = index + 1;
		return index >= array.Count() ? 0 : index;
	}

	private static int GetPrevious<T>(IEnumerable<T> array, int index) {
		index = index - 1;
		return index < 0 ? array.Count() - 1 : index;
	}
}