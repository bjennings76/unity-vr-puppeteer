using UnityEngine;
using Utils;
using VRTK;

public class UnlockOnThrow : MonoBehaviour {
	[SerializeField] private float m_StayUnlockedVelocity = 3f;

	private Rigidbody m_Rigidbody;
	private VRTK_InteractableObject m_InteractableObject;

	private void Start() {
		m_Rigidbody = GetComponent<Rigidbody>();
		m_InteractableObject = GetComponentInChildren<VRTK_InteractableObject>(true);
		m_InteractableObject.InteractableObjectGrabbed += OnGrabbed;
		m_InteractableObject.InteractableObjectUngrabbed += OnUngrabbed;
	}

	private void OnGrabbed(object sender, InteractableObjectEventArgs e) { m_Rigidbody.isKinematic = false; }

	private void OnUngrabbed(object sender, InteractableObjectEventArgs e) {
		var locked = m_Rigidbody.velocity.magnitude < m_StayUnlockedVelocity && !m_InteractableObject.IsInSnapDropZone();
		var interactingObjets = m_InteractableObject.GetTouchingObjects();
		XDebug.Log(this, "Touching objects = " + interactingObjets.AggregateString());
		XDebug.Log(this, "{0} release velocity: {1:N2} ({2})", e.interactingObject ? e.interactingObject.name : "unknown", m_Rigidbody.velocity.magnitude, locked ? "Locked" : "Ragdoll");
		m_Rigidbody.isKinematic = locked;
	}
}