using UnityEngine;
using Utils;
using VRTK;

public class UnlockOnGrab : MonoBehaviour {
	[SerializeField] private Rigidbody m_ObjectToUnlock;
	[SerializeField] private float m_StayUnlockedVelocity = 3f;

	private Transform m_InteractingObject;
	private Vector3 m_LastInteractingObjectPosition;
	private Collider[] m_Colliders;
	private Renderer[] m_Renderers;

	private float Velocity { get { return m_InteractingObject ? (m_InteractingObject.position - m_LastInteractingObjectPosition).magnitude / Time.deltaTime : 0; } }

	private void Start() {
		GetComponentsInChildren<VRTK_InteractableObject>().ForEach(RegisterInteractable);
		m_Colliders = m_ObjectToUnlock.GetComponentsInChildren<Collider>();
		m_Renderers = m_ObjectToUnlock.GetComponentsInChildren<Renderer>();
	}

	private void Update() {
		if (m_InteractingObject) {
			XDebug.Log(this, m_InteractingObject.name + " velocity: " + Velocity);
			m_LastInteractingObjectPosition = m_InteractingObject.position;
		}
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
		m_LastInteractingObjectPosition = m_InteractingObject.position;
	}

	private void OnUngrabbed(object sender, InteractableObjectEventArgs e) {
		if (Velocity < m_StayUnlockedVelocity) SetLock(true);
		m_InteractingObject = null;
	}

	private void SetLock(bool value) {
		m_ObjectToUnlock.isKinematic = value;
		m_Colliders.ForEach(c => c.enabled = value);
		m_Renderers.ForEach(r => r.enabled = value);
	}
}