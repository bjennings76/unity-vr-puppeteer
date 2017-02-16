using UnityEngine;

public class JointToggler : MonoBehaviour {
	[SerializeField] private Joint m_Joint;
	private Rigidbody m_ConnectedBody;

	private void Awake() {
		m_Joint = m_Joint ? m_Joint : GetComponent<Joint>();
		if (m_Joint) m_ConnectedBody = m_Joint.connectedBody;
		else Debug.LogError("No joint found.", this);
	}

	private void OnEnable() { m_Joint.connectedBody = m_ConnectedBody; }

	private void OnDisable() {
		m_Joint.connectedBody = null;
		m_ConnectedBody.WakeUp();
	}
}