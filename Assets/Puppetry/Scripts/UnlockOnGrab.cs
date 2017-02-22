using System.Linq;
using UnityEngine;
using Utils;
using VRTK;

public class UnlockOnGrab : MonoBehaviour {
	[SerializeField] private Rigidbody m_ObjectToUnlock;
	[SerializeField] private Transform m_ObjectToReset;
	[SerializeField] private float m_StayUnlockedVelocity = 3f;

	private Transform m_InteractingObject;
	private Collider[] m_Colliders;
	private Renderer[] m_Renderers;
	private JointLocker[] m_Joints;
	private Vector3 m_ResetObjectPosition;
	private Quaternion m_ResetObjectRotation;

	private void Start() {
		if (m_ObjectToReset) {
			m_ResetObjectPosition = m_ObjectToReset.localPosition;
			m_ResetObjectRotation = m_ObjectToReset.localRotation;
		}
		GetComponentsInChildren<VRTK_InteractableObject>(true).ForEach(RegisterInteractable);
		m_Colliders = m_ObjectToUnlock.GetComponentsInChildren<Collider>(true);
		m_Renderers = m_ObjectToUnlock.GetComponentsInChildren<Renderer>(true);
		m_Joints = m_ObjectToUnlock.GetComponentsInChildren<Joint>(true).Where(j => !j.GetComponent<SkipJointLock>()).Select(j => new JointLocker(j)).ToArray();
	}

	private void RegisterInteractable(VRTK_InteractableObject interactable) {
		if (interactable.GetComponentInParent<SkipUnlockOnGrab>()) return;
		interactable.InteractableObjectGrabbed += OnGrabbed;
		interactable.InteractableObjectUngrabbed += OnUngrabbed;
	}

	private void OnGrabbed(object sender, InteractableObjectEventArgs e) {
		SetLock(false);
		if (!e.interactingObject) return;
		m_InteractingObject = e.interactingObject.transform;
	}

	private void OnUngrabbed(object sender, InteractableObjectEventArgs e) {
		var locked = m_ObjectToUnlock.velocity.magnitude < m_StayUnlockedVelocity;
		XDebug.Log(this, "{0} release velocity: {1:N2} ({2})", m_InteractingObject.name, m_ObjectToUnlock.velocity.magnitude, locked ? "Locked" : "Ragdoll");
		SetLock(locked);
		m_InteractingObject = null;
	}

	public void SetLock(bool value) {
		if (value && m_ObjectToReset) {
			m_ObjectToReset.localPosition = m_ResetObjectPosition;
			m_ObjectToReset.localRotation = m_ResetObjectRotation;
		}
		m_ObjectToUnlock.isKinematic = value;
		m_Colliders.ForEach(c => c.enabled = value);
		m_Renderers.ForEach(r => r.enabled = value);
		m_Joints.ForEach(j => j.enabled = value);
	}

	private class JointLocker {
		private readonly Joint m_Joint;
		private readonly Rigidbody m_ConnectedBody;

		public bool enabled { set { m_Joint.connectedBody = value ? m_ConnectedBody : null; } }

		public JointLocker(Joint joint) {
			m_Joint = joint;
			m_ConnectedBody = joint.connectedBody;
		}
	}
}