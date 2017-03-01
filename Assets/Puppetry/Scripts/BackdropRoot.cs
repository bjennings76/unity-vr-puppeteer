using UnityEngine;
using Utils;

public class BackdropRoot : Singleton<BackdropRoot> {
	[SerializeField, ReadOnly] private BackdropProp m_CurrentBackdrop;

	private void Start() {
		var startingBackdrop = FindObjectOfType<BackdropProp>();
		if (startingBackdrop) SetBackdrop(startingBackdrop);
	}

	public void SetBackdrop(BackdropProp backdrop) {
		if (m_CurrentBackdrop == backdrop) return;
		if (m_CurrentBackdrop) UnityUtils.Destroy(m_CurrentBackdrop.gameObject);
		m_CurrentBackdrop = backdrop;
		backdrop.transform.SetParent(transform, true);
	}
}