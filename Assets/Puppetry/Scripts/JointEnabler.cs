using UnityEngine;

public class JointEnabler : MonoBehaviour {
	private Joint _Joint;
	private Rigidbody _ConnectedBody;

	private void Awake() {
		_Joint = GetComponent<Joint>();
		if (_Joint) _ConnectedBody = _Joint.connectedBody;
		else Debug.LogError("No joint found.", this);
	}

	private void OnEnable() { _Joint.connectedBody = _ConnectedBody; }

	private void OnDisable() {
		_Joint.connectedBody = null;
		_ConnectedBody.WakeUp();
	}
}