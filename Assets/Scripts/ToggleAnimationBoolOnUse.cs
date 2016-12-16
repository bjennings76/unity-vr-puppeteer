using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_InteractableObject))]
public class ToggleAnimationBoolOnUse : MonoBehaviour {
	[SerializeField] private Animator m_Animator;
	[SerializeField] private string m_BoolName;

	private VRTK_InteractableObject m_Interactable;

	private void Start() {
		m_Interactable = GetComponent<VRTK_InteractableObject>();
		if (m_Interactable) { m_Interactable.InteractableObjectUsed += OnUsed; }
	}

	private void OnUsed(object sender, InteractableObjectEventArgs e) {
		if (m_Animator) {
			var b = m_Animator.GetBool(m_BoolName);
			m_Animator.SetBool(m_BoolName, !b);
		}
	}
}