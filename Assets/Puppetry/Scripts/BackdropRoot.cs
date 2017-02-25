using UnityEngine;
using Utils;

public class BackdropRoot : Singleton<BackdropRoot> {
	[SerializeField, ReadOnly] private BackdropProp m_CurrentBackdrop;

	public void SetBackdrop(BackdropProp backdrop) {
		if (m_CurrentBackdrop) UnityUtils.Destroy(m_CurrentBackdrop.gameObject);
		m_CurrentBackdrop = backdrop;
	}
}