using UnityEngine;
using Utils;
using VRTK;

public class UnlockOnGrab : MonoBehaviour {
	[SerializeField] private Rigidbody m_ObjectToUnlock;

	private void Start() { GetComponentsInChildren<VRTK_InteractableObject>().ForEach(RegisterInteractable); }

	private void RegisterInteractable(VRTK_InteractableObject interactable) {
		if (interactable.GetComponentInParent<SkipUnlockOnGrab>()) return;
		interactable.InteractableObjectGrabbed += OnGrabbed;
		interactable.InteractableObjectUngrabbed += OnUngrabbed;
	}

	private void OnGrabbed(object sender, InteractableObjectEventArgs e) { m_ObjectToUnlock.isKinematic = false; }

	private void OnUngrabbed(object sender, InteractableObjectEventArgs e) { m_ObjectToUnlock.isKinematic = true; }
}