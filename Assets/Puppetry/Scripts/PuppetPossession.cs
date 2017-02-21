using UnityEngine;
using Utils;
using VRTK;

public class PuppetPossession : MonoBehaviour {
	private VRTK_InteractableObject[] m_Interactables;

	private void Start() {
		m_Interactables = GetComponentsInChildren<VRTK_InteractableObject>();
		m_Interactables.ForEach(i => i.InteractableObjectTouched += OnTouch);
	}

	private void OnTouch(object sender, InteractableObjectEventArgs e) {
		XDebug.Log(this, "{0} is interacting with {1}", sender, e.interactingObject.name);
	}
}