using UnityEngine;
using Utils;

public class Prop : MonoBehaviour {
	[SerializeField] private bool m_InPreview = true;

	private HideInPropPreview[] m_HideInPreviewBits;
	private bool m_LastInPreview;

	public bool InPreview {
		get { return m_InPreview; }
		set {
			m_InPreview = value;
			PreviewChanged();
		}
	}

	protected virtual void PreviewChanged() {
		m_LastInPreview = m_InPreview;
		if (InPreview) {
			m_HideInPreviewBits = GetComponentsInChildren<HideInPropPreview>(true);
			m_HideInPreviewBits.ForEach(h => h.Hide());
		}
		else { m_HideInPreviewBits.ForEach(h => h.Show()); }
	}

	protected virtual void Update() {
		if (m_InPreview != m_LastInPreview) PreviewChanged();
	}
}