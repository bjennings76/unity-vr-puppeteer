using UnityEngine;
using Utils;

public class IgnoreCollider : MonoBehaviour {
	[SerializeField] private Collider m_IgnoreMe;

	private int m_LastChildCount;

	private void Start() { Ignore(); }

	private void Ignore() {
		if (!m_IgnoreMe) return;
		m_LastChildCount = transform.childCount;
		GetComponentsInChildren<Collider>().ForEach(c => c.IgnoreCollisionsWith(m_IgnoreMe));
	}

	private void Update() {
		if (m_LastChildCount != transform.childCount) Ignore();
	}
}